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
    //tests that check organisers can manage events (create, update, delete)
    public class EventManagementTests : ApiTestBase
    {
        public EventManagementTests(ApiFactory api_factory) : base(api_factory) { }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //success: an organiser can create an event
        [Fact]
        public async Task Organiser_CanCreateEvent_ReturnsCreated()
        {
            TestUser organiser = await create_user(UserRole.Organiser);

            HttpResponseMessage response = await organiser.Client.PostAsJsonAsync("/api/events", make_event_request("Park Run"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //success: the new event can be viewed with all its details
        [Fact]
        public async Task CreatedEvent_CanBeViewedWithItsDetails()
        {
            TestUser organiser = await create_user(UserRole.Organiser);
            int event_id = await create_event(organiser.Client, "Sunrise Run");

            EventInfo? item = await organiser.Client.GetFromJsonAsync<EventInfo>("/api/events/" + event_id);

            Assert.Equal("Sunrise Run", item!.Name);
            Assert.Equal("Johannesburg", item.Location);
            Assert.Equal(10m, item.Distance);
            Assert.Equal("run", item.EventType);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //failure: an event with no name is rejected
        [Fact]
        public async Task CreateEvent_WithoutName_ReturnsBadRequest()
        {
            TestUser organiser = await create_user(UserRole.Organiser);
            EventRequest request = make_event_request("");

            HttpResponseMessage response = await organiser.Client.PostAsJsonAsync("/api/events", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //failure: an event with a distance of 0 is rejected
        [Fact]
        public async Task CreateEvent_WithZeroDistance_ReturnsBadRequest()
        {
            TestUser organiser = await create_user(UserRole.Organiser);
            EventRequest request = make_event_request("Zero Run");
            request.Distance = 0;

            HttpResponseMessage response = await organiser.Client.PostAsJsonAsync("/api/events", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //success: an organiser can update their own event
        [Fact]
        public async Task Organiser_CanUpdateOwnEvent()
        {
            TestUser organiser = await create_user(UserRole.Organiser);
            int event_id = await create_event(organiser.Client, "Old Name");

            HttpResponseMessage response = await organiser.Client.PutAsJsonAsync("/api/events/" + event_id, make_event_request("New Name"));
            EventInfo? item = await organiser.Client.GetFromJsonAsync<EventInfo>("/api/events/" + event_id);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("New Name", item!.Name);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //failure: an organiser cannot update another organiser's event
        [Fact]
        public async Task Organiser_CannotUpdateAnotherOrganisersEvent_ReturnsForbidden()
        {
            TestUser owner = await create_user(UserRole.Organiser);
            TestUser other = await create_user(UserRole.Organiser);
            int event_id = await create_event(owner.Client);

            HttpResponseMessage response = await other.Client.PutAsJsonAsync("/api/events/" + event_id, make_event_request("Stolen"));

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //failure: updating an event that does not exist gives 404
        [Fact]
        public async Task UpdateEvent_ThatDoesNotExist_ReturnsNotFound()
        {
            TestUser organiser = await create_user(UserRole.Organiser);

            HttpResponseMessage response = await organiser.Client.PutAsJsonAsync("/api/events/99999", make_event_request("Ghost"));

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //success: an organiser can delete their own event
        [Fact]
        public async Task Organiser_CanDeleteOwnEvent()
        {
            TestUser organiser = await create_user(UserRole.Organiser);
            int event_id = await create_event(organiser.Client);

            HttpResponseMessage response = await organiser.Client.DeleteAsync("/api/events/" + event_id);
            HttpResponseMessage after = await organiser.Client.GetAsync("/api/events/" + event_id);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, after.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //failure: an organiser cannot delete another organiser's event
        [Fact]
        public async Task Organiser_CannotDeleteAnotherOrganisersEvent_ReturnsForbidden()
        {
            TestUser owner = await create_user(UserRole.Organiser);
            TestUser other = await create_user(UserRole.Organiser);
            int event_id = await create_event(owner.Client);

            HttpResponseMessage response = await other.Client.DeleteAsync("/api/events/" + event_id);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //success: an organiser can add a category to their event and everyone can view it
        [Fact]
        public async Task Organiser_CanAddCategory_AndParticipantCanViewIt()
        {
            TestUser organiser = await create_user(UserRole.Organiser);
            TestUser participant = await create_user(UserRole.Participant);
            int event_id = await create_event(organiser.Client);
            await create_category(organiser.Client, event_id, "Under 20");

            List<CategoryInfo>? list = await participant.Client.GetFromJsonAsync<List<CategoryInfo>>("/api/events/" + event_id + "/categories");

            Assert.Single(list!);
            Assert.Equal("Under 20", list![0].CategoryName);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //failure: an organiser cannot add a category to another organiser's event
        [Fact]
        public async Task Organiser_CannotAddCategoryToAnotherOrganisersEvent_ReturnsForbidden()
        {
            TestUser owner = await create_user(UserRole.Organiser);
            TestUser other = await create_user(UserRole.Organiser);
            int event_id = await create_event(owner.Client);
            CategoryRequest request = new CategoryRequest { CategoryName = "Senior", CategoryType = CategoryType.Age };

            HttpResponseMessage response = await other.Client.PostAsJsonAsync("/api/events/" + event_id + "/categories", request);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
