using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

//introduction to unit testing can be found here: https://docs.microsoft.com/en-us/visualstudio/test/unit-test-basics?view=vs-2019

namespace FSEDataFeed.Tests
{
    [TestClass]
    public class FSEDataRequestTests
    {
        const string EMPTY_URL = "";
        const string INVALID_URL = "http://localhost/TestMakeModelURL";
        const string VALID_URL = @"https://server.fseconomy.net/data?userkey=TheUserKey&format=xml&query=aircraft&search=makemodel&makemodel=Cessna 172 Skyhawk";

        
        [TestMethod]
        [ExpectedException(typeof(UriFormatException), "An empty url was allowed.")]
        public void Constructor_ThrowsUriFormatException()
        {
            FSEDataRequest request = new FSEDataRequest(FSEDataRequestType.Aircraft_By_MakeModel, EMPTY_URL);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception),"An invalid url was allowed.")]
        public void Constructor_ThrowsException()
        {   
            FSEDataRequest request = new FSEDataRequest(FSEDataRequestType.Aircraft_By_MakeModel, INVALID_URL);
        }

        [TestMethod]
        public void Constructor_Success()
        {
            FSEDataRequest request = new FSEDataRequest(FSEDataRequestType.Aircraft_By_MakeModel, VALID_URL);
        }

        /*
        [TestMethod]
        public void CacheTest()
        {
            //create a new request for Boieng 737 aircraft

            //send the request and get the response

            //checked the cache policy that was used

            //send another request and see if the response is from the cache
        }
        */
    }
}
