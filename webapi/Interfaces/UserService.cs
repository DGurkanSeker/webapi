using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using backendProjesi.Models;

namespace webapi.Interfaces
{
    public class UserService : IUser
    {
        private readonly UsersContext _context;

        public UserService(UsersContext context)
        {
            _context = context;
        }
        public static string HashPassword(string password)
        {
            //rastgele salt oluştur
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt); // rastgele doldur
            }
            //salt+hash(password+salt+iterasyonsayısı+hashalgoritması)
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32); 
                byte[] hashBytes = new byte[48]; 
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 32);

                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {

            byte[] hashBytes = Convert.FromBase64String(storedHash);

            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);


            using (var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 10000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32); 

                for (int i = 0; i < 32; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        public async Task<User> CreateUserAsync(User user)
        {
            user.Id = Guid.NewGuid();
            user.Password = HashPassword(user.Password); 
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }


        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }


        public async Task<List<User>> GetUserByDateAsync(DateTime date)
        {
            return await _context.Users
                .Where(u => u.CreationTime.Date == date.Date)
                .OrderByDescending(u => u.CreationTime)
                .ToListAsync();
        }


        public async Task<User> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task UpdateUserAsync(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser != null)
            {
                if (existingUser.Password != user.Password)
                {
                    user.Password = HashPassword(user.Password); 
                }

                _context.Entry(existingUser).CurrentValues.SetValues(user);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<bool> VerifyUserAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                return VerifyPassword(password, user.Password);
            }

            return false;
        }
    }
}
