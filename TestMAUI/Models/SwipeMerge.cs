using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using TestMAUI.Helpers;

namespace TestMAUI.Models;

public class SwipeMerge : ReadOnlyObservableCollection<SwipeMergeTile>
{
    const int MaxRow = 4;
    const int MaxColumn = 4;

    private GameStatus status;
    public GameStatus GameStatus
    {
        get { return status; }
        private set { SetProperty(ref status, value); }
    }

    public SwipeMerge() : base(GenerateTiles())
    {

    }

    private static ObservableCollection<SwipeMergeTile> GenerateTiles()
    {
        ObservableCollection<SwipeMergeTile> board = new ObservableCollection<SwipeMergeTile>();
        for (int row = 0; row < MaxRow; row++)
        {
            for(int col = 0; col < MaxColumn; col++)
            {
                board.Add(new SwipeMergeTile(row, col));
            }
        }
        return board;
    }

    public void Start()
    {
        Clean();
        GameStatus = GameStatus.Playing;
        Generate(2);
    }

    public void Swipe(SwipeDirection direction)
    {
        switch (direction)
        {
            case SwipeDirection.Right:  SwipeRight();  break;
            case SwipeDirection.Left:   SwipeLeft();  break;
            case SwipeDirection.Up:     SwipeUp();  break;
            case SwipeDirection.Down:   SwipeUp();  break;
        }
    }

    public void SwipeRight() => SwipeHorizontal(false);
    public void SwipeLeft() => SwipeHorizontal(true);
    public void SwipeDown() => SwipeVertical(false);
    public void SwipeUp() => SwipeVertical(true);


    private void SwipeHorizontal(bool isLeft)
    {
        //int crement = isLeft ? -1 : 1;
        int crement = isLeft ? 1 : -1;
        SwipeDirection direction = isLeft
                    ? SwipeDirection.Left
                    : SwipeDirection.Right;

        if (!IsPlaying())
            return;

        for (//int col = isLeft ? 3 : 0;
             int col = isLeft ? 1 : 2;
             //isLeft ? col > 0 : col < 3;
             isLeft ? col <= 3 : col >= 0;
             col += crement)
        {
            MergeColumnToEnd(col, direction);
            MergeColumnToEnd(col + (1 * MaxRow), direction);
            MergeColumnToEnd(col + (2 * MaxRow), direction);
            MergeColumnToEnd(col + (3 * MaxRow), direction);
        }

        Generate(2);
    }

    private void SwipeVertical(bool isUp)
    {
        //int crement = isUp ? -4 : 4;
        int crement = isUp ? 4 : -4;
        SwipeDirection direction = isUp
                    ? SwipeDirection.Up
                    : SwipeDirection.Down;
        if (!IsPlaying())
            return;

        for (//int row = isUp ? 12 : 0;
             int row = isUp ? 4 : 8;
             //isUp ? row > 0 : row < 12;
             isUp ? row <= 12 : row >= 0;
             row += crement)
        {
            MergeColumnToEnd(row, direction);
            MergeColumnToEnd(row + 1, direction);
            MergeColumnToEnd(row + 2, direction);
            MergeColumnToEnd(row + 3, direction);
        }

        Generate(2);
    }

    private void MergeColumnToEnd(
        int index,
        SwipeDirection direction)
    {
        if (this[index].IsEmpty())
        {
            return;
        }

        int increment = 0;
        int endIndex = 0;
        bool isStopMerging = false;
        switch (direction)
        {
            case SwipeDirection.Left:
                increment = -1;
                endIndex = GetRowStartIndex(index);
                isStopMerging = index <= endIndex;
                break;
            case SwipeDirection.Right:
                increment = 1;
                endIndex = GetRowEndIndex(index);
                isStopMerging = index >= endIndex;
                break;
            case SwipeDirection.Up:
                increment = -4;
                endIndex = GetColumnStartIndex(index);
                isStopMerging = index <= endIndex;
                break;
            case SwipeDirection.Down:
                increment = 4;
                endIndex = GetColumnEndIndex(index);
                isStopMerging = index >= endIndex;
                break;
        }

        //isStopMerging = index == endIndex || index <= -1 || index >= Count;
        isStopMerging = isStopMerging || index <= -1 || index >= Count;

        int nextIndex = index + increment;
         Log($"value:{this[index].Value} ,index: {index}, endIndex:{endIndex}, isStopMerging:{isStopMerging}");
        if (!isStopMerging)
        {
            if (this[index] == this[nextIndex])
            {
                this[nextIndex].Add(this[index]);
                this[index].Clear();
            }
            else if (this[nextIndex].IsEmpty())
            {
                this[nextIndex].Value = this[index].Value;
                this[index].Clear();
            }

            MergeColumnToEnd(
                nextIndex,
                direction);
        }
    }

    public bool IsPlaying()
    {
        return GameStatus == GameStatus.Playing;
    }

    public bool IsBoardField()
    {
        return this.All(tile => !tile.IsEmpty());
    }

    private void Clean()
    {
        for (int index = 0; index < Count; index++)
        {
            this[index].Clear();
        }
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
                indexOfTile = Random.Shared.Next(0, Count);
                if(IsBoardField())
                {
                    //GameStatus = GameStatus.GameOver;
                    break;
                }
                if (!this[indexOfTile].IsEmpty())
                {
                    goto GENERATE_VALUE;
                }
                this[indexOfTile].Value = value;
            }
        }
        else
        {
            GameStatus = GameStatus.GameOver;
        }
    }

    #region RowCol Methods
    private static int DetermineRow(int index)
    {
        return (index) / 4;
    }

    private static int GetRowStartIndex(int index)
    {
        return DetermineRow(index) * 4;
    }

    private static int GetRowEndIndex(int index)
    {
        return GetRowStartIndex(index) + 3;
    }

    private static int GetColumnStartIndex(int index)
    {
        return index % 4;
    }

    private static int GetColumnEndIndex(int index)
    {
        return 11 + DetermineColumn(index);
    }

    private static int DetermineColumn(int index)
    {
        return index % 4;
    }
    #endregion

    private void SetProperty<T>(ref T oldValue, T newValue, [CallerMemberName] string memberName = "")
    {
        oldValue = newValue;
        OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(memberName));
    }

    private void Log(string message)
    {
        System.Diagnostics.Debug.WriteLine(message);
        //Console.WriteLine(message);
    }
}

