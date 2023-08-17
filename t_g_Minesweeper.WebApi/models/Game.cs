using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace t_g_Minesweeper.WebApi.models
{
    public class Game
    {
        [JsonPropertyName("game_id")]
        [Required]
        public Guid GameId { get; set; }
        [Required]
        public int Width { get; set; }
        [Required]
        public int Height { get; set; }
        [JsonPropertyName("mines_count")]
        [Required]
        public int MinesCount { get; set; }
        public string[,] Field { get; set; }
        public bool Completed { get; set; }

        public Game(InternalGame internalGame) {
            GameId = internalGame.GameId;
            Width = internalGame.Width;
            Height = internalGame.Height;
            Field = internalGame.Field;
            Completed = internalGame.Completed;
            MinesCount = internalGame.MinesCount;
        }

        public Game()
        {

        }
    }
}
