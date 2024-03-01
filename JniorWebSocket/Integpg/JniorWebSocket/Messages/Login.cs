using System;
using System.Security.Cryptography;
using System.Text;

namespace Integpg.JniorWebSocket.Messages
{
    public class Login: JniorMessage
    {
        public Login(string username, string password, string nonce)
        {
            var md5 = new MD5CryptoServiceProvider();
            var hash = md5.ComputeHash(Encoding.ASCII.GetBytes(username + ":" + nonce + ":" + password));
            var digest = BitConverter.ToString(hash).Replace("-", "");
            digest = username + ":" + digest;

            this["Auth-Digest"] = digest;
        }
    }
}
