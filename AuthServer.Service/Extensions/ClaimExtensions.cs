using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthServer.Service.Extensions
{
    public static class ClaimExtensions
    {
        public static void AddEmail(this ICollection<Claim> claims,string email)
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Email,email));
        }
        
        public static void AddName(this ICollection<Claim> claims,string name)
        {
            claims.Add(new Claim(ClaimTypes.Name,name));
        }
        public static void AddNameIdentifier(this ICollection<Claim> claims,string userId)
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier,userId));
        }
        public static void GenerateRandomJti(this ICollection<Claim> claims)
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti,new Guid().ToString()));
        }
    }
}
