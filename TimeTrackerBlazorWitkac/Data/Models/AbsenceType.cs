using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Interfaces;
using TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Models;
using TimeTrackerBlazorWitkac.Helpers;
using BlazorBootstrap;
using TimeTrackerBlazorWitkac.Resources;

namespace TimeTrackerBlazorWitkac.Data.Models;

// The AbsenceType record class represents the different types of absences available in the system.
// It provides structured data and validation for absence types.
public record AbsenceType : IModelKey<int>
{
    // Maximum length allowed for the name of an absence type.
    public const int MaxNameLength = 128;

    // Maximum length allowed for the icon of an absence type.
    public const int MaxIconLength = 64;

    // Maximum length allowed for the icon of an absence type.
    public const int MaxColorLength = 50;

    // Maximum length allowed for the description of an absence type.
    public const int MaxDescriptionLength = 360;

    public const string DefaultPendingIconColor = Constants.DefaultPendingIconColor;
    public const string DefaultPendingBackgroundColor = Constants.DefaultPendingBackgroundColor;

    public const string DefaultAcceptedIconColor = Constants.DefaultAcceptedIconColor;
    public const string DefaultAcceptedBackgroundColor = Constants.DefaultAcceptedBackgroundColor;

    // Private constructor used to enforce the use of the Create method for instantiation.
    private AbsenceType(int id, string name, IconName? icon, string pendingIconColor, string pendingBackgroundColor,
        string acceptedIconColor, string acceptedBackgroundColor, string? description)
    {
        Id = id;
        Name = name;
        Icon = icon;
        PendingIconColor = pendingIconColor;
        PendingBackgroundColor = pendingBackgroundColor;
        AcceptedIconColor = acceptedIconColor;
        AcceptedBackgroundColor = acceptedBackgroundColor;
        Description = description;
    }

    public int Id { get; init; }
    public string Name { get; }
    public IconName? Icon { get; }
    public string PendingIconColor { get; }
    public string PendingBackgroundColor { get; }
    public string AcceptedIconColor { get; }
    public string AcceptedBackgroundColor { get; }
    public string? Description { get; }
    public static AbsenceTypeBuilder Builder() => new AbsenceTypeBuilder();

    public class AbsenceTypeBuilder
    {
        private int _id;
        private string _name = "";
        private IconName? _icon;
        private string _pendingIconColor = DefaultPendingIconColor;
        private string _pendingBackgroundColor = DefaultPendingBackgroundColor;
        private string _acceptedIconColor = DefaultAcceptedIconColor;
        private string _acceptedBackgroundColor = DefaultAcceptedBackgroundColor;
        private string? _description;

        public AbsenceTypeBuilder SetId(int id)
        {
            _id = id;
            return this;
        }

        public AbsenceTypeBuilder SetName(string name)
        {
            _name = name;
            return this;
        }

        public AbsenceTypeBuilder SetIcon(IconName? icon)
        {
            _icon = icon;
            return this;
        }

        public AbsenceTypeBuilder SetPendingIconColor(string pendingIconColor)
        {
            _pendingIconColor = pendingIconColor;
            return this;
        }

        public AbsenceTypeBuilder SetPendingBackgroundColor(string pendingBackgroundColor)
        {
            _pendingBackgroundColor = pendingBackgroundColor;
            return this;
        }

        public AbsenceTypeBuilder SetAcceptedIconColor(string acceptedIconColor)
        {
            _acceptedIconColor = acceptedIconColor;
            return this;
        }

        public AbsenceTypeBuilder SetAcceptedBackgroundColor(string acceptedBackgroundColor)
        {
            _acceptedBackgroundColor = acceptedBackgroundColor;
            return this;
        }

        public AbsenceTypeBuilder SetDescription(string? description)
        {
            _description = description;
            return this;
        }

        public Result<AbsenceType> Build()
        {
            var validationResult = ValidateAbsenceTypeData(_name, _icon, _pendingIconColor,
                _pendingBackgroundColor,
                _acceptedIconColor, _acceptedBackgroundColor, _description);

            if (validationResult.IsFailure)
            {
                return Result.Failure<AbsenceType>(validationResult.Error);
            }

            return new AbsenceType(
                _id,
                _name,
                _icon,
                _pendingIconColor,
                _pendingBackgroundColor,
                _acceptedIconColor,
                _acceptedBackgroundColor,
                _description);
        }

        private Result ValidateAbsenceTypeData(string name, IconName? icon, string pendingIconColor,
            string pendingBackgroundColor, string acceptedIconColor, string acceptedBackgroundColor,
            string? description)
        {
            var stringValidation = ValidateStrings(name, description);
            if (stringValidation.IsFailure)
            {
                return stringValidation;
            }

            var iconValidation = ValidateIcon(icon);
            if (iconValidation.IsFailure)
            {
                return iconValidation;
            }

            var colorValidation = ValidateColors(pendingIconColor, pendingBackgroundColor, acceptedIconColor,
                acceptedBackgroundColor);

            if (colorValidation.IsFailure)
            {
                return colorValidation;
            }

            return Result.Success();
        }

        private Result ValidateIcon(IconName? icon)
        {
            if (icon == null)
                return Result.Success();
            
            if (!Enum.IsDefined(typeof(IconName), icon.Value))
            {
                return Result.Failure(AbsenceTypeErrorMessages.IconIsInvalid.GetDescription());
            }
            
            return Result.Success();
        }

        private Result ValidateStrings(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Result.Failure(AbsenceTypeErrorMessages.NameIsNullOrWhiteSpace.GetDescription());
            }

            if (name.Length >= MaxNameLength)
            {
                return Result.Failure(AbsenceTypeErrorMessages.NameTooLong.GetDescription());
            }

            if (description != null)
            {
                if (string.IsNullOrWhiteSpace(description))
                {
                    return Result.Failure(AbsenceTypeErrorMessages.DescriptionIsNullOrWhiteSpace.GetDescription());
                }

                if (description.Length >= MaxDescriptionLength)
                {
                    return Result.Failure(AbsenceTypeErrorMessages.DescriptionTooLong.GetDescription());
                }
            }

            return Result.Success();
        }

        private Result ValidateColors(params string[] colors)
        {
            foreach (var color in colors)
            {
                var validationResult = ValidateColor(color);
                if (validationResult.IsFailure)
                {
                    return validationResult;
                }
            }

            return Result.Success();
        }

        private Result ValidateColor(string color)
        {
            if (string.IsNullOrWhiteSpace(color))
            {
                return Result.Failure(AbsenceTypeErrorMessages.ColorIsNullOrWhiteSpace.GetDescription());
            }

            if (color.Length >= MaxColorLength)
            {
                return Result.Failure(AbsenceTypeErrorMessages.ColorTooLong.GetDescription());
            }

            if (!HexColorValidator.IsValidHexColor(color) && !HexColorValidator.IsValidRgbaColor(color))
            {
                return Result.Failure(AbsenceTypeErrorMessages.InvalidColorFormat.GetDescription());
            }

            return Result.Success();
        }
    }
}