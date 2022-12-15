using AuthServer.Core.Configurations;
using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SharedLibrary.Dtos;

namespace AuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private IConfiguration Configuration { get; }
        private readonly List<Client> _clients;
        private readonly ITokenService _tokenService;
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenService;

        public AuthenticationService(ITokenService tokenService, UserManager<UserApp> userManager,
                                     IUnitOfWork unitOfWork, IConfiguration configuration,
                                     IGenericRepository<UserRefreshToken> userRefreshTokenService)
        {
            Configuration = configuration;
            _clients = Configuration.GetSection("Clients").Get<List<Client>>();
            _tokenService = tokenService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userRefreshTokenService = userRefreshTokenService;
        }

        public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            if (loginDto == null) throw new ArgumentNullException(nameof(loginDto));

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) return Response<TokenDto>.Fail(400, "Email or Password is not correct!", true);

            var checkUserPassword = await _userManager.CheckPasswordAsync(user, password: loginDto.Password);
            if (!checkUserPassword) return Response<TokenDto>.Fail(400, "Email or Password is not correct!", true);

            var token = _tokenService.CreateToken(user);
            var refreshToken = await _userRefreshTokenService.Where(x => x.UserId == user.Id).SingleOrDefaultAsync();
            if (refreshToken == null)
            {
                await _userRefreshTokenService.AddAsync(new UserRefreshToken
                {
                    UserId = user.Id,
                    Code = token.RefreshToken,
                    Expiration = token.RefreshTokenExpiration
                });
            }
            else
            {
                refreshToken.Code = token.RefreshToken;
                refreshToken.Expiration = token.RefreshTokenExpiration;
            }
            await _unitOfWork.CommitAsync();

            return Response<TokenDto>.Success(token, 200);

        }

        public Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            var client = _clients.SingleOrDefault(x => x.Id == clientLoginDto.Id && x.Secret == clientLoginDto.Secret);
            if (client == null) return Response<ClientTokenDto>.Fail(404, "Client Id or Secret not Found!", true);

            var token = _tokenService.CreateTokenByClient(client);
            return Response<ClientTokenDto>.Success(token, 200);
        }

        public async Task<Response<TokenDto>> CreateTokenByRefreshTokenAsync(string refreshToken)
        {
            var exitsRefreshToken = await _userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
            if (exitsRefreshToken == null) return Response<TokenDto>.Fail(404, "Refresh Token not Found!", true);

            var user = await _userManager.FindByIdAsync(exitsRefreshToken.UserId);
            if (user == null) return Response<TokenDto>.Fail(404, "User Id not Found!", true);

            var tokenDto = _tokenService.CreateToken(user);
            exitsRefreshToken.Code = tokenDto.RefreshToken;
            exitsRefreshToken.Expiration = tokenDto.RefreshTokenExpiration;

            await _unitOfWork.CommitAsync();
            return Response<TokenDto>.Success(tokenDto, 200);
        }

        public async Task<Response<NoContentDto>> RevokeRefreshTokenAsync(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
            if (existRefreshToken == null) return Response<NoContentDto>.Fail(404, "Refresh Token not Found!", true);

            _userRefreshTokenService.Remove(existRefreshToken);
            await _unitOfWork.CommitAsync();
            return Response<NoContentDto>.Success(200); 

        }
    }
}
