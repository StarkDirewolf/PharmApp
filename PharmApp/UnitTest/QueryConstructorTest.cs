using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PharmApp;

namespace UnitTest
{
    [TestClass]
    public class QueryConstructorTest
    {
        [TestMethod]
        public void DispenseQuery()
        {
            string dispensed = @"
SELECT V.DrugDescription, V.Quantity, Pa.Description, Pa.UnitsPerPack
FROM PMR.PrescriptionCollectionSummaryView V
JOIN PMR.PrescriptionItemDispensed P ON P.PrescriptionItemId = V.PrescriptionItemId
JOIN PackSearchView Pa ON P.PackCodeId = Pa.PackCodeId";


            QueryConstructor query = new QueryConstructor(QueryConstructor.QueryType.DISPENSED);
            Assert.AreEqual(dispensed, query.ToString());
        }

        [TestMethod]
        public void DispenseDateQuery()
        {
            string dispensedDate = @"
SELECT V.DrugDescription, V.Quantity, Pa.Description, Pa.UnitsPerPack
FROM PMR.PrescriptionCollectionSummaryView V
JOIN PMR.PrescriptionItemDispensed P ON P.PrescriptionItemId = V.PrescriptionItemId
JOIN PackSearchView Pa ON P.PackCodeId = Pa.PackCodeId WHERE CONVERT(VARCHAR(10), V.AddedDate, 111) = '2020/02/20'";


            QueryConstructor query = new QueryConstructor(QueryConstructor.QueryType.DISPENSED);

            query.specificDay(DateTime.Parse("20/02/2020"));
            Assert.AreEqual(dispensedDate, query.ToString());
        }

        [TestMethod]
        public void NewETPForPatient()
        {
            string desiredQuery = @"
SELECT i.DrugDescription, s.PatientGivenName, s.PatientSurname, s.TokenPrinted, s.RepeatNumber, v.ApprovedName, v.Strength, v.DrugForm FROM ETP.EtpSummaryView s
JOIN ETP.EtpSummaryItemView i ON s.PrescriptionId = i.PrescriptionId
JOIN PKBRuntime.Mapping.DmdPreparation p ON i.DrugIdentifier = p.DmdProductCodeId
JOIN PKBRuntime.Pharmacy.Preparation v ON p.PreparationCodeId = v.PreparationCodeId WHERE PatientNHSNumber = '9999999999' AND s.PrescriptionStatusId = 3 AND i.PrescriptionItemStatusId = 1";


            QueryConstructor query = new QueryConstructor(QueryConstructor.QueryType.ETPSCRIPTS);

            query.nhsNumber("9999999999");
            query.newETP();
            query.toBeDispensed();
            Assert.AreEqual(desiredQuery, query.ToString());
        }
    }
}
