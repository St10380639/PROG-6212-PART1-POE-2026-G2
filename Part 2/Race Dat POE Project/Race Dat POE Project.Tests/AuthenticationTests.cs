//------------------------------------------------------------------------
//Code written by Claude AI (Anthropic, 2026).
//Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model].
//Available at: https://claude.ai [Accessed 16 October 2026].
//------------------------------------------------------------------------

//libraries needed
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Race_Dat_POE_Project.Models;
using Xunit;

namespace Race_Dat_POE_Project.Tests
{
    //Code written by Claude AI (Anthropic, 2026).
    //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
    //tests that check logged in and NOT logged in requests, and the profile endpoints
    public class AuthenticationTests : ApiTestBase
    {
        public AuthenticationTests(ApiFactory api_factory) : base(api_factory) { }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //failure: no token on a protected endpoint gives 401
        [Fact]
        public async Task GetEvents_WithoutToken_ReturnsUnauthorized()
        {
            HttpClient client = create_client();
            HttpResponseMessage response = await client.GetAsync("/api/events");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //failure: no token on the profile endpoint gives 401
        [Fact]
        public async Task GetProfile_WithoutToken_ReturnsUnauthorized()
        {
            HttpClient client = create_client();
            HttpResponseMessage response = await client.GetAsync("/api/profile");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //failure: no token when creating an event gives 401
        [Fact]
        public async Task CreateEvent_WithoutToken_ReturnsUnauthorized()
        {
            HttpClient client = create_client();
            HttpResponseMessage response = await client.PostAsJsonAsync("/api/events", make_event_request("No Token Race"));
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //failure: a made up token gives 401
        [Fact]
        public async Task GetEvents_WithFakeToken_ReturnsUnauthorized()
        {
            HttpClient client = create_client();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "this.is.not.a.real.token");
            HttpResponseMessage response = await client.GetAsync("/api/events");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //success: a logged in participant can view events
        [Fact]
        public async Task GetEvents_AsParticipant_ReturnsOk()
        {
            TestUser participant = await create_user(UserRole.Participant);
            HttpResponseMessage response = await participant.Client.GetAsync("/api/events");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //success: a logged in organiser can view events
        [Fact]
        public async Task GetEvents_AsOrganiser_ReturnsOk()
        {
            TestUser organiser = await create_user(UserRole.Organiser);
            HttpResponseMessage response = await organiser.Client.GetAsync("/api/events");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //success: a user can view their own profile
        [Fact]
        public async Task GetProfile_AsParticipant_ReturnsOwnProfile()
        {
            TestUser participant = await create_user(UserRole.Participant);

            ProfileInfo? profile = await participant.Client.GetFromJsonAsync<ProfileInfo>("/api/profile");

            Assert.Equal(participant.Email, profile!.Email);
            Assert.Equal("Participant", profile.RoleName);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //success: a user can update their own profile
        [Fact]
        public async Task UpdateProfile_ChangesTheNameAndPhone()
        {
            TestUser organiser = await create_user(UserRole.Organiser);
            UpdateProfileRequest request = new UpdateProfileRequest { FirstName = "Newname", LastName = "Newsurname", PhoneNumber = "0821234567" };

            HttpResponseMessage response = await organiser.Client.PutAsJsonAsync("/api/profile", request);
            ProfileInfo? profile = await organiser.Client.GetFromJsonAsync<ProfileInfo>("/api/profile");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Newname", profile!.FirstName);
            Assert.Equal("0821234567", profile.PhoneNumber);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //failure: the profile update needs a first name
        [Fact]
        public async Task UpdateProfile_WithoutFirstName_ReturnsBadRequest()
        {
            TestUser organiser = await create_user(UserRole.Organiser);
            UpdateProfileRequest request = new UpdateProfileRequest { FirstName = "", LastName = "x", PhoneNumber = "1" };

            HttpResponseMessage response = await organiser.Client.PutAsJsonAsync("/api/profile", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
