using CommunityToolkit.Mvvm.ComponentModel;

namespace TestMAUI.Models
{
    public class BaseTile : ObservableObject
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public BaseTile(int row, int column)
		{
            Row = row;
            Column = column;
        }

        public bool IsAroundTile(int row, int col)
        {
            return GetTopLeftTile()     == (row - 1, col - 1) ||
                   GetTopTile()         == (row - 1, col    ) ||
                   GetTopRightTile()    == (row - 1, col + 1) ||
                   GetRightTile()       == (row    , col + 1) ||
                   GetBottomRightTile() == (row + 1, col + 1) ||
                   GetBottomTile()      == (row + 1, col    ) ||
                   GetBottomLeftTile()  == (row + 1, col - 1) ||
                   GetLeftTile()        == (row    , col - 1);
            //return (Row == row - 1 && Column == col - 1) || //tl
            //       (Row == row - 1 && Column == col) || // t-top
            //       (Row == row - 1 && Column == col + 1) || // tr
            //       (Row == row && Column == col + 1) || // r-right
            //       (Row == row + 1 && Column == col + 1) || // br
            //       (Row == row + 1 && Column == col) || // b-bottom
            //       (Row == row + 1 && Column == col - 1) || // bl
            //       (Row == row && Column == col - 1) // l-left
            //       ;
        }

        public (int Row, int Column) GetTopLeftTile() => (Row - 1, Column - 1);
        public (int Row, int Column) GetTopTile() => (Row - 1, Column);
        public (int Row, int Column) GetTopRightTile() => (Row - 1, Column + 1);
        public (int Row, int Column) GetRightTile() => (Row, Column + 1);
        public (int Row, int Column) GetBottomRightTile() => (Row + 1, Column + 1);
        public (int Row, int Column) GetBottomTile() => (Row + 1, Column);
        public (int Row, int Column) GetBottomLeftTile() => (Row + 1, Column - 1);
        public (int Row, int Column) GetLeftTile() => (Row, Column - 1);
    }
}

