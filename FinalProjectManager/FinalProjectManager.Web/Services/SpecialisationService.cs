using FinalProjectManager.Data.Data;
using FinalProjectManager.Data.Models;
using FinalProjectManager.Web.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace FinalProjectManager.Web.Services;

public class SpecialisationService : ISpecialisationService
{
    private readonly ApplicationDbContext _context;

    public SpecialisationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Specialisation>> GetAllAsync()
        => await _context.Specialisations.Include(s => s.Students).OrderBy(s => s.Name).ToListAsync();

    public async Task<Specialisation?> GetByIdAsync(int id)
        => await _context.Specialisations.Include(s => s.Students).FirstOrDefaultAsync(s => s.Id == id);

    public async Task CreateAsync(Specialisation specialisation)
    {
        _context.Specialisations.Add(specialisation);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Specialisation specialisation)
    {
        _context.Specialisations.Update(specialisation);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var specialisation = await _context.Specialisations.FindAsync(id);
        if (specialisation != null)
        {
            _context.Specialisations.Remove(specialisation);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
        => await _context.Specialisations.AnyAsync(s => s.Id == id);

    public async Task<bool> HasStudentsAsync(int id)
        => await _context.Students.AnyAsync(s => s.SpecialisationId == id);
}
