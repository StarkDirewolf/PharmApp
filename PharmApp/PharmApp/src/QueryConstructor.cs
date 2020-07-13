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
SELECT i.DrugDescription, s.PatientGivenName, s.PatientSurname, s.TokenPrinted, s.RepeatNumber, v.ApprovedName, v.Strength, v.DrugForm FROM ETP.EtpSummaryView s
LEFT JOIN ETP.EtpSummaryItemView i ON s.PrescriptionId = i.PrescriptionId
LEFT JOIN PKBRuntime.Mapping.DmdPreparation p ON i.DrugIdentifier = p.DmdProductCodeId
LEFT JOIN PKBRuntime.Pharmacy.Preparation v ON p.PreparationCodeId = v.PreparationCodeId";

        private const string FILTER = " WHERE ";

        private const string FILTER_DATE = "CONVERT(VARCHAR(10), V.AddedDate, 111) = ";

        private const string DATE_FORMAT = "yyyy/MM/dd";

        private const string FILTER_NHSNO = "PatientNHSNumber = '";
        private const string FILTER_NHSNO_END = "'";

        private const string FILTER_NEW_ETP = "s.PrescriptionStatusId = 3";
        private const string FILTER_TO_BE_DISPENSED = "i.PrescriptionItemStatusId = 1";

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
            ETPSCRIPTS
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
            TOBEDISPENSED
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
        private void AddCondition(Condition cond, string data)
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
                        str = FILTER_NHSNO + condition.Value + FILTER_NHSNO_END;
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
