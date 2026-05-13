using FinalProjectManager.Data.Data;
using FinalProjectManager.Data.Models;
using FinalProjectManager.Web.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace FinalProjectManager.Web.Services;

public class SupervisorService : ISupervisorService
{
    private readonly ApplicationDbContext _context;

    public SupervisorService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Supervisor>> GetAllAsync(string? search = null, int? specialisationId = null)
    {
        var query = _context.Supervisors.Include(s => s.Specialisation).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s => s.FullName.Contains(search) || s.Email.Contains(search));

        if (specialisationId.HasValue)
            query = query.Where(s => s.SpecialisationId == specialisationId.Value);

        return await query.OrderBy(s => s.FullName).ToListAsync();
    }

    public async Task<Supervisor?> GetByIdAsync(int id)
        => await _context.Supervisors.Include(s => s.Specialisation).FirstOrDefaultAsync(s => s.Id == id);

    public async Task CreateAsync(Supervisor supervisor)
    {
        _context.Supervisors.Add(supervisor);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Supervisor supervisor)
    {
        _context.Supervisors.Update(supervisor);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var supervisor = await _context.Supervisors.FindAsync(id);
        if (supervisor == null) return;

        var supervised = await _context.Students.Where(s => s.SupervisorId == id).ToListAsync();
        foreach (var s in supervised) s.SupervisorId = null;

        var reviewed = await _context.Students.Where(s => s.ReviewerId == id).ToListAsync();
        foreach (var s in reviewed) s.ReviewerId = null;

        _context.Supervisors.Remove(supervisor);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
        => await _context.Supervisors.AnyAsync(s => s.Id == id);
}
