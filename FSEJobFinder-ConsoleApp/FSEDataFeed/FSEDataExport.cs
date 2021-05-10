﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml.Serialization;


namespace FSEDataFeed
{
    /// <summary>
    /// This class utilizes the FSE Data feed to get information about the FSE game world.
    /// Details can be found here: https://sites.google.com/site/fseoperationsguide/expansion/data-feeds
    /// 
    /// There are some limitation to keep in mind. We are using the user access key to interact with the API which means we are rate limited.
    /// 
    /// "Timing: The Access Key can hit the server 10 times within any rolling 60 second window, including multiple simultaneous hits. 
    /// Read Access Keys are also limited to 40 hits within a 6 hour rolling window. "
    /// </summary>
    public class FSEDataExport  
    {   
        private string userKey;
        private int accessKeyHitCount;
        private string FSEEndpoint;
        private FSEDataRequestTracker requestTracker;

        //toggle between static debug data and live data
        bool debugEnabled;

        /// <summary>
        /// Default FSEDataExport constructor. Debug is disabled by default.
        /// </summary>
        public FSEDataExport()
        {
            //by default we should use live data
            debugEnabled = false;

            //get the user key
            GetUserKey();

            FSEEndpoint = @"http://server.fseconomy.net/data?userkey=" + userKey + "&format=xml";

            requestTracker = new FSEDataRequestTracker();
        }

        /// <summary>
        /// FSEDataExport constructor that takes one param to set the debug mode.
        /// </summary>
        /// <param name="debug">Toggle between static debug data (True) and live data(False). </param>
        public FSEDataExport(bool debug)
        {
            debugEnabled = debug;
        }

        /// <summary>
        /// Get the user key from the local user's UserKey.txt file
        /// </summary>
        private void GetUserKey()
        {
            //use static test data
            string filePath = Environment.CurrentDirectory + "\\StaticFiles\\UserKey.txt";
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                userKey = streamReader.ReadLine();
            }
        }

        /// <summary>
        /// Get all aircraft from FSE given the passed in make and model
        /// </summary>
        /// <param name="makeModel">the airplane we want to query FSE for.</param>
        /// <returns>the AircraftItems which is a list of the aircraft in FSE that match the MakeModel</returns>
        public  AircraftItems GetAircraftByMakeModel(string makeModel)
        {
            //TODO: If we have recently made a requests to get the AircraftItems for this MakeModel, 
            //just return that data instead of call FSE again

            AircraftItems aircraftItems = null;
            if (!debugEnabled)
            {
                string url = FSEEndpoint + @"&query=aircraft&search=makemodel&makemodel=" + makeModel;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);                

                XmlSerializer serializer = new XmlSerializer(typeof(AircraftItems));

                accessKeyHitCount++;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())                                
                {                    
                    aircraftItems = (AircraftItems)serializer.Deserialize(stream);
                }

                //TODO: Log the AircraftItems to a file so we can just reference that in later calls instead of calling FSE
                
            }
            else
            {
                //use static test data for AircraftItems
                string filePath = Environment.CurrentDirectory + "\\StaticFiles\\AircraftItems_AirbusA320_MSFS.xml";
                XmlSerializer serializer = new XmlSerializer(typeof(AircraftItems));
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    aircraftItems = (AircraftItems)serializer.Deserialize(fileStream);                    
                }
            }
            return aircraftItems;
        }

        public IcaoJobsFrom GetIcaoJobsFrom(List<string> ICAOs)
        {
            //TODO: name this better. its really a list of all of the assignments that start at one of the ICAOs in the passed in list.
            IcaoJobsFrom allICAOWithJobs = null;

            if (!debugEnabled)
            {   
                string allICAOs = "";

                if(ICAOs.Count == 0)
                {
                    //TODO: throw an error or maybe return null because there are no ICAOs to lookup jobs for
                }
                else if(ICAOs.Count == 1)
                {
                    //TODO: handle case where there is only one ICAO
                    //build an ICAO string with the one ICAO written out 3 times to get around the 3 ICAO min for this request
                }
                else
                {
                    foreach(string str in ICAOs)
                    {
                        allICAOs += str + "-";
                    }

                    //trim the last "-" off

                    allICAOs = allICAOs.Substring(0, allICAOs.Length - 1);
                }

                //build the request string with all of the icao
                string url = FSEEndpoint + @"&format=xml&query=icao&search=jobsfrom&icaos=" + allICAOs;

                //submit the request
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                accessKeyHitCount++;

                //deserialize the response
                XmlSerializer serializer = new XmlSerializer(typeof(IcaoJobsFrom));
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                {
                    allICAOWithJobs = (IcaoJobsFrom)serializer.Deserialize(stream);
                }
                

            }
            else
            {
                //use static test data
                string filePath = Environment.CurrentDirectory + "\\StaticFiles\\ICAOJobsFrom-KLBB.xml";
                XmlSerializer serializer = new XmlSerializer(typeof(IcaoJobsFrom));
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    allICAOWithJobs = (IcaoJobsFrom)serializer.Deserialize(fileStream);
                }                
            }

            return allICAOWithJobs;
        }
    }
}
