using System.Text.Json.Serialization;

namespace WebAPI.Models.Token
{
    public class AuthenticateResponse
    {
        public AccountInfo AccountInfo { get; set; }

        public string AccessToken{ get; set; }

        //[JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public AuthenticateResponse(AccountInfo user, string jwtToken, string refreshToken)
        {
            AccountInfo = user;
            AccessToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}
