using AuthServer.Core.Configurations;
using AuthServer.Core.Dtos;
using AuthServer.Core.Models;

namespace AuthServer.Core.Services
{
    public interface ITokenService
    {
        TokenDto CreateToken(UserApp user);
        ClientTokenDto CreateTokenByClient(Client client);
    }
}
