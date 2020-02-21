using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// middle 的摘要描述
/// </summary>
/// 
public partial class ASPdemo
{
    public class Middle
    {


        public static bool IsLogin()
        {
            try
            {
                if (HttpContext.Current.Session["currentUser"] != null)
                {
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }

        public static void LoginedToGo()
        {
            if (IsLogin())
            {
                HttpContext.Current.Response.Redirect("/");
            }
        }
        public static void LoginedToGo(string where)
        {
            if (IsLogin())
            {
                HttpContext.Current.Response.Redirect(where);
            }
        }

        public static void NeedLogin()
        {
            if (!IsLogin())
            {
                HttpContext.Current.Response.Redirect("/login.aspx");
            }
        }

        public static void SetUserInfo(DataTable dt)
        {
            HttpContext.Current.Session["currentUser"] = new UserInfo(
                dt.Rows[0]["uid"].ToString(),
                dt.Rows[0]["username"].ToString(),
                dt.Rows[0]["shop_cart_id"].ToString());
            HttpContext.Current.Session["uid"] = dt.Rows[0]["uid"].ToString();
        }

        public static UserInfo GetCurrentUser()
        {
            if (IsLogin())
            {
                try
                {
                    return (UserInfo)HttpContext.Current.Session["currentUser"];
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                throw new Exception("no login");
            }
        }

        public static void AddUser(string usr, string pw)
        {
            UserInfo.AddUser(usr, pw);
        }

        public static void Login(string usr, string pw)
        {
            DataTable dt = DatabaseFunc.User.GetInfo(usr);

            if (dt != null)
            {
                string saltedpw = ASPdemo.Func.Encrypt(ASPdemo.Func.Salt + pw);

                if (saltedpw == (string)dt.Rows[0]["password"])
                {
                    SetUserInfo(dt);
                }
                else
                {
                    throw new System.Exception("password is wrong");
                }
            }
            else
            {
                throw new System.Exception("this username doesn't exists");
            }
        }

        public static void Logout(string where)
        {
            HttpContext.Current.Session["currentUser"] = null;
            HttpContext.Current.Session["uid"] = null;
            HttpContext.Current.Response.Redirect(where);
        }

        public static void PutInShopcart(string order_id,
                 string item_id, string price, string quantity)
        {
            OrderDetail od = new OrderDetail
            {
                order_id = order_id,
                item_id = item_id,
                price = price,
                quantity = quantity
            };
            od.ToShopcart();
        }



    }
}
