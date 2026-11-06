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

namespace Race_Dat_POE_Project.Controllers
{
    //Code written by Claude AI (Anthropic, 2026).
    //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
    //this controller lets a logged in user view and update their OWN profile
    [ApiController]
    [Route("api/profile")]
    //Authorize means the user must be logged in (both roles are allowed)
    [Authorize]
    public class ProfileController : AppControllerBase
    {//start of class ProfileController

        //variable to store the repository
        private IUserRepository users;

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //constructor
        public ProfileController(IUserRepository user_repository)
        {
            users = user_repository;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        /// <summary>
        /// View my own profile.
        /// </summary>
        /// <remarks>
        /// Both roles can use this. No data needs to be sent.
        /// The user is found from the login token, so nobody can see another person's profile.
        /// </remarks>
        /// <response code="200">Returns the profile of the logged in user.</response>
        /// <response code="401">The user is not logged in.</response>
        /// <response code="404">The user was not found.</response>
        [HttpGet]
        [ProducesResponseType(typeof(ProfileInfo), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(string), 404)]
        public IActionResult get_profile()
        {//start of method get_profile
            try
            {//start of try

                //get the id of the logged in user from the token
                int user_id = get_user_id();

                ProfileInfo? profile = users.get_profile(user_id);

                if (profile == null)
                {
                    return NotFound("User not found.");
                }

                return Ok(profile);

            }//end of try
            catch (Exception error)
            {//start of catch
                return StatusCode(500, error.Message);
            }//end of catch
        }//end of method get_profile

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        /// <summary>
        /// Update my own profile.
        /// </summary>
        /// <remarks>
        /// Both roles can use this.
        /// Data expected (JSON body): FirstName (required), LastName and PhoneNumber.
        /// Only the logged in user's profile is changed.
        /// </remarks>
        /// <param name="request">The new profile details.</param>
        /// <response code="200">The profile was updated.</response>
        /// <response code="400">The details are not valid.</response>
        /// <response code="401">The user is not logged in.</response>
        /// <response code="404">The user was not found.</response>
        [HttpPut]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(string), 404)]
        public IActionResult update_profile(UpdateProfileRequest request)
        {//start of method update_profile
            try
            {//start of try

                //get the id of the logged in user from the token
                int user_id = get_user_id();

                //if the user left these out, save empty text instead of nothing
                if (request.LastName == null)
                {
                    request.LastName = "";
                }
                if (request.PhoneNumber == null)
                {
                    request.PhoneNumber = "";
                }

                bool updated = users.update_profile(user_id, request);

                if (updated == false)
                {
                    return NotFound("User not found.");
                }

                return Ok("Profile updated.");

            }//end of try
            catch (Exception error)
            {//start of catch
                return StatusCode(500, error.Message);
            }//end of catch
        }//end of method update_profile

    }//end of class ProfileController
}
