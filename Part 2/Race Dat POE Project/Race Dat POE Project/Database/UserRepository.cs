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
    //start of class UserRepository
    //this class runs the SQL for the Users table (the real database version)
    public class UserRepository : IUserRepository
    {
        //variable to store the Db class
        private Db db;

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //constructor
        public UserRepository(Db database)
        {
            db = database;
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //check if an email is already registered
        public bool email_exists(string email)
        {
            using (SqlConnection connect = db.get_connection())
            {
                connect.Open();

                //COUNT(*) gives the number of users with this email
                string query = "SELECT COUNT(*) FROM Users WHERE email = @email";
                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@email", email);
                int count = Convert.ToInt32(cmd.ExecuteScalar());

                return count > 0;
            }
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //save a new user, gives false if the role name is not in the Role table
        public bool create_user(string email, string password_hash, string first_name, string last_name, string role_name)
        {
            using (SqlConnection connect = db.get_connection())
            {
                connect.Open();

                //it looks up the role_id from the Role table using the role name
                string query = @"INSERT INTO Users (role_id, email, password_hash, first_name, last_name)
                                 SELECT role_id, @email, @hash, @first, @last
                                 FROM [Role] WHERE role_name = @role";

                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@hash", password_hash);
                cmd.Parameters.AddWithValue("@first", first_name);
                cmd.Parameters.AddWithValue("@last", last_name);
                cmd.Parameters.AddWithValue("@role", role_name);

                //0 rows means the role name was not found
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //find the id, password hash and role of the user with this email
        public UserLogin? find_login(string email)
        {
            using (SqlConnection connect = db.get_connection())
            {
                connect.Open();

                string query = @"SELECT u.user_id, u.password_hash, r.role_name
                                 FROM Users u
                                 INNER JOIN [Role] r ON r.role_id = u.role_id
                                 WHERE u.email = @email";

                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@email", email);
                SqlDataReader reader = cmd.ExecuteReader();

                //no row means no user has this email
                if (reader.Read() == false)
                {
                    reader.Close();
                    return null;
                }

                UserLogin login = new UserLogin();
                login.UserId = Db.read_int(reader, "user_id");
                login.PasswordHash = Db.read_text(reader, "password_hash");
                login.RoleName = Db.read_text(reader, "role_name");
                reader.Close();
                return login;
            }
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //get the profile of one user
        public ProfileInfo? get_profile(int user_id)
        {
            using (SqlConnection connect = db.get_connection())
            {
                connect.Open();

                string query = @"SELECT u.user_id, u.email, u.first_name, u.last_name, u.phone_number,
                                        r.role_name, u.created_at, u.updated_at
                                 FROM Users u
                                 INNER JOIN [Role] r ON r.role_id = u.role_id
                                 WHERE u.user_id = @id";

                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@id", user_id);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read() == false)
                {
                    reader.Close();
                    return null;
                }

                ProfileInfo profile = new ProfileInfo();
                profile.UserId = Db.read_int(reader, "user_id");
                profile.Email = Db.read_text(reader, "email");
                profile.FirstName = Db.read_text(reader, "first_name");
                profile.LastName = Db.read_text(reader, "last_name");
                profile.PhoneNumber = Db.read_text(reader, "phone_number");
                profile.RoleName = Db.read_text(reader, "role_name");
                profile.CreatedAt = Db.read_date(reader, "created_at");
                profile.UpdatedAt = Db.read_date(reader, "updated_at");
                reader.Close();
                return profile;
            }
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //update the profile of one user, gives false if the user was not found
        public bool update_profile(int user_id, UpdateProfileRequest request)
        {
            using (SqlConnection connect = db.get_connection())
            {
                connect.Open();

                //the trigger in the database updates updated_at by itself
                string query = @"UPDATE Users
                                 SET first_name = @first, last_name = @last, phone_number = @phone
                                 WHERE user_id = @id";

                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@first", request.FirstName);
                cmd.Parameters.AddWithValue("@last", request.LastName);
                cmd.Parameters.AddWithValue("@phone", request.PhoneNumber);
                cmd.Parameters.AddWithValue("@id", user_id);

                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }
    }//end of class UserRepository
}
