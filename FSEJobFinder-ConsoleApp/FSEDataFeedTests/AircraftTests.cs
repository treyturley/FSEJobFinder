using Microsoft.VisualStudio.TestTools.UnitTesting;
using FSEDataFeed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace FSEDataFeed.Tests
{
    //NOTE: All tests should use the static debug data unless they are marked as using the live data.
    [TestClass()]
    public class AircraftTests
    {
        /// <summary>
        /// Tests the deserialization of AircraftItems from the 172TestFile.xml 
        /// </summary>
        [TestMethod()]
        public void DeserializeTest()
        {
            int expectedAircraftCount = 5;
            string expectedRegistrationForAircraft1 = "EC-JNB";

            string filePath = Environment.CurrentDirectory + "\\StaticFiles\\172TestFile.xml";

            AircraftItems result = null;

            XmlSerializer serializer = new XmlSerializer(typeof(AircraftItems));
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                result = (AircraftItems)serializer.Deserialize(fileStream);
            }

            Assert.IsNotNull(result,"Deserialization Failed");

            //test correct number of aircraft deserialized
            Assert.IsTrue(result.AircraftList.Count == expectedAircraftCount, "Incorrect number of aircraft found");

            //spot check for some of the Aircraft elements
            Assert.IsTrue(result.AircraftList[0].Registration.CompareTo(expectedRegistrationForAircraft1) == 0);
        }

        [TestMethod()]
        public void IsRentableTest_True()
        {
            //index of rentable aircraft from the test file
            int rentalAircraftIndex = 1;

            //deserialize the 172TestFile into AircraftItems
            string filePath = Environment.CurrentDirectory + "\\StaticFiles\\172TestFile.xml";

            AircraftItems result = null;

            XmlSerializer serializer = new XmlSerializer(typeof(AircraftItems));
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                result = (AircraftItems)serializer.Deserialize(fileStream);
            }

            Assert.IsTrue(result.AircraftList[rentalAircraftIndex].isRentable());
        }

        [TestMethod()]
        public void IsRentableTest_False()
        {   
            //index of rentable aircraft from the test file
            int notRentable_NoDryWetRentPrice = 0;
            int notRentable_NeedsRepair = 2;
            int notRantable_AlreadyRented = 4;

            //deserialize the 172TestFile into AircraftItems
            string filePath = Environment.CurrentDirectory + "\\StaticFiles\\172TestFile.xml";

            AircraftItems result = null;

            XmlSerializer serializer = new XmlSerializer(typeof(AircraftItems));
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                result = (AircraftItems)serializer.Deserialize(fileStream);
            }

            //see if we can rent a plane with no rental prices set
            Assert.IsFalse(result.AircraftList[notRentable_NoDryWetRentPrice].isRentable());

            //see if we can rent a plane that needs repairing
            Assert.IsFalse(result.AircraftList[notRentable_NeedsRepair].isRentable());

            //see if we can rent a plance that is already rented
            Assert.IsFalse(result.AircraftList[notRantable_AlreadyRented].isRentable());

        }
    }
}