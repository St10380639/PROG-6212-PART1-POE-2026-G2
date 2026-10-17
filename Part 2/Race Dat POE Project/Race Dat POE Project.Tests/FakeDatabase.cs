//------------------------------------------------------------------------
//Code written by Claude AI (Anthropic, 2026).
//Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model].
//Available at: https://claude.ai [Accessed 16 October 2026].
//------------------------------------------------------------------------

//libraries needed
using Race_Dat_POE_Project.Models;

namespace Race_Dat_POE_Project.Tests
{
    //Code written by Claude AI (Anthropic, 2026).
    //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
    //a user stored in the fake database
    public class FakeUser
    {
        public int UserId { get; set; }
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string RoleName { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }

    //Code written by Claude AI (Anthropic, 2026).
    //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
    //an enrolment stored in the fake database
    public class FakeEnrolment
    {
        public int EnrolmentId { get; set; }
        public int ParticipantId { get; set; }
        public int EventId { get; set; }
        public int CategoryId { get; set; }
        public DateTime EnrolmentDate { get; set; }
    }

    //Code written by Claude AI (Anthropic, 2026).
    //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
    //a result stored in the fake database
    public class FakeResult
    {
        public int ResultId { get; set; }
        public int EnrolmentId { get; set; }
        public TimeSpan FinishTime { get; set; }
        public int FinishPosition { get; set; }
    }

    //Code written by Claude AI (Anthropic, 2026).
    //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
    //start of class FakeDatabase
    //the unit tests use this instead of SQL Server
    //it is just lists in memory, so the tests run on any computer (and on GitHub)
    public class FakeDatabase
    {
        public List<FakeUser> Users = new List<FakeUser>();
        public List<EventInfo> Events = new List<EventInfo>();
        public List<CategoryInfo> Categories = new List<CategoryInfo>();
        public List<FakeEnrolment> Enrolments = new List<FakeEnrolment>();
        public List<FakeResult> Results = new List<FakeResult>();

        //counter used to give every new row an id (like IDENTITY in SQL Server)
        private int next_id = 1;

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        //gives the next id and moves the counter up by one
        public int get_next_id()
        {
            int id = next_id;
            next_id = next_id + 1;
            return id;
        }
    }//end of class FakeDatabase
}
