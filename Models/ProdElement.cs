using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace Models
{
    [JsonObject(MemberSerialization.OptOut)]
    [Serializable()]
    [Table("Prod")]
    public class ProdElement : AbstractEntity
    {
        public ProdElement()
        {
            price = new PriceElement();
            text = new TextElement();
            uri = new UriElement();
            vertical = new VerticalElement();
        }

        [Column("lang")]
        [XmlAttribute()]
        public string lang { get; set; }

        [Column("id")]
        [XmlAttribute()]
        public long id { get; set; }

        [Column("web_offer")]
        [XmlAttribute()]
        public string web_offer { get; set; }

        [Column("stock_quantity")]
        [XmlAttribute()]
        public string stock_quantity { get; set; }

        [Column("pre_order")]
        [XmlAttribute()]
        public string pre_order { get; set; }

        [Column("is_for_sale")]
        [XmlAttribute()]
        public string is_for_sale { get; set; }

        [Column("in_stock")]
        [XmlAttribute()]
        public string in_stock { get; set; }

        public string brand { get; set; }

        public string cat { get; set; }

        public PriceElement price { get; set; }

        public TextElement text { get; set; }

        public UriElement uri { get; set; }

        public VerticalElement vertical { get; set; }

        [Column("pId")]
        public string pId { get; set; }
    }
}