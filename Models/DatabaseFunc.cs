using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Configuration;
using System.Data.SqlClient;

/// <summary>
/// DatabaseFunc 的摘要描述
/// </summary>
/// 
public partial class ASPdemo
{
    public partial class DatabaseFunc
    {

        public static void DBRun(SqlCommand command)
        {
            SqlConnection Conn = new SqlConnection();
            string connectionString = WebConfigurationManager.ConnectionStrings["asp_demoConnectionString"].ConnectionString;

            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                command.Connection = connection;
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    command.Cancel();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        public static DataTable DBQuery(SqlCommand command)
        {
            DataTable result = null;
            SqlConnection Conn = new SqlConnection();
            string connectionString = WebConfigurationManager.ConnectionStrings["asp_demoConnectionString"].ConnectionString;

            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                command.Connection = connection;
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        result = new DataTable();
                        result.Load(reader);
                    }
                    command.Cancel();    //cancel first to break continuing
                    reader.Close();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return result;
        }

    }




    public class Item : DatabaseFunc
    {

    }


}
