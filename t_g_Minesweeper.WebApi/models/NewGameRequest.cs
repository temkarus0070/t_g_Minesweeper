using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace t_g_Minesweeper.WebApi.models
{
    public class NewGameRequest
    {
        [Required]
        public int Width { get; set; }
        [Required]
        public int Height { get; set; }
        [JsonPropertyName("mines_count")]
        [Required]
        public int MinesCount { get; set; }
    }
}
