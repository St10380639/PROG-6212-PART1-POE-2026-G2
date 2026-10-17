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
    //tests for registration and login
    public class AuthTests : ApiTestBase
    {
        //constructor, passes the factory to the base class
        public AuthTests(ApiFactory api_factory) : base(api_factory) { }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //success: a valid registration works
        [Fact]
        public async Task Register_WithValidDetails_ReturnsOk()
        {
            HttpClient client = create_client();
            RegisterRequest request = new RegisterRequest { FullName = "Sam Runner", Email = new_email(), Password = "Password123!", Role = UserRole.Participant };

            HttpResponseMessage response = await client.PostAsJsonAsync("/api/auth/register", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //success: the password is hashed, not saved as plain text
        [Fact]
        public async Task Register_SavesHashedPassword_NotThePlainPassword()
        {
            HttpClient client = create_client();
            string email = new_email();
            RegisterRequest request = new RegisterRequest { FullName = "Sam Runner", Email = email, Password = "Password123!", Role = UserRole.Participant };

            await client.PostAsJsonAsync("/api/auth/register", request);

            FakeUser? saved = factory.Database.Users.Find(u => u.Email == email);
            Assert.NotNull(saved);
            Assert.NotEqual("Password123!", saved!.PasswordHash);
            //BCrypt hashes start with $2
            Assert.StartsWith("$2", saved.PasswordHash);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //failure: the same email cannot register twice
        [Fact]
        public async Task Register_WithSameEmailTwice_ReturnsConflict()
        {
            HttpClient client = create_client();
            RegisterRequest request = new RegisterRequest { FullName = "Sam Runner", Email = new_email(), Password = "Password123!", Role = UserRole.Participant };

            await client.PostAsJsonAsync("/api/auth/register", request);
            HttpResponseMessage second = await client.PostAsJsonAsync("/api/auth/register", request);

            Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //failure: a password shorter than 8 characters is rejected
        [Fact]
        public async Task Register_WithShortPassword_ReturnsBadRequest()
        {
            HttpClient client = create_client();
            RegisterRequest request = new RegisterRequest { FullName = "Sam Runner", Email = new_email(), Password = "123", Role = UserRole.Participant };

            HttpResponseMessage response = await client.PostAsJsonAsync("/api/auth/register", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //failure: an email that is not a real email is rejected
        [Fact]
        public async Task Register_WithInvalidEmail_ReturnsBadRequest()
        {
            HttpClient client = create_client();
            RegisterRequest request = new RegisterRequest { FullName = "Sam Runner", Email = "not-an-email", Password = "Password123!", Role = UserRole.Participant };

            HttpResponseMessage response = await client.PostAsJsonAsync("/api/auth/register", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //success: login with the right password gives a token and the role
        [Fact]
        public async Task Login_WithCorrectPassword_ReturnsTokenAndRole()
        {
            HttpClient client = create_client();
            string email = new_email();
            await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest { FullName = "Olivia Organiser", Email = email, Password = "Password123!", Role = UserRole.Organiser });

            HttpResponseMessage response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest { Email = email, Password = "Password123!" });
            LoginResponse? body = await response.Content.ReadFromJsonAsync<LoginResponse>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(string.IsNullOrEmpty(body!.Token));
            Assert.Equal("Organiser", body.Role);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //failure: login with the wrong password is rejected
        [Fact]
        public async Task Login_WithWrongPassword_ReturnsUnauthorized()
        {
            HttpClient client = create_client();
            string email = new_email();
            await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest { FullName = "Sam Runner", Email = email, Password = "Password123!", Role = UserRole.Participant });

            HttpResponseMessage response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest { Email = email, Password = "WrongPassword1!" });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //failure: login with an email that is not registered is rejected
        [Fact]
        public async Task Login_WithUnknownEmail_ReturnsUnauthorized()
        {
            HttpClient client = create_client();

            HttpResponseMessage response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest { Email = "nobody@test.com", Password = "Password123!" });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
