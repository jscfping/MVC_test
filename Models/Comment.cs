using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Configuration;
using System.Data.SqlClient;

/// <summary>
/// Comment 的摘要描述
/// </summary>
/// 
public partial class ASPdemo
{

    public class Comment
    {
        public string cid = "";
        public string uid = "";
        public string comment = "";

        public Comment()
        {

        }

        public void Update(string whosUid)
        {
            DataTable dt = DatabaseFunc.Comment.ReadUserComment(whosUid, cid);
            if (dt != null)
            {
                DatabaseFunc.Comment.UpdateComment(cid, whosUid, comment);
            }
            else
            {
                throw new Exception("permission denied");
            }
            
        }

        public void Delete(string whosUid)
        {
            DataTable dt = DatabaseFunc.Comment.ReadUserComment(whosUid, cid);
            if (dt != null)
            {
                DatabaseFunc.Comment.DeleteComment(cid);
            }
            else
            {
                throw new Exception("permission denied");
            }
            
        }
    }
}

public partial class ASPdemo
{
    public partial class DatabaseFunc
    {
        public class Comment : DatabaseFunc
        {
            public static void CreateNewComment(string uid, string comment)
            {
                string queryString = "INSERT INTO comments(author_id, comment) VALUES(@uid, @comment)";
                SqlCommand command = new SqlCommand(queryString);
                command.Parameters.Add("@uid", SqlDbType.UniqueIdentifier).Value = Guid.Parse(uid);
                command.Parameters.Add("@comment", SqlDbType.NVarChar).Value = comment;
                DBRun(command);
            }

            public static DataTable ReadAllComment()
            {
                string queryString = "SELECT A.*, B.username FROM[comments]A, [users]B "
                    + "WHERE A.author_id=B.uid "
                    + "ORDER BY A.id DESC";
                SqlCommand command = new SqlCommand(queryString);
                return DBQuery(command);
            }

            public static DataTable ReadUserComment(string uid)
            {
                string queryString = "SELECT A.*, B.username FROM[comments]A, [users]B "
                    + "WHERE A.author_id=B.uid and [author_id]=@uid "
                    + "ORDER BY A.id DESC";
                SqlCommand command = new SqlCommand(queryString);
                command.Parameters.Add("@uid", SqlDbType.UniqueIdentifier).Value = Guid.Parse(uid);
                return DBQuery(command);
            }

            public static DataTable ReadUserComment(string uid, string cid)
            {
                string queryString = "SELECT A.*, B.username FROM[comments]A, [users]B "
                    + "WHERE A.author_id=B.uid and [author_id]=@uid "
                    + "and A.comment_id=@cid";
                SqlCommand command = new SqlCommand(queryString);
                command.Parameters.Add("@uid", SqlDbType.UniqueIdentifier).Value = Guid.Parse(uid);
                command.Parameters.Add("@cid", SqlDbType.UniqueIdentifier).Value = Guid.Parse(cid);
                return DBQuery(command);
            }



            public static void UpdateComment(string cid, string uid, string comment)
            {
                SqlCommand command = new SqlCommand();
                string queryString = "UPDATE[order_details] SET ";

                queryString += "author_id = @uid ";
                command.Parameters.Add("@uid", SqlDbType.UniqueIdentifier).Value = Guid.Parse(uid);

                queryString += "comment = @comment ";
                command.Parameters.Add("@comment", SqlDbType.NVarChar).Value = comment;

                queryString += "edited_time = @now ";
                command.Parameters.Add("@now", SqlDbType.DateTime).Value = DateTime.Now;

                queryString += "WHERE comment_id = @cid";
                command.Parameters.Add("@cid", SqlDbType.UniqueIdentifier).Value = Guid.Parse(cid);

                command.CommandText = queryString;
                DBRun(command);
            }

            public static void DeleteComment(string cid)
            {
                SqlCommand command = new SqlCommand();
                string queryString = "DELETE FROM [comments] ";
                queryString += "WHERE comment_id = @cid";
                command.Parameters.Add("@cid", SqlDbType.UniqueIdentifier).Value = Guid.Parse(cid);

                command.CommandText = queryString;
                DBRun(command);
            }
        }
    }
}