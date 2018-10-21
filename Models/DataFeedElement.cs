using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Models
{
    [Serializable()]
    public class DataFeedElement : AbstractEntity
    {
        public DataFeedElement()
        {
            merchantElement = new MerchantElement();
        }

        private string _merchantName;

        [XmlAttribute()]
        public string merchantName
        {
            get
            {
                return _merchantName;
            }

            set
            {
                _merchantName = value;
                merchantElement.merchantName = value;
            }
        }

        private int _merchantId;

        [XmlAttribute()]
        public int merchantId
        {
            get
            {
                return _merchantId;
            }

            set
            {
                _merchantId = value;
                merchantElement.merchantId = value;
            }
        }

        private int _id;

        [XmlAttribute()]
        public int id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
                merchantElement.id = value;
            }
        }

        //[XmlAttribute()]
        //public string merchantName { get; set; }

        //[XmlAttribute()]
        //public int merchantId { get; set; }

        //[XmlAttribute()]
        //public int id { get; set; }

        public MerchantElement merchantElement { get; set; }

        [XmlElement("prod")]
        public List<ProdElement> prod { get; set; }
    }
}