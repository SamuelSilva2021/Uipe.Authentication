using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Authentication.Infrastructure.Configuration
{
    public class SigningConfigurations
    {
        public static SecurityKey Key { get; set; }
        public static SigningCredentials SigningCredentials { get; set; }

        static SigningConfigurations()
        {
            using (RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(2048))
            {
                Key = new RsaSecurityKey(rSACryptoServiceProvider.ExportParameters(includePrivateParameters: true));
            }

            SigningCredentials = new SigningCredentials(Key, "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");
        }
    }
}
