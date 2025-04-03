using SIMS.Models;

namespace SIMS.Repositories
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAll();
        User GetUserByEmail(string email);
        int CreateUser(User user);
        int UpdateUser(User user);
        int DeleteUser(int id);
        User AuthenticateUser(string email, string password);
    }
}
