using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Interfaces.Repository;

/// <summary>
/// Interface for the Companies repository, inheriting from a generic CRUD repository.
/// This interface defines methods for accessing Company records in the database.
/// </summary>
public interface ICompaniesRepository : ICrudRepository<CompanyEntity, Company, int>
{
    /// <summary>
    /// Retrieves all Company records from the database.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the Company records to.</typeparam>
    /// <returns>A list of Company records projected to the specified type.</returns>
    Task<IList<TProjectTo>> GetAllAsync<TProjectTo>();

    /// <summary>
    /// Retrieves a Company record by its name.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the Company record to.</typeparam>
    /// <param name="name">The name of the Company to retrieve.</param>
    /// <returns>A Company record projected to the specified type, or null if not found.</returns>
    Task<TProjectTo?> GetByNameAsync<TProjectTo>(string name);
}