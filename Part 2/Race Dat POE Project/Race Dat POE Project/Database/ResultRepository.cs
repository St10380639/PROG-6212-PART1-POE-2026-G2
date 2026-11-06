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
    //start of class ResultRepository
    //this class runs the SQL for the Result table (the real database version)
    public class ResultRepository : IResultRepository
    {
        //variable to store the Db class
        private Db db;

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //constructor
        public ResultRepository(Db database)
        {
            db = database;
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //save a result, gives back the new id or DbCodes.AlreadyExists
        public int capture_result(int enrolment_id, TimeSpan finish_time, int finish_position)
        {
            try
            {
                using (SqlConnection connect = db.get_connection())
                {
                    connect.Open();

                    string query = @"INSERT INTO Result (enrolment_id, finish_time, finish_position)
                                     OUTPUT INSERTED.result_id
                                     VALUES (@enrolment, @time, @position)";

                    SqlCommand cmd = new SqlCommand(query, connect);
                    cmd.Parameters.AddWithValue("@enrolment", enrolment_id);
                    cmd.Parameters.AddWithValue("@time", finish_time);
                    cmd.Parameters.AddWithValue("@position", finish_position);

                    int new_id = Convert.ToInt32(cmd.ExecuteScalar());
                    return new_id;
                }
            }
            catch (SqlException sql_error)
            {
                //2627 or 2601 means this enrolment already has a result
                if (sql_error.Number == 2627 || sql_error.Number == 2601)
                {
                    return DbCodes.AlreadyExists;
                }
                throw;
            }
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //update a result
        public void update_result(int result_id, TimeSpan finish_time, int finish_position)
        {
            using (SqlConnection connect = db.get_connection())
            {
                connect.Open();

                string query = @"UPDATE Result
                                 SET finish_time = @time, finish_position = @position
                                 WHERE result_id = @id";

                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@time", finish_time);
                cmd.Parameters.AddWithValue("@position", finish_position);
                cmd.Parameters.AddWithValue("@id", result_id);
                cmd.ExecuteNonQuery();
            }
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //get the results of one participant
        public List<MyResultInfo> get_my_results(int participant_id)
        {
            List<MyResultInfo> results = new List<MyResultInfo>();

            using (SqlConnection connect = db.get_connection())
            {
                connect.Open();

                string query = @"SELECT r.result_id, r.finish_time, r.finish_position,
                                        e.event_id, e.name AS event_name, e.event_date,
                                        c.category_name
                                 FROM Result r
                                 INNER JOIN Enrolment en ON en.enrolment_id = r.enrolment_id
                                 INNER JOIN [Event] e ON e.event_id = en.event_id
                                 INNER JOIN Category c ON c.category_id = en.category_id
                                 WHERE en.participant_id = @participant
                                 ORDER BY e.event_date DESC";

                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@participant", participant_id);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    MyResultInfo item = new MyResultInfo();
                    item.ResultId = Db.read_int(reader, "result_id");
                    item.FinishTime = Db.read_text(reader, "finish_time");
                    item.FinishPosition = Db.read_int(reader, "finish_position");
                    item.EventId = Db.read_int(reader, "event_id");
                    item.EventName = Db.read_text(reader, "event_name");
                    item.EventDate = Db.read_date(reader, "event_date");
                    item.CategoryName = Db.read_text(reader, "category_name");
                    results.Add(item);
                }
                reader.Close();
            }
            return results;
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //get the results of one event, best position first
        public List<EventResultInfo> get_event_results(int event_id)
        {
            List<EventResultInfo> results = new List<EventResultInfo>();

            using (SqlConnection connect = db.get_connection())
            {
                connect.Open();

                string query = @"SELECT r.result_id, r.finish_position, r.finish_time,
                                        u.first_name, u.last_name, c.category_name
                                 FROM Result r
                                 INNER JOIN Enrolment en ON en.enrolment_id = r.enrolment_id
                                 INNER JOIN Users u ON u.user_id = en.participant_id
                                 INNER JOIN Category c ON c.category_id = en.category_id
                                 WHERE en.event_id = @event
                                 ORDER BY r.finish_position";

                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@event", event_id);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    EventResultInfo item = new EventResultInfo();
                    item.ResultId = Db.read_int(reader, "result_id");
                    item.FinishPosition = Db.read_int(reader, "finish_position");
                    item.FinishTime = Db.read_text(reader, "finish_time");
                    item.FirstName = Db.read_text(reader, "first_name");
                    item.LastName = Db.read_text(reader, "last_name");
                    item.CategoryName = Db.read_text(reader, "category_name");
                    results.Add(item);
                }
                reader.Close();
            }
            return results;
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //find the organiser id of the event this result belongs to, gives 0 if not found
        public int get_result_owner(int result_id)
        {
            using (SqlConnection connect = db.get_connection())
            {
                connect.Open();

                string query = @"SELECT e.organiser_id
                                 FROM Result r
                                 INNER JOIN Enrolment en ON en.enrolment_id = r.enrolment_id
                                 INNER JOIN [Event] e ON e.event_id = en.event_id
                                 WHERE r.result_id = @id";

                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@id", result_id);
                object result = cmd.ExecuteScalar();

                if (result == null)
                {
                    return 0;
                }
                return Convert.ToInt32(result);
            }
        }
    }//end of class ResultRepository
}
