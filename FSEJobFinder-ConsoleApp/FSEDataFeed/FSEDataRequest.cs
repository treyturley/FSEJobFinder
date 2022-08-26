using System;
using System.IO;
using System.Net;

namespace FSEDataFeed
{
    public class FSEDataRequest : IEquatable<FSEDataRequest>, IComparable<FSEDataRequest>
    {

        //all FSE Request URLs have Query strings. When we split it up, there will always be atleast 3 key value pairs in the query string
        //The part we are most interested in is everything after "query="
        //Sample URL: https://server.fseconomy.net/data?userkey=TheUserKey&format=xml&query=aircraft&search=configs
        private const int MIN_QUERY_STRINGS = 3;

        //We are mostly interested in everything after "Query=". When we split a full URL on '&' this Query part will start at index 2
        private const int QUERY_STARTING_INDEX = 2;

        public const string TIME_FORMAT = "M-d-yyyy_h-m_tt";

        private DateTime timeStamp;
        private string url;
        private HttpWebRequest request;
        private FSEDataRequestType requestType;
        private string requestQuery;
        private string responseData;
        private string responseFileName;

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
            SetRequestQuery();
            responseFileName = requestType.ToString() + "_" + timeStamp.ToString(TIME_FORMAT) + ".xml";
        }


        public FSEDataRequest(FSEDataRequestType requestType, string url, DateTime timeStamp)
        {
            this.requestType = requestType;
            this.url = url;
            request = (HttpWebRequest)WebRequest.Create(url);
            this.timeStamp = timeStamp;
            SetRequestQuery();
            responseFileName = requestType.ToString() + "_" + timeStamp.ToString(TIME_FORMAT) + ".xml";
        }

        public FSEDataRequest(string requestObjAsString)
        {
            //split it into parts
            string[] objParts = requestObjAsString.Split(',');

            //TODO: some validation here could help incase the file gets modifed outside of the program

            requestType = (FSEDataRequestType)Enum.Parse(typeof(FSEDataRequestType), objParts[0]);
            //requestQuery = objParts[1];
            url = objParts[1];
            responseFileName = objParts[2];
            timeStamp = DateTime.Parse(objParts[3]);
            request = (HttpWebRequest)WebRequest.Create(url);
            SetRequestQuery();
        }

        /// <summary>
        /// Gets the deserialized response data for this request.
        /// </summary>
        /// <returns>The deserialized response string for this response.</returns>
        public string getResponseData()
        {
            return responseData;
        }

        public void setResponseData(string responseData)
        {
            this.responseData = responseData;
        }

        public HttpWebResponse GetResponse()
        {
            //update the request timestamp to now because now we are actually getting a response from FSE
            timeStamp = DateTime.Now;
            return (HttpWebResponse)request.GetResponse();
        }

        public string getResponseString()
        {
            string responseStr = "";

            timeStamp = DateTime.Now;

            //handle the response here so that we can close the connection/stream and free the connection up
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                //check for valid response
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    responseStr = reader.ReadToEnd();
                }
                else
                {
                    //TODO: handle the case were the response is not what we expected
                }

            }
            return responseStr;
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

        public string GetRequestQuery()
        {
            return requestQuery;
        }

        public bool IsInTimeWindow(DateTime start, DateTime end)
        {
            //check to see if this request was fired off in the last 6 hours
            if ((GetTimestamp() > start) && (GetTimestamp() < end))
            {
                return true;
            }
            return false;
        }

        private void SetRequestQuery()
        {
            if (url.Length != 0)
            {
                //try to split the url on the '&' signs
                string[] urlParts = url.Split('&');

                //after splitting we should have atleast 3 parts. the query starts in the third part and goes to the end of the URL
                //TODO: deal with this magic number
                if (urlParts.Length < MIN_QUERY_STRINGS)
                {
                    //we have an invalid URL
                    throw new Exception("Error processing URL: Invalid URL, too short");
                }

                string queryStr = "";

                for (int i = QUERY_STARTING_INDEX; i < urlParts.Length; i++)
                {
                    //validate that the first part from the URL contains "Query=" indicating we have a valid FSEDataExport URL
                    if (i == QUERY_STARTING_INDEX)
                    {
                        if (!urlParts[i].ToLower().StartsWith("query="))
                        {
                            //we have a problem
                            throw new Exception("Error processing URL: Invalid query string.");
                        }
                        queryStr += urlParts[i];
                        queryStr += "&";
                    }
                    else
                    {
                        //just add this part of the query String
                        queryStr += urlParts[i];

                        //if this isnt the last part of the query string, add the '&' back into to preserve the full query string
                        if (i < (urlParts.Length - 1))
                        {
                            queryStr += "&";
                        }
                    }
                }
                requestQuery = queryStr;
            }
            else
            {
                throw new Exception("Error processing URL: URL is empty");
            }
        }

        public override string ToString()
        {
            string result = "";

            result += requestType.ToString() + ",";
            // result += requestQuery + ",";
            result += url + ",";
            result += responseFileName + ",";
            result += timeStamp.ToString();

            return result;
        }

        /// <summary>
        /// override equals to see if this Data Request is the same of the passed in object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>        
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            FSEDataRequest objAsDataRequest = obj as FSEDataRequest;
            if (objAsDataRequest == null)
            {
                return false;
            }

            return Equals(objAsDataRequest);
        }

        /// <summary>
        /// Returns true if this Data Request and the other data request have the same
        /// request type, url, and timestamp
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(FSEDataRequest other)
        {
            if (this.requestType == other.requestType)
            {
                if (this.url.CompareTo(other.url) == 0)
                {
                    // requests are considered to be the same when type and url match
                    return true;

                    //when comparing date times, by default either the ticks (when using equals)
                    //or the milliseconds when using CompareTo get compared. The datetime from the file doesnt have this info.

                    //TODO: see if there is a better way to do this. 
                    //manually compare each element
                    /*if (this.timeStamp.Year.Equals(other.timeStamp.Year) &&
                        this.timeStamp.Month.Equals(other.timeStamp.Month) &&
                        this.timeStamp.Day.Equals(other.timeStamp.Day) &&
                        this.timeStamp.Hour.Equals(other.timeStamp.Hour) &&
                        this.timeStamp.Minute.Equals(other.timeStamp.Minute) &&
                        this.timeStamp.Second.Equals(other.timeStamp.Second))
                    {
                        return true;
                    }*/
                }
            }
            return false;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return this.url.GetHashCode();
        }

        public int CompareTo(FSEDataRequest other)
        {
            if (requestType == other.requestType)
            {
                if (requestQuery.CompareTo(other.requestQuery) == 0)
                {
                    if (timeStamp.CompareTo(other.timeStamp) == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return timeStamp.CompareTo(other.timeStamp);
                    }
                }
                else
                {
                    return requestQuery.CompareTo(other.requestQuery);
                }
            }
            else
            {
                return requestType.CompareTo(other.requestType);
            }
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
