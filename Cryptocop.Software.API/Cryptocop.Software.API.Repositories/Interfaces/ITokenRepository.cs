using Cryptocop.Software.API.Models.Dtos;

namespace Cryptocop.Software.API.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        JwtTokenDto CreateNewToken();
        bool IsTokenBlacklisted(int tokenId);
        void VoidToken(int tokenId);
    }
}