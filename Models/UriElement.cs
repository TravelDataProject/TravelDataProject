using Newtonsoft.Json;
using System;

namespace Models
{
    [JsonObject(MemberSerialization.OptOut)]
    [Serializable()]
    public class UriElement : AbstractEntity
    {
        public string awTrack { get; set; }

        public string alternateImageTwo { get; set; }

        public string alternateImageThree { get; set; }

        public string awImage { get; set; }

        public string awThumb { get; set; }

        public string mImage { get; set; }

        public string mLink { get; set; }
    }
}