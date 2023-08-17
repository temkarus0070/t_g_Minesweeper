using t_g_Minesweeper.WebApi.services;

namespace t_g_Minesweeper.Test
{
    public class UnitTest1
    {
        private GameService service=new GameService();
        [Fact]
        public void Test1()
        {
            var game=service.CreateWithNewGuid(new WebApi.models.InternalGame() { MinesCount=8,Height=8,Width=8});
           var gameAfterAction= service.DoGameAction(new WebApi.models.GameAction() { Column=0, Row=0, GameId=game.GameId });
            Console.WriteLine(game);
        }
    }
}