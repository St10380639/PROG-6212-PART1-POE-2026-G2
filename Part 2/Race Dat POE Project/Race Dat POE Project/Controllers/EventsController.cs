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
    //this controller handles events
    //everyone logged in can VIEW events, only organisers can create, update and delete
    [ApiController]
    [Route("api/events")]
    [Authorize]
    public class EventsController : AppControllerBase
    {//start of class EventsController

        //variable to store the repository
        private IEventRepository events;

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //constructor
        public EventsController(IEventRepository event_repository)
        {
            events = event_repository;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        /// <summary>
        /// View all events.
        /// </summary>
        /// <remarks>
        /// Both roles can use this. No data needs to be sent.
        /// </remarks>
        /// <response code="200">Returns the list of events, the earliest date first.</response>
        /// <response code="401">The user is not logged in.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<EventInfo>), 200)]
        [ProducesResponseType(401)]
        public IActionResult get_all_events()
        {//start of method get_all_events
            try
            {//start of try
                List<EventInfo> list = events.get_all_events();
                return Ok(list);
            }//end of try
            catch (Exception error)
            {//start of catch
                return StatusCode(500, error.Message);
            }//end of catch
        }//end of method get_all_events

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        /// <summary>
        /// View one event.
        /// </summary>
        /// <remarks>
        /// Both roles can use this. Data expected: the event id in the address.
        /// </remarks>
        /// <param name="id">The id of the event.</param>
        /// <response code="200">Returns the event.</response>
        /// <response code="401">The user is not logged in.</response>
        /// <response code="404">The event was not found.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EventInfo), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(string), 404)]
        public IActionResult get_event_by_id(int id)
        {//start of method get_event_by_id
            try
            {//start of try
                EventInfo? item = events.get_event(id);

                if (item == null)
                {
                    return NotFound("Event not found.");
                }

                return Ok(item);
            }//end of try
            catch (Exception error)
            {//start of catch
                return StatusCode(500, error.Message);
            }//end of catch
        }//end of method get_event_by_id

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        /// <summary>
        /// Create an event (Organisers only).
        /// </summary>
        /// <remarks>
        /// Data expected (JSON body): Name, Description, EventDate, Location, Distance in km and EventType (Run, Walk or Cycle).
        /// The logged in organiser becomes the owner of the event.
        /// </remarks>
        /// <param name="request">The event details.</param>
        /// <response code="201">The event was created. Returns a message and the new event id.</response>
        /// <response code="400">The details are not valid.</response>
        /// <response code="401">The user is not logged in.</response>
        /// <response code="403">The user is a Participant, not an Organiser.</response>
        [HttpPost]
        [Authorize(Roles = Roles.Organiser)]
        [ProducesResponseType(typeof(CreatedResponse), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public IActionResult create_event(EventRequest request)
        {//start of method create_event
            try
            {//start of try

                //the organiser is the logged in user (from the token)
                int organiser_id = get_user_id();

                //if no description was typed, save empty text
                if (request.Description == null)
                {
                    request.Description = "";
                }

                int new_id = events.create_event(organiser_id, request);

                //send back a message and the new id
                CreatedResponse response = new CreatedResponse();
                response.Message = "Event created.";
                response.Id = new_id;
                return StatusCode(201, response);

            }//end of try
            catch (Exception error)
            {//start of catch
                return StatusCode(500, error.Message);
            }//end of catch
        }//end of method create_event

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        /// <summary>
        /// Update an event (Organisers only, their own events).
        /// </summary>
        /// <remarks>
        /// Data expected: the event id in the address and the event details in the JSON body.
        /// </remarks>
        /// <param name="id">The id of the event.</param>
        /// <param name="request">The new event details.</param>
        /// <response code="200">The event was updated.</response>
        /// <response code="401">The user is not logged in.</response>
        /// <response code="403">The user is a Participant, or the event belongs to another organiser.</response>
        /// <response code="404">The event was not found.</response>
        [HttpPut("{id:int}")]
        [Authorize(Roles = Roles.Organiser)]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(string), 404)]
        public IActionResult update_event(int id, EventRequest request)
        {//start of method update_event
            try
            {//start of try

                //find out which organiser owns this event (0 means not found)
                int owner_id = events.get_event_owner(id);

                if (owner_id == 0)
                {
                    return NotFound("Event not found.");
                }

                //an organiser may only change their OWN events
                if (owner_id != get_user_id())
                {
                    return StatusCode(403, "You can only change your own events.");
                }

                if (request.Description == null)
                {
                    request.Description = "";
                }

                events.update_event(id, request);
                return Ok("Event updated.");

            }//end of try
            catch (Exception error)
            {//start of catch
                return StatusCode(500, error.Message);
            }//end of catch
        }//end of method update_event

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        /// <summary>
        /// Delete an event (Organisers only, their own events).
        /// </summary>
        /// <remarks>
        /// Data expected: the event id in the address.
        /// The event's categories, enrolments and results are deleted with it.
        /// </remarks>
        /// <param name="id">The id of the event.</param>
        /// <response code="200">The event was deleted.</response>
        /// <response code="401">The user is not logged in.</response>
        /// <response code="403">The user is a Participant, or the event belongs to another organiser.</response>
        /// <response code="404">The event was not found.</response>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = Roles.Organiser)]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(string), 404)]
        public IActionResult delete_event(int id)
        {//start of method delete_event
            try
            {//start of try

                int owner_id = events.get_event_owner(id);

                if (owner_id == 0)
                {
                    return NotFound("Event not found.");
                }

                //an organiser may only delete their OWN events
                if (owner_id != get_user_id())
                {
                    return StatusCode(403, "You can only delete your own events.");
                }

                events.delete_event(id);
                return Ok("Event deleted.");

            }//end of try
            catch (Exception error)
            {//start of catch
                return StatusCode(500, error.Message);
            }//end of catch
        }//end of method delete_event

    }//end of class EventsController
}
