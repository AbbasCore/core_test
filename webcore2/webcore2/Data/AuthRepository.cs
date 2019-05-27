using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webcore2.Models;

namespace webcore2.Data
{
    public class AuthRepository : IAuthRepository
    {
        public readonly DataContext _context;

        public AuthRepository (DataContext context)
        {
            _context = context;
        }


        public async Task<User> Login(string username, string password)
        {
            var user = await _context.users.FirstOrDefaultAsync(x => x.userName == username);
            if (user == null) return null;
            if (!VerifyPassword(password, user.passwordSalt, user.passwordHash)) return null;
            return user;
        }


        private bool VerifyPassword(string password, byte[] passwordSalt, byte[] passwordHash)
        {
            using (var hmac=new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var com = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i=0;i<com.Length;i++)
                {
                    if (com[i] != passwordHash[i]) return false;
                }
                return true;
            }
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.passwordSalt = passwordSalt;
            user.passwordHash = passwordHash;
            await _context.users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
        }

        public async Task<bool> UserExists(string userName)
        {
            if (await _context.users.AnyAsync(x => x.userName == userName)) return true;
            return false;
        }
    }
}
