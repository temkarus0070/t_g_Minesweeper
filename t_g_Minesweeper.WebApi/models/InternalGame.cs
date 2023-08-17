using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace t_g_Minesweeper.WebApi.models
{
    public class InternalGame
    {
        public Guid GameId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int MinesCount { get; set; }
        public string[,] Field { get; set; }
        public string[,] InternalField { get; set; }
        public bool IsPlayed { get; set; }
        public bool Completed { get; set; }
        public int OpenedPointsCount { get; set; }

        public InternalGame(NewGameRequest game)
        {
            Width = game.Width; 
            Height = game.Height;
            MinesCount = game.MinesCount;

        }
    }
}
