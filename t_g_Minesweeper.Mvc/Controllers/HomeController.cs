using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using t_g_Minesweeper.Mvc.Models;
using t_g_Minesweeper.WebApi.models;

namespace t_g_Minesweeper.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private GameApiService _gameApiService;
        public HomeController(ILogger<HomeController> logger, GameApiService gameApiService)
        {
            _logger = logger;
            _gameApiService = gameApiService;   
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> New(int minesCount, int height,int width)
        {
            Game newGame = await _gameApiService.Create(new Game() { Height=height, MinesCount=minesCount, Width=width});
            var gameStr = _gameApiService.GetGame(newGame);
            ViewData["game"] = gameStr;
            ViewData["gameId"] = newGame.GameId.ToString();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> New([FromForm]GameAction game)
        {
            Game newGame = await _gameApiService.DoAction(game);
            var gameStr=_gameApiService.GetGame(newGame);
            ViewData["game"]=gameStr;
            ViewData["gameId"]=newGame.GameId.ToString();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}