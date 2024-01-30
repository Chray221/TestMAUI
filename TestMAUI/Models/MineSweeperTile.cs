using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TestMAUI.Models;

public partial class MineSweeperTile : BaseTile
{
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

    public MineSweeperTile() : base(0,0)
	{
    }

    public MineSweeperTile(int row, int column) : base(row, column)
    {
    }

    public void Clear()
    {
        IsBomb = false;
        IsFlagged = false;
        IsOpen = false;
        BombInAreaCount = 0;
    }

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

