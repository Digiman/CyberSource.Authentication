using CyberSource.Authentication.Core;

namespace CyberSource.Authentication.Interfaces
{
    public interface ITokenGenerator
    {
        Token GetToken();
    }
}
