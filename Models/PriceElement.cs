using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace Models
{
    [JsonObject(MemberSerialization.OptOut)]
    [Serializable()]
    public class PriceElement : AbstractEntity
    {
        [Column("[price.curr]")]
        [XmlAttribute()]
        public string curr { get; set; }

        public double buynow { get; set; }

        public double rrp { get; set; }

        public double store { get; set; }

        public double basePrice { get; set; }
    }
}