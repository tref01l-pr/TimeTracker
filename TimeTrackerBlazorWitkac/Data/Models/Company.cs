using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Interfaces;
using TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Models;
using TimeTrackerBlazorWitkac.Helpers;

namespace TimeTrackerBlazorWitkac.Data.Models;

// The Company record class represents a company in the system, including its foundational data.
public record Company : IModelKey<int>
{
    // Maximum length allowed for the company's name.
    public const int MaxNameLength = 128;

    // Private constructor to ensure the Create method is used for instantiation.
    private Company(
        int id,
        string name,
        DateOnly? dateOfFoundation)
    {
        Id = id;
        Name = name;
        DateOfFoundation = dateOfFoundation;
    }

    public int Id { get; init; }
    public string Name { get; }
    public DateOnly? DateOfFoundation { get; }
    
    public static CompanyBuilder Builder() => new CompanyBuilder();

    public class CompanyBuilder
    {
        private int _id = 0;
        private string _name;
        private DateOnly? _dateOfFoundation;

        public CompanyBuilder SetId(int id)
        {
            _id = id;
            return this;
        }
        
        public CompanyBuilder SetName(string name)
        {
            _name = name;
            return this;
        }
        
        public CompanyBuilder SetDateOfFoundation(DateOnly dateOfFoundation)
        {
            _dateOfFoundation = dateOfFoundation;
            return this;
        }

        public Result<Company> Build()
        {
            var validationResult = ValidateCompanyData(_name, _dateOfFoundation);

            if (validationResult.IsFailure)
            {
                return Result.Failure<Company>(validationResult.Error);
            }

            return new Company(_id, _name, _dateOfFoundation);
        }

        private static Result ValidateCompanyData(string name, DateOnly? dateOfFoundation)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Result.Failure(CompanyErrorMessages.InvalidName.GetDescription());
            }

            if (name.Length >= MaxNameLength)
            {
                return Result.Failure(CompanyErrorMessages.NameTooLong.GetDescription());
            }

            if (dateOfFoundation.HasValue && dateOfFoundation.Value >= DateOnly.FromDateTime(DateTime.Now))
            {
                return Result.Failure(CompanyErrorMessages.InvalidDateOfFoundation.GetDescription());
            }

            return Result.Success();
        }
    }
}