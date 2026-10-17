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
    //start of class CategoryRepository
    //this class runs the SQL for the Category table (the real database version)
    public class CategoryRepository : ICategoryRepository
    {
        //variable to store the Db class
        private Db db;

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //constructor
        public CategoryRepository(Db database)
        {
            db = database;
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //get the categories of one event
        public List<CategoryInfo> get_categories(int event_id)
        {
            List<CategoryInfo> categories = new List<CategoryInfo>();

            using (SqlConnection connect = db.get_connection())
            {
                connect.Open();

                string query = @"SELECT category_id, event_id, category_name, category_type
                                 FROM Category
                                 WHERE event_id = @event
                                 ORDER BY category_name";

                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@event", event_id);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    CategoryInfo item = new CategoryInfo();
                    item.CategoryId = Db.read_int(reader, "category_id");
                    item.EventId = Db.read_int(reader, "event_id");
                    item.CategoryName = Db.read_text(reader, "category_name");
                    item.CategoryType = Db.read_text(reader, "category_type");
                    categories.Add(item);
                }
                reader.Close();
            }
            return categories;
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //save a new category and give back its new id
        public int create_category(int event_id, CategoryRequest request)
        {
            using (SqlConnection connect = db.get_connection())
            {
                connect.Open();

                string query = @"INSERT INTO Category (event_id, category_name, category_type)
                                 OUTPUT INSERTED.category_id
                                 VALUES (@event, @name, @type)";

                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@event", event_id);
                cmd.Parameters.AddWithValue("@name", request.CategoryName);
                //the database only accepts age or distance in lowercase
                cmd.Parameters.AddWithValue("@type", request.CategoryType.ToString().ToLower());

                int new_id = Convert.ToInt32(cmd.ExecuteScalar());
                return new_id;
            }
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //update a category
        public void update_category(int category_id, CategoryRequest request)
        {
            using (SqlConnection connect = db.get_connection())
            {
                connect.Open();

                string query = @"UPDATE Category
                                 SET category_name = @name, category_type = @type
                                 WHERE category_id = @id";

                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@name", request.CategoryName);
                cmd.Parameters.AddWithValue("@type", request.CategoryType.ToString().ToLower());
                cmd.Parameters.AddWithValue("@id", category_id);
                cmd.ExecuteNonQuery();
            }
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //delete a category, gives false if it still has enrolments
        public bool delete_category(int category_id)
        {
            try
            {
                using (SqlConnection connect = db.get_connection())
                {
                    connect.Open();

                    string query = "DELETE FROM Category WHERE category_id = @id";
                    SqlCommand cmd = new SqlCommand(query, connect);
                    cmd.Parameters.AddWithValue("@id", category_id);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (SqlException sql_error)
            {
                //error 547 means another table (Enrolment) still uses this category
                if (sql_error.Number == 547)
                {
                    return false;
                }
                //any other database error is passed on
                throw;
            }
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //find the organiser id of the event this category belongs to, gives 0 if not found
        public int get_category_owner(int category_id)
        {
            using (SqlConnection connect = db.get_connection())
            {
                connect.Open();

                string query = @"SELECT e.organiser_id
                                 FROM Category c
                                 INNER JOIN [Event] e ON e.event_id = c.event_id
                                 WHERE c.category_id = @id";

                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@id", category_id);
                object result = cmd.ExecuteScalar();

                if (result == null)
                {
                    return 0;
                }
                return Convert.ToInt32(result);
            }
        }
    }//end of class CategoryRepository
}
