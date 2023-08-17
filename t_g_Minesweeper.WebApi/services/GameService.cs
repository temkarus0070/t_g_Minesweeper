﻿using System.Collections.Concurrent;
using System.Drawing;
using t_g_Minesweeper.WebApi.models;

namespace t_g_Minesweeper.WebApi.services
{
    public class GameService
    {
        private ConcurrentDictionary<Guid, InternalGame> games = new ConcurrentDictionary<Guid, InternalGame>();

        public Game CreateWithNewGuid(NewGameRequest newGame)
        {
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
            for (int i = 0; i < game.MinesCount; i++)
            {
                int row = 0;
                int column = 0;
                Point currentPoint=new Point();
                do
                {
                    row = rnd.Next(game.Height);
                    column = rnd.Next(game.Width);
                    currentPoint = new Point(column, row);
                }
                while (row == gameAction.Row && column == gameAction.Column || generatedPoints.Contains(currentPoint));
                generatedPoints.Add(currentPoint);
                game.InternalField[column,row] = "X";
            }


            game.OpenedPointsCount++;

            HashSet<Point> points = new HashSet<Point>();
            HashSet<Point> adjustedPointsFromAdjustedPoints = new HashSet<Point>();
            foreach (var point in GetAdjustedPoints(game, gameAction.Row, gameAction.Column))
            {
                if (game.InternalField[point.X,point.Y] != "X")
                {
                    game.Field[point.X,point.Y] = "0";

                    game.OpenedPointsCount++;
                }
                points.Add(point);
                foreach (var adjustedPoint in GetAdjustedPoints(game, point.Y, point.X))
                {
                    adjustedPointsFromAdjustedPoints.Add(adjustedPoint);
                }
            }
            adjustedPointsFromAdjustedPoints = adjustedPointsFromAdjustedPoints.Except(points).ToHashSet();
            game.OpenedPointsCount += adjustedPointsFromAdjustedPoints.Count;
            NumerateMines(game, adjustedPointsFromAdjustedPoints);

        }

        private void NumerateMines(InternalGame game, IEnumerable<Point> points)
        {
            foreach (var point in points)
            {
                if (game.InternalField[point.X,point.Y] != "X")
                {
                    int minesCount = 0;
                    foreach (var adjustedPoint in GetAdjustedPoints(game, point.Y, point.X))
                    {
                        if (game.InternalField[adjustedPoint.X, adjustedPoint.Y] == "X")
                            minesCount++;
                    }
                    game.Field[point.X,point.Y] = minesCount.ToString();
                }
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
                if (internalGame.Height - 1 < gameAction.Row || internalGame.Width - 1 < gameAction.Column)
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
                    internalGame.IsPlayed=true;
                }
                else if (internalGame.Field[gameAction.Row,gameAction.Column]!=" ")
                {
                    throw new InvalidOperationException("Данная ячейка уже проверена");
                }
                else if (internalGame.InternalField[gameAction.Column, gameAction.Row] == "X")
                {
                    MarkMines(internalGame, "X");
                    internalGame.Completed = true;
                }
                else
                {
                    internalGame.OpenedPointsCount++;
                    NumerateMines(internalGame, new Point[] { new Point(gameAction.Column, gameAction.Row) });
                    if (internalGame.OpenedPointsCount + internalGame.MinesCount == internalGame.InternalField.Length)
                    {
                        internalGame.Completed = true;
                        MarkMines(internalGame, "M");
                    }
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
