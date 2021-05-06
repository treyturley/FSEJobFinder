using System.Collections.Generic;

namespace FSEDataFeed
{
    class FSEDataExportURLBuilder
    {
        List<string> DataFeedURLs;

        public FSEDataExportURLBuilder()
        {
            //TODO: load the URL Templates from a file
            DataFeedURLs = new List<string>();
            DataFeedURLs.Add("http://server.fseconomy.net/data?userkey=&format=xml&query=aircraft&search=makemodel&makemodel=");
            DataFeedURLs.Add("http://server.fseconomy.net/data?userkey=&format=xml&query=icao&search=jobsfrom&icaos=");
        }

        enum DataFeed
        {
            AircraftByMakeModel,
            JobsFromIcaos
        }
    }
}
