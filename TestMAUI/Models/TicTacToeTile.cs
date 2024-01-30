using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TestMAUI.Models
{
	public partial class TicTacToeTile : BaseTile
	{
        [ObservableProperty] private char value;
        public TicTacToeTile(int row, int column) : base(row, column)
        {
            Clear();
        }

        public void Clear()
        {
            Value = ' ';
        }
    }

    public enum TicTacToeTurn
    {
        X,
        O
    }

    public enum TileDirection
    {
        Horizontal,
        Vertical,
        DiagonalLeft,
        DiagonalRight
    }
}

