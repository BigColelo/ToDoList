using todolist.Models;

namespace todolist.DAO;

public interface IActivitiesDAO
{
    Task<Activity> GetActivityById(int id);
    Task<IEnumerable<Activity>> GetActivities();
    Task<IEnumerable<Activity>> GetActivitiesByUserId(int userId);
    Task AddActivity(Activity activity);
    Task UpdateActivity(Activity activity);
    Task DeleteActivity(int id);
}