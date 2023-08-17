using System.Text.Json.Serialization;

namespace t_g_Minesweeper.WebApi.models
{
    public class GameAction
    {
        [JsonPropertyName("game_id")]
        public Guid GameId { get; set; }
        [JsonPropertyName("col")]
        public int Column { get; set; }
        public int Row { get; set; }
    }
}
