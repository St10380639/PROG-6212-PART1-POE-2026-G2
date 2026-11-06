//------------------------------------------------------------------------
//Code written by Claude AI (Anthropic, 2026).
//Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model].
//Available at: https://claude.ai [Accessed 16 October 2026].
//------------------------------------------------------------------------

//library needed for the validation rules like [Required]
using System.ComponentModel.DataAnnotations;

namespace Race_Dat_POE_Project.Models
{
    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //class that stores the two role names
    //I use this so I do not have to type "Organiser" and "Participant" everywhere
    public static class Roles
    {
        public const string Organiser = "Organiser";
        public const string Participant = "Participant";
    }

    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //enums give a dropdown list in swagger
    public enum UserRole { Participant, Organiser }
    public enum EventType { Run, Walk, Cycle }
    public enum CategoryType { Age, Distance }

    //------------------------------------------------------------------
    //REQUEST CLASSES (the data the user sends to the api)
    //------------------------------------------------------------------

    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //data needed to register
    public class RegisterRequest
    {
        //[Required] means the user must fill this in
        [Required, StringLength(200)]
        public string FullName { get; set; } = "";

        //[EmailAddress] checks it looks like a real email
        [Required, EmailAddress, StringLength(255)]
        public string Email { get; set; } = "";

        //password must be at least 8 characters
        [Required, MinLength(8), StringLength(100)]
        public string Password { get; set; } = "";

        //the user picks Participant or Organiser from the dropdown
        public UserRole Role { get; set; } = UserRole.Participant;
    }

    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //data needed to login
    public class LoginRequest
    {
        [Required]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";
    }

    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //data needed to update a profile
    public class UpdateProfileRequest
    {
        [Required, StringLength(100)]
        public string FirstName { get; set; } = "";

        [StringLength(100)]
        public string LastName { get; set; } = "";

        [StringLength(20)]
        public string PhoneNumber { get; set; } = "";
    }

    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //data needed to create or update an event
    public class EventRequest
    {
        [Required, StringLength(150)]
        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public DateTime EventDate { get; set; }

        [Required, StringLength(255)]
        public string Location { get; set; } = "";

        //distance in km, must be more than 0
        [Range(0.01, 999.99)]
        public decimal Distance { get; set; }

        //run, walk or cycle (dropdown)
        public EventType EventType { get; set; }
    }

    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //data needed to create or update a category
    public class CategoryRequest
    {
        //for example Under 20, Senior, 10km, 21km
        [Required, StringLength(100)]
        public string CategoryName { get; set; } = "";

        //age or distance (dropdown)
        public CategoryType CategoryType { get; set; }
    }

    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //data needed to enrol in an event
    public class EnrolRequest
    {
        //the id of the category the participant chose
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }
    }

    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //data needed to capture a result
    public class ResultRequest
    {
        //finish time in the format hh:mm:ss, for example 00:45:30
        public TimeSpan FinishTime { get; set; }

        //finishing position, must be 1 or more
        [Range(1, int.MaxValue)]
        public int FinishPosition { get; set; }
    }

    //------------------------------------------------------------------
    //RESPONSE CLASSES (the data the api sends back)
    //------------------------------------------------------------------

    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //what the create endpoints send back (a message and the new id)
    public class CreatedResponse
    {
        public string Message { get; set; } = "";
        public int Id { get; set; }
    }

    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //what the login sends back
    public class LoginResponse
    {
        public string Token { get; set; } = "";
        public string Role { get; set; } = "";
        public int ExpiresInMinutes { get; set; }
    }

    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //the user's profile
    public class ProfileInfo
    {
        public int UserId { get; set; }
        public string Email { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string RoleName { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //one event
    public class EventInfo
    {
        public int EventId { get; set; }
        public int OrganiserId { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = "";
        public decimal Distance { get; set; }
        public string EventType { get; set; } = "";
    }

    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //one category
    public class CategoryInfo
    {
        public int CategoryId { get; set; }
        public int EventId { get; set; }
        public string CategoryName { get; set; } = "";
        public string CategoryType { get; set; } = "";
    }

    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //an enrolment as the participant sees it
    public class MyEnrolmentInfo
    {
        public int EnrolmentId { get; set; }
        public DateTime EnrolmentDate { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; } = "";
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = "";
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
    }

    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //an enrolment as the organiser sees it
    public class EventEnrolmentInfo
    {
        public int EnrolmentId { get; set; }
        public DateTime EnrolmentDate { get; set; }
        public int ParticipantId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
    }

    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //a result as the participant sees it
    public class MyResultInfo
    {
        public int ResultId { get; set; }
        public string FinishTime { get; set; } = "";
        public int FinishPosition { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; } = "";
        public DateTime EventDate { get; set; }
        public string CategoryName { get; set; } = "";
    }

    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //a result as the organiser sees it
    public class EventResultInfo
    {
        public int ResultId { get; set; }
        public int FinishPosition { get; set; }
        public string FinishTime { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string CategoryName { get; set; } = "";
    }
}
