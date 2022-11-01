using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthServer.Service.Services
{
    public static class SignService
    {
        public static SecurityKey GetSymetricSecurityKey(string securityKey){
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        }

        public static SigningCredentials CreateSigningCredentials(SecurityKey securityKey){
            return new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha512Signature);
        }
    }
}
