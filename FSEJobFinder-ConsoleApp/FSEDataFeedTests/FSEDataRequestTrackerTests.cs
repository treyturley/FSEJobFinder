using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FSEDataFeed.Tests
{
    [TestClass]
    public class FSEDataRequestTrackerTests
    {
        [TestMethod]
        public void SaveRequestsTest()
        {
            //TODO: need to cleanup and saved requests files created from previous test executions before starting this test.

            FSEDataRequestTracker requests = new FSEDataRequestTracker();
            requests.AddRequest(new FSEDataRequest(FSEDataRequestType.Aircraft_By_MakeModel, "http://localhost/TestMakeModelURL"));
            requests.AddRequest(new FSEDataRequest(FSEDataRequestType.ICAO_Jobs_From, "http://localhost/TestICAOJobsFromURL"));

            Console.Error.WriteLine(requests.ToString());
            Console.WriteLine(requests.ToString());

            requests.SaveRequests();

            //read the requests
            FSEDataRequestTracker loggedRequests = new FSEDataRequestTracker();

            for(int i =0; i<requests.getRequests().Count; i++)
            {
                Assert.IsTrue(requests.getRequests()[i].Equals(loggedRequests.getRequests()[i]));
            }
            
        }
    }
}
