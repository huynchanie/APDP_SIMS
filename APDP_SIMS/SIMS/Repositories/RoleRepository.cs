using SIMS.Data;
using SIMS.Models;

namespace SIMS.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DataContext _context;

        public RoleRepository(DataContext context)
        {
            _context = context;
        }
        public IEnumerable<Role> GetAll()
        {
            return _context.Roles.ToList();
        }

        public Role GetRoleById(int id)
        {
            return _context.Roles.FirstOrDefault(r => r.RoleId == id);
        }

        public Role GetRoleByName(string roleName)
        {
            try
            {
                return _context.Roles.FirstOrDefault(r => r.RoleName == roleName);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
