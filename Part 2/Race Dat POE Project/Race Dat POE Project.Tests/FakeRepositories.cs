//------------------------------------------------------------------------
//Code written by Claude AI (Anthropic, 2026).
//Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model].
//Available at: https://claude.ai [Accessed 16 October 2026].
//------------------------------------------------------------------------

//libraries needed
using Race_Dat_POE_Project.Data;
using Race_Dat_POE_Project.Models;

namespace Race_Dat_POE_Project.Tests
{
    //Code written by Claude AI (Anthropic, 2026).
    //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
    //start of class FakeUserRepository
    //fake version of UserRepository, it uses lists instead of SQL
    public class FakeUserRepository : IUserRepository
    {
        private FakeDatabase database;

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public FakeUserRepository(FakeDatabase fake_database)
        {
            database = fake_database;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public bool email_exists(string email)
        {
            return database.Users.Exists(u => u.Email == email);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public bool create_user(string email, string password_hash, string first_name, string last_name, string role_name)
        {
            //only the two real roles are allowed (like the Role table)
            if (role_name != "Organiser" && role_name != "Participant")
            {
                return false;
            }

            FakeUser user = new FakeUser();
            user.UserId = database.get_next_id();
            user.Email = email;
            user.PasswordHash = password_hash;
            user.FirstName = first_name;
            user.LastName = last_name;
            user.RoleName = role_name;
            user.CreatedAt = DateTime.Now;
            database.Users.Add(user);
            return true;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public UserLogin? find_login(string email)
        {
            FakeUser? user = database.Users.Find(u => u.Email == email);
            if (user == null)
            {
                return null;
            }

            UserLogin login = new UserLogin();
            login.UserId = user.UserId;
            login.PasswordHash = user.PasswordHash;
            login.RoleName = user.RoleName;
            return login;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public ProfileInfo? get_profile(int user_id)
        {
            FakeUser? user = database.Users.Find(u => u.UserId == user_id);
            if (user == null)
            {
                return null;
            }

            ProfileInfo profile = new ProfileInfo();
            profile.UserId = user.UserId;
            profile.Email = user.Email;
            profile.FirstName = user.FirstName;
            profile.LastName = user.LastName;
            profile.PhoneNumber = user.PhoneNumber;
            profile.RoleName = user.RoleName;
            profile.CreatedAt = user.CreatedAt;
            profile.UpdatedAt = user.CreatedAt;
            return profile;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public bool update_profile(int user_id, UpdateProfileRequest request)
        {
            FakeUser? user = database.Users.Find(u => u.UserId == user_id);
            if (user == null)
            {
                return false;
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            return true;
        }
    }//end of class FakeUserRepository

    //Code written by Claude AI (Anthropic, 2026).
    //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
    //start of class FakeEventRepository
    //fake version of EventRepository
    public class FakeEventRepository : IEventRepository
    {
        private FakeDatabase database;

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public FakeEventRepository(FakeDatabase fake_database)
        {
            database = fake_database;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public List<EventInfo> get_all_events()
        {
            //copy the list and sort it by date, like ORDER BY event_date
            List<EventInfo> list = new List<EventInfo>(database.Events);
            list.Sort((a, b) => a.EventDate.CompareTo(b.EventDate));
            return list;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public EventInfo? get_event(int event_id)
        {
            return database.Events.Find(e => e.EventId == event_id);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public int create_event(int organiser_id, EventRequest request)
        {
            EventInfo item = new EventInfo();
            item.EventId = database.get_next_id();
            item.OrganiserId = organiser_id;
            item.Name = request.Name;
            item.Description = request.Description;
            item.EventDate = request.EventDate.Date;
            item.Location = request.Location;
            item.Distance = request.Distance;
            item.EventType = request.EventType.ToString().ToLower();
            database.Events.Add(item);
            return item.EventId;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public void update_event(int event_id, EventRequest request)
        {
            EventInfo? item = database.Events.Find(e => e.EventId == event_id);
            if (item == null)
            {
                return;
            }

            item.Name = request.Name;
            item.Description = request.Description;
            item.EventDate = request.EventDate.Date;
            item.Location = request.Location;
            item.Distance = request.Distance;
            item.EventType = request.EventType.ToString().ToLower();
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public void delete_event(int event_id)
        {
            //like the cascade in the database: remove results, enrolments and categories of this event too
            List<FakeEnrolment> event_enrolments = database.Enrolments.FindAll(en => en.EventId == event_id);
            foreach (FakeEnrolment enrolment in event_enrolments)
            {
                database.Results.RemoveAll(r => r.EnrolmentId == enrolment.EnrolmentId);
            }
            database.Enrolments.RemoveAll(en => en.EventId == event_id);
            database.Categories.RemoveAll(c => c.EventId == event_id);
            database.Events.RemoveAll(e => e.EventId == event_id);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public int get_event_owner(int event_id)
        {
            EventInfo? item = database.Events.Find(e => e.EventId == event_id);
            if (item == null)
            {
                return 0;
            }
            return item.OrganiserId;
        }
    }//end of class FakeEventRepository

    //Code written by Claude AI (Anthropic, 2026).
    //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
    //start of class FakeCategoryRepository
    //fake version of CategoryRepository
    public class FakeCategoryRepository : ICategoryRepository
    {
        private FakeDatabase database;

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public FakeCategoryRepository(FakeDatabase fake_database)
        {
            database = fake_database;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public List<CategoryInfo> get_categories(int event_id)
        {
            return database.Categories.FindAll(c => c.EventId == event_id);
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public int create_category(int event_id, CategoryRequest request)
        {
            CategoryInfo item = new CategoryInfo();
            item.CategoryId = database.get_next_id();
            item.EventId = event_id;
            item.CategoryName = request.CategoryName;
            item.CategoryType = request.CategoryType.ToString().ToLower();
            database.Categories.Add(item);
            return item.CategoryId;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public void update_category(int category_id, CategoryRequest request)
        {
            CategoryInfo? item = database.Categories.Find(c => c.CategoryId == category_id);
            if (item == null)
            {
                return;
            }

            item.CategoryName = request.CategoryName;
            item.CategoryType = request.CategoryType.ToString().ToLower();
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public bool delete_category(int category_id)
        {
            //a category that has enrolments cannot be deleted
            if (database.Enrolments.Exists(en => en.CategoryId == category_id))
            {
                return false;
            }

            database.Categories.RemoveAll(c => c.CategoryId == category_id);
            return true;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public int get_category_owner(int category_id)
        {
            CategoryInfo? category = database.Categories.Find(c => c.CategoryId == category_id);
            if (category == null)
            {
                return 0;
            }

            EventInfo? item = database.Events.Find(e => e.EventId == category.EventId);
            if (item == null)
            {
                return 0;
            }
            return item.OrganiserId;
        }
    }//end of class FakeCategoryRepository

    //Code written by Claude AI (Anthropic, 2026).
    //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
    //start of class FakeEnrolmentRepository
    //fake version of EnrolmentRepository
    public class FakeEnrolmentRepository : IEnrolmentRepository
    {
        private FakeDatabase database;

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public FakeEnrolmentRepository(FakeDatabase fake_database)
        {
            database = fake_database;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public int enrol(int participant_id, int event_id, int category_id)
        {
            EventInfo? item = database.Events.Find(e => e.EventId == event_id);
            CategoryInfo? category = database.Categories.Find(c => c.CategoryId == category_id);

            //the event and category must exist and the category must belong to the event
            if (item == null || category == null || category.EventId != event_id)
            {
                return DbCodes.InvalidReference;
            }

            //a participant can only enrol once in the same event
            if (database.Enrolments.Exists(en => en.ParticipantId == participant_id && en.EventId == event_id))
            {
                return DbCodes.AlreadyExists;
            }

            FakeEnrolment enrolment = new FakeEnrolment();
            enrolment.EnrolmentId = database.get_next_id();
            enrolment.ParticipantId = participant_id;
            enrolment.EventId = event_id;
            enrolment.CategoryId = category_id;
            enrolment.EnrolmentDate = DateTime.Now;
            database.Enrolments.Add(enrolment);
            return enrolment.EnrolmentId;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public List<MyEnrolmentInfo> get_my_enrolments(int participant_id)
        {
            List<MyEnrolmentInfo> list = new List<MyEnrolmentInfo>();

            foreach (FakeEnrolment enrolment in database.Enrolments.FindAll(en => en.ParticipantId == participant_id))
            {
                EventInfo? item = database.Events.Find(e => e.EventId == enrolment.EventId);
                CategoryInfo? category = database.Categories.Find(c => c.CategoryId == enrolment.CategoryId);
                if (item == null || category == null)
                {
                    continue;
                }

                MyEnrolmentInfo info = new MyEnrolmentInfo();
                info.EnrolmentId = enrolment.EnrolmentId;
                info.EnrolmentDate = enrolment.EnrolmentDate;
                info.EventId = item.EventId;
                info.EventName = item.Name;
                info.EventDate = item.EventDate;
                info.Location = item.Location;
                info.CategoryId = category.CategoryId;
                info.CategoryName = category.CategoryName;
                list.Add(info);
            }
            return list;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public List<EventEnrolmentInfo> get_event_enrolments(int event_id)
        {
            List<EventEnrolmentInfo> list = new List<EventEnrolmentInfo>();

            foreach (FakeEnrolment enrolment in database.Enrolments.FindAll(en => en.EventId == event_id))
            {
                FakeUser? user = database.Users.Find(u => u.UserId == enrolment.ParticipantId);
                CategoryInfo? category = database.Categories.Find(c => c.CategoryId == enrolment.CategoryId);
                if (user == null || category == null)
                {
                    continue;
                }

                EventEnrolmentInfo info = new EventEnrolmentInfo();
                info.EnrolmentId = enrolment.EnrolmentId;
                info.EnrolmentDate = enrolment.EnrolmentDate;
                info.ParticipantId = user.UserId;
                info.FirstName = user.FirstName;
                info.LastName = user.LastName;
                info.Email = user.Email;
                info.CategoryId = category.CategoryId;
                info.CategoryName = category.CategoryName;
                list.Add(info);
            }
            return list;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public int get_enrolment_owner(int enrolment_id)
        {
            FakeEnrolment? enrolment = database.Enrolments.Find(en => en.EnrolmentId == enrolment_id);
            if (enrolment == null)
            {
                return 0;
            }

            EventInfo? item = database.Events.Find(e => e.EventId == enrolment.EventId);
            if (item == null)
            {
                return 0;
            }
            return item.OrganiserId;
        }
    }//end of class FakeEnrolmentRepository

    //Code written by Claude AI (Anthropic, 2026).
    //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
    //start of class FakeResultRepository
    //fake version of ResultRepository
    public class FakeResultRepository : IResultRepository
    {
        private FakeDatabase database;

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public FakeResultRepository(FakeDatabase fake_database)
        {
            database = fake_database;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public int capture_result(int enrolment_id, TimeSpan finish_time, int finish_position)
        {
            //an enrolment can only have one result
            if (database.Results.Exists(r => r.EnrolmentId == enrolment_id))
            {
                return DbCodes.AlreadyExists;
            }

            FakeResult result = new FakeResult();
            result.ResultId = database.get_next_id();
            result.EnrolmentId = enrolment_id;
            result.FinishTime = finish_time;
            result.FinishPosition = finish_position;
            database.Results.Add(result);
            return result.ResultId;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public void update_result(int result_id, TimeSpan finish_time, int finish_position)
        {
            FakeResult? result = database.Results.Find(r => r.ResultId == result_id);
            if (result == null)
            {
                return;
            }

            result.FinishTime = finish_time;
            result.FinishPosition = finish_position;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public List<MyResultInfo> get_my_results(int participant_id)
        {
            List<MyResultInfo> list = new List<MyResultInfo>();

            foreach (FakeResult result in database.Results)
            {
                FakeEnrolment? enrolment = database.Enrolments.Find(en => en.EnrolmentId == result.EnrolmentId);
                if (enrolment == null || enrolment.ParticipantId != participant_id)
                {
                    continue;
                }

                EventInfo? item = database.Events.Find(e => e.EventId == enrolment.EventId);
                CategoryInfo? category = database.Categories.Find(c => c.CategoryId == enrolment.CategoryId);
                if (item == null || category == null)
                {
                    continue;
                }

                MyResultInfo info = new MyResultInfo();
                info.ResultId = result.ResultId;
                info.FinishTime = result.FinishTime.ToString();
                info.FinishPosition = result.FinishPosition;
                info.EventId = item.EventId;
                info.EventName = item.Name;
                info.EventDate = item.EventDate;
                info.CategoryName = category.CategoryName;
                list.Add(info);
            }
            return list;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public List<EventResultInfo> get_event_results(int event_id)
        {
            List<EventResultInfo> list = new List<EventResultInfo>();

            foreach (FakeResult result in database.Results)
            {
                FakeEnrolment? enrolment = database.Enrolments.Find(en => en.EnrolmentId == result.EnrolmentId);
                if (enrolment == null || enrolment.EventId != event_id)
                {
                    continue;
                }

                FakeUser? user = database.Users.Find(u => u.UserId == enrolment.ParticipantId);
                CategoryInfo? category = database.Categories.Find(c => c.CategoryId == enrolment.CategoryId);
                if (user == null || category == null)
                {
                    continue;
                }

                EventResultInfo info = new EventResultInfo();
                info.ResultId = result.ResultId;
                info.FinishPosition = result.FinishPosition;
                info.FinishTime = result.FinishTime.ToString();
                info.FirstName = user.FirstName;
                info.LastName = user.LastName;
                info.CategoryName = category.CategoryName;
                list.Add(info);
            }

            //best position first, like ORDER BY finish_position
            list.Sort((a, b) => a.FinishPosition.CompareTo(b.FinishPosition));
            return list;
        }

        //Code written by Claude AI (Anthropic, 2026).
        //Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model]. Available at: https://claude.ai [Accessed 16 October 2026].
        public int get_result_owner(int result_id)
        {
            FakeResult? result = database.Results.Find(r => r.ResultId == result_id);
            if (result == null)
            {
                return 0;
            }

            FakeEnrolment? enrolment = database.Enrolments.Find(en => en.EnrolmentId == result.EnrolmentId);
            if (enrolment == null)
            {
                return 0;
            }

            EventInfo? item = database.Events.Find(e => e.EventId == enrolment.EventId);
            if (item == null)
            {
                return 0;
            }
            return item.OrganiserId;
        }
    }//end of class FakeResultRepository
}
