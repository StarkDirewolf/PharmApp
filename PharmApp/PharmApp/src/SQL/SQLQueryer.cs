using PharmApp;
using PharmApp.src;
using PharmApp.src.GUI;
using PharmApp.src.Product_Info;
using PharmApp.src.Requests;
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

        public static bool FindSurgeryDetailsFromCode(int code, out string name, out string email)
        {
            QueryConstructor query = new QueryConstructor(QueryConstructor.QueryType.SURGERYDETAILS);
            query.SurgeryCode(code);

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query.ToString(), connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        name = reader[0].ToString();
                        if (reader.IsDBNull(1))
                        {
                            email = Util.NOTFOUND;
                        }
                        else
                        {
                            email = reader[1].ToString();
                        }
                        return true;
                    }

                    // Not found if method hasn't returned yet
                    name = Util.NOTFOUND;
                    email = Util.NOTFOUND;
                    return false;
                }
            }
        }

        public static void SaveSurgeryEmail(Surgery surgery, string email)
        {
            QueryConstructor query = new QueryConstructor(QueryConstructor.QueryType.SAVESURGERYEMAIL);
            query.SurgeryCode(surgery.id);
            query.Email(email);

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query.ToString(), connection);

                command.ExecuteNonQuery();
            }
        }

        public static void UpdateSentRequests(List<Request> requests, Surgery surgery, string id)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                QueryConstructor insertSentEmail = new QueryConstructor(QueryConstructor.QueryType.RECORDSENTEMAIL);

                insertSentEmail.EmailID(id);
                insertSentEmail.SurgeryCode(surgery.id);

                foreach(Request request in requests)
                {
                    insertSentEmail.RequestID(request.ID);

                    SqlCommand sentEmailCommand = new SqlCommand(insertSentEmail.ToString(), connection);
                    sentEmailCommand.ExecuteNonQuery();

                    if (request.Status == Request.StatusType.TOBEREQUESTED)
                    {
                        QueryConstructor changeTrackStatus = new QueryConstructor(QueryConstructor.QueryType.CHANGEREQUESTSTATUS);

                        changeTrackStatus.RequestID(request.ID);
                        changeTrackStatus.RequestStatus(((int)Request.StatusType.REQUESTED).ToString());

                        SqlCommand statusCommand = new SqlCommand(changeTrackStatus.ToString(), connection);
                        statusCommand.ExecuteNonQuery();
                    }
                }

                

                
            }
        }

        public static void CleanRMS1()
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(QueryConstructor.CLEAN_RMS_1, connection);
                command.ExecuteNonQuery();
            }
        }

        public static void CleanRMS2()
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(QueryConstructor.CLEAN_RMS_2, connection);
                command.ExecuteNonQuery();
            }
        }

        public static void CleanRMS3()
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(QueryConstructor.CLEAN_RMS_3, connection);
                command.ExecuteNonQuery();
            }
        }

        public static void PopulateRequests()
        {

            RequestManager requestManager = RequestManager.Get();

            requestManager.ClearData();

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(QueryConstructor.GETREQUESTS, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        DateTime dateAdded = reader.GetDateTime(0);
                        int requestId = reader.GetInt32(1);
                        int requestItemId = reader.GetInt32(2);
                        int requestTrackingId = reader.GetInt32(3);
                        Nullable<int> itemTrackingId = null;
                        if (!reader.IsDBNull(4)) itemTrackingId = reader.GetInt32(4);
                        int surgeryId = reader.GetInt32(5);
                        Request.StatusType requestStatus = (Request.StatusType)reader.GetByte(6);
                        string surgeryName = reader.GetString(7);
                        string surgeryEmail = null;
                        if (!reader.IsDBNull(8)) surgeryEmail = reader.GetString(8);
                        int patientId = reader.GetInt32(9);
                        string patientFirstName = reader.GetString(10);
                        string patientLastName = reader.GetString(11);
                        DateTime patientDob = reader.GetDateTime(12);
                        string addressNumber = reader.GetString(13);
                        string addressLine1 = reader.GetString(14);
                        string addressPostcode = reader.GetString(15);
                        string requestNotes = reader.GetString(16);
                        string itemName = reader.GetString(17);
                        string itemStrength = reader.GetString(18);
                        Nullable<decimal> itemQty = null;
                        if(!reader.IsDBNull(19)) itemQty = reader.GetDecimal(19);
                        string itemForm = reader.GetString(20);
                        string itemNotes = null;
                        if (!reader.IsDBNull(21)) itemNotes = reader.GetString(21);

                        bool requestNeedsAdding = false;

                        Request request = requestManager.findRequestByID(requestId);

                        if (request == null)
                        {
                            request = new Request(requestId, (Request.StatusType)requestStatus, dateAdded, requestNotes, requestTrackingId);
                            requestNeedsAdding = true;
                            requestManager.AddRequest(request);
                        }

                        if (!request.ContainsItem(requestItemId))
                        {
                            RequestItem requestItem = new RequestItem(requestItemId, itemName, itemStrength, itemForm, itemQty, itemNotes, itemTrackingId);

                            request.AddItem(requestItem);
                        }
                        


                        Surgery surgery = requestManager.findSurgeryByID(surgeryId);

                        if (surgery == null)
                        {
                            surgery = new Surgery(surgeryId, surgeryName, surgeryEmail);
                            requestManager.AddSurgery(surgery);
                        }


                        Patient patient = requestManager.findPatientByID(patientId);

                        if (patient == null)
                        {
                            patient = new Patient(patientId, addressNumber + " " + addressLine1, addressPostcode, patientFirstName, patientLastName, patientDob);
                            surgery.AddPatient(patient);
                            requestManager.AddPatient(patient);
                        }

                        if (requestNeedsAdding) patient.AddRequest(request);

                    }
                }
            }

        }


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
                
                query.SortByName();
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

        public static object GetProductPatientHistory(Product product)
        {
            QueryConstructor query = new QueryConstructor(QueryConstructor.QueryType.PRODUCTPATIENTHISTORY);

            if (product.isGeneric)
            {
                query.GenericCode(product.genericID);
            }
            else
            {
                query.PipCode(product.pipcode);
            }

            query.SortByDateAdded();

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query.ToString(), connection);

                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = command;

                DataTable table = new DataTable();
                adapter.Fill(table);
                
                return table;
            }
        }

        public static OrderHistory GetOrderHistory(string code, DateTime fromDate, DateTime endDate, bool byGenericID)
        {
            QueryConstructor query = new QueryConstructor(QueryConstructor.QueryType.ORDERHISTORY);

            if (byGenericID)
            {
                query.GenericCode(code);
            }
            else
            {
                query.PipCode(code);
            }
            
            query.BetweenDays(fromDate, endDate);
            query.SortByDateModified();

            OrderHistory history = new OrderHistory();

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query.ToString(), connection);

                int itemID = 0;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int thisItemID = reader.GetInt32(6);
                        if (itemID == thisItemID)
                        {
                            continue;
                        }
                        itemID = thisItemID;

                        DateTime orderDate = reader.GetDateTime(0);
                        string description = reader.GetString(1);
                        string statusComment;
                        if (reader.IsDBNull(2))
                        {
                            statusComment = "NONE";
                        }
                        else
                        {
                            statusComment = reader.GetString(2);
                        }
                        decimal orderQty = reader.GetDecimal(3);
                        decimal receivedQty = reader.GetDecimal(4);
                        string supplier;
                        if (reader.IsDBNull(5))
                        {
                            supplier = "NONE";
                        }
                        else
                        {
                            supplier = reader.GetString(5);
                        }

                        OrderHistoryLine line = new OrderHistoryLine(description, orderDate, supplier, orderQty, receivedQty, statusComment);
                        history.AddOrderLine(line);
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
                        prod.dtPrice = reader.GetDecimal(5);
                        if (!reader.IsDBNull(6))
                        {
                            OrderingNote note = new OrderingNote(reader.GetString(6), reader.GetBoolean(7));
                            prod.orderingNote = note;
                        }
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

        public static OrderPadQuantities GetOrderPadLine(string pip)
        {
            QueryConstructor query = new QueryConstructor(QueryConstructor.QueryType.ORDERPAD);
            query.PageNumber("1");
            query.NotDeleted();
            query.PipCode(pip);

            Decimal eCass = 0;
            Decimal aah = 0;

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query.ToString(), connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.GetInt32(2) == 1)
                        {
                            aah += reader.GetDecimal(1);
                        }
                        else if (reader.GetInt32(2) == 2)
                        {
                            eCass += reader.GetDecimal(1);
                        }
                    }
                }
            }

            return new OrderPadQuantities(eCass, aah);
        }

        public static List<string> GetOrderPadPIPs()
        {
            QueryConstructor query = new QueryConstructor(QueryConstructor.QueryType.ORDERPAD);
            query.PageNumber("1");
            query.NotDeleted();

            List<string> pips = new List<string>();

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {

                connection.Open();

                SqlCommand command = new SqlCommand(query.ToString(), connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pips.Add(reader.GetString(0));
                    }
                }
            }

            return pips;
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

        public static bool SaveOrderingNote(string pipcode, OrderingNote orderingNote)
        {
            QueryConstructor query = new QueryConstructor(QueryConstructor.QueryType.UPDATEORDERINGNOTE);
            query.PipCode(pipcode);
            query.OrderingNote(orderingNote.note);
            query.RequiresAction(orderingNote.requiresAction);

            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query.ToString(), connection);

                rowsAffected = command.ExecuteNonQuery();
            }

            return (rowsAffected > 0);
        }

        public static bool DeleteOrderingNote(string pipcode)
        {
            QueryConstructor query = new QueryConstructor(QueryConstructor.QueryType.DELETEORDERINGNOTE);
            query.PipCode(pipcode);

            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query.ToString(), connection);

                rowsAffected = command.ExecuteNonQuery();
            }

            return (rowsAffected > 0);
        }

        public static OrderingNote GetOrderingNote(string pipcode)
        {
            QueryConstructor query = new QueryConstructor(QueryConstructor.QueryType.GETORDERINGNOTE);
            query.PipCode(pipcode);

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query.ToString(), connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            return new OrderingNote(reader.GetString(0), reader.GetBoolean(1));
                        }
                    }
                }
            }

            return null;
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
