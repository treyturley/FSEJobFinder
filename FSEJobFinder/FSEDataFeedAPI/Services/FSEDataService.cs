using FSEDataFeed;
using Microsoft.AspNetCore.Mvc;

namespace FSEDataFeedAPI.Services
{
    //TODO: Implement FSEDataService
    /*
        This service will be used to keep track of who is 
        calling us by tracking the supplied FSE User Keys.
        
        Maybe something like one FSEDataAPI instance per key which would
        allow us to track requests by user and would help with the rate 
        limiting that is required by FSE.
     */


#pragma warning disable CS1591
    public class FSEDataService
    {
        private Dictionary<string, FSEDataAPI> fseServices;

        public FSEDataService()
        {
            fseServices = new();
        }

        public FSEDataAPI GetService(string userKey)
        {
            // FIXME: make sure userkey is not null or empty

            if (fseServices.ContainsKey(userKey))
            {
                return fseServices[userKey];
            } else
            {
                FSEDataAPI service = new(userKey);
                fseServices.Add(userKey, service);
                return service;
            }

        }

        /// <summary>
        /// Checks to see if the userkey is valid.
        /// </summary>
        /// <param name="userKey">The userkey to validate.</param>
        /// <returns>True if the userkey is not null and not an empty string.</returns>
        public bool userKeyIsValid(string userKey)
        {
            bool result = false;
            if(userKey != null && userKey != string.Empty)
            {
                result = true;
            }
            return result;
        }
    }
#pragma warning restore CS1591
}
