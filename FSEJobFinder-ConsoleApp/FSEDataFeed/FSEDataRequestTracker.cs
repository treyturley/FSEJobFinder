using System;
using System.Collections.Generic;
using System.IO;

namespace FSEDataFeed
{
    //"Timing: The Access Key can hit the server 10 times within any rolling 60 second window, including multiple simultaneous hits. 
    // Read Access Keys are also limited to 40 hits within a 6 hour rolling window."

    /// <summary>
    /// 
    /// </summary>
    public class FSEDataRequestTracker
    {
        private List<FSEDataRequest> requests;
        private const int SMALL_WINDOW_LIMIT = 10;
        private const int BIG_WINDOW_LIMIT = 40;
        private string requestsFileName = "FSEDataRequests.txt";
        private string RequestsfilePath;

        public FSEDataRequestTracker()
        {
            requests = new List<FSEDataRequest>();

            RequestsfilePath = Directory.GetCurrentDirectory() + "\\" + requestsFileName;

            //read in all of the previosuly saved requests
            LoadRequests();

            //prune the list of requests and remove any that dont fall into one of the rolling windows
            PruneRequests();
        }

        public void AddRequest(FSEDataRequest request)
        {
            requests.Add(request);
        }

        public List<FSEDataRequest> getRequests()
        {
            return requests;
        }

        
        /// <summary>
        /// Reuqests to FSE Data API are rate limited. check to see if we have exceeded that rate yet or if we can make another request.
        /// </summary>
        /// <returns></returns>
        public bool CanMakeRequest()
        {
            //if we have less than 40 requests in the last 6 hours and less than 10 requests 
            //in the last 60 seconds then we can make another requests           
            if(RequestsInSecondWindow() < BIG_WINDOW_LIMIT && RequestsInFirstWindow() < SMALL_WINDOW_LIMIT)
            {
                return true;
            }
            return false;
        }

        public int remainingRequests()
        {
            int result = 0;

            if (CanMakeRequest())
            {
                //check to see which window has the smaller number of available requests
                int bigWindow, smallWindow = 0;

                bigWindow = BIG_WINDOW_LIMIT - RequestsInSecondWindow();
                smallWindow = SMALL_WINDOW_LIMIT - RequestsInFirstWindow();
                return Math.Min(bigWindow, smallWindow);
            }

            return result;
        }

        public TimeSpan TimeUntilNextRequest()
        {
            TimeSpan nextRequestTime = TimeSpan.Zero;

            if (!CanMakeRequest())
            {
                //see which limit we hit

                //TODO: instead of just saying we ahve to wait the full limit,
                //calculate when the oldest request will roll out of the window and return how long until that happens
                if(RequestsInSecondWindow() == BIG_WINDOW_LIMIT)
                {
                    return TimeSpan.FromHours(6);
                }
                else if (RequestsInFirstWindow() == SMALL_WINDOW_LIMIT)
                {
                    return TimeSpan.FromMinutes(10);
                }
            }
            return nextRequestTime;
        }

        /// <summary>
        /// Every now and then we can prune requests from this list if they are outside of both of the rolling windows
        /// </summary>
        private void PruneRequests()
        {
            if(requests.Count != 0)
            {
                //start and end times for the second rolling window (40 hits in 6 hours)
                DateTime end = DateTime.Now;
                DateTime start = end.Subtract(TimeSpan.FromHours(6));

                List<FSEDataRequest> requestsToRemove = new List<FSEDataRequest>();

                File.Delete(RequestsfilePath);

                using (StreamWriter writer = new StreamWriter(RequestsfilePath, true))
                {
                    foreach (FSEDataRequest request in requests)
                    {
                        if (!request.isInTimeWindow(start, end))
                        {
                            requestsToRemove.Add(request);
                        } else
                        {
                            writer.WriteLine(request.ToString());
                        }
                    }
                }

                requests.RemoveAll(request => requestsToRemove.Contains(request));
            }
        }

        /// <summary>
        /// Load any existing requests from previous program executions.
        /// </summary>
        private void LoadRequests()
        {   
            //See if the file exists
            if (File.Exists(RequestsfilePath))
            {
                string[] requestObjects = File.ReadAllLines(RequestsfilePath);
                //read line by line
                foreach(string requestObjectStr in requestObjects)
                {
                    //parse into a request
                    requests.Add(new FSEDataRequest(requestObjectStr));
                }
            }
        }

        public void SaveRequest(FSEDataRequest request)
        {
            using (StreamWriter writer = new StreamWriter(RequestsfilePath, true))
            {   
                writer.WriteLine(request.ToString());
            }
        }

        /// <summary>
        /// This returns the number of requests that had occured during the first rollowing window.
        /// The first rolling time window covers the past 10 minutes.
        /// </summary>
        /// <returns>The number of requests that have occured in the past 10 minutes.</returns>
        private int RequestsInFirstWindow()
        {
            DateTime end = DateTime.Now;
            DateTime start = end.Subtract(TimeSpan.FromMinutes(10));

            int requestCount = 0;
            if (requests.Count == 0)
            {
                return requestCount;
            }
            else
            {
                foreach (FSEDataRequest request in requests)
                {
                    //check to see if this request was fired off in the last 10 minutes
                    if((request.GetTimestamp() > start ) && (request.GetTimestamp() < end)){
                        requestCount++;
                    }
                }
            }
            return requestCount;
        }

        /// <summary>
        /// This returns the number of requests that had occured during the second rollowing window.
        /// The second rolling time window covers the past 24 hours.
        /// </summary>
        /// <returns>The number of requests that have occured in the past 10 minutes.</returns>
        private int RequestsInSecondWindow()
        {
            DateTime end = DateTime.Now;
            DateTime start = end.Subtract(TimeSpan.FromHours(6));

            int requestCount = 0;
            if (requests.Count == 0)
            {
                return requestCount;
            }
            else
            {
                foreach (FSEDataRequest request in requests)
                {
                    //TODO: refactor using the new request method for checking on time windows

                    //check to see if this request was fired off in the last 6 hours
                    if ((request.GetTimestamp() > start) && (request.GetTimestamp() < end))
                    {
                        requestCount++;
                    }
                }
            }
            return requestCount;
        }

        public FSEDataRequest GetOldestRequest()
        {
            DateTime oldestDateTime = DateTime.Now;
            FSEDataRequest oldestRequest = null;

            foreach(FSEDataRequest request in requests)
            {
                if(request.GetTimestamp().CompareTo(oldestDateTime) < 0)
                {
                    oldestRequest = request;
                    oldestDateTime = oldestRequest.GetTimestamp();
                }
            }

            return oldestRequest;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
           
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            FSEDataRequestTracker objAsDataRequestTracker = obj as FSEDataRequestTracker;
            if (objAsDataRequestTracker == null)
            {
                return false;
            }

            return Equals(objAsDataRequestTracker);
        }

        public bool Equals(FSEDataRequestTracker other)
        {
            bool result = true;
            if(requests.Count == other.requests.Count)
            {
                //See if other contains the same requests
                foreach (FSEDataRequest request in requests)
                {
                    if (!other.requests.Contains(request))
                    {
                        result = false;
                    }
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            //trying out a hashcode algorithm seen here: https://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-overriding-gethashcode
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + requests.GetHashCode();
                hash = hash * 23 + SMALL_WINDOW_LIMIT;
                hash = hash * 23 + BIG_WINDOW_LIMIT;
                hash = hash * 23 + requestsFileName.GetHashCode();
                hash = hash * 23 + RequestsfilePath.GetHashCode();
                
                return hash;
            }
        }
    }
}
