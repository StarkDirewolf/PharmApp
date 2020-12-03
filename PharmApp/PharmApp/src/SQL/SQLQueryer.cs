using PharmApp;
using PharmApp.src;
using PharmApp.src.Product_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp
{
    class SQLQueryer
    {
        private const string CONNECTION_STRING = @"Data Source=12212360VSVR\PHARMACYE14;Initial Catalog=ProScriptConnect;Integrated Security=true;";

        public static Dictionary<string, string> GetDeliveryNHSNumbers(DateTime fromDay, DateTime toDay, bool onlyNomads)
        {
            QueryConstructor query = new QueryConstructor(QueryConstructor.QueryType.PATIENTS_WITH_NOTES_DISPENSED_TO);

            Dictionary<string, string> nhsNumNameLookup = new Dictionary<string, string>();

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                if (fromDay.Date == toDay.Date)
                {
                    query.SpecificDay(fromDay);
                } else
                {
                    query.BetweenDays(fromDay, toDay);
                }
                
                query.SortBy();
                SqlCommand command = new SqlCommand(query.ToString(), connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        // NHS number first
                        // Patient first name second
                        // Patient second name third
                        // Patient notes fourth
                        string nhsNum = reader[0].ToString();
                        if (!nhsNumNameLookup.ContainsKey(nhsNum))
                        {
                            if (reader[4].ToString().Equals("1"))
                            {
                                nhsNumNameLookup.Add(nhsNum, reader[2] + ", " + reader[1]);
                            } else
                            {
                                if (reader[3].ToString().ToLower().Contains("deliver"))
                                {
                                    if (onlyNomads)
                                    {
                                        if (reader[3].ToString().ToLower().Contains("nomad") || reader[3].ToString().ToLower().Contains("dosset")
                                            || reader[3].ToString().ToLower().Contains("doset"))
                                        {
                                            nhsNumNameLookup.Add(nhsNum, reader[2] + ", " + reader[1]);
                                        }
                                    }
                                    else
                                    {
                                        nhsNumNameLookup.Add(nhsNum, reader[2] + ", " + reader[1]);
                                    }

                                }
                            }
                            
                        }
                    }
                }
                
            }

            return nhsNumNameLookup;
        }

        public static OrderHistory GetOrderHistory(string pip, DateTime fromDate, DateTime endDate)
        {
            QueryConstructor query = new QueryConstructor(QueryConstructor.QueryType.ORDERHISTORY);

            query.PipCode(pip);
            query.BetweenDays(fromDate, endDate);

            OrderHistory history = new OrderHistory();

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query.ToString(), connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime orderDate = reader.GetDateTime(0);
                        string description = reader.GetString(1);
                        string statusComment = reader.GetString(2);
                        decimal orderQty = reader.GetDecimal(3);
                        decimal receivedQty = reader.GetDecimal(4);
                        string supplier = reader.GetString(5);

                        OrderLine line = new OrderLine(description, orderDate, supplier, orderQty, receivedQty, statusComment);
                        history.addOrderLine(line);
                    }
                }
            }

            return history;
        }

        public static Product PopulateFromPIP(string pip)
        {
            QueryConstructor query = new QueryConstructor(QueryConstructor.QueryType.PRODUCTINFO);

            query.PipCode(pip);

            Product prod = new Product();
            prod.pipcode = pip;

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query.ToString(), connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Could possibly be null if not found, e.g. own drugs? Dont think this is the case anymore
                        prod.description = reader.GetString(0);
                        prod.genericID = reader.GetString(1);
                        prod.unitsPerPack = reader.GetDecimal(2);
                        prod.isGeneric = reader.GetBoolean(3);
                        prod.supplier = reader.GetString(4);
                    }
                }

                //query = new QueryConstructor(QueryConstructor.QueryType.ORDERPAD);
                //query.PrepCode(prod.preparationCode);
                //query.NotDeleted();

                //command = new SqlCommand(query.ToString(), connection);

                //using (SqlDataReader reader = command.ExecuteReader())
                //{
                //    prod.quantity = 0;
                //    while (reader.Read())
                //    {
                //        prod.quantity += reader.GetDecimal(1);
                //    }
                //}
            }

            return prod;
        }

        public static bool NewETPs(string nhsNum, out bool unprinted, out bool batch)
        {
            QueryConstructor query = new QueryConstructor(QueryConstructor.QueryType.ETPSCRIPTS);

            query.NewETP();
            query.NhsNumber(nhsNum);

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query.ToString(), connection);

                unprinted = false;
                batch = false;
                bool anyResults = false;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        anyResults = true;
                        if (!reader.IsDBNull(4))
                        {
                            batch = true;
                            anyResults = false;
                        }

                        if (reader.IsDBNull(3))
                        {
                            unprinted = true;
                        }
                        else
                        {
                            if (reader.GetInt32(3) == 0)
                            {
                                unprinted = true;
                            }

                        }

                        //int tokensPrinted = reader.GetInt32(3);
                        //if (tokensPrinted > 0)
                        //{
                        //    unprinted = true;
                        //    return true;
                        //}
                    }
                }

                return anyResults;
            }
        }

        //public static Product SearchOrderPad(string pipcode)
        //{
        //    QueryConstructor query = new QueryConstructor(QueryConstructor.QueryType.PREPARATIONCODE);

        //    query.PipCode(pipcode);

        //    Product prod = new Product();

        //    using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
        //    {
        //        connection.Open();

        //        SqlCommand command = new SqlCommand(query.ToString(), connection);

        //        using (SqlDataReader reader = command.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                // Could possibly be null if not found, e.g. own drugs? Dont think this is the case anymore
        //                prod.preparationCode = reader.GetInt64(0).ToString();
        //            }
        //        }

        //        query = new QueryConstructor(QueryConstructor.QueryType.ORDERPAD);
        //        query.PrepCode(prod.preparationCode);
        //        query.NotDeleted();

        //        command = new SqlCommand(query.ToString(), connection);

        //        using (SqlDataReader reader = command.ExecuteReader())
        //        {
        //            prod.quantity = 0;
        //            while (reader.Read())
        //            {
        //                prod.quantity += reader.GetDecimal(1);
        //            }
        //        }
        //    }

        //    return prod;
        //}
    }
}
