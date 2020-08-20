//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PharmApp
//{
//    /// <summary>
//    /// Object to build and represent a string to be used for a query, eventually using the getString() method.
//    /// </summary>
//    public class QueryConstructor
//    {

//        private const string DISPENSED = @"
//SELECT V.DrugDescription, V.Quantity, Pa.Description, Pa.UnitsPerPack
//FROM PMR.PrescriptionCollectionSummaryView V
//JOIN PMR.PrescriptionItemDispensed P ON P.PrescriptionItemId = V.PrescriptionItemId
//JOIN PackSearchView Pa ON P.PackCodeId = Pa.PackCodeId";

//        private const string ETPSCRIPTS = @"
//SELECT i.DrugDescription, s.PatientGivenName, s.PatientSurname, s.TokenPrinted, s.RepeatNumber, v.ApprovedName, v.Strength, v.DrugForm FROM ETP.EtpSummaryView s
//JOIN ETP.EtpSummaryItemView i ON s.PrescriptionId = i.PrescriptionId
//JOIN PKBRuntime.Mapping.DmdPreparation p ON i.DrugIdentifier = p.DmdProductCodeId
//JOIN PKBRuntime.Pharmacy.Preparation v ON p.PreparationCodeId = v.PreparationCodeId";


//        private const string FILTER = " WHERE ";

//        private const string FILTER_DATE = "CONVERT(VARCHAR(10), V.AddedDate, 111) = '";
//        private const string FILTER_DATE_END = "'";

//        private const string FILTER_NHSNO = "PatientNHSNumber = '";
//        private const string FILTER_NHSNO_END = "'";

//        private const string FILTER_NEW_ETP = "s.PrescriptionStatusId = 3";
//        private const string FILTER_TO_BE_DISPENSED = "i.PrescriptionItemStatusId = 1";

//        private const string DATE_FORMAT = "yyyy/MM/dd";

//        private const string FILTER_AND = " AND ";

//        private QueryType type;

//        private List<KeyValuePair<Condition, string>> conditions = new List<KeyValuePair<Condition, string>>();

//        public enum QueryType
//        {
//            /// <summary>
//            /// Grabs all items dispensed, selecting DrugDescription, Quantity, Prescription, Description, and UnitsPerPack
//            /// </summary>
//            DISPENSED,
//            ETPSCRIPTS
//        }

//        private enum Condition
//        {
//            /// <summary>
//            /// Filters results by a specific day
//            /// </summary>
//            DATE,
//            NHSNO,
//            NEWETP,
//            TOBEDISPENSED
//        }

//        /// <summary>Creates an object to represent a string used for a query.</summary>
//        /// <param name="type"> the type of query to run</param>
//        public QueryConstructor(QueryType type)
//        {
//            this.type = type;
//        }

//        /// <summary>
//        /// Filters the query to the specified day
//        /// </summary>
//        /// <param name="day"> the day to filter by</param>
//        public void specificDay(DateTime day)
//        {
//            string dateString = day.ToString(DATE_FORMAT);
//            removeCondition(Condition.DATE);
//            addCondition(Condition.DATE, dateString);
//        }

//        public void newETP()
//        {
//            removeCondition(Condition.NEWETP);
//            addCondition(Condition.NEWETP, null);
//        }

//        public void toBeDispensed()
//        {
//            removeCondition(Condition.TOBEDISPENSED);
//            addCondition(Condition.TOBEDISPENSED, null);
//        }

//        public void nhsNumber(string nhsNumber)
//        {
//            removeCondition(Condition.NHSNO);
//            addCondition(Condition.NHSNO, nhsNumber);
//        }

//        /// <summary>
//        /// Removes a condition from the query if there is one, otherwise does nothing
//        /// </summary>
//        /// <param name="cond">the type of condition to remove</param>
//        private void removeCondition(Condition cond)
//        {
//            if (conditions.Exists(x => x.Key == cond))
//            {
//                var condition = conditions.Find(x => x.Key == cond);
//                conditions.Remove(condition);
//            }
//        }

//        /// <summary>
//        /// Adds a condition to the list of conditions
//        /// </summary>
//        /// <param name="cond">the type of condition to add</param>
//        /// <param name="data">how the condition is filtered</param>
//        private void addCondition(Condition cond, string data)
//        {
//            conditions.Add(new KeyValuePair<Condition, string>(cond, data));
//        }

//        private string conditionsStringBuilder(List<KeyValuePair<Condition, string>> conditionList)
//        {
//            string str = "";
//            if (conditionList.Count > 0)
//            {
//                KeyValuePair<Condition, string> condition = conditionList.First();
//                switch (condition.Key)
//                {
//                    case Condition.DATE:
//                        str = FILTER_DATE + condition.Value + FILTER_DATE_END;
//                        break;

//                    case Condition.NEWETP:
//                        str = FILTER_NEW_ETP;
//                        break;

//                    case Condition.TOBEDISPENSED:
//                        str = FILTER_TO_BE_DISPENSED;
//                        break;

//                    case Condition.NHSNO:
//                        str = FILTER_NHSNO + condition.Value + FILTER_NHSNO_END;
//                        break;
//                }
//                conditionList.RemoveAt(0);
//                if (conditionList.Count > 0)
//                {
//                    str = str + FILTER_AND + conditionsStringBuilder(conditionList);
//                }
//            }

//            return str;
//        }

//        /// <summary>
//        /// Used after specifying all conditions to generate a string to use for the query.
//        /// </summary>
//        /// <returns>string to be used when making an SQL query.</returns>
//        public override string ToString()
//        {
//            string str = "";
//            switch (type)
//            {
//                case QueryType.DISPENSED:
//                    str = DISPENSED;
//                    break;

//                case QueryType.ETPSCRIPTS:
//                    str = ETPSCRIPTS;
//                    break;
//            }

//            if (conditions.Count > 0)
//            {
//                str = str + FILTER + conditionsStringBuilder(conditions);
//            }

//            return str;
//        }
//    }
//}
