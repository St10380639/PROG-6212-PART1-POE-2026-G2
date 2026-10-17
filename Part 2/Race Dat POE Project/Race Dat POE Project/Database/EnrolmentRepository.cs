//------------------------------------------------------------------------
//Code written by Claude AI (Anthropic, 2026).
//Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model].
//Available at: https://claude.ai [Accessed 16 October 2026].
//------------------------------------------------------------------------

//libraries needed
using Microsoft.Data.SqlClient;
using Race_Dat_POE_Project.Models;

namespace Race_Dat_POE_Project.Data
{
    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //start of class EnrolmentRepository
    //this class runs the SQL for the Enrolment table (the real database version)
    public class EnrolmentRepository : IEnrolmentRepository
    {
        //variable to store the Db class
        private Db db;

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //constructor
        public EnrolmentRepository(Db database)
        {
            db = database;
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //save an enrolment, gives back the new id or an error code from DbCodes
        public int enrol(int participant_id, int event_id, int category_id)
        {
            try
            {
                using (SqlConnection connect = db.get_connection())
                {
                    connect.Open();

                    string query = @"INSERT INTO Enrolment (participant_id, event_id, category_id)
                                     OUTPUT INSERTED.enrolment_id
                                     VALUES (@participant, @event, @category)";

                    SqlCommand cmd = new SqlCommand(query, connect);
                    cmd.Parameters.AddWithValue("@participant", participant_id);
                    cmd.Parameters.AddWithValue("@event", event_id);
                    cmd.Parameters.AddWithValue("@category", category_id);

                    int new_id = Convert.ToInt32(cmd.ExecuteScalar());
                    return new_id;
                }
            }
            catch (SqlException sql_error)
            {
                //2627 or 2601 means the participant is already enrolled in this event
                if (sql_error.Number == 2627 || sql_error.Number == 2601)
                {
                    return DbCodes.AlreadyExists;
                }

                //547 means the event or category does not exist, or the category is from another event
                if (sql_error.Number == 547)
                {
                    return DbCodes.InvalidReference;
                }

                throw;
            }
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //get the enrolments of one participant
        public List<MyEnrolmentInfo> get_my_enrolments(int participant_id)
        {
            List<MyEnrolmentInfo> enrolments = new List<MyEnrolmentInfo>();

            using (SqlConnection connect = db.get_connection())
            {
                connect.Open();

                //joins the Event and Category tables to get their names
                string query = @"SELECT en.enrolment_id, en.enrolment_date,
                                        e.event_id, e.name AS event_name, e.event_date, e.location,
                                        c.category_id, c.category_name
                                 FROM Enrolment en
                                 INNER JOIN [Event] e ON e.event_id = en.event_id
                                 INNER JOIN Category c ON c.category_id = en.category_id
                                 WHERE en.participant_id = @participant
                                 ORDER BY e.event_date";

                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@participant", participant_id);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    MyEnrolmentInfo item = new MyEnrolmentInfo();
                    item.EnrolmentId = Db.read_int(reader, "enrolment_id");
                    item.EnrolmentDate = Db.read_date(reader, "enrolment_date");
                    item.EventId = Db.read_int(reader, "event_id");
                    item.EventName = Db.read_text(reader, "event_name");
                    item.EventDate = Db.read_date(reader, "event_date");
                    item.Location = Db.read_text(reader, "location");
                    item.CategoryId = Db.read_int(reader, "category_id");
                    item.CategoryName = Db.read_text(reader, "category_name");
                    enrolments.Add(item);
                }
                reader.Close();
            }
            return enrolments;
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //get all the enrolments of one event
        public List<EventEnrolmentInfo> get_event_enrolments(int event_id)
        {
            List<EventEnrolmentInfo> enrolments = new List<EventEnrolmentInfo>();

            using (SqlConnection connect = db.get_connection())
            {
                connect.Open();

                //joins Users and Category to get the participant and category details
                string query = @"SELECT en.enrolment_id, en.enrolment_date,
                                        u.user_id AS participant_id, u.first_name, u.last_name, u.email,
                                        c.category_id, c.category_name
                                 FROM Enrolment en
                                 INNER JOIN Users u ON u.user_id = en.participant_id
                                 INNER JOIN Category c ON c.category_id = en.category_id
                                 WHERE en.event_id = @event
                                 ORDER BY u.last_name, u.first_name";

                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@event", event_id);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    EventEnrolmentInfo item = new EventEnrolmentInfo();
                    item.EnrolmentId = Db.read_int(reader, "enrolment_id");
                    item.EnrolmentDate = Db.read_date(reader, "enrolment_date");
                    item.ParticipantId = Db.read_int(reader, "participant_id");
                    item.FirstName = Db.read_text(reader, "first_name");
                    item.LastName = Db.read_text(reader, "last_name");
                    item.Email = Db.read_text(reader, "email");
                    item.CategoryId = Db.read_int(reader, "category_id");
                    item.CategoryName = Db.read_text(reader, "category_name");
                    enrolments.Add(item);
                }
                reader.Close();
            }
            return enrolments;
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //find the organiser id of the event this enrolment belongs to, gives 0 if not found
        public int get_enrolment_owner(int enrolment_id)
        {
            using (SqlConnection connect = db.get_connection())
            {
                connect.Open();

                string query = @"SELECT e.organiser_id
                                 FROM Enrolment en
                                 INNER JOIN [Event] e ON e.event_id = en.event_id
                                 WHERE en.enrolment_id = @id";

                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@id", enrolment_id);
                object result = cmd.ExecuteScalar();

                if (result == null)
                {
                    return 0;
                }
                return Convert.ToInt32(result);
            }
        }
    }//end of class EnrolmentRepository
}
