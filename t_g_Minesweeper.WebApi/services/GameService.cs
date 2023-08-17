using System.Collections.Concurrent;
using System.Drawing;
using t_g_Minesweeper.WebApi.models;

namespace t_g_Minesweeper.WebApi.services
{
    public class GameService
    {
        private ConcurrentDictionary<Guid, InternalGame> games = new ConcurrentDictionary<Guid, InternalGame>();

        public Game CreateWithNewGuid(NewGameRequest newGame)
        {
            if (newGame.MinesCount >= newGame.Width * newGame.Height)
            {
                throw new InvalidOperationException("Нельзя устанавливать количество мин больше или равно кол-ву клеток в игровом поле");
            }
            var internalGame = new InternalGame(newGame);

            internalGame.GameId = Guid.NewGuid();
            games[internalGame.GameId] = internalGame;
            internalGame.Field = new string[internalGame.Width, internalGame.Height];
            internalGame.InternalField = new string[internalGame.Width, internalGame.Height];
            for (int i = 0; i < internalGame.Field.GetLength(0); i++)
            {
                for (int j = 0; j < internalGame.Field.GetLength(1); j++)
                {
                    internalGame.Field[i, j] = " ";
                }
            }
            return new Game(internalGame);
        }

        private void Generate(InternalGame game, GameAction gameAction)
        {
            Random rnd = Random.Shared;
            HashSet<Point> generatedPoints = new();
            generatedPoints.Add(new Point(gameAction.Column, gameAction.Row));
            for (int i = 0; i < game.MinesCount; i++)
            {
                int row = 0;
                int column = 0;
                Point currentPoint = new Point();
                do
                {
                    row = rnd.Next(game.Height);
                    column = rnd.Next(game.Width);
                    currentPoint = new Point(column, row);
                }
                while (generatedPoints.Contains(currentPoint));
                generatedPoints.Add(currentPoint);
                game.InternalField[row, column] = "X";
            }

            for (int i = 0; i < game.Height; i++)
            {
                for (int j = 0; j < game.Width; j++)
                {
                    NumerateMines(game, i, j);
                }
            }


            game.OpenedPointsCount++;
            game.Field[gameAction.Row, gameAction.Column] = game.InternalField[gameAction.Row, gameAction.Column];
            HashSet<Point> adjustedPointsToPlayerAction = new HashSet<Point>();
            HashSet<Point> adjustedPointsToAdjustedPoints = new HashSet<Point>();
            foreach (var point in GetAdjustedPoints(game, gameAction.Row, gameAction.Column))
            {
                if (game.InternalField[point.Y, point.X] == "0")
                {
                    game.Field[point.Y, point.X] = "0";

                    game.OpenedPointsCount++;
                }
                adjustedPointsToPlayerAction.Add(point);
                foreach (var adjustedPoint in GetAdjustedPoints(game, point.Y, point.X))
                {
                    adjustedPointsToAdjustedPoints.Add(adjustedPoint);
                }
            }
            adjustedPointsToAdjustedPoints = adjustedPointsToAdjustedPoints.Except(adjustedPointsToPlayerAction).ToHashSet();
            foreach (var point in adjustedPointsToAdjustedPoints)
            {
                if (game.InternalField[point.Y, point.X] == "0")
                {
                    game.Field[point.Y, point.X] = "0";

                    game.OpenedPointsCount++;
                }
            }

        }


        private void NumerateMines(InternalGame game, int row, int column)
        {

            if (game.InternalField[row, column] != "X")
            {
                int minesCount = 0;
                foreach (var adjustedPoint in GetAdjustedPoints(game, row, column))
                {
                    if (game.InternalField[adjustedPoint.Y, adjustedPoint.X] == "X")
                        minesCount++;
                }
                game.InternalField[row, column] = minesCount.ToString();
            }
        }

        private IEnumerable<Point> GetAdjustedPoints(InternalGame game, int row, int col)
        {
            int firstColumn = col;
            if (firstColumn - 1 >= 0)
            {
                firstColumn--;
            }

            int lastColumn = col;
            if (lastColumn + 1 < game.Width)
            {
                lastColumn++;
            }
            int firstRow = row;
            if (firstRow - 1 >= 0)
            {
                firstRow--;
            }
            int lastRow = row; if (lastRow + 1 < game.Height)
            {
                lastRow++;
            }

            for (int j = firstRow; j <= lastRow; j++)
            {
                for (int i = firstColumn; i <= lastColumn; i++)
                {
                    yield return new Point(i, j);
                }
            }
        }


        public Game DoGameAction(GameAction gameAction)
        {
            InternalGame internalGame;
            if (games.TryGetValue(gameAction.GameId, out internalGame))
            {
                if (internalGame.Height - 1 < gameAction.Row || internalGame.Width - 1 < gameAction.Column || gameAction.Column < 0 || gameAction.Row < 0)
                {
                    throw new InvalidOperationException("Ваш ход вышел за пределы игрового поля");
                }
                if (internalGame.Completed)
                {
                    throw new InvalidOperationException("Нельзя делать ход после окончания игры");
                }
                if (!internalGame.IsPlayed)
                {
                    Generate(internalGame, gameAction);
                    internalGame.IsPlayed = true;
                }
                else if (internalGame.Field[gameAction.Row, gameAction.Column] != " ")
                {
                    throw new InvalidOperationException("Данная ячейка уже проверена");
                }
                else if (internalGame.InternalField[gameAction.Row, gameAction.Column] == "X")
                {
                    MarkMines(internalGame, "X");
                    internalGame.Completed = true;
                }
                else
                {
                    internalGame.OpenedPointsCount++;
                    internalGame.Field[gameAction.Row, gameAction.Column] = internalGame.InternalField[gameAction.Row, gameAction.Column];
                }

                if (internalGame.OpenedPointsCount + internalGame.MinesCount == internalGame.InternalField.Length)
                {
                    internalGame.Completed = true;
                    MarkMines(internalGame, "M");
                }
                return new Game(internalGame);
            }
            else throw new InvalidOperationException("не найдено игры с таким id");
        }

        private void MarkMines(InternalGame game, string symbol)
        {
            for (int i = 0; i < game.Field.GetLength(0); i++)
            {
                for (int j = 0; j < game.Field.GetLength(1); j++)
                {
                    if (game.InternalField[i, j] == "X")
                    {
                        game.Field[i, j] = symbol;
                    }
                }
            }
        }
    }
}
