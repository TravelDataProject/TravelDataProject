using System;
using System.Xml.Serialization;

namespace Models
{
    [Serializable()]
    [XmlRoot("cafProductFeed")]
    public class cafProductFeed : AbstractEntity
    {
        [XmlElement("datafeed")]
        public DataFeedElement dataFeed { get; set; }

        public cafProductFeed()
        {
            dataFeed = new DataFeedElement();
        }
    }
}