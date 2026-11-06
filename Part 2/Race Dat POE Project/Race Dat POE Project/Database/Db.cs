//------------------------------------------------------------------------
//Code written by Claude AI (Anthropic, 2026).
//Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model].
//Available at: https://claude.ai [Accessed 16 October 2026].
//------------------------------------------------------------------------

//libraries needed for the database
using Microsoft.Data.SqlClient;

namespace Race_Dat_POE_Project.Data
{
    //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
    //start of class Db
    //this class holds the database connection and a few small helper methods
    //the repository classes use it to talk to SQL Server
    public class Db
    {
        //variable to store the connection string from appsettings.json
        private string connection_string = "";

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //constructor, it runs when the class is created
        public Db(IConfiguration config)
        {
            //get the connection string from appsettings.json
            connection_string = config.GetConnectionString("DefaultConnection") ?? "";
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //method that creates a new connection to the database
        public SqlConnection get_connection()
        {
            SqlConnection connect = new SqlConnection(connection_string);
            return connect;
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //read a text column, gives "" if the column is empty
        public static string read_text(SqlDataReader reader, string column_name)
        {
            object value = reader[column_name];
            if (value == DBNull.Value)
            {
                return "";
            }
            return value.ToString() ?? "";
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //read a whole number column, gives 0 if the column is empty
        public static int read_int(SqlDataReader reader, string column_name)
        {
            object value = reader[column_name];
            if (value == DBNull.Value)
            {
                return 0;
            }
            return Convert.ToInt32(value);
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //read a decimal number column, gives 0 if the column is empty
        public static decimal read_decimal(SqlDataReader reader, string column_name)
        {
            object value = reader[column_name];
            if (value == DBNull.Value)
            {
                return 0;
            }
            return Convert.ToDecimal(value);
        }

        //Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
        //read a date column, gives the smallest date if the column is empty
        public static DateTime read_date(SqlDataReader reader, string column_name)
        {
            object value = reader[column_name];
            if (value == DBNull.Value)
            {
                return DateTime.MinValue;
            }
            return Convert.ToDateTime(value);
        }
    }//end of class Db
}
