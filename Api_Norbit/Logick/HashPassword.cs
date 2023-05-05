using System.Security.Cryptography;
using System.Text;

namespace Api_Norbit.Logick
{
    /// <summary>
    /// Хэширование пароля.
    /// </summary>
    public class HashPassword
    {
        /// <summary>
        /// Захэшированный пароль.
        /// </summary>
        public string hashPassword;

        /// <summary>
        /// Конструтор принимающий пароль и сразу хэширующий.
        /// </summary>
        /// <param name="password">Пароль который будет хэшироваться.</param>
        public HashPassword(string password)
        {
            MD5 md5 = MD5.Create();
            byte[] b = Encoding.ASCII.GetBytes(password);
            byte[] hash = md5.ComputeHash(b);
            StringBuilder hashPassword = new StringBuilder();
            foreach (var e in hash)
                hashPassword.Append(e.ToString("X2"));
            this.hashPassword = Convert.ToString(hashPassword);
        }
    }
}
