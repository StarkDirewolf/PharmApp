using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp
{
    /// <summary>
    /// Object to build and represent a string to be used for a query, eventually using the getString() method.
    /// </summary>
    public class QueryConstructor
    {

        private const string DISPENSED = @"
SELECT V.DrugDescription, V.Quantity, Pa.Description, Pa.UnitsPerPack
FROM PMR.PrescriptionCollectionSummaryView V
JOIN PMR.PrescriptionItemDispensed P ON P.PrescriptionItemId = V.PrescriptionItemId
JOIN PackSearchView Pa ON P.PackCodeId = Pa.PackCodeId";

        // NHS number first
        // Patient first name second
        // Patient second name third
        // Patient notes fourth
        private const string PATIENTS_WITH_NOTES_DISPENSED_TO = @"
SELECT P.NhsNumber, P.GivenName, P.Surname, N.Note, X.PatientPropertyId 
FROM PMR.PrescriptionCollectionSummaryView V 
JOIN dbo.PatientLiteView P ON V.PatientId = P.PatientId 
JOIN dbo.PatientNote N ON V.PatientId = N.PatientId
LEFT JOIN dbo.PatientPatientProperty X ON X.PatientId = V.PatientId ";

        private const string ETPSCRIPTS = @"
SELECT DISTINCT i.DrugDescription, s.PatientGivenName, s.PatientSurname, s.TokenPrinted, s.RepeatNumber, v.ApprovedName, v.Strength, v.DrugForm FROM ETP.EtpSummaryView s
LEFT JOIN ETP.EtpSummaryItemView i ON s.PrescriptionId = i.PrescriptionId
LEFT JOIN PKBRuntime.Mapping.DmdPreparation p ON i.DrugIdentifier = p.DmdProductCodeId
LEFT JOIN PKBRuntime.Pharmacy.Preparation v ON p.PreparationCodeId = v.PreparationCodeId";

        private const string VIRTUALPRODCODE = @"
SELECT V.VirtualProductPackId FROM PKBRuntime.Pharmacy.PackOrderCode O
LEFT JOIN PKBRuntime.Mapping.DmdPack D ON O.PackCodeId = D.PackCodeId
LEFT JOIN PKBRuntime.Dmd.ActualProductPack V ON D.DmdProductPackCodeId = V.ActualProductPackId";

        private const string PREPARATIONCODE = @"
SELECT TOP 1 P.PreparationCodeId FROM PKBRuntime.Pharmacy.PackOrderCode O
JOIN PKBRuntime.Pharmacy.PreparationPack P ON O.PackCodeId = P.PackCodeId";

        //        private const string ORDERPAD = @"
        //SELECT O.Description, O.Quantity, O.WholeSalerId, O.PageNo FROM ProScriptConnect.Ordering.OrderPad O
        //JOIN PKBRuntime.Pharmacy.PreparationPack P ON O.PackCodeId = P.PackCodeId";

            // Just grabs orders on page 1 of both suppliers
        private const string ORDERPAD = @"
SELECT OrderCode, Quantity, WholeSalerId FROM ProScriptConnect.Ordering.OrderPad";

        private const string PRODUCTINFO = @"
SELECT Description, Gncs, UnitsPerPack, K.IsGeneric, SupplierName, Price, OrderingNotes, RequiresAction FROM PKBRuntime.Pharmacy.PackOrderCode O
JOIN PKBRuntime.Pharmacy.Pack K ON O.PackCodeId = K.PackCodeId
JOIN PKBRuntime.Pharmacy.PreparationPack P ON K.PreparationCodeId = P.PreparationCodeId AND K.PackCodeId = P.PackCodeId
JOIN (SELECT * FROM PKBRuntime.Pharmacy.PreparationSearchView WHERE RegionId = 0) V ON P.PreparationCodeId = V.PreparationCodeId
JOIN PKBRuntime.Pharmacy.Supplier S ON K.SupplierId = S.SupplierId
JOIN (SELECT * FROM PKBRuntime.Pharmacy.PackRegion R WHERE RegionId = 0) R ON O.PackCodeId = R.PackCodeId
LEFT JOIN App.dbo.CustomNotes A ON O.OrderCode = A.Pipcode";

        private const string ORDERHISTORY = @"
SELECT DateModified, I.Description, I.OrderStatusReason, I.OrderQuantity, I.ReceivedQuantity, I.SuppliedBy, OrderHistoryItemId FROM ProScriptConnect.Ordering.OrderHistoryItem I
JOIN ProScriptConnect.Ordering.OrderHistory O ON I.OrderHistoryId = O.OrderHistoryId
JOIN PKBRuntime.Pharmacy.PreparationPack P ON I.PackCodeId = P.PackCodeId";

        private const string PRODUCTPATIENTHISTORY = @"
SELECT DISTINCT V.AddedDate, Pat.Surname, Pat.CallingName, Pa.Description, P.Quantity
FROM ProScriptConnect.PMR.PrescriptionCollectionSummaryView V
JOIN ProScriptConnect.PMR.PrescriptionItemDispensed P ON P.PrescriptionItemId = V.PrescriptionItemId
JOIN ProScriptConnect.dbo.PackSearchView Pa ON P.PackCodeId = Pa.PackCodeId
JOIN ProScriptConnect.dbo.Patient Pat ON V.PatientId = Pat.PatientId
JOIN PKBRUntime.Pharmacy.PreparationPack K ON P.PackCodeId = K.PackCodeId";

        private const string UPDATEORDERINGNOTE_1 = @"
MERGE
INTO App.dbo.CustomNotes WITH (HOLDLOCK) AS target
USING (SELECT '";

        private const string UPDATEORDERINGNOTE_2 = @"' AS PipCode, '";

        private const string UPDATEORDERNOTE_3 = @"' AS OrderingNotes, ";

        private const string UPDATEORDERINGNOTE_4 = @" AS RequiresAction) AS source
(PipCode, OrderingNotes, RequiresAction)
ON (target.PipCode = source.PipCode)
WHEN MATCHED 
THEN UPDATE
SET OrderingNotes = source.OrderingNotes,
RequiresAction = source.RequiresAction
WHEN NOT MATCHED
THEN INSERT (PipCode, OrderingNotes, RequiresAction)
VALUES (source.PipCode, source.OrderingNotes, Source.RequiresAction);";

        private const string DELETEORDERINGNOTE = @"DELETE FROM App.dbo.CustomNotes";

        private const string GETORDERINGNOTE = @"SELECT OrderingNotes, RequiresAction FROM App.dbo.CustomNotes";

        private const string FILTER = " WHERE ";

        private const string FILTER_DATE = "CONVERT(VARCHAR(10), V.AddedDate, 111) = ";

        private const string FILTER_ORDER_HISTORY_DATE = "DateModified BETWEEN '";
        private const string FILTER_ORDER_HISTORY_DATE_MIDDLE = "' AND '";
        private const string FILTER_ORDER_HISTORY_DATE_END = "'";

        private const string DATE_FORMAT = "yyyy/MM/dd";
        private const string DATE_FORMAT_ORDER_HISTORY = "yyyy-MM-dd";

        private const string FILTER_NHSNO = "PatientNHSNumber = '";
        private const string FILTER_QUOTE_END = "'";

        private const string FILTER_NEW_ETP = "s.PrescriptionStatusId = 3";
        private const string FILTER_TO_BE_DISPENSED = "i.PrescriptionItemStatusId = 1";
        private const string FILTER_ORDERCODE = "OrderCode = '";
        private const string FILTER_PIPCODE = "PipCode = '";
        private const string FILTER_GENERIC_ORDER_HISTORY = "Gncs = '";
        private const string FILTER_NOT_DELETED = "Deleted = 0";
        private const string FILTER_VIRTUAL_ID = "VirtualProductPackId = '";
        private const string FILTER_PREP_CODE = "PreparationCodeId = '";
        private const string FILTER_PAGE = "PageNo = ";

        private const string FILTER_AND = " AND ";

        private const string ORDER_BY_DATE_MODIFIED = " ORDER BY DateModified DESC";
        private const string ORDER_BY_DATE_ADDED = " ORDER BY AddedDate DESC";

        private readonly QueryType type;

        private List<KeyValuePair<Condition, string>> conditions = new List<KeyValuePair<Condition, string>>();

        private string sort;

        public enum QueryType
        {
            /// <summary>
            /// Grabs all items dispensed, selecting DrugDescription, Quantity, Prescription, Description, and UnitsPerPack
            /// </summary>
            DISPENSED,
            PATIENTS_WITH_NOTES_DISPENSED_TO,
            ETPSCRIPTS,
            ORDERPAD,
            VIRTUALPRODCODE,
            PREPARATIONCODE,
            PRODUCTINFO,
            ORDERHISTORY,
            UPDATEORDERINGNOTE,
            DELETEORDERINGNOTE,
            GETORDERINGNOTE,
            PRODUCTPATIENTHISTORY
        }

        private enum Condition
        {
            /// <summary>
            /// Filters results by a specific day
            /// </summary>
            DATE,
            BETWEEN_DATES,
            NHSNO,
            NEWETP,
            TOBEDISPENSED,
            PIPCODE,
            NOTDELETED,
            VIRTUALID,
            PREPCODE,
            GENERICCODE,
            PAGENO,
            ORDERINGNOTE,
            REQUIRESACTION
        }

        /// <summary>Creates an object to represent a string used for a query.</summary>
        /// <param name="type"> the type of query to run</param>
        public QueryConstructor(QueryType type)
        {
            this.type = type;
        }

        /// <summary>
        /// Filters the query to the specified day
        /// </summary>
        /// <param name="day"> the day to filter by</param>
        public void SpecificDay(DateTime day)
        {
            string dateString;
            if (type == QueryType.ORDERHISTORY)
            {
                BetweenDays(day, day);
            }
            else
            {
                dateString = day.ToString(DATE_FORMAT);
                RemoveCondition(Condition.DATE);
                RemoveCondition(Condition.BETWEEN_DATES);
                AddCondition(Condition.DATE, dateString);
            }
            
        }

        public void NewETP()
        {
            RemoveCondition(Condition.NEWETP);
            AddCondition(Condition.NEWETP);
        }

        public void ToBeDispensed()
        {
            RemoveCondition(Condition.TOBEDISPENSED);
            AddCondition(Condition.TOBEDISPENSED);
        }

        public void NhsNumber(string nhsNumber)
        {
            RemoveCondition(Condition.NHSNO);
            AddCondition(Condition.NHSNO, nhsNumber);
        }

        public void PipCode(string pipcode)
        {
            RemoveCondition(Condition.PIPCODE);
            AddCondition(Condition.PIPCODE, pipcode);
        }

        public void VirtualID(string virtualID)
        {
            RemoveCondition(Condition.VIRTUALID);
            AddCondition(Condition.VIRTUALID, virtualID);
        }

        public void PrepCode(string prepCode)
        {
            RemoveCondition(Condition.PREPCODE);
            AddCondition(Condition.PREPCODE, prepCode);
        }

        public void NotDeleted()
        {
            RemoveCondition(Condition.NOTDELETED);
            AddCondition(Condition.NOTDELETED);
        }

        public void GenericCode(string code)
        {
            RemoveCondition(Condition.GENERICCODE);
            AddCondition(Condition.GENERICCODE, code);
        }
        
        public void PageNumber(string page)
        {
            RemoveCondition(Condition.PAGENO);
            AddCondition(Condition.PAGENO, page);
        }

        public void OrderingNote(string note)
        {
            RemoveCondition(Condition.ORDERINGNOTE);
            AddCondition(Condition.ORDERINGNOTE, note);
        }

        public void RequiresAction(bool requiresAction)
        {
            RemoveCondition(Condition.REQUIRESACTION);
            AddCondition(Condition.REQUIRESACTION, requiresAction.ToString());
        }

        /// <summary>
        /// Removes a condition from the query if there is one, otherwise does nothing
        /// </summary>
        /// <param name="cond">the type of condition to remove</param>
        private void RemoveCondition(Condition cond)
        {
            if (conditions.Exists(x => x.Key == cond))
            {
                var condition = conditions.Find(x => x.Key == cond);
                conditions.Remove(condition);
            }
        }

        // Temporary hardcode
        public void SortByName()
        {
            sort = "ORDER BY P.Surname";
        }

        public void SortByDateModified()
        {
            sort = ORDER_BY_DATE_MODIFIED;
        }

        public void SortByDateAdded()
        {
            sort = ORDER_BY_DATE_ADDED;
        }

        public void BetweenDays(DateTime fromDay, DateTime toDay)
        {
            string str = "";
            if (type == QueryType.ORDERHISTORY)
            {
                str = FILTER_ORDER_HISTORY_DATE;
                str += fromDay.ToString(DATE_FORMAT_ORDER_HISTORY);
                str += FILTER_ORDER_HISTORY_DATE_MIDDLE;
                str += toDay.AddDays(1).ToString(DATE_FORMAT_ORDER_HISTORY);
                str += FILTER_ORDER_HISTORY_DATE_END;
            }
            else
            {

                for (DateTime day = fromDay; (toDay.Date - day.Date).Days >= 0; day = day.AddDays(1))
                {
                    str = str + FILTER_DATE + "'" + day.ToString(DATE_FORMAT) + "' ";
                    if (toDay.Date != day.Date) str += "OR ";
                }
            }
            RemoveCondition(Condition.DATE);
            RemoveCondition(Condition.BETWEEN_DATES);
            AddCondition(Condition.BETWEEN_DATES, str);
        }

        /// <summary>
        /// Adds a condition to the list of conditions
        /// </summary>
        /// <param name="cond">the type of condition to add</param>
        /// <param name="data">how the condition is filtered</param>
        private void AddCondition(Condition cond, string data = "")
        {
            KeyValuePair<Condition, string> kv = new KeyValuePair<Condition, string>(cond, data);
            conditions.Add(kv);
        }

        private string ConditionsStringBuilder(List<KeyValuePair<Condition, string>> conditionList)
        {
            string str = "";
            if (conditionList.Count > 0)
            {
                KeyValuePair<Condition, string> condition = conditionList.First();
                switch (condition.Key)
                {
                    case Condition.DATE:
                        str = FILTER_DATE + "'" + condition.Value + "'";
                        break;

                    case Condition.BETWEEN_DATES:
                        str = condition.Value;
                        break;

                    case Condition.NEWETP:
                        str = FILTER_NEW_ETP;
                        break;

                    case Condition.TOBEDISPENSED:
                        str = FILTER_TO_BE_DISPENSED;
                        break;

                    case Condition.NHSNO:
                        str = FILTER_NHSNO + condition.Value + FILTER_QUOTE_END;
                        break;

                    case Condition.PIPCODE:
                        if (type == QueryType.ORDERHISTORY || type == QueryType.GETORDERINGNOTE || type == QueryType.DELETEORDERINGNOTE || type == QueryType.PRODUCTPATIENTHISTORY)
                        {
                            str = FILTER_PIPCODE + condition.Value + FILTER_QUOTE_END;
                        }
                        else
                        {
                            str = FILTER_ORDERCODE + condition.Value + FILTER_QUOTE_END;
                        }
                        break;

                    case Condition.NOTDELETED:
                        str = FILTER_NOT_DELETED;
                        break;

                    case Condition.VIRTUALID:
                        str = FILTER_VIRTUAL_ID + condition.Value + FILTER_QUOTE_END;
                        break;

                    case Condition.PREPCODE:
                        str = FILTER_PREP_CODE + condition.Value + FILTER_QUOTE_END;
                        break;

                    case Condition.GENERICCODE:
                        str = FILTER_GENERIC_ORDER_HISTORY + condition.Value + FILTER_QUOTE_END;
                        break;

                    case Condition.PAGENO:
                        str = FILTER_PAGE + condition.Value + " ";
                        break;
                }
                conditionList.RemoveAt(0);
                if (conditionList.Count > 0)
                {
                    str = str + FILTER_AND + ConditionsStringBuilder(conditionList);
                }
            }

            return str;
        }

        /// <summary>
        /// Used after specifying all conditions to generate a string to use for the query.
        /// </summary>
        /// <returns>string to be used when making an SQL query.</returns>
        public override string ToString()
        {
            string str = "";
            switch (type)
            {
                case QueryType.DISPENSED:
                    str = DISPENSED;
                    break;

                case QueryType.PATIENTS_WITH_NOTES_DISPENSED_TO:
                    str = PATIENTS_WITH_NOTES_DISPENSED_TO;
                    break;

                case QueryType.ETPSCRIPTS:
                    str = ETPSCRIPTS;
                    break;

                case QueryType.ORDERPAD:
                    str = ORDERPAD;
                    break;

                case QueryType.VIRTUALPRODCODE:
                    str = VIRTUALPRODCODE;
                    break;

                case QueryType.PREPARATIONCODE:
                    str = PREPARATIONCODE;
                    break;

                case QueryType.PRODUCTINFO:
                    str = PRODUCTINFO;
                    break;

                case QueryType.ORDERHISTORY:
                    str = ORDERHISTORY;
                    break;

                case QueryType.UPDATEORDERINGNOTE:
                    str = UPDATEORDERINGNOTE_1;
                    str += conditions.Find(c => c.Key == Condition.PIPCODE).Value;
                    str += UPDATEORDERINGNOTE_2;
                    str += conditions.Find(c => c.Key == Condition.ORDERINGNOTE).Value;
                    str += UPDATEORDERNOTE_3;
                    if (conditions.Find(c => c.Key == Condition.REQUIRESACTION).Value == "True")
                    {
                        str += "1";
                    }
                    else
                    {
                        str += "0";
                    }
                    str += UPDATEORDERINGNOTE_4;
                    conditions.Clear();
                    break;

                case QueryType.DELETEORDERINGNOTE:
                    str = DELETEORDERINGNOTE;
                    break;

                case QueryType.GETORDERINGNOTE:
                    str = GETORDERINGNOTE;
                    break;

                case QueryType.PRODUCTPATIENTHISTORY:
                    str = PRODUCTPATIENTHISTORY;
                    break;
            }

            if (conditions.Count > 0)
            {
                str = str + FILTER + ConditionsStringBuilder(conditions);
            }

            if(sort != null)
            {
                str += sort;
            }

            return str;
        }
    }
}
