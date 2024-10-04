using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Interfaces.Repository;

/// <summary>
/// Interface for managing AbsenceType entities.
/// </summary>
public interface IAbsenceTypesRepository : ICrudRepository<AbsenceTypeEntity, AbsenceType, int>
{
    /// <summary>
    /// Retrieves all AbsenceType records from the database.
    /// </summary>
    /// <returns>An list of AbsenceType records.</returns>
    Task<IList<TProjectTo>> GetAllAsync<TProjectTo>();
}