using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

#nullable disable

namespace SharedLibraries.Entities
{
    public partial class AppUser
    {
        public AppUser()
        {
            Errands = new HashSet<Issues>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public byte[] Uhash { get; set; }
        public byte[] Usalt { get; set; }
        public string DisplayName => $"{FirstName} {LastName}";

        public virtual ICollection<Issues> Errands { get; set; }


        public void GeneratePassword(string password)
        {
            using (var hmac = new HMACSHA512())
            {
                Usalt = hmac.Key;
                Uhash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public bool ValidatePassword(string password)
        {
            using (var hmac = new HMACSHA512(Usalt))
            {
                var ch = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < ch.Length; i++)
                {
                    if (ch[i] != Uhash[i])
                        return false;
                }
            }

            return true;
        }
    }
}
