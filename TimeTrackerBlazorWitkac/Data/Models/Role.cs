using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Models;
using TimeTrackerBlazorWitkac.Helpers;
using TimeTrackerBlazorWitkac.Interfaces;

namespace TimeTrackerBlazorWitkac.Data.Models;

public record Role : IModelKey<string>
{
    private Role(string id, string name)
    {
        Id = id;
        Name = name;
    }
    
    public string Id { get; init; }
    public string Name { get; }

    public static RoleBuilder Builder() => new RoleBuilder();
    
    public class RoleBuilder
    {
        private string _id;
        private string _name;

        public RoleBuilder SetId(string id)
        {
            _id = id;
            return this;
        }

        public RoleBuilder SetName(string name)
        {
            _name = name;
            return this;
        }
        
        public Result<Role> Build()
        {
            var validation = ValidateRoleData();

            if (validation.IsFailure)
            {
                return Result.Failure<Role>(validation.Error);
            }

            return Result.Success(new Role(_id, _name));
        }

        private Result ValidateRoleData()
        {
            if (string.IsNullOrWhiteSpace(_name))
            {
                return Result.Failure(RoleErrorMessages.InvalidName.GetDescription());
            }

            if (string.IsNullOrWhiteSpace(_id))
            {
                return Result.Failure(RoleErrorMessages.InvalidId.GetDescription());
            }

            return Result.Success();
        }
    }
}
