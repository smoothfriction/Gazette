using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Raven.Client;

namespace Gazette.Controllers
{
    public interface IAuthenticationService
    {
        bool ValidateUser(string username, string password);
    }

    class AuthenticationService : IAuthenticationService
    {
        private readonly IDocumentSession _session;

        public AuthenticationService(IDocumentSession session)
        {
            _session = session;
        }

        public bool ValidateUser(string username, string password)
        {
            var user = _session.Query<GazetteUser>().SingleOrDefault(x => x.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase));

            if(user == null || !user.IsPasswordCorrect(password))
                throw new UnauthorizedAccessException();

            return true;
        }
    }

    internal class GazetteUser
    {
        /* Based on user in RaccoonBlog */
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public string PasswordHash { get; set; }
        
        public string PasswordSalt
        {
            get { return _passwordSalt ?? Guid.NewGuid().ToString("N"); }
            set { _passwordSalt = value; }
        }
        private string _passwordSalt;

        public static GazetteUser CreateUser(string username, string password)
        {
            var salt = Guid.NewGuid().ToString("N");
            return new GazetteUser
                {
                    Username = username,
                    PasswordSalt = salt,
                    PasswordHash = CreatePasswordHash(password, salt)
                };
        }

        private static string CreatePasswordHash(string password, string salt)
        {
            using(var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(Encoding.Unicode.GetBytes(salt + password));
                return Convert.ToBase64String(hash);
            }
        }

        public bool IsPasswordCorrect(string password)
        {
            return CreatePasswordHash(password, PasswordSalt) == PasswordHash;
        }
    }
}