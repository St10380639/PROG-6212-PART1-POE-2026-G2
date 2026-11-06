//------------------------------------------------------------------------
//Code written by Claude AI (Anthropic, 2026).
//Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model].
//Available at: https://claude.ai [Accessed 16 October 2026].
//------------------------------------------------------------------------

//libraries needed
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Race_Dat_POE_Project.Data;

namespace Race_Dat_POE_Project.Tests
{
    //Code written by Claude AI (Anthropic, 2026).
    //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
    //start of class ApiFactory
    //this class starts my real api inside the test, but gives it the FAKE repositories
    //so the tests never touch SQL Server
    public class ApiFactory : WebApplicationFactory<Program>
    {
        //the fake database for this factory
        public FakeDatabase Database = new FakeDatabase();

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //this method runs when the test api starts
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                //replace each real repository with its fake version
                services.AddSingleton<IUserRepository>(new FakeUserRepository(Database));
                services.AddSingleton<IEventRepository>(new FakeEventRepository(Database));
                services.AddSingleton<ICategoryRepository>(new FakeCategoryRepository(Database));
                services.AddSingleton<IEnrolmentRepository>(new FakeEnrolmentRepository(Database));
                services.AddSingleton<IResultRepository>(new FakeResultRepository(Database));
            });
        }
    }//end of class ApiFactory
}
