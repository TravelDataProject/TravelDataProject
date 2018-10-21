using Newtonsoft.Json;
using System;

namespace Models
{
    [JsonObject(MemberSerialization.OptOut)]
    [Serializable()]
    public class VerticalElement : AbstractEntity
    {
        public int id { get; set; }

        public string name { get; set; }

        public int availability { get; set; }

        public string departureDate { get; set; }

        public string destinationCity { get; set; }

        public string destinationCountry { get; set; }

        public string destinationRegion { get; set; }

        public string destinationType { get; set; }

        public int duration { get; set; }

        public string hotelAddress { get; set; }

        public string latitude { get; set; }

        public string longitude { get; set; }

        public string returnDate { get; set; }

        public string roomType { get; set; }

        public double startingFromPrice { get; set; }

        public string travelRating { get; set; }

        public string travelType { get; set; }
    }
}