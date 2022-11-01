using AuthServer.Core.Configurations;
using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using AuthServer.Service.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configurations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AuthServer.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly CustomTokenOptions _tokenOptions;
        private IConfiguration Configuration { get; }

        public TokenService(UserManager<UserApp> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            Configuration = configuration;
            _tokenOptions = Configuration.GetSection("TokenOptions").Get<CustomTokenOptions>();
        }

        private string CreateRefreshToken()
        {
            var numberByte = new byte[64];
            using var random = RandomNumberGenerator.Create();
            random.GetBytes(numberByte);
            return Convert.ToBase64String(numberByte);
        }

        private IEnumerable<Claim> SetClaims(UserApp userApp, List<string> audiences)
        {
            //var userList = new List<Claim>
            //{
            //    new Claim(ClaimTypes.NameIdentifier,userApp.Id),
            //    new Claim(JwtRegisteredClaimNames.Email,userApp.Email),
            //    new Claim(ClaimTypes.Name,userApp.UserName),
            //    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            //};

            // You are able to use old version of this operation
            // Extension method has been written for reason of open-closed principle of SOLID
            List<Claim> claims = new();
            claims.AddEmail(userApp.Email);
            claims.AddNameIdentifier(userApp.Id);
            claims.AddName(userApp.UserName);
            claims.GenerateRandomJti();
            claims.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            return claims;
        }

        private IEnumerable<Claim> GetClaimsByClient(Client client)
        {
            var claims = new List<Claim>{
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub,client.Id.ToString())
             };
            claims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            return claims;
        }

        private JwtSecurityToken CreateJwtSecurityToken(SecurityKey securityKey, DateTime accessTokenExpiration,
                                                        SigningCredentials signingCredentials, UserApp user)
        {
            JwtSecurityToken jwtSecurityToken = new (
                issuer: _tokenOptions.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: SetClaims(user, _tokenOptions.Audience),
                signingCredentials: signingCredentials
            );
            return jwtSecurityToken;

        }
        public TokenDto CreateToken(UserApp user)
        {
            //Token expiration durations has been created at here
            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);
            var refreshTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.RefreshTokenExpiration);

            var securityKey = SignService.GetSymetricSecurityKey(_tokenOptions.SecurityKey);
            var signingCredentials = SignService.CreateSigningCredentials(securityKey);
            var jwtSecurityToken = CreateJwtSecurityToken(securityKey, accessTokenExpiration, signingCredentials, user);
            var handler = new JwtSecurityTokenHandler();
            var token = handler.WriteToken(jwtSecurityToken);

            return new TokenDto
            {
                AccessToken = token,
                AccessTokenExpiration = accessTokenExpiration,
                RefreshToken = CreateRefreshToken(),
                RefreshTokenExpiration = refreshTokenExpiration
            };
        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);
            var securityKey = SignService.GetSymetricSecurityKey(_tokenOptions.SecurityKey);
            SigningCredentials signingCredentials = SignService.CreateSigningCredentials(securityKey);

            JwtSecurityToken jwtSecurityToken = new (
                issuer: _tokenOptions.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: GetClaimsByClient(client),
                signingCredentials: signingCredentials
            );

            var handler = new JwtSecurityTokenHandler();
            var token = handler.WriteToken(jwtSecurityToken);
            return new ClientTokenDto
            {
                AccessToken = token,
                AccessTokenExpiration = accessTokenExpiration
            };
        }
    }
}
