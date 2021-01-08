using System.Text;

namespace NetREST.Common.Settings
{
    public class ApiSettings
    {
        public string PublicKey { get; set; }
        public int AccessTokenExpiresInMinutes { get; set; }
        public int RefreshTokenExpiresInMinutes { get; set; }
        public string ISSUER { get; set; }
        public string AUDIENCE { get; set; }
        public byte[] PublicKeyBytes => Encoding.UTF8.GetBytes($"{PublicKey}");
    }
}