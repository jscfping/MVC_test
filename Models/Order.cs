using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Configuration;
using System.Data.SqlClient;

/// <summary>
/// Order 的摘要描述
/// </summary>
/// 
public partial class ASPdemo
{
    public class Order
    {
        public static void ShopcartToOrder(UserInfo usr)
        {
            string newid = DatabaseFunc.Order.CreateNewOrder(usr.Uid).ToString();
            DatabaseFunc.Order.UpdateOrderDetailOid(usr.ShoppingCartId, newid);
        }
    }

    public class OrderDetail
    {
        public string order_id = "";
        public string item_id = "";
        public string price = "";
        public string quantity = "";

        public void ToShopcart()
        {
            DataTable dt = DatabaseFunc.Order.GetShopcart(order_id, item_id);

            if (dt == null)
            {
                DatabaseFunc.Order.CreateNewOrderDetail(order_id, item_id,
                    price, quantity);
            }
            else
            {
                double oldqty = double.Parse(dt.Rows[0]["quantity"].ToString());
                double addqty = double.Parse(quantity);
                OrderDetail od = new OrderDetail
                {
                    quantity = (oldqty + addqty).ToString()
                };
                DatabaseFunc.Order.UpdateOrderDetail(
                    dt.Rows[0]["order_detail_id"].ToString(), od);
            }
        }
    }
}




public partial class ASPdemo
{
    public partial class DatabaseFunc
    {
        public class Order : DatabaseFunc
        {

            public static void CreateNewShopcart(Guid uid)
            {
                string queryString = "INSERT INTO shop_carts(customer_id) VALUES(@uid)";
                SqlCommand command = new SqlCommand(queryString);
                command.Parameters.Add("@uid", SqlDbType.UniqueIdentifier).Value = uid;
                DBRun(command);
            }

            public static Guid CreateNewOrder(string uid)
            {
                Guid orderId = Guid.NewGuid();
                string queryString = "INSERT INTO orders(order_id, customer_id) VALUES(@order_id ,@uid)";
                SqlCommand command = new SqlCommand(queryString);
                command.Parameters.Add("@order_id", SqlDbType.UniqueIdentifier).Value = orderId;
                command.Parameters.Add("@uid", SqlDbType.UniqueIdentifier).Value = Guid.Parse(uid);
                DBRun(command);

                return orderId;
            }

            public static void CreateNewOrderDetail(string order_id,
                 string item_id, string price, string quantity)
            {
                string queryString = "INSERT INTO order_details"
                    + "(order_id, item_id, price, quantity) "
                    + "VALUES(@oid, @iid, @pce, @qty)";
                SqlCommand command = new SqlCommand(queryString);
                command.Parameters.Add("@oid", SqlDbType.UniqueIdentifier).Value = Guid.Parse(order_id);
                command.Parameters.Add("@iid", SqlDbType.UniqueIdentifier).Value = Guid.Parse(item_id);
                command.Parameters.Add("@pce", SqlDbType.Float).Value = double.Parse(price);
                command.Parameters.Add("@qty", SqlDbType.Float).Value = double.Parse(quantity);
                DBRun(command);
            }


            public static DataTable GetShopcart(string order_id)
            {
                string queryString = "SELECT A.* "
                    + "FROM [order_details]A "
                    + "WHERE A.order_id=@oid";
                SqlCommand command = new SqlCommand(queryString);
                command.Parameters.Add("@oid", SqlDbType.UniqueIdentifier).Value = Guid.Parse(order_id);
                return DBQuery(command);
            }

            public static DataTable GetShopcart(string order_id,
                 string item_id)
            {
                string queryString = "SELECT A.* "
                    + "FROM [order_details]A "
                    + "WHERE A.order_id=@oid and A.item_id=@iid";
                SqlCommand command = new SqlCommand(queryString);
                command.Parameters.Add("@oid", SqlDbType.UniqueIdentifier).Value = Guid.Parse(order_id);
                command.Parameters.Add("@iid", SqlDbType.UniqueIdentifier).Value = Guid.Parse(item_id);
                return DBQuery(command);
            }


            //could be better
            public static void UpdateOrderDetail(string order_detail_id,
                OrderDetail od)
            {
                SqlCommand command = new SqlCommand();
                string queryString = "UPDATE[order_details] SET ";

                //studid...
                bool addDot = false;
                if (od.order_id != "")
                {
                    queryString += "order_id = @order_id";
                    command.Parameters.Add("@order_id", SqlDbType.UniqueIdentifier).Value = Guid.Parse(od.order_id);
                    addDot = true;
                }
                if (od.item_id != "")
                {
                    if (addDot)
                    {
                        queryString += ", ";
                    }
                    queryString += "item_id = @item_id";
                    command.Parameters.Add("@item_id", SqlDbType.UniqueIdentifier).Value = Guid.Parse(od.item_id);
                    addDot = true;
                }
                if (od.price != "")
                {
                    if (addDot)
                    {
                        queryString += ", ";
                    }
                    queryString += "price = @price";
                    command.Parameters.Add("@price", SqlDbType.Float).Value = double.Parse(od.price);
                    addDot = true;
                }
                if (od.quantity != "")
                {
                    if (addDot)
                    {
                        queryString += ", ";
                    }
                    queryString += "quantity = @quantity";
                    command.Parameters.Add("@quantity", SqlDbType.Float).Value = double.Parse(od.quantity);
                }
                queryString += " WHERE order_detail_id = @order_detail_id";
                command.Parameters.Add("@order_detail_id", SqlDbType.UniqueIdentifier).Value = Guid.Parse(order_detail_id);
                command.CommandText = queryString;
                DBRun(command);
            }


            public static void UpdateOrderDetailOid(
                string oldOid, string newOid)
            {
                string queryString = "UPDATE[order_details] SET "
                    + "order_id = @newOid "
                    + "WHERE order_id = @oldOid";
                SqlCommand command = new SqlCommand(queryString);
                command.Parameters.Add("@oldOid", SqlDbType.UniqueIdentifier).Value = Guid.Parse(oldOid);
                command.Parameters.Add("@newOid", SqlDbType.UniqueIdentifier).Value = Guid.Parse(newOid);
                DBRun(command);
            }




            }
    }
}