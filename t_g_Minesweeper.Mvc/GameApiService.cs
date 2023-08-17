using System.Text;
using t_g_Minesweeper.WebApi.models;
using t_g_Minesweeper.WebApi.services;

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
            StringBuilder header = new("||");
            for (int j = 0; j < game.Field.GetLength(1); j++)
            {
                header.Append(j + " ");
            }
            result.Add(header.ToString());
            header.Clear();
            for (int i = 0; i < game.Field.GetLength(0); i++)
            {
                header.Append(i + " ");
                for (int j = 0; j < game.Field.GetLength(1); j++)
                {
                    if (game.Field[i, j] != " ")
                    {
                        header.Append(game.Field[i, j] + " ");
                    }
                    else header.Append("-" + " ");
                }
                result.Add(header.ToString());
                header.Clear();
            }
            if(game.Completed)
            {
                result.Add("game complete");
            }
            return result;
        }

        public async Task<Game> Create(NewGameRequest newGameRequest)
        {
            var client = _httpClientFactory.CreateClient("game");
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "new");
            httpRequestMessage.Content = JsonContent.Create<NewGameRequest>(newGameRequest);
            HttpResponseMessage message = await client.SendAsync(httpRequestMessage);
            if(message.StatusCode!= System.Net.HttpStatusCode.OK)
            {
                throw new InvalidOperationException(message.Content.ReadAsStringAsync().Result);
            }
            Game newGame = await message.Content.ReadFromJsonAsync<Game>();
            return newGame;
        }

        public async Task<Game> DoAction(GameAction gameAction)
        {
            var client = _httpClientFactory.CreateClient("game");
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "turn");
            httpRequestMessage.Content = JsonContent.Create<GameAction>(gameAction);
            HttpResponseMessage message = await client.SendAsync(httpRequestMessage);
            if (message.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new InvalidOperationException(message.Content.ReadAsStringAsync().Result);
            }
            Game newGame = await message.Content.ReadFromJsonAsync<Game>();
            return newGame;
        }
    }
}
