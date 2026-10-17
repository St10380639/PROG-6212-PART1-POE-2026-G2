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
    //this controller handles categories (for example Under 20, Senior, 10km, 21km)
    //everyone logged in can VIEW categories, only organisers can add, change and delete them
    [ApiController]
    [Route("api")]
    [Authorize]
    public class CategoriesController : AppControllerBase
    {//start of class CategoriesController

        //variables to store the repositories
        private ICategoryRepository categories;
        private IEventRepository events;

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //constructor
        public CategoriesController(ICategoryRepository category_repository, IEventRepository event_repository)
        {
            categories = category_repository;
            events = event_repository;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        /// <summary>
        /// View the categories of one event.
        /// </summary>
        /// <remarks>
        /// Both roles can use this. Data expected: the event id in the address.
        /// </remarks>
        /// <param name="eventId">The id of the event.</param>
        /// <response code="200">Returns the list of categories of the event.</response>
        /// <response code="401">The user is not logged in.</response>
        [HttpGet("events/{eventId:int}/categories")]
        [ProducesResponseType(typeof(List<CategoryInfo>), 200)]
        [ProducesResponseType(401)]
        public IActionResult get_categories(int eventId)
        {//start of method get_categories
            try
            {//start of try
                List<CategoryInfo> list = categories.get_categories(eventId);
                return Ok(list);
            }//end of try
            catch (Exception error)
            {//start of catch
                return StatusCode(500, error.Message);
            }//end of catch
        }//end of method get_categories

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        /// <summary>
        /// Add a category to an event (Organisers only, their own events).
        /// </summary>
        /// <remarks>
        /// Data expected: the event id in the address and in the JSON body CategoryName (for example Under 20 or 10km) and CategoryType (Age or Distance).
        /// </remarks>
        /// <param name="eventId">The id of the event.</param>
        /// <param name="request">The category details.</param>
        /// <response code="201">The category was created. Returns a message and the new category id.</response>
        /// <response code="400">The details are not valid.</response>
        /// <response code="401">The user is not logged in.</response>
        /// <response code="403">The user is a Participant, or the event belongs to another organiser.</response>
        /// <response code="404">The event was not found.</response>
        [HttpPost("events/{eventId:int}/categories")]
        [Authorize(Roles = Roles.Organiser)]
        [ProducesResponseType(typeof(CreatedResponse), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(string), 404)]
        public IActionResult create_category(int eventId, CategoryRequest request)
        {//start of method create_category
            try
            {//start of try

                //find out which organiser owns the event
                int owner_id = events.get_event_owner(eventId);

                if (owner_id == 0)
                {
                    return NotFound("Event not found.");
                }

                //an organiser may only add categories to their OWN events
                if (owner_id != get_user_id())
                {
                    return StatusCode(403, "You can only add categories to your own events.");
                }

                int new_id = categories.create_category(eventId, request);

                CreatedResponse response = new CreatedResponse();
                response.Message = "Category created.";
                response.Id = new_id;
                return StatusCode(201, response);

            }//end of try
            catch (Exception error)
            {//start of catch
                return StatusCode(500, error.Message);
            }//end of catch
        }//end of method create_category

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        /// <summary>
        /// Change a category (Organisers only, their own events).
        /// </summary>
        /// <remarks>
        /// Data expected: the category id in the address and the new CategoryName and CategoryType in the JSON body.
        /// </remarks>
        /// <param name="id">The id of the category.</param>
        /// <param name="request">The new category details.</param>
        /// <response code="200">The category was updated.</response>
        /// <response code="401">The user is not logged in.</response>
        /// <response code="403">The user is a Participant, or the category belongs to another organiser's event.</response>
        /// <response code="404">The category was not found.</response>
        [HttpPut("categories/{id:int}")]
        [Authorize(Roles = Roles.Organiser)]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(string), 404)]
        public IActionResult update_category(int id, CategoryRequest request)
        {//start of method update_category
            try
            {//start of try

                int owner_id = categories.get_category_owner(id);

                if (owner_id == 0)
                {
                    return NotFound("Category not found.");
                }

                if (owner_id != get_user_id())
                {
                    return StatusCode(403, "You can only change categories of your own events.");
                }

                categories.update_category(id, request);
                return Ok("Category updated.");

            }//end of try
            catch (Exception error)
            {//start of catch
                return StatusCode(500, error.Message);
            }//end of catch
        }//end of method update_category

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        /// <summary>
        /// Delete a category (Organisers only, their own events).
        /// </summary>
        /// <remarks>
        /// Data expected: the category id in the address.
        /// A category that already has enrolments cannot be deleted.
        /// </remarks>
        /// <param name="id">The id of the category.</param>
        /// <response code="200">The category was deleted.</response>
        /// <response code="401">The user is not logged in.</response>
        /// <response code="403">The user is a Participant, or the category belongs to another organiser's event.</response>
        /// <response code="404">The category was not found.</response>
        /// <response code="409">The category has enrolments.</response>
        [HttpDelete("categories/{id:int}")]
        [Authorize(Roles = Roles.Organiser)]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 409)]
        public IActionResult delete_category(int id)
        {//start of method delete_category
            try
            {//start of try

                int owner_id = categories.get_category_owner(id);

                if (owner_id == 0)
                {
                    return NotFound("Category not found.");
                }

                if (owner_id != get_user_id())
                {
                    return StatusCode(403, "You can only delete categories of your own events.");
                }

                //false means the category still has enrolments
                bool deleted = categories.delete_category(id);

                if (deleted == false)
                {
                    return Conflict("This category has enrolments, so it cannot be deleted.");
                }

                return Ok("Category deleted.");

            }//end of try
            catch (Exception error)
            {//start of catch
                return StatusCode(500, error.Message);
            }//end of catch
        }//end of method delete_category

    }//end of class CategoriesController
}
