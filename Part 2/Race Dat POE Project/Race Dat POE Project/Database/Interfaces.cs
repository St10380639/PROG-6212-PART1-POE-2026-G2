//------------------------------------------------------------------------
//Code written by Claude AI (Anthropic, 2026).
//Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model].
//Available at: https://claude.ai [Accessed 16 October 2026].
//------------------------------------------------------------------------

//libraries needed
using Race_Dat_POE_Project.Models;

namespace Race_Dat_POE_Project.Data
{
    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //numbers the repositories send back when something goes wrong
    //(a real id is always 1 or more, so negative numbers can be used as error codes)
    public static class DbCodes
    {
        //the row is already there (for example the participant is already enrolled)
        public const int AlreadyExists = -1;

        //the event or category does not exist, or they do not belong together
        public const int InvalidReference = -2;
    }

    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //holds what we need from the database to check a login
    public class UserLogin
    {
        public int UserId { get; set; }
        public string PasswordHash { get; set; } = "";
        public string RoleName { get; set; } = "";
    }

    //an interface is a list of methods a class promises to have
    //I use interfaces so the unit tests can use a fake class instead of the real database

    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //methods for the Users table
    public interface IUserRepository
    {
        bool email_exists(string email);
        bool create_user(string email, string password_hash, string first_name, string last_name, string role_name);
        UserLogin? find_login(string email);
        ProfileInfo? get_profile(int user_id);
        bool update_profile(int user_id, UpdateProfileRequest request);
    }

    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //methods for the Event table
    public interface IEventRepository
    {
        List<EventInfo> get_all_events();
        EventInfo? get_event(int event_id);
        int create_event(int organiser_id, EventRequest request);
        void update_event(int event_id, EventRequest request);
        void delete_event(int event_id);
        int get_event_owner(int event_id);
    }

    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //methods for the Category table
    public interface ICategoryRepository
    {
        List<CategoryInfo> get_categories(int event_id);
        int create_category(int event_id, CategoryRequest request);
        void update_category(int category_id, CategoryRequest request);
        bool delete_category(int category_id);
        int get_category_owner(int category_id);
    }

    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //methods for the Enrolment table
    public interface IEnrolmentRepository
    {
        int enrol(int participant_id, int event_id, int category_id);
        List<MyEnrolmentInfo> get_my_enrolments(int participant_id);
        List<EventEnrolmentInfo> get_event_enrolments(int event_id);
        int get_enrolment_owner(int enrolment_id);
    }

    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //methods for the Result table
    public interface IResultRepository
    {
        int capture_result(int enrolment_id, TimeSpan finish_time, int finish_position);
        void update_result(int result_id, TimeSpan finish_time, int finish_position);
        List<MyResultInfo> get_my_results(int participant_id);
        List<EventResultInfo> get_event_results(int event_id);
        int get_result_owner(int result_id);
    }
}
