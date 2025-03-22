
using todolist.Models;

namespace todolist.BL;

public interface IActivitiesBL
{
    Task<Activity> GetActivityById(int id);
    Task<IEnumerable<Activity>> GetActivities();
    Task<IEnumerable<Activity>> GetActivitiesByUserId(int userId);
    Task AddActivity(Activity activity);
    Task UpdateActivity(Activity activity);
    Task MarkActivityAsDoneAsync(int activityId);
    Task DeleteActivity(int id);
}