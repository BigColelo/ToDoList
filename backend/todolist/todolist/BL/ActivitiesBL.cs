using todolist.DAO;
using todolist.DTO;
using todolist.Models;

namespace todolist.BL;

public class ActivitiesBL : IActivitiesBL
{
    private readonly IActivitiesDAO _activitiesDAO;

    public ActivitiesBL(IActivitiesDAO activitiesDAO)
    {
        _activitiesDAO = activitiesDAO;
    }

    public async Task<Activity> GetActivityById(int id)
    {
        return await _activitiesDAO.GetActivityById(id);
    }

    public async Task<IEnumerable<Activity>> GetActivities()
    {
        return await _activitiesDAO.GetActivities();
    }

    // Aggiungi questo metodo mancante
    public async Task<IEnumerable<Activity>> GetActivitiesByUserId(int userId)
    {
        return await _activitiesDAO.GetActivitiesByUserId(userId);
    }

    public async Task AddActivity(Activity activity)
    {
        var act = new Activity
        {
            Title = activity.Title,
            Description = activity.Description,
            Priority = activity.Priority,
            Status = ActivityStatus.ToDo,
            UserId = activity.UserId
        };
        await _activitiesDAO.AddActivity(act);
    }

    public async Task UpdateActivity(Activity activity)
    {
        var act = await _activitiesDAO.GetActivityById(activity.Id);
        if (act != null)
        {
            act.Title = activity.Title;
            if (activity.Description != act.Description) act.Description = activity.Description;
            if (activity.Priority != act.Priority) act.Priority = activity.Priority;
            act.UpdatedAt = DateTime.UtcNow;
            
            await _activitiesDAO.UpdateActivity(act);
        }
    }

    public async Task MarkActivityAsDoneAsync(int activityId)
    {
        // Recupera l'attività dal database
        var activity = await _activitiesDAO.GetActivityById(activityId);
        
        if (activity != null)
        {
            // Cambia lo status a "Done"
            activity.Status = ActivityStatus.Done;  // Imposta lo status su "Done"
            activity.UpdatedAt = DateTime.UtcNow;   // Imposta la data di aggiornamento
            
            // Salva l'attività aggiornata nel database
            await _activitiesDAO.UpdateActivity(activity);
        }
    }

    public async Task DeleteActivity(int id)
    {
        await _activitiesDAO.DeleteActivity(id);
    }
}