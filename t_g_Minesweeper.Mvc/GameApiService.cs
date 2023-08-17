using System.Text;
using t_g_Minesweeper.WebApi.models;

namespace t_g_Minesweeper.Mvc
{
    public class GameApiService
    {
        private IHttpClientFactory _httpClientFactory;

        public GameApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IEnumerable<string> GetGame(Game game)
        {
            List<string> result = new List<string>();
            StringBuilder header = new(" ");
            for (int j = 0; j < game.Field.GetLength(1); j++)
            {
                header.Append(j+" ");
            }
            result.Add(header.ToString());
                    header.Clear();
            for (int i = 0; i < game.Field.GetLength(0); i++)
            {
                header.Append(i+" ");
                for(int j = 0; j < game.Field.GetLength(1); j++)
                {
                    header.Append(game.Field[i,j]+" ");
                }
                result.Add(header.ToString());
                header.Clear();
            }
            return result;
        }

        public async Task<Game> Create(Game game)
        {
          var client=  _httpClientFactory.CreateClient("game");
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "new");
            httpRequestMessage.Content = JsonContent.Create<Game>(game);
            Game newGame=await client.SendAsync(httpRequestMessage).Result.Content.ReadFromJsonAsync<Game>();
            return newGame;
        }  

        public async Task<Game> DoAction(GameAction gameAction)
        {
          var client=  _httpClientFactory.CreateClient("game");
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "turn");
            httpRequestMessage.Content = JsonContent.Create<GameAction>(gameAction);
            Game game=await client.SendAsync(httpRequestMessage).Result.Content.ReadFromJsonAsync<Game>();
            return game;
        }
    }
}
