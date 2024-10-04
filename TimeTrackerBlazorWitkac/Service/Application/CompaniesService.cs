using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Services;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Helpers;
using TimeTrackerBlazorWitkac.Interfaces.Repository;
using TimeTrackerBlazorWitkac.Interfaces.Services.Application;
using TimeTrackerBlazorWitkac.Service.Application.BaseServices;

namespace TimeTrackerBlazorWitkac.Service.Application;

/// <summary>
/// Service class for managing Company entities, extending CRUD operations.
/// </summary>
public class CompaniesService : CrudService<ICompaniesRepository, CompanyEntity, Company, int>, ICompaniesService
{
    /// <summary>
    /// Initializes a new instance of the CompaniesService with the specified repository and transaction repository.
    /// </summary>
    /// <param name="repository">The repository for accessing Company data.</param>
    /// <param name="transactionRepository">The repository for handling transactions.</param>
    public CompaniesService(
        ICompaniesRepository repository,
        ITransactionRepository transactionRepository) : base(repository, transactionRepository) { }

    /// <summary>
    /// Retrieves all Company records asynchronously.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the results to.</typeparam>
    /// <returns>A Result containing a list of Company records or an error message.</returns>
    public async Task<Result<IList<TProjectTo>>> GetAllAsync<TProjectTo>()
    {
        try
        {
            var result = await _repository.GetAllAsync<TProjectTo>();
            return Result.Success(result);
        }
        catch (Exception e)
        {
            return Result.Failure<IList<TProjectTo>>(e.Message);
        }
    }

    /// <summary>
    /// Updates an existing Company record asynchronously.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the result to.</typeparam>
    /// <param name="model">The Company model with updated information.</param>
    /// <returns>A Result containing the updated Company or an error message.</returns>
    public override async Task<Result<TProjectTo>> UpdateAsync<TProjectTo>(Company model)
    {
        try
        {
            var company = await _repository.GetByIdAsync<Company>(model.Id);
            if (company == null)
            {
                throw new Exception(CompaniesServiceErrorMessages.CompanyNotFound.GetDescription());
            }

            if (company.Name != model.Name && await _repository.GetByNameAsync<Company>(model.Name) != null)
            {
                throw new Exception(CompaniesServiceErrorMessages.CompanyNameExists.GetDescription());
            }
            
            return await base.UpdateAsync<TProjectTo>(model);
        }
        catch (Exception e)
        {
            return Result.Failure<TProjectTo>(e.Message);
        }
    }

    /// <summary>
    /// Creates a new Company record asynchronously.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the result to.</typeparam>
    /// <param name="model">The Company model to create.</param>
    /// <returns>A Result containing the created Company or an error message.</returns>
    public override async Task<Result<TProjectTo>> CreateAsync<TProjectTo>(Company model)
    {
        try
        {
            var nameExist = await _repository.GetByNameAsync<Company>(model.Name);
            if (nameExist != null)
            {
                throw new Exception(CompaniesServiceErrorMessages.CompanyNameExists.GetDescription());
            }
            return await base.CreateAsync<TProjectTo>(model);
        }
        catch (Exception e)
        {
            return Result.Failure<TProjectTo>(e.Message);
        }
    }
}