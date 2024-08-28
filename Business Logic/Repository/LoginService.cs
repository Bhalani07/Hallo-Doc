using Business_Logic.Interface;
using Data_Access.Custom_Models;
using Data_Access.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Repository
{
    public class LoginService : ILoginService
    {
        private readonly ApplicationDbContext _context;

        public LoginService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool IsLoginValid(loginCm loginCm) 
        {
            var aspData = _context.Aspnetusers.FirstOrDefault(f => f.Email == loginCm.Email);

            if (aspData!= null)
            {
                var password = aspData.Passwordhash;
                bool verified = BCrypt.Net.BCrypt.Verify(loginCm.Passwordhash, password);

                return _context.Aspnetusers.Any(mh => mh.Email == loginCm.Email && verified);
            }

            return false;
        }

        public Aspnetuser Login(loginCm loginCm)
        {
            var password = _context.Aspnetusers.FirstOrDefault(f => f.Email == loginCm.Email).Passwordhash;
            bool verified = BCrypt.Net.BCrypt.Verify(loginCm.Passwordhash, password);

            Aspnetuser aspnetuser = _context.Aspnetusers.FirstOrDefault(i => i.Email == loginCm.Email && verified);

            return aspnetuser;
        }

        public Aspnetuser CheckAspUser(string email)
        {
            Aspnetuser? aspnetuser = _context.Aspnetusers.FirstOrDefault(mh => mh.Email == email);

            return aspnetuser;
        }

        public bool CheckBlockedUser(string email, string phone)
        {
            if(email != "")
            {
                return _context.Blockrequests.Any(x => x.Email == email && x.Isactive == new BitArray(1, true));
            }

            return _context.Blockrequests.Any(x => x.Phonenumber == phone && x.Isactive == new BitArray(1, true));
        }

        public string Encrypt(string clearText)
        {
            string EncryptionKey = "abc123";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "abc123";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }


    }
}
