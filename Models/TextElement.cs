using Newtonsoft.Json;
using System;

namespace Models
{
    [JsonObject(MemberSerialization.OptOut)]
    [Serializable()]
    public class TextElement : AbstractEntity
    {
        public string name { get; set; }
    }
}