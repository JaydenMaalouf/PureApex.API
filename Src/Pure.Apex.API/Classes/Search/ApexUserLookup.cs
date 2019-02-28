using System.Xml.Serialization;
using System.Collections.Generic;

namespace Pure.Apex.API.Classes.Search
{
    [XmlRoot(ElementName = "users")]
    public class ApexUserLookup
    {
        [XmlElement(ElementName = "user")]
        public List<ApexUser> Users { get; set; }
    }
}
