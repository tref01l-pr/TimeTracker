using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Interfaces;
using TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Models;
using TimeTrackerBlazorWitkac.Helpers;

namespace TimeTrackerBlazorWitkac.Data.Models;

// The Holiday record class represents a holiday or observance.
// It provides a structured way to work with holiday data in the application.
public record Holiday : IModelKey<int>
{
    // Maximum allowed length for the summary of the holiday.
    // If the summary exceeds this length, it is considered invalid.
    public const int MaxNameLength = 128;
    
    // Maximum allowed length for the description of the holiday.
    // If the description exceeds this length, it is considered invalid.
    public const int MaxDescriptionLength = 360;

    // Private constructor to enforce the use of the Create method for object creation.
    private Holiday(
        int id,
        string name,
        string localName,
        DateOnly startDate,
        DateOnly endDate,
        string? description
        )
    {
        Id = id;
        Name = name;
        LocalName = localName;
        StartDate = startDate;
        EndDate = endDate;
        Description = description;
    }
    
    public int Id { get; init; }
    public string Name { get; }
    public string LocalName { get; }
    public DateOnly StartDate { get; }
    public DateOnly EndDate { get; }
    public string? Description { get; }

    public static HolidayBuilder Builder() => new HolidayBuilder();
    
    public class HolidayBuilder
    {
        private int _id;
        private string _name;
        private string _localName;
        private DateOnly _startDate;
        private DateOnly _endDate;
        private string? _description;

        public HolidayBuilder SetId(int id)
        {
            _id = id;
            return this;
        }

        public HolidayBuilder SetName(string name)
        {
            _name = name;
            return this;
        }
        
        public HolidayBuilder SetLocalName(string localName)
        {
            _localName = localName;
            return this;
        }

        public HolidayBuilder SetStartDate(DateOnly startDate)
        {
            _startDate = startDate;
            return this;
        }

        public HolidayBuilder SetEndDate(DateOnly endDate)
        {
            _endDate = endDate;
            return this;
        }

        public HolidayBuilder SetDescription(string? description)
        {
            _description = description;
            return this;
        }

        public Result<Holiday> Build()
        {
            Result validationResult = ValidateHolidayData(_name, _localName, _startDate, _endDate, _description);
        
            if (validationResult.IsFailure)
            {
                return Result.Failure<Holiday>(validationResult.Error);
            }

            _description = NormalizeDescription(_description);

            return new Holiday(_id, _localName, _name, _startDate, _endDate, _description);
        }
    }

    private static Result ValidateHolidayData(string name, string localName, DateOnly startDate, DateOnly endDate, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Holiday>(HolidayErrorMessages.InvalidName.GetDescription());
        }
        
        if (name.Length >= MaxNameLength)
        {
            return Result.Failure<Holiday>(HolidayErrorMessages.NameTooLong.GetDescription());
        }
        
        if (string.IsNullOrWhiteSpace(localName))
        {
            return Result.Failure<Holiday>(HolidayErrorMessages.InvalidLocalName.GetDescription());
        }
        
        if (localName.Length >= MaxNameLength)
        {
            return Result.Failure<Holiday>(HolidayErrorMessages.LocalNameTooLong.GetDescription());
        }

        if (startDate > endDate)
        {
            return Result.Failure<Holiday>(HolidayErrorMessages.InvalidDateRange.GetDescription());
        }

        if (description != null && description.Length >= MaxDescriptionLength)
        { 
            return Result.Failure<Holiday>(HolidayErrorMessages.DescriptionTooLong.GetDescription());
        }

        return Result.Success();
    }
    
    private static string? NormalizeDescription(string? description)
    {
        return string.IsNullOrWhiteSpace(description) ? null : description;
    }
}