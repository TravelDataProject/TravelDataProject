using System;
using System.Xml.Serialization;

namespace Models
{
    [Serializable()]
    public class MerchantElement
    {
        [XmlAttribute()]
        public string merchantName { get; set; }

        [XmlAttribute()]
        public int merchantId { get; set; }

        [XmlAttribute()]
        public int id { get; set; }
    }
}