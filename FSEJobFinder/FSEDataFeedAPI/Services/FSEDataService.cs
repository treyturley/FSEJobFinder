using FSEDataFeed;

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


    public class FSEDataService
    {
        private FSEDataAPI instance;

        private FSEDataService()
        {
            if(instance == null)
            {
                instance = new FSEDataAPI();    
            }
        }

        public FSEDataAPI GetInstance()
        {
            return instance;
        }
    }
}
