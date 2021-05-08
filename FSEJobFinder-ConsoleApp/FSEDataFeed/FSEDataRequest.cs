using System;
using System.Net;

namespace FSEDataFeed
{
    class FSEDataRequest
    {
        private DateTime timeStamp;
        private string url;
        private HttpWebRequest request;
        private FSEDataRequestType requestType;

        /// <summary>
        /// Used to create a new FSEDataRequest with the timestamp set to the current time.
        /// </summary>
        /// <param name="requestType">the type of the FSE Data Request</param>
        /// <param name="url">The URL string of the request</param>
        public FSEDataRequest(FSEDataRequestType requestType, string url)
        {
            this.url = url;
            request = (HttpWebRequest)WebRequest.Create(url);
            timeStamp = DateTime.Now;
            this.requestType = requestType;
        }


        public FSEDataRequest(FSEDataRequestType requestType, string url, DateTime timeStamp)
        {
            this.requestType = requestType;
            this.url = url;
            request = (HttpWebRequest)WebRequest.Create(url);
            this.timeStamp = timeStamp;

        }

        public FSEDataRequest(string requestObjAsString)
        {
            //split it into parts
            string[] objParts = requestObjAsString.Split(',');

            //TODO: some validatio here could help incase the file get modifed outside of the program

            requestType = (FSEDataRequestType)Enum.Parse(typeof(FSEDataRequestType), objParts[0]);
            url = objParts[1];
            request = (HttpWebRequest)WebRequest.Create(url);
            timeStamp = DateTime.Parse(objParts[2]);
        }

        public HttpWebResponse GetResponse()
        {
            return (HttpWebResponse)request.GetResponse();
        }

        public string GetURL()
        {
            return url;
        }

        public DateTime GetTimestamp()
        {
            return timeStamp;
        }

        public FSEDataRequestType GetRequestType()
        {
            return requestType;
        }

        public override string ToString()
        {
            string result = "";

            result += requestType.ToString() + ",";
            result += url + ",";
            result += timeStamp.ToString();

            return result;
        }

    }

    public enum FSEDataRequestType
    {
        Aircraft_Configs,
        Aircraft_Aliases,
        Aircraft_For_Sale,
        Aircraft_By_MakeModel,
        Aircraft_By_Owner_Name,
        Aircraft_By_Registration,
        Aircraft_By_Id,
        Aircraft_By_Key,
        Assignments_By_Key,
        Commodities_By_Key,
        Facilities_By_Key,
        FBOs_By_Key,
        FBOs_For_Sale,
        FBO_Monthly_Summary_by_ICAO,
        Flight_Logs_By_Key_Month_Year,
        Flight_Logs_By_Reg_Month_Year,
        Flight_Logs_By_serialnumber_Month_Year,
        Flight_Logs_By_Key_From_Id, //limted to 500
        Flight_Logs_By_Key_From_Id_for_ALL_group_aircraft, //limted to 500
        Flight_Logs_By_Reg_From_Id, //limted to 500
        Flight_Logs_By_serialnumber_From_Id, //limted to 500
        Group_Members,
        ICAO_Listing_of_Aircraft,
        ICAO_Listing_of_FBOs,
        ICAO_Jobs_To,
        ICAO_Jobs_From,
        Payments_By_Month_Year,
        Payments_From_Id_, //limted to 500
        Statistics_By_Key,
    }
}
