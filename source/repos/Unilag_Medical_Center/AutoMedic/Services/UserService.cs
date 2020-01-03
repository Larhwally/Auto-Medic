using AutoMedic.Helpers;
using AutoMedic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoMedic.Services
{
    public interface IUserService
    {
        Users Authenticate(string username, string password);
        IEnumerable<Users> GetAll();
        Users GetById(int id);
        Users Create(Users user, string password);
        void Update(Users user, string password = null);
        void Delete(int id);
    }
    public class UserService : IUserService
    {
        private DataContext _context;

        public DataContext Context { get => _context; set => _context = value; }

        public UserService(DataContext context) => context = _context;

        public Users Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.Users.SingleOrDefault(x => x.Username == username);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }

        public IEnumerable<Users> GetAll()
        {
            return _context.Users;
        }

        public Users GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public Users Create(Users users, string Password)
        {   //validate
            if (string.IsNullOrWhiteSpace(Password))
                throw new AppException("Password is required");

            if (_context.Users.Any(x => x.Username == users.Username))
                throw new AppException("Username \"" + users.Username + "\" is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(Password, out passwordHash, out passwordSalt);

            users.PasswordHash = passwordHash;
            users.PasswordSalt = passwordSalt;

            _context.Users.Add(users);
            _context.SaveChanges();

            return users;
        }

        public void Update(Users userparam, string password = null)
        {
            var user = _context.Users.Find(userparam.ItbId);

            if (user == null)
                throw new AppException("User not found");
            if(userparam.Username != user.Username)
            {
                // username has changed so check if the new username is already taken
                if (_context.Users.Any(x => x.Username == userparam.Username))
                    throw new AppException("Username " + userparam.Username + " is already taken");
            }

            // update user properties
            user.Username = userparam.Username;
            user.User_type = userparam.User_type;

            //update password if it was emntered
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

            }

            _context.Users.Update(user);
            _context.SaveChanges();

        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }


        //Supportive methods
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            {
                using (var hmac = new System.Security.Cryptography.HMACSHA256())
                {
                    passwordSalt = hmac.Key;
                    passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF32.GetBytes(password));
                }
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF32.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }
            return true;
        }
    }
}
