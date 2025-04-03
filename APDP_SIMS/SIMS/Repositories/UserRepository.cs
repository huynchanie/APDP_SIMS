using SIMS.Data;
using SIMS.Models;

namespace SIMS.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
        }


        public int CreateUser(User user)
        {
            try
            {
                _context.Users.Add(user);
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public int DeleteUser(int id)
        {
            try
            {
                var user = _context.Users.Find(id);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    return _context.SaveChanges();
                }
                return 0;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users.ToList();
        }
        public User GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public User AuthenticateUser(string email, string password)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        public int UpdateUser(User user)
        {
          try
            {
                _context.Users.Update(user);
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return 0;
            }
        }
    }
}
