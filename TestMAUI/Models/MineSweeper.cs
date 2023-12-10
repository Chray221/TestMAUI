using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using TestMAUI.Helpers;

namespace TestMAUI.Models
{
	public class MineSweeper : ObservableCollection<MineSweeperTile>
    {
        private GameStatus _gameStatus;
        private int _numberOfOpenedTile;

        public int TotalRows { get; init; }
        public int TotalColumns { get; init; }
        public int TotalBombs { get; init; }
        public int TotalTiles { get { return TotalRows * TotalColumns; } }
        public int TotalFlags { get { return this.Count( x => x.IsFlagged); } }

        public int NumberOfOpenedTile
        {
            get { return _numberOfOpenedTile; }
            private set { SetProperty(ref _numberOfOpenedTile, value); }
        }

        public GameStatus GameStatus
        {
            get { return _gameStatus; }
            private set { SetProperty(ref _gameStatus, value); }
        }

        public MineSweeper(): base()
		{
            TotalRows = 10;
            TotalColumns = 10;
            TotalBombs = 10;

        }

        public MineSweeper(int numberOfRow = 10, int numberOfCol = 10, int numberOfBomb = 10) : base()
        {
            TotalRows = numberOfRow;
            TotalColumns = numberOfCol;
            TotalBombs = numberOfBomb;
            //Generate();
        }

        public void Generate()
        {
            for (int tileRow = 0; tileRow < TotalRows; tileRow++)
            {
                for (int tileCol = 0; tileCol < TotalColumns; tileCol++)
                {
                    Add(new MineSweeperTile(tileRow, tileCol));
                }
            }
            GenerateBombs();
        }

        public void ReGenerate()
        {
            foreach (var tile in this)
            {
                tile.Clear();
            }
            GameStatus = GameStatus.Playing;
            NumberOfOpenedTile = 0;
            GenerateBombs();
        }

        private void GenerateBombs()
        {
            for (int bomb = 0; bomb < TotalBombs; bomb++)
            {
                GenerateBomb();
            }
        }

        private void GenerateBomb()
        {
            int row = RandomizerHelper.GetInt(0, TotalRows);
            int col = RandomizerHelper.GetInt(0, TotalColumns);
            MineSweeperTile? bombTile = this.FirstOrDefault(tile => tile.Row == row && tile.Column == col);
            if (bombTile != null && !bombTile.IsBomb)
            {
                bombTile.IsBomb = true;
                // set around bomb
                foreach (MineSweeperTile aroundBombTile in GetTilesAroundTile(bombTile))
                {
                    if (aroundBombTile.IsBomb)
                    {
                        aroundBombTile.BombInAreaCount = 0;
                    }
                    else
                    { 
                        aroundBombTile.BombInAreaCount++;
                    }
                }
                return;
            }
            GenerateBomb();
        }

        public IEnumerable<MineSweeperTile> GetTilesAroundTile(MineSweeperTile centerTile)
        {
            return this.Where(tile => tile.IsAroundTile(centerTile.Row, centerTile.Column));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tile"></param>
        /// <returns>false if tile is a bomb</returns>
        public bool OpenTile(int row, int col)
        {
            row = Math.Max(0, Math.Min(TotalRows, row));
            col = Math.Max(0, Math.Min(TotalColumns, col));
            MineSweeperTile? tile = this.FirstOrDefault(tile => tile.Row == row && tile.Column == col);
            if (tile != null)
            {
                return OpenTile(tile);
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tile"></param>
        /// <returns>false if tile is a bomb</returns>
        public bool OpenTile(MineSweeperTile tile)
        {
            if(GameStatus != GameStatus.Playing)
            {
                //switch (GameStatus)
                //{
                //    case GameStatus.GameOver:
                //        throw new Exception("Unable to open tile, game is already GameOver");
                //    case GameStatus.Winner:
                //        throw new Exception("Unable to open tile, game already ");
                //}
                //throw new Exception("Unable to open tile, game has already been ended.");
                return false;
            }
            ShowNonBombTiles(tile);
            GameStatus = tile.IsBomb ? GameStatus.GameOver : GameStatus;
            return !tile.IsBomb;
        }

        private void ShowNonBombTiles(MineSweeperTile tile)
        {
            if (!tile.IsOpen)
            {
                tile.IsOpen = true;
                if (!tile.IsBomb)
                {
                    NumberOfOpenedTile++;
                    if (tile.BombInAreaCount == 0)
                    {
                        foreach (MineSweeperTile nonBombTile in GetTilesAroundTile(tile))
                        {
                            ShowNonBombTiles(nonBombTile);
                        }
                    }
                }
            }
            if (GameStatus == GameStatus.Playing && (NumberOfOpenedTile + TotalBombs) >= (TotalRows * TotalColumns))
            {
                GameStatus = GameStatus.Winner;
            }
        }

        public bool IsGameEnded()
        {
            return GameStatus != GameStatus.Playing;
        }

        protected virtual void SetProperty<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = "")
        {
            if (oldValue == null ? true : !oldValue.Equals(newValue))
            {
                oldValue = newValue;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public enum GameStatus
    {
        Playing,
        GameOver,
        Winner
    }
}

