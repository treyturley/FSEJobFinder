using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace FSEDataFeed.Tests
{
    [TestClass]
    public class FSEDataRequestTrackerTests
    {
        [TestMethod]
        public void SaveRequestsTest()
        {   
            string requestsFileName = "FSEDataRequests.txt";
            string RequestsfilePath = Directory.GetCurrentDirectory() + "\\" + requestsFileName;

            //clear the contents of any previously used requests file
            if (File.Exists(RequestsfilePath))
            {
                //clear the contents
                using(StreamWriter writer = new StreamWriter(RequestsfilePath, false))
                {
                    writer.Write("");
                }
            }

            FSEDataRequestTracker requests = new FSEDataRequestTracker();
            requests.AddRequest(new FSEDataRequest(FSEDataRequestType.Aircraft_By_MakeModel, "http://localhost/TestMakeModelURL"));
            requests.AddRequest(new FSEDataRequest(FSEDataRequestType.ICAO_Jobs_From, "http://localhost/TestICAOJobsFromURL"));

            Console.Error.WriteLine(requests.ToString());
            Console.WriteLine(requests.ToString());

            requests.SaveRequests();

            //read the requests
            FSEDataRequestTracker loggedRequests = new FSEDataRequestTracker();

            //test to see if each request we saved was found in the file
            for(int i =0; i<requests.getRequests().Count; i++)
            {
                Assert.IsTrue(requests.getRequests()[i].Equals(loggedRequests.getRequests()[i]));
            }

            //see if the request trackers are the same
            Assert.IsTrue(requests.Equals(loggedRequests));
        }
    }
}
