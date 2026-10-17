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
    //tests for event enrolments and results
    public class EnrolmentTests : ApiTestBase
    {
        public EnrolmentTests(ApiFactory api_factory) : base(api_factory) { }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //success: a participant can enrol in an event
        [Fact]
        public async Task Participant_CanEnrol_ReturnsCreated()
        {
            TestUser organiser = await create_user(UserRole.Organiser);
            TestUser participant = await create_user(UserRole.Participant);
            int event_id = await create_event(organiser.Client);
            int category_id = await create_category(organiser.Client, event_id);

            HttpResponseMessage response = await participant.Client.PostAsJsonAsync("/api/events/" + event_id + "/enrolments", new EnrolRequest { CategoryId = category_id });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //success: the enrolment is recorded with the right participant, event and category
        [Fact]
        public async Task Enrolment_IsRecordedCorrectly()
        {
            TestUser organiser = await create_user(UserRole.Organiser);
            TestUser participant = await create_user(UserRole.Participant);
            int event_id = await create_event(organiser.Client, "City Marathon");
            int category_id = await create_category(organiser.Client, event_id, "21km");
            await enrol(participant.Client, event_id, category_id);

            //the participant sees the enrolment
            List<MyEnrolmentInfo>? mine = await participant.Client.GetFromJsonAsync<List<MyEnrolmentInfo>>("/api/enrolments/mine");
            Assert.Single(mine!);
            Assert.Equal(event_id, mine![0].EventId);
            Assert.Equal("City Marathon", mine[0].EventName);
            Assert.Equal(category_id, mine[0].CategoryId);
            Assert.Equal("21km", mine[0].CategoryName);

            //the organiser sees the same enrolment with the participant's email
            List<EventEnrolmentInfo>? all = await organiser.Client.GetFromJsonAsync<List<EventEnrolmentInfo>>("/api/events/" + event_id + "/enrolments");
            Assert.Single(all!);
            Assert.Equal(participant.Email, all![0].Email);
            Assert.Equal(category_id, all[0].CategoryId);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //failure: the same participant cannot enrol twice in the same event
        [Fact]
        public async Task Participant_CannotEnrolTwice_ReturnsConflict()
        {
            TestUser organiser = await create_user(UserRole.Organiser);
            TestUser participant = await create_user(UserRole.Participant);
            int event_id = await create_event(organiser.Client);
            int category_id = await create_category(organiser.Client, event_id);
            await enrol(participant.Client, event_id, category_id);

            HttpResponseMessage second = await participant.Client.PostAsJsonAsync("/api/events/" + event_id + "/enrolments", new EnrolRequest { CategoryId = category_id });

            Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //failure: a category from a different event cannot be used
        [Fact]
        public async Task Enrol_WithCategoryFromAnotherEvent_ReturnsBadRequest()
        {
            TestUser organiser = await create_user(UserRole.Organiser);
            TestUser participant = await create_user(UserRole.Participant);
            int event_one = await create_event(organiser.Client, "Event One");
            int event_two = await create_event(organiser.Client, "Event Two");
            int category_of_two = await create_category(organiser.Client, event_two);

            HttpResponseMessage response = await participant.Client.PostAsJsonAsync("/api/events/" + event_one + "/enrolments", new EnrolRequest { CategoryId = category_of_two });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //failure: an event that does not exist cannot be enrolled in
        [Fact]
        public async Task Enrol_InEventThatDoesNotExist_ReturnsBadRequest()
        {
            TestUser participant = await create_user(UserRole.Participant);

            HttpResponseMessage response = await participant.Client.PostAsJsonAsync("/api/events/99999/enrolments", new EnrolRequest { CategoryId = 1 });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //failure: an organiser cannot see the enrolments of another organiser's event
        [Fact]
        public async Task Organiser_CannotViewEnrolmentsOfAnotherOrganisersEvent_ReturnsForbidden()
        {
            TestUser owner = await create_user(UserRole.Organiser);
            TestUser other = await create_user(UserRole.Organiser);
            int event_id = await create_event(owner.Client);

            HttpResponseMessage response = await other.Client.GetAsync("/api/events/" + event_id + "/enrolments");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //success: an organiser captures a result and the participant can view it
        [Fact]
        public async Task Organiser_CanCaptureResult_AndParticipantSeesIt()
        {
            TestUser organiser = await create_user(UserRole.Organiser);
            TestUser participant = await create_user(UserRole.Participant);
            int event_id = await create_event(organiser.Client, "Result Race");
            int category_id = await create_category(organiser.Client, event_id);
            int enrolment_id = await enrol(participant.Client, event_id, category_id);
            ResultRequest request = new ResultRequest { FinishTime = new TimeSpan(0, 45, 30), FinishPosition = 3 };

            HttpResponseMessage response = await organiser.Client.PostAsJsonAsync("/api/enrolments/" + enrolment_id + "/result", request);
            List<MyResultInfo>? mine = await participant.Client.GetFromJsonAsync<List<MyResultInfo>>("/api/results/mine");

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Single(mine!);
            Assert.Equal(3, mine![0].FinishPosition);
            Assert.Equal("00:45:30", mine[0].FinishTime);
            Assert.Equal("Result Race", mine[0].EventName);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //failure: a second result for the same enrolment is rejected
        [Fact]
        public async Task CaptureResult_Twice_ReturnsConflict()
        {
            TestUser organiser = await create_user(UserRole.Organiser);
            TestUser participant = await create_user(UserRole.Participant);
            int event_id = await create_event(organiser.Client);
            int category_id = await create_category(organiser.Client, event_id);
            int enrolment_id = await enrol(participant.Client, event_id, category_id);
            ResultRequest request = new ResultRequest { FinishTime = new TimeSpan(0, 50, 0), FinishPosition = 1 };
            await organiser.Client.PostAsJsonAsync("/api/enrolments/" + enrolment_id + "/result", request);

            HttpResponseMessage second = await organiser.Client.PostAsJsonAsync("/api/enrolments/" + enrolment_id + "/result", request);

            Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //failure: another organiser cannot capture a result for this event
        [Fact]
        public async Task Organiser_CannotCaptureResultForAnotherOrganisersEvent_ReturnsForbidden()
        {
            TestUser owner = await create_user(UserRole.Organiser);
            TestUser other = await create_user(UserRole.Organiser);
            TestUser participant = await create_user(UserRole.Participant);
            int event_id = await create_event(owner.Client);
            int category_id = await create_category(owner.Client, event_id);
            int enrolment_id = await enrol(participant.Client, event_id, category_id);
            ResultRequest request = new ResultRequest { FinishTime = new TimeSpan(0, 50, 0), FinishPosition = 1 };

            HttpResponseMessage response = await other.Client.PostAsJsonAsync("/api/enrolments/" + enrolment_id + "/result", request);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //failure: a position of 0 is rejected
        [Fact]
        public async Task CaptureResult_WithPositionZero_ReturnsBadRequest()
        {
            TestUser organiser = await create_user(UserRole.Organiser);
            TestUser participant = await create_user(UserRole.Participant);
            int event_id = await create_event(organiser.Client);
            int category_id = await create_category(organiser.Client, event_id);
            int enrolment_id = await enrol(participant.Client, event_id, category_id);
            ResultRequest request = new ResultRequest { FinishTime = new TimeSpan(0, 50, 0), FinishPosition = 0 };

            HttpResponseMessage response = await organiser.Client.PostAsJsonAsync("/api/enrolments/" + enrolment_id + "/result", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //a participant only sees THEIR OWN results, not other people's
        [Fact]
        public async Task Participant_OnlySeesTheirOwnResults()
        {
            TestUser organiser = await create_user(UserRole.Organiser);
            TestUser first = await create_user(UserRole.Participant);
            TestUser second = await create_user(UserRole.Participant);
            int event_id = await create_event(organiser.Client);
            int category_id = await create_category(organiser.Client, event_id);
            int first_enrolment = await enrol(first.Client, event_id, category_id);
            int second_enrolment = await enrol(second.Client, event_id, category_id);
            await organiser.Client.PostAsJsonAsync("/api/enrolments/" + first_enrolment + "/result", new ResultRequest { FinishTime = new TimeSpan(0, 40, 0), FinishPosition = 1 });
            await organiser.Client.PostAsJsonAsync("/api/enrolments/" + second_enrolment + "/result", new ResultRequest { FinishTime = new TimeSpan(0, 50, 0), FinishPosition = 2 });

            List<MyResultInfo>? first_results = await first.Client.GetFromJsonAsync<List<MyResultInfo>>("/api/results/mine");

            Assert.Single(first_results!);
            Assert.Equal(1, first_results![0].FinishPosition);
        }
    }
}
