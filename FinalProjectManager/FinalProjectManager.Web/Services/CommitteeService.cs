using FinalProjectManager.Data.Data;
using FinalProjectManager.Data.Models;
using FinalProjectManager.Web.Services.Interfaces;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinalProjectManager.Web.Services;

public class CommitteeService : ICommitteeService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly INotificationService _notificationService;

    public CommitteeService(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        INotificationService notificationService)
    {
        _context = context;
        _userManager = userManager;
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<DefenseCommittee>> GetAllAsync() =>
        await _context.DefenseCommittees
            .Include(c => c.Members).ThenInclude(m => m.Supervisor)
            .Include(c => c.Students)
            .OrderBy(c => c.Name)
            .ToListAsync();

    public async Task<DefenseCommittee?> GetByIdAsync(int id) =>
        await _context.DefenseCommittees
            .Include(c => c.Members).ThenInclude(m => m.Supervisor)
            .Include(c => c.Students).ThenInclude(s => s.Topic)
            .Include(c => c.Students).ThenInclude(s => s.Supervisor)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task GenerateAsync(int committeeCount, int supervisorsPerCommittee)
    {
        await ClearAllAsync();

        var committees = Enumerable.Range(1, committeeCount)
            .Select(i => new DefenseCommittee { Name = $"Комисия {i}" })
            .ToList();
        _context.DefenseCommittees.AddRange(committees);
        await _context.SaveChangesAsync();

        var students = await _context.Students
            .Where(s => s.SupervisorId != null)
            .OrderBy(s => s.FullName)
            .ToListAsync();

        if (students.Count == 0)
            throw new InvalidOperationException("Няма ученици с назначени ръководители. Първо назначете ръководители.");

        for (var i = 0; i < students.Count; i++)
            students[i].CommitteeId = committees[i % committeeCount].Id;

        await _context.SaveChangesAsync();

        var supervisors = await _context.Supervisors.ToListAsync();

        // Load ApplicationUsers for all supervisors (to check CanLeadCommittee)
        var supervisorUsers = new Dictionary<int, ApplicationUser?>();
        foreach (var sv in supervisors)
            supervisorUsers[sv.Id] = await _userManager.FindByEmailAsync(sv.Email);

        foreach (var committee in committees)
        {
            var blockedIds = students
                .Where(s => s.CommitteeId == committee.Id)
                .Select(s => s.SupervisorId)
                .ToHashSet();

            var eligible = supervisors
                .Where(sv => !blockedIds.Contains(sv.Id))
                .OrderBy(_ => Guid.NewGuid())
                .Take(supervisorsPerCommittee)
                .ToList();

            if (eligible.Count < supervisorsPerCommittee)
                throw new InvalidOperationException(
                    $"{committee.Name}: само {eligible.Count} подходящи учители налични, необходими са {supervisorsPerCommittee}. " +
                    $"Намалете броя учители на комисия или добавете повече учители.");

            // Chair must have CanLeadCommittee = true; fall back to any eligible if none qualify
            var chairCandidates = eligible
                .Where(sv => supervisorUsers.GetValueOrDefault(sv.Id)?.CanLeadCommittee == true)
                .ToList();

            var chair = chairCandidates.Count > 0 ? chairCandidates[0] : eligible[0];

            foreach (var member in eligible)
            {
                _context.CommitteeMembers.Add(new CommitteeMember
                {
                    CommitteeId = committee.Id,
                    SupervisorId = member.Id,
                    IsChair = member.Id == chair.Id
                });
            }
        }

        await _context.SaveChangesAsync();

        // Notify students
        foreach (var student in students)
        {
            var committee = committees.First(c => c.Id == student.CommitteeId);
            await _notificationService.CreateForEmailAsync(
                student.Email,
                "Комисия за защита",
                $"Разпределени сте в {committee.Name}.");
        }

        // Notify supervisors
        var allMembers = await _context.CommitteeMembers
            .Include(m => m.Supervisor)
            .Include(m => m.Committee)
            .ToListAsync();

        foreach (var member in allMembers)
        {
            var roleText = member.IsChair ? "председател" : "член";
            await _notificationService.CreateForEmailAsync(
                member.Supervisor.Email,
                "Комисия за защита",
                $"Назначени сте като {roleText} на {member.Committee.Name}.");
        }
    }

    public async Task SetChairAsync(int committeeId, int supervisorId)
    {
        var members = await _context.CommitteeMembers
            .Where(m => m.CommitteeId == committeeId)
            .ToListAsync();

        foreach (var m in members)
            m.IsChair = m.SupervisorId == supervisorId;

        await _context.SaveChangesAsync();
    }

    public async Task ClearAllAsync()
    {
        var students = await _context.Students.Where(s => s.CommitteeId != null).ToListAsync();
        foreach (var s in students) s.CommitteeId = null;
        await _context.SaveChangesAsync();

        _context.CommitteeMembers.RemoveRange(_context.CommitteeMembers);
        _context.DefenseCommittees.RemoveRange(_context.DefenseCommittees);
        await _context.SaveChangesAsync();
    }
}
