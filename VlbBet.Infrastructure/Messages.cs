using System.Collections.Generic;
using System.Text.Json.Serialization;
using VlbBet.Core;

namespace VlbBet.Infrastructure
{
    public class GamesLineMessage
    {
        [JsonPropertyName("active")]
        public bool Active { get; set; }

        [JsonPropertyName("num")]
        public int Num { get; set; }           // número de evento

        [JsonPropertyName("games")]
        public List<Game> Games { get; set; } = new List<Game>();

        public void Normalize()
        {
            if (Games == null) return;
            foreach (var g in Games)
                g?.NormalizeFromWire();
        }
    }

    public class HourMessage
    {
        [JsonPropertyName("hour")]
        public long Hour { get; set; }         // timestamp unix (segundos)
    }
}
