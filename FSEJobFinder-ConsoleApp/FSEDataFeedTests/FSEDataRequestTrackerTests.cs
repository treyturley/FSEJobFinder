using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;

namespace FSEDataFeed.Tests
{
    [TestClass]
    public class FSEDataRequestTrackerTests
    {
        //these URLs dont have a valid key so they wont work in a real request to FSEData API. For testing only!
        const string SAMPLE_VALID_MAKEMODEL_URL = @"https://server.fseconomy.net/data?userkey=TheUserKey&format=xml&query=aircraft&search=makemodel&makemodel=Cessna 172 Skyhawk";
        const string SAMPLE_VALID_ICAOJOBSFROM_URL = @"https://server.fseconomy.net/data?userkey=TheUserKey&format=xml&query=icao&search=jobsfrom&icaos=KAUS-KDFW-KIAH";

        [TestMethod]
        public void SaveRequests_LoadRequests_Success()
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

            foreach (FSEDataRequest request in requests.getRequests())
            {
                requests.SaveRequest(request);
            }

            //create a new request tracker that will read in the saved requests
            FSEDataRequestTracker loggedRequests = new FSEDataRequestTracker();

            //test to see if each request we saved was found in the file
            for(int i =0; i<requests.getRequests().Count; i++)
            {
                Assert.IsTrue(requests.getRequests()[i].Equals(loggedRequests.getRequests()[i]));
            }

            //see if the request trackers are the same
            Assert.IsTrue(requests.Equals(loggedRequests));
        }

        [TestMethod]
        public void GetOldestRequest_ReturnsCorrectResult()
        {
            //in this setup the Aircraft_By_MakeModel request is the older of the two requests
            FSEDataRequestType OLDEST_REQUEST_TYPE = FSEDataRequestType.Aircraft_By_MakeModel;


            FSEDataRequestTracker requests = new FSEDataRequestTracker();
            requests.AddRequest(new FSEDataRequest(FSEDataRequestType.Aircraft_By_MakeModel, SAMPLE_VALID_MAKEMODEL_URL));
            Thread.Sleep(1000);
            requests.AddRequest(new FSEDataRequest(FSEDataRequestType.ICAO_Jobs_From, SAMPLE_VALID_ICAOJOBSFROM_URL));


            Assert.IsTrue(requests.GetOldestRequest().GetRequestType().Equals(OLDEST_REQUEST_TYPE));
        }
    }
}
