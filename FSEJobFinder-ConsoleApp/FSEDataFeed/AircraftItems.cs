using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FSEDataFeed
{
    //TODO: need to see if we can use a set of aircraft instead of a list of aircraft
    [Serializable, XmlRoot("AircraftItems", Namespace = "https://server.fseconomy.net")]
    public class AircraftItems
    {
        [XmlElement("Aircraft")]
        public List<Aircraft> AircraftList { get; set; }
    }
}
