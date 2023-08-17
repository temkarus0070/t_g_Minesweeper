using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using t_g_Minesweeper.WebApi.models;
using t_g_Minesweeper.WebApi.services;

namespace t_g_Minesweeper.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CustomExceptionFilter]
    public class GameController : ControllerBase
    {
        private GameService gameService;


        public GameController(GameService gameService)
        {
            this.gameService = gameService;
        }

        [HttpPost("new")]
        public Game New([FromBody] NewGameRequest newGameRequest)
        {
            return gameService.CreateWithNewGuid(newGameRequest);
        }

        [HttpPost("turn")]
        public Game Turn([FromBody] GameAction gameAction)
        {
            return gameService.DoGameAction(gameAction);

        }
    }
}
