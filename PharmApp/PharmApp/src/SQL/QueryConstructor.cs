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

        private const string ORDERPAD = @"
SELECT O.Description, O.Quantity, O.WholeSalerId, O.PageNo FROM PKBRuntime.Dmd.ActualProductPack A
JOIN PKBRuntime.Mapping.DmdPack D ON A.ActualProductPackId = D.DmdProductPackCodeId
JOIN ProScriptConnect.Ordering.OrderPad O ON D.PackCodeId = O.PackCodeId";

        private const string FILTER = " WHERE ";

        private const string FILTER_DATE = "CONVERT(VARCHAR(10), V.AddedDate, 111) = ";

        private const string DATE_FORMAT = "yyyy/MM/dd";

        private const string FILTER_NHSNO = "PatientNHSNumber = '";
        private const string FILTER_QUOTE_END = "'";

        private const string FILTER_NEW_ETP = "s.PrescriptionStatusId = 3";
        private const string FILTER_TO_BE_DISPENSED = "i.PrescriptionItemStatusId = 1";
        private const string FILTER_PIP = "OrderCode = '";
        private const string FILTER_NOT_DELETED = "Deleted = 0";
        private const string FILTER_VIRTUAL_ID = "VirtualProductPackId = '";

        private const string FILTER_AND = " AND ";

        private readonly QueryType type;

        private readonly List<KeyValuePair<Condition, string>> conditions = new List<KeyValuePair<Condition, string>>();

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
            VIRTUALPRODCODE
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
            VIRTUALID
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
            string dateString = day.ToString(DATE_FORMAT);
            RemoveCondition(Condition.DATE);
            RemoveCondition(Condition.BETWEEN_DATES);
            AddCondition(Condition.DATE, dateString);
        }

        public void NewETP()
        {
            RemoveCondition(Condition.NEWETP);
            AddCondition(Condition.NEWETP, null);
        }

        public void ToBeDispensed()
        {
            RemoveCondition(Condition.TOBEDISPENSED);
            AddCondition(Condition.TOBEDISPENSED, null);
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
        public void SortBy()
        {
            sort = "ORDER BY P.Surname";
        }

        public void BetweenDays(DateTime fromDay, DateTime toDay)
        {
            string str = "";
            for (DateTime day = fromDay; (toDay.Date - day.Date).Days >= 0; day = day.AddDays(1))
            {
                str = str + FILTER_DATE + "'" + day.ToString(DATE_FORMAT) + "' ";
                if (toDay.Date != day.Date) str += "OR ";
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
            conditions.Add(new KeyValuePair<Condition, string>(cond, data));
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
                        str = FILTER_PIP + condition.Value + FILTER_QUOTE_END;
                        break;

                    case Condition.NOTDELETED:
                        str = FILTER_NOT_DELETED;
                        break;

                    case Condition.VIRTUALID:
                        str = FILTER_VIRTUAL_ID + condition.Value + FILTER_QUOTE_END;
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
