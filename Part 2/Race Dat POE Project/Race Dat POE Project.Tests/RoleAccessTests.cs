//------------------------------------------------------------------------
//Code written by Claude AI (Anthropic, 2026).
//Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model].
//Available at: https://claude.ai [Accessed 16 October 2026].
//------------------------------------------------------------------------

//libraries needed
using System.Net;
using System.Net.Http.Json;
using Race_Dat_POE_Project.Models;
using Xunit;

namespace Race_Dat_POE_Project.Tests
{
    //Code written by Claude AI (Anthropic, 2026).
    //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
    //tests that check the wrong role is rejected (role based access)
    public class RoleAccessTests : ApiTestBase
    {
        public RoleAccessTests(ApiFactory api_factory) : base(api_factory) { }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //a Participant must not create events
        [Fact]
        public async Task Participant_CannotCreateEvent_ReturnsForbidden()
        {
            TestUser participant = await create_user(UserRole.Participant);
            HttpResponseMessage response = await participant.Client.PostAsJsonAsync("/api/events", make_event_request("Not Allowed"));
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //a Participant must not update events
        [Fact]
        public async Task Participant_CannotUpdateEvent_ReturnsForbidden()
        {
            TestUser organiser = await create_user(UserRole.Organiser);
            TestUser participant = await create_user(UserRole.Participant);
            int event_id = await create_event(organiser.Client);

            HttpResponseMessage response = await participant.Client.PutAsJsonAsync("/api/events/" + event_id, make_event_request("Hacked"));

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //a Participant must not delete events
        [Fact]
        public async Task Participant_CannotDeleteEvent_ReturnsForbidden()
        {
            TestUser organiser = await create_user(UserRole.Organiser);
            TestUser participant = await create_user(UserRole.Participant);
            int event_id = await create_event(organiser.Client);

            HttpResponseMessage response = await participant.Client.DeleteAsync("/api/events/" + event_id);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //a Participant must not create categories
        [Fact]
        public async Task Participant_CannotCreateCategory_ReturnsForbidden()
        {
            TestUser organiser = await create_user(UserRole.Organiser);
            TestUser participant = await create_user(UserRole.Participant);
            int event_id = await create_event(organiser.Client);
            CategoryRequest request = new CategoryRequest { CategoryName = "Senior", CategoryType = CategoryType.Age };

            HttpResponseMessage response = await participant.Client.PostAsJsonAsync("/api/events/" + event_id + "/categories", request);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //an Organiser must not enrol in an event (that is for Participants)
        [Fact]
        public async Task Organiser_CannotEnrol_ReturnsForbidden()
        {
            TestUser organiser = await create_user(UserRole.Organiser);
            int event_id = await create_event(organiser.Client);
            int category_id = await create_category(organiser.Client, event_id);
            EnrolRequest request = new EnrolRequest { CategoryId = category_id };

            HttpResponseMessage response = await organiser.Client.PostAsJsonAsync("/api/events/" + event_id + "/enrolments", request);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //an Organiser must not use the participant's "my enrolments" endpoint
        [Fact]
        public async Task Organiser_CannotViewMyEnrolments_ReturnsForbidden()
        {
            TestUser organiser = await create_user(UserRole.Organiser);
            HttpResponseMessage response = await organiser.Client.GetAsync("/api/enrolments/mine");
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //a Participant must not view the enrolments of an event (organiser only)
        [Fact]
        public async Task Participant_CannotViewEventEnrolments_ReturnsForbidden()
        {
            TestUser organiser = await create_user(UserRole.Organiser);
            TestUser participant = await create_user(UserRole.Participant);
            int event_id = await create_event(organiser.Client);

            HttpResponseMessage response = await participant.Client.GetAsync("/api/events/" + event_id + "/enrolments");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //a Participant must not capture results
        [Fact]
        public async Task Participant_CannotCaptureResult_ReturnsForbidden()
        {
            TestUser organiser = await create_user(UserRole.Organiser);
            TestUser participant = await create_user(UserRole.Participant);
            int event_id = await create_event(organiser.Client);
            int category_id = await create_category(organiser.Client, event_id);
            int enrolment_id = await enrol(participant.Client, event_id, category_id);
            ResultRequest request = new ResultRequest { FinishTime = TimeSpan.FromMinutes(45), FinishPosition = 1 };

            HttpResponseMessage response = await participant.Client.PostAsJsonAsync("/api/enrolments/" + enrolment_id + "/result", request);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //an Organiser must not use the participant's "my results" endpoint
        [Fact]
        public async Task Organiser_CannotViewMyResults_ReturnsForbidden()
        {
            TestUser organiser = await create_user(UserRole.Organiser);
            HttpResponseMessage response = await organiser.Client.GetAsync("/api/results/mine");
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
