using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Configuration;
using System.Data.SqlClient;

/// <summary>
/// usrInfo 的摘要描述
/// </summary>
/// 
public partial class ASPdemo
{
    public class UserInfo
    {
        public string Uid { get; set; }
        public string Username { get; set; }
        public string ShoppingCartId { get; set; }

        public UserInfo(string uid, string username, string shoppingCartId)
        {
            Uid = uid;
            Username = username;
            ShoppingCartId = shoppingCartId;
        }

        public static void AddUser(string usr, string pw)
        {
            string saltedpw = ASPdemo.Func.Encrypt(ASPdemo.Func.Salt + pw);
            DataTable dt = DatabaseFunc.User.GetInfo(usr);

            if (dt == null)
            {
                Guid uid = DatabaseFunc.User.Add(usr, saltedpw);
                DatabaseFunc.Order.CreateNewShopcart(uid);
            }
            else
            {
                throw new Exception("create fail, because username already exists");
            }
        }

        public void Checkout()
        {
            string shopCartId = Middle.GetCurrentUser().ShoppingCartId.ToString();
            DataTable dt = DatabaseFunc.Order.GetShopcart(shopCartId);
            if (dt != null)
            {
                Order.ShopcartToOrder(this);
            }
            else
            {
                throw new Exception("nothing in shopping cart");
            }
            
        }

        public DataTable GetShopCartDataTable()
        {
            return DatabaseFunc.Order.GetShopcart(ShoppingCartId);
        }


    }
}

public partial class ASPdemo
{
    public partial class DatabaseFunc
    {
        public class User : DatabaseFunc
        {

            public static Guid Add(string usr, string pw)
            {
                //there could not be a return id, so make it here
                Guid uId = Guid.NewGuid();
                string queryString = "INSERT INTO users(uid, username, password) VALUES(@uid, @usr, @pw)";
                SqlCommand command = new SqlCommand(queryString);
                command.Parameters.Add("@uid", SqlDbType.UniqueIdentifier).Value = uId;
                command.Parameters.Add("@usr", SqlDbType.NVarChar, 50).Value = usr;
                command.Parameters.Add("@pw", SqlDbType.NVarChar, 256).Value = pw;
                DBRun(command);
                return uId;
            }

            public static DataTable GetInfo(string usr)
            {
                string qy = "SELECT A.*, B.shop_cart_id "
                    + "FROM [users]A, [shop_carts]B "
                    + "WHERE A.username = @usr and B.customer_id=A.uid";
                SqlCommand command = new SqlCommand(qy);
                command.Parameters.AddWithValue("@usr", usr);

                return DBQuery(command);
            }

        }
    }
}

