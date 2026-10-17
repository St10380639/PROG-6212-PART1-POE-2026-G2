//------------------------------------------------------------------------
//Code written by Claude AI (Anthropic, 2026).
//Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model].
//Available at: https://claude.ai [Accessed 16 October 2026].
//------------------------------------------------------------------------

//libraries needed
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Race_Dat_POE_Project.Models;
using Xunit;

namespace Race_Dat_POE_Project.Tests
{
    //Code written by Claude AI (Anthropic, 2026).
    //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
    //a logged in test user: the client already has the token in its header
    public class TestUser
    {
        public HttpClient Client { get; set; } = new HttpClient();
        public string Email { get; set; } = "";
    }

    //Code written by Claude AI (Anthropic, 2026).
    //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
    //start of class ApiTestBase
    //all my test classes copy (inherit) from this class so they can share these helper methods
    public class ApiTestBase : IClassFixture<ApiFactory>
    {
        //the factory that starts the api
        protected ApiFactory factory;

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //constructor, xUnit gives us the factory
        public ApiTestBase(ApiFactory api_factory)
        {
            factory = api_factory;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //creates a client that is NOT logged in
        protected HttpClient create_client()
        {
            WebApplicationFactoryClientOptions options = new WebApplicationFactoryClientOptions();
            options.BaseAddress = new Uri("https://localhost");
            return factory.CreateClient(options);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //makes a new email so tests do not clash with each other
        protected string new_email()
        {
            return "user" + Guid.NewGuid().ToString("N").Substring(0, 8) + "@test.com";
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //registers a new user, logs in, and gives back a client that has the token
        protected async Task<TestUser> create_user(UserRole role)
        {
            HttpClient client = create_client();
            string email = new_email();

            //register
            RegisterRequest register = new RegisterRequest();
            register.FullName = "Test User";
            register.Email = email;
            register.Password = "Password123!";
            register.Role = role;
            HttpResponseMessage register_response = await client.PostAsJsonAsync("/api/auth/register", register);
            if (register_response.IsSuccessStatusCode == false)
            {
                string register_text = await register_response.Content.ReadAsStringAsync();
                throw new Exception("Register failed. Status " + (int)register_response.StatusCode + ". Answer: " + register_text);
            }

            //login
            LoginRequest login = new LoginRequest();
            login.Email = email;
            login.Password = "Password123!";
            HttpResponseMessage response = await client.PostAsJsonAsync("/api/auth/login", login);
            LoginResponse body = await read_json<LoginResponse>(response, "Login");

            //put the token in the header so every request from this client is logged in
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.Token);

            TestUser user = new TestUser();
            user.Client = client;
            user.Email = email;
            return user;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //builds a valid event request
        protected EventRequest make_event_request(string name)
        {
            EventRequest request = new EventRequest();
            request.Name = name;
            request.Description = "A test race";
            request.EventDate = DateTime.Today.AddDays(30);
            request.Location = "Johannesburg";
            request.Distance = 10;
            request.EventType = EventType.Run;
            return request;
        }

        //creates an event as the given organiser and gives back the new event id
        protected async Task<int> create_event(HttpClient organiser, string name = "Test Race")
        {
            HttpResponseMessage response = await organiser.PostAsJsonAsync("/api/events", make_event_request(name));
            CreatedResponse created = await read_json<CreatedResponse>(response, "Create event");
            return created!.Id;
        }

        //creates a category for an event and gives back the new category id
        protected async Task<int> create_category(HttpClient organiser, int event_id, string name = "10km")
        {
            CategoryRequest request = new CategoryRequest();
            request.CategoryName = name;
            request.CategoryType = CategoryType.Distance;
            HttpResponseMessage response = await organiser.PostAsJsonAsync("/api/events/" + event_id + "/categories", request);
            CreatedResponse created = await read_json<CreatedResponse>(response, "Create category");
            return created!.Id;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //enrols a participant in an event and gives back the new enrolment id
        protected async Task<int> enrol(HttpClient participant, int event_id, int category_id)
        {
            EnrolRequest request = new EnrolRequest();
            request.CategoryId = category_id;
            HttpResponseMessage response = await participant.PostAsJsonAsync("/api/events/" + event_id + "/enrolments", request);
            CreatedResponse created = await read_json<CreatedResponse>(response, "Enrol");
            return created!.Id;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //reads the json answer from the api
        //if the api did not answer with success, it stops and shows the status and the text of the answer
        //this makes a failing test easy to understand (instead of a confusing json error)
        protected async Task<T> read_json<T>(HttpResponseMessage response, string step)
        {
            string text = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception(step + " failed. Status " + (int)response.StatusCode + ". Answer: " + text);
            }

            System.Text.Json.JsonSerializerOptions options = new System.Text.Json.JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;
            T? value = System.Text.Json.JsonSerializer.Deserialize<T>(text, options);
            return value!;
        }
    }//end of class ApiTestBase
}
