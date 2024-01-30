using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Linq;

namespace TestMAUI.Models
{
    public partial class TicTacToe : ReadOnlyObservableCollection<TicTacToeTile>
    {
        private TicTacToeTurn turn;
        public TicTacToeTurn Turn
        {
            get { return turn; }
            private set { SetProperty(ref turn, value); }
        }

        private GameStatus status;
        public GameStatus Status
        {
            get { return status; }
            private set { SetProperty(ref status, value); }
        }

        private static List<int[]> WinningIndex = new List<int[]>
        {             // top -      , topmiddle |       , right |
            new int[] { 0, 1, 2 }, new int[] { 1, 4, 7 }, new int[] { 2, 5, 8 },
                      // left |     ,    blank          , rightmid -
            new int[] { 0, 3, 6 }, new int[] { 0, 3, 6 }, new int[] { 3, 4, 5 },
                      // botleft /  , bot -             , botright \
            new int[] { 6, 4, 2 }, new int[] { 6, 7, 8 }, new int[] { 0, 4, 8 },
        };

        public TicTacToe() : base(GenerateTiles())
		{
            
		}

        public void Start()
        {
            foreach(TicTacToeTile tile in this)
            {
                tile.Clear();
            }
            Turn = Random.Shared.Next(0, 100) % 2 == 0 ? TicTacToeTurn.X : TicTacToeTurn.O;
            Status = GameStatus.Playing;
        }

        public bool IsGameEnded()
        {
            return Status != GameStatus.Playing;
        }

        public bool SetTile(TicTacToeTile tile)
        {
            tile.Value = Turn.ToString()[0];

            bool isWinner = CheckWinner(tile,true);
            //Console.WriteLine($"isWinner: {isWinner}");
            if (IsBoardFilled() || isWinner)
            {
                Status = isWinner ? GameStatus.Winner : GameStatus.Draw;
            }

            if (IsGameEnded()) return true;

            Turn = Turn == TicTacToeTurn.X ? TicTacToeTurn.O : TicTacToeTurn.X;
            return false;
        }

        private bool IsBoardFilled()
        {
            return this.All(x => x.Value != ' ');
        }

        public bool CheckWinner(TicTacToeTile tile, bool @void)
        {
            return WinningIndex
                .Where(indexes => indexes.Contains(tile.Row * 3 + tile.Column))
                .Any(indexes => indexes.All(index => this[index].Value == tile.Value));
        }

        public bool CheckWinner(TicTacToeTile tile)
        {
            //hor || ver
            bool check = Check(tile, TileDirection.Horizontal) ||
                         Check(tile, TileDirection.Vertical);

            //Console.Write($"checking({check}");
            //tl 
            switch ((tile.Row, tile.Column))
            {
                case (0, 0): //tl
                case (2, 2): //br
                    return check || Check(tile, TileDirection.DiagonalRight);
                case (0, 2): //tr
                case (2, 0): // bl
                    return check || Check(tile, TileDirection.DiagonalLeft);
            }
            bool checkDR = Check(tile, TileDirection.DiagonalRight);
            bool checkDL = Check(tile, TileDirection.DiagonalLeft);

            //Console.WriteLine($",{checkDR},{checkDL}");
            return check || checkDR || checkDL;
        }

        private bool Check(TicTacToeTile tile, TileDirection direction, int count = 0)
        {
            int row = tile.Row;
            int col = tile.Column;

            bool current = tile.Value == Turn.ToString()[0];

            if (count >= 2)
            {
                return current;
            }

            switch (direction)
            {
                case TileDirection.Horizontal:
                    col = col + 1 > 2 ? 0 : col + 1;
                    break;
                case TileDirection.Vertical:
                    row = row + 1 > 2 ? 0 : row + 1;
                    break;
                case TileDirection.DiagonalRight:
                    row = row + 1 > 2 ? 0 : row + 1;
                    col = col + 1 > 2 ? 0 : col + 1;
                    break;
                case TileDirection.DiagonalLeft:
                    row = row - 1 < 0 ? 2 : row - 1;
                    col = col - 1 < 0 ? 2 : col - 1;
                    break;
            }

            return current && Check(this[row * 3 + col], direction, count + 1);
        }

        private static ObservableCollection<TicTacToeTile> GenerateTiles()
		{
            return new ObservableCollection<TicTacToeTile>()
            {
                new TicTacToeTile(0, 0), new TicTacToeTile(0, 1), new TicTacToeTile(0, 2),
                new TicTacToeTile(1, 0), new TicTacToeTile(1, 1), new TicTacToeTile(1, 2),
                new TicTacToeTile(2, 0), new TicTacToeTile(2, 1), new TicTacToeTile(2, 2)
            };
        }

		private void SetProperty<T>(ref T oldValue, T newValue , [CallerMemberName] string memberName = "" )
        {
            oldValue = newValue;
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(memberName));
        }
	}
}

