using SIMS.Models;

namespace SIMS.Repositories
{
    public interface IRoleRepository
    {
        Role GetRoleByName(string roleName);
        IEnumerable<Role> GetAll();
        Role GetRoleById(int id);
    }
}
