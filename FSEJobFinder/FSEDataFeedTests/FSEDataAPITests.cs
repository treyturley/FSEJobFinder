using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSEDataFeed.Tests
{
    //NOTE: These tests should use static test data so that we dont run up the hits to the FSE data api
    [TestClass()]
    public class FSEDataAPITests
    {
        [TestMethod()]
        public void getBestCommercialFlightTest()
        {
            //this implementation will hit the live data so it has been disabled 
            //until a new implementation that uses static test data is ready

            //throw new NotImplementedException();


            string makeModel = "Boeing 737-800";

            //TODO: Use my key for testing?
            FSEDataAPI fse = new FSEDataAPI("userkey");
            //TODO: make a better assert using a list of assignments that is then fed into the get best method
            //TODO: make sure this is actually return the best and not the best minus one due to index weirdness
            Assignment actualBestAssignment = fse.getBestCommercialAssignment(makeModel);
            string actualBestAssignmentLocation = actualBestAssignment.Location;

            //TODO: make this assert better          
            Assert.IsTrue(actualBestAssignmentLocation.Length == 4);
        }

        [TestMethod()]
        public void getAllCommercialFlightsTest()
        {
            throw new NotImplementedException("Not Implemented");
        }

    }
}