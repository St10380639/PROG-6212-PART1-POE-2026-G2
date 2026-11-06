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
    //this controller handles enrolments (a participant entering an event)
    //the enrolment links the Participant, the event and the chosen category
    [ApiController]
    [Route("api")]
    [Authorize]
    public class EnrolmentsController : AppControllerBase
    {//start of class EnrolmentsController

        //variables to store the repositories
        private IEnrolmentRepository enrolments;
        private IEventRepository events;

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //constructor
        public EnrolmentsController(IEnrolmentRepository enrolment_repository, IEventRepository event_repository)
        {
            enrolments = enrolment_repository;
            events = event_repository;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        /// <summary>
        /// Enter an event by choosing a category (Participants only).
        /// </summary>
        /// <remarks>
        /// Data expected: the event id in the address and the CategoryId in the JSON body.
        /// The enrolment records the participant (from the login token), the event and the category.
        /// </remarks>
        /// <param name="eventId">The id of the event.</param>
        /// <param name="request">The id of the chosen category.</param>
        /// <response code="201">The enrolment was recorded. Returns a message and the new enrolment id.</response>
        /// <response code="400">The event was not found, or the category does not belong to this event.</response>
        /// <response code="401">The user is not logged in.</response>
        /// <response code="403">The user is an Organiser, not a Participant.</response>
        /// <response code="409">The participant is already enrolled in this event.</response>
        [HttpPost("events/{eventId:int}/enrolments")]
        [Authorize(Roles = Roles.Participant)]
        [ProducesResponseType(typeof(CreatedResponse), 201)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(string), 409)]
        public IActionResult enrol_in_event(int eventId, EnrolRequest request)
        {//start of method enrol_in_event
            try
            {//start of try

                //the participant is the logged in user (from the token)
                int participant_id = get_user_id();

                //save the enrolment, we get a new id or an error code (a negative number)
                int result = enrolments.enrol(participant_id, eventId, request.CategoryId);

                if (result == DbCodes.AlreadyExists)
                {
                    return Conflict("You are already enrolled in this event.");
                }

                if (result == DbCodes.InvalidReference)
                {
                    return BadRequest("Event not found, or that category does not belong to this event.");
                }

                CreatedResponse response = new CreatedResponse();
                response.Message = "Enrolled successfully.";
                response.Id = result;
                return StatusCode(201, response);

            }//end of try
            catch (Exception error)
            {//start of catch
                return StatusCode(500, error.Message);
            }//end of catch
        }//end of method enrol_in_event

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        /// <summary>
        /// View my own enrolments (Participants only).
        /// </summary>
        /// <remarks>
        /// No data needs to be sent. Only the logged in participant's enrolments are returned.
        /// </remarks>
        /// <response code="200">Returns the list of my enrolments with the event and category names.</response>
        /// <response code="401">The user is not logged in.</response>
        /// <response code="403">The user is an Organiser, not a Participant.</response>
        [HttpGet("enrolments/mine")]
        [Authorize(Roles = Roles.Participant)]
        [ProducesResponseType(typeof(List<MyEnrolmentInfo>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public IActionResult get_my_enrolments()
        {//start of method get_my_enrolments
            try
            {//start of try
                int participant_id = get_user_id();
                List<MyEnrolmentInfo> list = enrolments.get_my_enrolments(participant_id);
                return Ok(list);
            }//end of try
            catch (Exception error)
            {//start of catch
                return StatusCode(500, error.Message);
            }//end of catch
        }//end of method get_my_enrolments

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        /// <summary>
        /// View all enrolments for one of my events (Organisers only).
        /// </summary>
        /// <remarks>
        /// Data expected: the event id in the address. An organiser can only see enrolments for their own events.
        /// </remarks>
        /// <param name="eventId">The id of the event.</param>
        /// <response code="200">Returns the participants who enrolled, with their categories.</response>
        /// <response code="401">The user is not logged in.</response>
        /// <response code="403">The user is a Participant, or the event belongs to another organiser.</response>
        /// <response code="404">The event was not found.</response>
        [HttpGet("events/{eventId:int}/enrolments")]
        [Authorize(Roles = Roles.Organiser)]
        [ProducesResponseType(typeof(List<EventEnrolmentInfo>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(string), 404)]
        public IActionResult get_event_enrolments(int eventId)
        {//start of method get_event_enrolments
            try
            {//start of try

                int owner_id = events.get_event_owner(eventId);

                if (owner_id == 0)
                {
                    return NotFound("Event not found.");
                }

                //an organiser may only see enrolments for their OWN events
                if (owner_id != get_user_id())
                {
                    return StatusCode(403, "You can only view enrolments for your own events.");
                }

                List<EventEnrolmentInfo> list = enrolments.get_event_enrolments(eventId);
                return Ok(list);

            }//end of try
            catch (Exception error)
            {//start of catch
                return StatusCode(500, error.Message);
            }//end of catch
        }//end of method get_event_enrolments

    }//end of class EnrolmentsController
}
