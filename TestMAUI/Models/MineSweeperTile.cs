using System;
using System.ComponentModel;
using Android.Service.QuickSettings;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TestMAUI.Models;

public partial class MineSweeperTile : ObservableObject
{
    public int Row { get; set; }
    public int Column { get; set; }
    public bool IsBomb { get; set; }
    //public bool IsShowFlag { get; set; }
    //public int BombInAreaCount { get; set; }

    //[ObservableProperty]
    //private bool isBomb;
    [ObservableProperty]
    private bool isFlagged;
    [ObservableProperty]
    private bool isOpen;
    [ObservableProperty]
    public int bombInAreaCount;

    public TileStatus Status
    {
        get
        {
            if (IsOpen)
            {
                return IsBomb
                        ? TileStatus.IsBomb
                        : BombInAreaCount > 0
                            ? TileStatus.IsOpen
                            : TileStatus.IsOpenEmpty;
            }
            else if(IsFlagged)
            {
                return TileStatus.IsFlagged;
            }
            return TileStatus.Default;
        }
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if(!e.PropertyName.Equals(nameof(Status)))
        {
            OnPropertyChanged(nameof(Status));
        }
    }

    public MineSweeperTile()
	{
    }

    public MineSweeperTile(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public void Clear()
    {
        IsBomb = false;
        IsFlagged = false;
        IsOpen = false;
        BombInAreaCount = 0;
    }

    public bool IsAroundTile(int row, int col)
    {
        return (Row == row-1 && Column == col-1) || //tl
               (Row == row-1 && Column == col) || // t-top
               (Row == row-1 && Column == col+1) || // tr
               (Row == row   && Column == col+1) || // r-right
               (Row == row+1 && Column == col+1) || // br
               (Row == row+1 && Column == col) || // b-bottom
               (Row == row+1 && Column == col-1) || // bl
               (Row == row && Column == col-1) // l-left
               ;
    }

    public (int Row, int Column) GetTopLeftTile()     => (Row - 1, Column - 1);
    public (int Row, int Column) GetTopTile()         => (Row - 1, Column);
    public (int Row, int Column) GetTopRightTile()    => (Row - 1, Column + 1);
    public (int Row, int Column) GetRightTile()       => (Row    , Column + 1);
    public (int Row, int Column) GetBottomRightTile() => (Row + 1, Column + 1);
    public (int Row, int Column) GetBottomTile()      => (Row + 1, Column);
    public (int Row, int Column) GetBottomLeftTile()  => (Row + 1, Column - 1);
    public (int Row, int Column) GetLeftTile()        => (Row, Column - 1);

    public static MineSweeperTile CreateBomb(int row, int column)
    {
        return new MineSweeperTile(row, column) { IsBomb = true };
    }
}

public enum TileStatus
{
    Default,
    IsFlagged,
    IsOpen,
    IsOpenEmpty,
    IsBomb
}

