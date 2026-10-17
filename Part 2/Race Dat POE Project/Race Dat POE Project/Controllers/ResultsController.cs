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
    //this controller handles results (finish time and finishing position)
    //organisers capture the results, participants can only see their own
    [ApiController]
    [Route("api")]
    [Authorize]
    public class ResultsController : AppControllerBase
    {//start of class ResultsController

        //variables to store the repositories
        private IResultRepository results;
        private IEnrolmentRepository enrolments;
        private IEventRepository events;

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //constructor
        public ResultsController(IResultRepository result_repository, IEnrolmentRepository enrolment_repository, IEventRepository event_repository)
        {
            results = result_repository;
            enrolments = enrolment_repository;
            events = event_repository;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        /// <summary>
        /// Capture a finish time and position for an enrolment (Organisers only, their own events).
        /// </summary>
        /// <remarks>
        /// Data expected: the enrolment id in the address and in the JSON body FinishTime (hh:mm:ss, for example 00:45:30) and FinishPosition (1 or more).
        /// </remarks>
        /// <param name="enrolmentId">The id of the enrolment.</param>
        /// <param name="request">The finish time and position.</param>
        /// <response code="201">The result was captured. Returns a message and the new result id.</response>
        /// <response code="400">The details are not valid.</response>
        /// <response code="401">The user is not logged in.</response>
        /// <response code="403">The user is a Participant, or the enrolment belongs to another organiser's event.</response>
        /// <response code="404">The enrolment was not found.</response>
        /// <response code="409">This enrolment already has a result.</response>
        [HttpPost("enrolments/{enrolmentId:int}/result")]
        [Authorize(Roles = Roles.Organiser)]
        [ProducesResponseType(typeof(CreatedResponse), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 409)]
        public IActionResult capture_result(int enrolmentId, ResultRequest request)
        {//start of method capture_result
            try
            {//start of try

                //find out which organiser owns the event of this enrolment
                int owner_id = enrolments.get_enrolment_owner(enrolmentId);

                if (owner_id == 0)
                {
                    return NotFound("Enrolment not found.");
                }

                //an organiser may only capture results for their OWN events
                if (owner_id != get_user_id())
                {
                    return StatusCode(403, "You can only capture results for your own events.");
                }

                int result = results.capture_result(enrolmentId, request.FinishTime, request.FinishPosition);

                if (result == DbCodes.AlreadyExists)
                {
                    return Conflict("This enrolment already has a result. Use the update result endpoint to change it.");
                }

                CreatedResponse response = new CreatedResponse();
                response.Message = "Result captured.";
                response.Id = result;
                return StatusCode(201, response);

            }//end of try
            catch (Exception error)
            {//start of catch
                return StatusCode(500, error.Message);
            }//end of catch
        }//end of method capture_result

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        /// <summary>
        /// Correct a result (Organisers only, their own events).
        /// </summary>
        /// <remarks>
        /// Data expected: the result id in the address and FinishTime and FinishPosition in the JSON body.
        /// </remarks>
        /// <param name="id">The id of the result.</param>
        /// <param name="request">The new finish time and position.</param>
        /// <response code="200">The result was updated.</response>
        /// <response code="401">The user is not logged in.</response>
        /// <response code="403">The user is a Participant, or the result belongs to another organiser's event.</response>
        /// <response code="404">The result was not found.</response>
        [HttpPut("results/{id:int}")]
        [Authorize(Roles = Roles.Organiser)]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(string), 404)]
        public IActionResult update_result(int id, ResultRequest request)
        {//start of method update_result
            try
            {//start of try

                int owner_id = results.get_result_owner(id);

                if (owner_id == 0)
                {
                    return NotFound("Result not found.");
                }

                if (owner_id != get_user_id())
                {
                    return StatusCode(403, "You can only change results for your own events.");
                }

                results.update_result(id, request.FinishTime, request.FinishPosition);
                return Ok("Result updated.");

            }//end of try
            catch (Exception error)
            {//start of catch
                return StatusCode(500, error.Message);
            }//end of catch
        }//end of method update_result

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        /// <summary>
        /// View my own results (Participants only).
        /// </summary>
        /// <remarks>
        /// No data needs to be sent. Only the logged in participant's results are returned.
        /// </remarks>
        /// <response code="200">Returns the list of my results.</response>
        /// <response code="401">The user is not logged in.</response>
        /// <response code="403">The user is an Organiser, not a Participant.</response>
        [HttpGet("results/mine")]
        [Authorize(Roles = Roles.Participant)]
        [ProducesResponseType(typeof(List<MyResultInfo>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public IActionResult get_my_results()
        {//start of method get_my_results
            try
            {//start of try
                int participant_id = get_user_id();
                List<MyResultInfo> list = results.get_my_results(participant_id);
                return Ok(list);
            }//end of try
            catch (Exception error)
            {//start of catch
                return StatusCode(500, error.Message);
            }//end of catch
        }//end of method get_my_results

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        /// <summary>
        /// View the results of one of my events (Organisers only).
        /// </summary>
        /// <remarks>
        /// Data expected: the event id in the address. The best position comes first.
        /// </remarks>
        /// <param name="eventId">The id of the event.</param>
        /// <response code="200">Returns the results of the event.</response>
        /// <response code="401">The user is not logged in.</response>
        /// <response code="403">The user is a Participant, or the event belongs to another organiser.</response>
        /// <response code="404">The event was not found.</response>
        [HttpGet("events/{eventId:int}/results")]
        [Authorize(Roles = Roles.Organiser)]
        [ProducesResponseType(typeof(List<EventResultInfo>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(string), 404)]
        public IActionResult get_event_results(int eventId)
        {//start of method get_event_results
            try
            {//start of try

                int owner_id = events.get_event_owner(eventId);

                if (owner_id == 0)
                {
                    return NotFound("Event not found.");
                }

                //an organiser may only see results for their OWN events
                if (owner_id != get_user_id())
                {
                    return StatusCode(403, "You can only view results for your own events.");
                }

                List<EventResultInfo> list = results.get_event_results(eventId);
                return Ok(list);

            }//end of try
            catch (Exception error)
            {//start of catch
                return StatusCode(500, error.Message);
            }//end of catch
        }//end of method get_event_results

    }//end of class ResultsController
}
