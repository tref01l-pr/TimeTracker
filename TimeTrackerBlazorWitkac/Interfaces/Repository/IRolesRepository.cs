using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Interfaces.Repository;

public interface IRolesRepository : ICrudRepository<RoleEntity, Role, string>
{
    Task<IList<TProjectTo>> GetAllAsync<TProjectTo>();
    Task<TProjectTo?> GetByNameAsync<TProjectTo>(string name);
}