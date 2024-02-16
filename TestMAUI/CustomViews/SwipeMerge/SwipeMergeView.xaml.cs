#nullable enable

using System.Windows.Input;
using TestMAUI.Helpers;
using TestMAUI.Models;
using TestMAUI.Models.EventHandlers;
using TestMAUI.Services;
using TestMAUI.Services.Interfaces;

namespace TestMAUI.CustomViews.SwipeMerge;

public partial class SwipeMergeView : ContentView
{
    const int MaxRow = 4;
    const int MaxColumn = 4;
    const int MaxTiles = MaxRow * MaxColumn;

    public static readonly BindableProperty GameStatusProperty = BindableProperty.Create(propertyName: nameof(GameStatus), returnType: typeof(GameStatus), declaringType: typeof(SwipeMergeView), defaultValue: GameStatus.Playing, propertyChanged: OnGameStatusChanged);

    public static readonly BindableProperty GameStatusChangedCommandProperty = BindableProperty.Create(propertyName: nameof(GameStatusChangedCommand), returnType: typeof(ICommand), declaringType: typeof(SwipeMergeView), defaultValue: null);

    public ICommand GameStatusChangedCommand
    {
        get => (ICommand)GetValue(GameStatusChangedCommandProperty);
        set => SetValue(GameStatusChangedCommandProperty, value);
    }

    public GameStatus GameStatus
    {
        get => (GameStatus)GetValue(GameStatusProperty);
        set => SetValue(GameStatusProperty, value);
    }

    public SwipeMergeView()
	{
		InitializeComponent();
        SwipeMergeService.Instance.SetService(this);
        
        Generate(2);
    }

    private void Generate(int numOfTile = 2, int min = 1, int max = 2)
    {
        int indexOfTile;
        int value;
        if (IsPlaying() && !IsBoardField())
        {
            for (int index = 0; index < numOfTile; index++)
            {
            GENERATE_VALUE:
                value = RandomizerHelper.GetInt(min, max);
                indexOfTile = Random.Shared.Next(0, MaxTiles);
                if (IsBoardField())
                {
                    //GameStatus = GameStatus.GameOver;
                    break;
                }
                if(FindTileByIndex(indexOfTile) is SwipeMergeTileView tileView)
                {
                    if (!tileView.IsEmpty())
                    {
                        goto GENERATE_VALUE;
                    }
                    tileView.Value = value;
                }
            }
        }
        else
        {
            GameStatus = GameStatus.GameOver;
        }
    }

    #region Tiles Utilities
    static readonly Func<SwipeMergeTileView, bool> IsNotNull = tile => !tile.IsEmpty();

    private static bool IsMergeable(SwipeMergeTileView tile, SwipeDirection direction) => direction switch
    {
        SwipeDirection.Right => tile.Column < 3,
        SwipeDirection.Left => tile.Column > 0,
        SwipeDirection.Down => tile.Row < 3,
        SwipeDirection.Up => tile.Row > 0,
        _ => false
    };

    private SwipeMergeTileView? FindTileByIndex(int index)
    {
        int row = GetRow(index);
        int column = GetColumn(index);
        return FindTile(row, column);
    }

    private SwipeMergeTileView? FindTile(int row, int col)
    {
        //Log($"Finding r:{row} c:{col} (tile{row}{col})");
        if (FindByName($"tile{row}{col}") is SwipeMergeTileView tileView)
        {
            return tileView;
        }
        return null;
    }

    private static int GetRow(int index) => index / (MaxColumn);

    private static int GetColumn(int index) => index % (MaxColumn - 1);

    public bool IsPlaying() => GameStatus == GameStatus.Playing;

    public bool IsBoardField() => GetTiles().All(tile => !tile.IsEmpty());
    #endregion

    #region Tiles Query
    private IEnumerable<IGrouping<int, SwipeMergeTileView>> GetNotEmptyRowTiles() => GetTiles(tile => !tile.IsEmpty()).GroupBy(tile => tile.Row);

    private IEnumerable<IGrouping<int, SwipeMergeTileView>> GetNotEmptyColumnTiles() => GetTiles(tile => !tile.IsEmpty()).GroupBy(tile => tile.Column);

    private IEnumerable<SwipeMergeTileView> GetRowTiles(int row, Func<SwipeMergeTileView, bool>? condition = null)
    {
        row = Math.Min(Math.Max(row, 0), MaxRow - 1);
        return GetTiles(tile => tile.Row == row && (condition?.Invoke(tile) ?? true));
    }

    private IEnumerable<SwipeMergeTileView> GetColumnTiles(int column, Func<SwipeMergeTileView, bool>? condition = null)
    {
        column = Math.Min(Math.Max(column, 0), MaxRow - 1);
        return GetTiles(tile => tile.Column == column && (condition?.Invoke(tile) ?? true));
    }

    private IEnumerable<SwipeMergeTileView> GetTiles(Func<SwipeMergeTileView, bool>? condition = null)
    {
        foreach (IView view in board)
        {
            if (view is SwipeMergeTileView tileView &&
                (condition == null || condition.Invoke(tileView)))
            {
                yield return tileView;
            }
        }
    }

    #endregion

    #region Version
    public async Task SwipeToAsync(SwipeDirection direction)
    {
        if (!isSwiping)
        {
            isSwiping = true;
            await Task.WhenAll(
                MergeLine4(0, direction),
                MergeLine4(1, direction),
                MergeLine4(2, direction),
                MergeLine4(3, direction));
            Generate(1);
            isSwiping = false;
        }
    }

    /// <summary>
    /// if left or right = row |
    /// if up or down = column
    /// </summary>
    /// <param name="lineNum"></param>
    /// <param name="direction"></param>
    async Task MergeLine4(int lineNum, SwipeDirection direction)
    {
        List<Task> tasks = new List<Task>();
        IEnumerable<SwipeMergeTileView> tiles = direction switch
        {
            SwipeDirection.Left =>  GetRowTiles(lineNum, IsNotNull).OrderBy(tile => tile.Column),
            SwipeDirection.Right => GetRowTiles(lineNum, IsNotNull).OrderByDescending(tile => tile.Column),
            SwipeDirection.Up => GetColumnTiles(lineNum, IsNotNull).OrderBy(tile => tile.Row),
            _ or SwipeDirection.Down => GetColumnTiles(lineNum, IsNotNull).OrderByDescending(tile => tile.Row),
        };
        foreach (var tile in tiles)
        {
            await tile.MergeAsync(direction);
        }
    }
    #endregion

    private void Clean()
    {
        foreach(SwipeMergeTileView tile in GetTiles(IsNotNull))
        {
            tile.Clear();
        }
    }

    public void Reset()
    {
        Clean();
        Generate(2);
    }

    public int GetScore() => GetTiles().Max(tile => tile.Value);

    bool isSwiping = false;
    async void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
    {
        await SwipeToAsync(e.Direction);
    }
    
    public void Log(string message)
    {
        System.Diagnostics.Debug.WriteLine(message);
    }

    private static void OnGameStatusChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SwipeMergeView view &&
            newValue is GameStatus newGameStatus)
        {
            view.GameStatusChangedCommand?.Execute(new GameStatusChangedEventArgs(newGameStatus, view.GetScore()));
        }
    }
}
