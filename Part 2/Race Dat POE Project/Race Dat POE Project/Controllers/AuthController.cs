//------------------------------------------------------------------------
//Code written by Claude AI (Anthropic, 2026).
//Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model].
//Available at: https://claude.ai [Accessed 16 October 2026].
//------------------------------------------------------------------------

//libraries needed
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Race_Dat_POE_Project.Data;
using Race_Dat_POE_Project.Models;
using Race_Dat_POE_Project.Services;

namespace Race_Dat_POE_Project.Controllers
{
    //Code written by Claude AI (Anthropic, 2026).
    //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
    //this controller handles register and login
    [ApiController]
    [Route("api/auth")]
    //AllowAnonymous means anyone can use these two endpoints (they are not logged in yet)
    //every other controller needs a login token
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {//start of class AuthController

        //variables to store the classes this controller uses
        private IUserRepository users;
        private TokenService token_service;

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //constructor, the api gives us the repository and the TokenService
        public AuthController(IUserRepository user_repository, TokenService tokens)
        {
            users = user_repository;
            token_service = tokens;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        /// <summary>
        /// Register a new user as a Participant or an Organiser.
        /// </summary>
        /// <remarks>
        /// Anyone can use this endpoint (no login needed).
        /// Data expected (JSON body): FullName, Email, Password (at least 8 characters) and Role (Participant or Organiser).
        /// The password is hashed with BCrypt before it is saved.
        /// </remarks>
        /// <param name="request">The registration details.</param>
        /// <response code="200">The user was registered.</response>
        /// <response code="400">The details are not valid (for example a short password or a bad email).</response>
        /// <response code="409">That email is already registered.</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(string), 409)]
        public IActionResult register_user(RegisterRequest request)
        {//start of method register
            try
            {//start of try

                //make the email lowercase and remove spaces so emails are always saved the same way
                string email = request.Email.Trim().ToLower();

                //check if this email is already registered
                if (users.email_exists(email))
                {
                    return Conflict("That email is already registered.");
                }

                //the user typed one full name, but my table has first_name and last_name
                //so I split the name at the first space
                string full_name = request.FullName.Trim();
                string first_name = full_name;
                string last_name = "";
                int space_position = full_name.IndexOf(' ');

                //if there is a space, the text before it is the first name and the rest is the last name
                if (space_position > 0)
                {
                    first_name = full_name.Substring(0, space_position);
                    last_name = full_name.Substring(space_position + 1).Trim();
                }

                //hash the password so the real password is never saved in the database
                string hashed_password = BCrypt.Net.BCrypt.HashPassword(request.Password);

                //save the user
                bool saved = users.create_user(email, hashed_password, first_name, last_name, request.Role.ToString());

                //false means the role name was not found in the Role table
                if (saved == false)
                {
                    return BadRequest("Invalid role. Check the Role table has Organiser and Participant.");
                }

                return Ok("User registered successfully.");

            }//end of try
            catch (Exception error)
            {//start of catch
                return StatusCode(500, error.Message);
            }//end of catch
        }//end of method register

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        /// <summary>
        /// Log in and get a token.
        /// </summary>
        /// <remarks>
        /// Anyone can use this endpoint (no login needed).
        /// Data expected (JSON body): Email and Password.
        /// Copy the token from the response, click the Authorize button at the top of this page and paste it in.
        /// </remarks>
        /// <param name="request">The email and password.</param>
        /// <response code="200">Login worked. Returns the token, the role and how many minutes the token lasts.</response>
        /// <response code="401">The email or password is wrong.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        [ProducesResponseType(typeof(string), 401)]
        public IActionResult login_user(LoginRequest request)
        {//start of method login
            try
            {//start of try

                //make the email lowercase and remove spaces
                string email = request.Email.Trim().ToLower();

                //find the user in the database
                UserLogin? login = users.find_login(email);

                //I use the same message for a wrong email and a wrong password
                //so nobody can find out which emails are registered
                if (login == null)
                {
                    return Unauthorized("Invalid email or password.");
                }

                //Verify checks the typed password against the saved hash
                bool password_ok = BCrypt.Net.BCrypt.Verify(request.Password, login.PasswordHash);

                if (password_ok == false)
                {
                    return Unauthorized("Invalid email or password.");
                }

                //the password is right, so create the token
                string token = token_service.create_token(login.UserId, email, login.RoleName);

                //put the answer in a LoginResponse object and send it back
                LoginResponse response = new LoginResponse();
                response.Token = token;
                response.Role = login.RoleName;
                response.ExpiresInMinutes = TokenService.ExpiryMinutes;
                return Ok(response);

            }//end of try
            catch (Exception error)
            {//start of catch
                return StatusCode(500, error.Message);
            }//end of catch
        }//end of method login

    }//end of class AuthController
}
