using todolist.Models;
using todolist.Context;
using Microsoft.EntityFrameworkCore;

namespace todolist.DAO;

public class ActivitiesDAO : IActivitiesDAO
{
    private readonly ToDoContext _context;

    public ActivitiesDAO(ToDoContext context)
    {
        _context = context;
    }

    public async Task<Activity> GetActivityById(int id)
    {
        return await _context.Activities.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Activity>> GetActivities()
    {
        return await _context.Activities.ToListAsync();
    }

    public async Task<IEnumerable<Activity>> GetActivitiesByUserId(int userId)
    {
        return await _context.Activities
            .Where(a => a.UserId == userId)
            .ToListAsync();
    }

    public async Task AddActivity(Activity activity)
    {
        await _context.Activities.AddAsync(activity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateActivity(Activity activity)
    {
        _context.Activities.Update(activity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteActivity(int id)
    {
        var activity = await _context.Activities.FindAsync(id);
        if (activity != null)
        {
            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();
        }
    }
}