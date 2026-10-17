//------------------------------------------------------------------------
//Code written by Claude AI (Anthropic, 2026).
//Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model].
//Available at: https://claude.ai [Accessed 16 October 2026].
//------------------------------------------------------------------------

//libraries needed
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Race_Dat_POE_Project.Controllers
{
    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //start of class AppControllerBase
    //my other controllers copy (inherit) from this class
    //so they can all use get_user_id without typing the code again
    public abstract class AppControllerBase : ControllerBase
    {
        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //method that gets the id of the user who is logged in
        //it reads the id from the token, NOT from what the user typed
        //so a user cannot pretend to be someone else
        protected int get_user_id()
        {
            //look for the user id inside the token
            Claim? id_claim = User.FindFirst(ClaimTypes.NameIdentifier);

            //if there is no id, return 0
            if (id_claim == null)
            {
                return 0;
            }

            //turn the text into a number and send it back
            return int.Parse(id_claim.Value);
        }
    }//end of class AppControllerBase
}
