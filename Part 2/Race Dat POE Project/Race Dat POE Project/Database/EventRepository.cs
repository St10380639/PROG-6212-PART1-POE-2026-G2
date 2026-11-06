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
    //start of class EventRepository
    //this class runs the SQL for the Event table (the real database version)
    public class EventRepository : IEventRepository
    {
        //variable to store the Db class
        private Db db;

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //constructor
        public EventRepository(Db database)
        {
            db = database;
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //copies the current row of the reader into an EventInfo object
        private EventInfo read_event(SqlDataReader reader)
        {
            EventInfo item = new EventInfo();
            item.EventId = Db.read_int(reader, "event_id");
            item.OrganiserId = Db.read_int(reader, "organiser_id");
            item.Name = Db.read_text(reader, "name");
            item.Description = Db.read_text(reader, "description");
            item.EventDate = Db.read_date(reader, "event_date");
            item.Location = Db.read_text(reader, "location");
            item.Distance = Db.read_decimal(reader, "distance");
            item.EventType = Db.read_text(reader, "event_type");
            return item;
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //get all events, the earliest date first
        public List<EventInfo> get_all_events()
        {
            List<EventInfo> events = new List<EventInfo>();

            using (SqlConnection connect = db.get_connection())
            {
                connect.Open();

                string query = @"SELECT event_id, organiser_id, name, description, event_date,
                                        location, distance, event_type
                                 FROM [Event]
                                 ORDER BY event_date";

                SqlCommand cmd = new SqlCommand(query, connect);
                SqlDataReader reader = cmd.ExecuteReader();

                //loop through every row and add it to the list
                while (reader.Read())
                {
                    events.Add(read_event(reader));
                }
                reader.Close();
            }
            return events;
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //get one event, gives null if it is not found
        public EventInfo? get_event(int event_id)
        {
            using (SqlConnection connect = db.get_connection())
            {
                connect.Open();

                string query = @"SELECT event_id, organiser_id, name, description, event_date,
                                        location, distance, event_type
                                 FROM [Event]
                                 WHERE event_id = @id";

                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@id", event_id);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read() == false)
                {
                    reader.Close();
                    return null;
                }

                EventInfo item = read_event(reader);
                reader.Close();
                return item;
            }
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //save a new event and give back its new id
        public int create_event(int organiser_id, EventRequest request)
        {
            using (SqlConnection connect = db.get_connection())
            {
                connect.Open();

                //OUTPUT INSERTED.event_id gives back the new id
                string query = @"INSERT INTO [Event] (organiser_id, name, description, event_date, location, distance, event_type)
                                 OUTPUT INSERTED.event_id
                                 VALUES (@organiser, @name, @description, @date, @location, @distance, @type)";

                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@organiser", organiser_id);
                cmd.Parameters.AddWithValue("@name", request.Name);
                cmd.Parameters.AddWithValue("@description", request.Description);
                //.Date removes the time part, the database column only stores the date
                cmd.Parameters.AddWithValue("@date", request.EventDate.Date);
                cmd.Parameters.AddWithValue("@location", request.Location);
                cmd.Parameters.AddWithValue("@distance", request.Distance);
                //the database only accepts run, walk or cycle in lowercase
                cmd.Parameters.AddWithValue("@type", request.EventType.ToString().ToLower());

                int new_id = Convert.ToInt32(cmd.ExecuteScalar());
                return new_id;
            }
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //update an event
        public void update_event(int event_id, EventRequest request)
        {
            using (SqlConnection connect = db.get_connection())
            {
                connect.Open();

                string query = @"UPDATE [Event]
                                 SET name = @name, description = @description, event_date = @date,
                                     location = @location, distance = @distance, event_type = @type
                                 WHERE event_id = @id";

                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@name", request.Name);
                cmd.Parameters.AddWithValue("@description", request.Description);
                cmd.Parameters.AddWithValue("@date", request.EventDate.Date);
                cmd.Parameters.AddWithValue("@location", request.Location);
                cmd.Parameters.AddWithValue("@distance", request.Distance);
                cmd.Parameters.AddWithValue("@type", request.EventType.ToString().ToLower());
                cmd.Parameters.AddWithValue("@id", event_id);
                cmd.ExecuteNonQuery();
            }
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //delete an event (the database also deletes its categories, enrolments and results)
        public void delete_event(int event_id)
        {
            using (SqlConnection connect = db.get_connection())
            {
                connect.Open();

                string query = "DELETE FROM [Event] WHERE event_id = @id";
                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@id", event_id);
                cmd.ExecuteNonQuery();
            }
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //find the organiser id of an event, gives 0 if the event is not found
        public int get_event_owner(int event_id)
        {
            using (SqlConnection connect = db.get_connection())
            {
                connect.Open();

                string query = "SELECT organiser_id FROM [Event] WHERE event_id = @id";
                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@id", event_id);
                object result = cmd.ExecuteScalar();

                if (result == null)
                {
                    return 0;
                }
                return Convert.ToInt32(result);
            }
        }
    }//end of class EventRepository
}
