using Project2.DTOs;

namespace Project2.Interfaces
{
    public interface IAuthorizeService
    {
        string TestAPI();
        bool AuthenticateUser(string pUserName, string pPassword);
        AuthenticationModel CreateToken(string userName);
    }
}
