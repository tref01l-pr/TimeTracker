using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Interfaces.Services.Application;

/// <summary>
/// Interface for the Companies service, inheriting from a generic CRUD service.
/// This interface defines methods for business logic related to Company entities.
/// </summary>
public interface ICompaniesService : ICrudService<CompanyEntity, Company, int>
{
    /// <summary>
    /// Retrieves all Company records asynchronously.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the Company records to.</typeparam>
    /// <returns>A Result containing a list of Company records projected to the specified type.</returns>
    Task<Result<IList<TProjectTo>>> GetAllAsync<TProjectTo>();
}