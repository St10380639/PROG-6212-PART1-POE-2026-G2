//------------------------------------------------------------------------
//Code written by Claude AI (Anthropic, 2026).
//Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model].
//Available at: https://claude.ai [Accessed 16 October 2026].
//------------------------------------------------------------------------

//libraries needed to make the login token
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Race_Dat_POE_Project.Services
{
    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //start of class TokenService
    //this class creates the token the user gets after they login
    //the token proves who the user is and what their role is
    public class TokenService
    {
        //how long the token works for (in minutes)
        public const int ExpiryMinutes = 120;

        //variable to store the settings from appsettings.json
        private IConfiguration config;

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //constructor, it runs when the class is created
        public TokenService(IConfiguration configuration)
        {
            config = configuration;
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //method that creates the token
        public string create_token(int user_id, string email, string role)
        {
            //get the secret key from appsettings.json
            string secret = config["Jwt:Key"] ?? "";

            //turn the secret text into a security key
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            //credentials are used to sign the token so nobody can change it
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //claims are the pieces of information stored inside the token
            List<Claim> claims = new List<Claim>();

            //store the user id
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user_id.ToString()));

            //store the email
            claims.Add(new Claim(ClaimTypes.Email, email));

            //store the role (Organiser or Participant)
            claims.Add(new Claim(ClaimTypes.Role, role));

            //create the token with the claims and the expiry time
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(ExpiryMinutes),
                signingCredentials: credentials);

            //turn the token into text and send it back
            string token_text = new JwtSecurityTokenHandler().WriteToken(token);
            return token_text;
        }
    }//end of class TokenService
}
