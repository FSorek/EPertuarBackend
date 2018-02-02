using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using J = Newtonsoft.Json.JsonPropertyAttribute;
namespace EPertuarWeb.Models
{
    public class RatingItem
    {
        [J("cleanliness")] public long Cleanliness { get; set; }
        [J("id_Cinema")] public long Id_Cinema { get; set; }
        [J("id_Movie")] public long Id_Movie { get; set; }
        [J("id_StringUser")] public string Id_StringUser { get; set; }
        [J("id_User")] public long Id_User { get; set; }
        [J("popcorn")] public long Popcorn { get; set; }
        [J("screen")] public long Screen { get; set; }
        [J("seat")] public long Seat { get; set; }
        [J("sound")] public long Sound { get; set; }
        public List<RatingItem> FromJson(string json) => JsonConvert.DeserializeObject<List<RatingItem>>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this List<RatingItem> self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    public class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}
