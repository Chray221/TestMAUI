using System.Windows.Input;
using TestMAUI.Helpers;
using TestMAUI.Models;
using TestMAUI.Models.EventHandlers;
using TestMAUI.Services.Interfaces;

namespace TestMAUI.CustomViews;

public partial class TicTacToeView : ContentView, ITicTacToeService
{
    const int MaxRow = 3;
    const int MaxCol = 3;
    const string EmptyTileText = "";


    public static readonly BindableProperty TurnProperty = BindableProperty.Create(propertyName: nameof(Turn), returnType: typeof(TicTacToeTurn), declaringType: typeof(TicTacToeView), defaultValue: RandomizerHelper.GetBoolean() ? TicTacToeTurn.X : TicTacToeTurn.O);
    public static readonly BindableProperty GameStatusProperty = BindableProperty.Create(propertyName: nameof(GameStatus), returnType: typeof(GameStatus), declaringType: typeof(TicTacToeView), defaultValue: GameStatus.Playing);
    public static readonly BindableProperty TileSelectedCommandProperty = BindableProperty.Create(propertyName: nameof(TileSelectedCommand), returnType: typeof(ICommand), declaringType: typeof(TicTacToeView), defaultValue: null);

    public TicTacToeTurn Turn
    {
        get => (TicTacToeTurn)GetValue(TurnProperty);
        set => SetValue(TurnProperty, value);
    }
    public GameStatus GameStatus
    {
        get => (GameStatus)GetValue(GameStatusProperty);
        set => SetValue(GameStatusProperty, value);
    }
    public ICommand TileSelectedCommand
    {
        get => (ICommand)GetValue(TileSelectedCommandProperty);
        set => SetValue(TileSelectedCommandProperty, value);
    }



    public void Start()
    {
        foreach (IView tile in Board.Children)
        {
            if(tile is Label labelTile)
            {
                labelTile.Text = EmptyTileText;
            }
        }
        Turn = Random.Shared.Next(0, 100) % 2 == 0 ? TicTacToeTurn.X : TicTacToeTurn.O;
        GameStatus = GameStatus.Playing;
    }

    private static List<int[]> WinningIndex = new List<int[]>
        {             // top -      , topmiddle |       , right |
            new int[] { 0, 1, 2 }, new int[] { 1, 4, 7 }, new int[] { 2, 5, 8 },
                      // left |     ,    blank          , rightmid -
            new int[] { 0, 3, 6 }, new int[] { 0, 3, 6 }, new int[] { 3, 4, 5 },
                      // botleft /  , bot -             , botright \
            new int[] { 6, 4, 2 }, new int[] { 6, 7, 8 }, new int[] { 0, 4, 8 },
        };

    public TicTacToeView()
	{
		InitializeComponent();
        GenerateBoard();
        Start();
    }

    private void GenerateBoard()
    {
        int size = 100;
        var rowDef = new RowDefinition(size);
        var columnDef = new ColumnDefinition(size);
        bool isColumnDefCreating = true;
        for (int row = 0; row < MaxRow; row++)
        {
            Board.RowDefinitions.Add(rowDef);
            for (int col = 0; col < MaxCol; col++)
            {
                if (isColumnDefCreating)
                {
                    Board.ColumnDefinitions.Add(columnDef);
                }

                Color Gray400 = Colors.Gray;
                App.Current.Resources.TryGetRefValue("Gray400", ref Gray400);
                // create tile
                Label newLabel = new Label()
                {
                    BackgroundColor = Gray400,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    TextColor = Colors.Black,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 50,
                };
                var tapGest = new TapGestureRecognizer();
                tapGest.Tapped += Tile_Tapped;
                newLabel.GestureRecognizers.Add(tapGest);
                Board.Add(newLabel);
                Grid.SetRow(newLabel, row);
                Grid.SetColumn(newLabel, col);
            }
            isColumnDefCreating = false;
        }
    }

    private async void Tile_Tapped(object sender, TappedEventArgs e)
    {
        if(sender is Label label)
        {
            SetTile(label);

            //Board.Children.IndexOf(label);
            TileSelectedCommand?.Execute(new TicTacToeTileClickedEventArg(
                turn: Turn,
                status: GameStatus,
                indexSelected: Board.Children.IndexOf(label)));

            string endTitle = "";
            string endMessage = "";
            switch (GameStatus)
            {
                case GameStatus.GameOver:
                    endTitle = "GAME OVER!";
                    endMessage = "Game Over!!, \n\nYou hit a bomb,\n\n Do you want to continue?";
                    //Message = "GAME OVER";
                    break;
                case GameStatus.Winner:
                    endTitle = $"{Turn} is WINNER";
                    endMessage = "You win the game,\n Do you want to continue?";
                    //Message = "!!! WINNER !!!";
                    break;
                case GameStatus.Draw:
                    endTitle = "DRAW";
                    endMessage = "The Game is a Draw,\n Do you want to continue?";
                    //Message = "!!! DRAW !!!";
                    break;
            }
            if (IsGameEnded())
            {
                //Toast.Make(Message).Show();
                bool answer = await Shell.Current.DisplayAlert(endTitle, endMessage, "Yes", "No");
                if (answer)
                {
                    Start();
                }
            }
            //SetTurnMessage();
        }
    }


    public bool IsGameEnded()
    {
        return GameStatus != GameStatus.Playing;
    }

    public bool SetTile(Label tile)
    {
        tile.Text = $"{Turn.ToString()[0]}";

        bool isWinner = CheckWinner(tile, true);
        //Console.WriteLine($"isWinner: {isWinner}");
        if (IsBoardFilled() || isWinner)
        {
            GameStatus = isWinner ? GameStatus.Winner : GameStatus.Draw;
        }

        if (IsGameEnded()) return true;

        Turn = Turn == TicTacToeTurn.X ? TicTacToeTurn.O : TicTacToeTurn.X;
        return false;
    }

    private bool IsBoardFilled()
    {
        return Board.Children.All(x => (x as Label).Text != EmptyTileText);
    }

    public bool CheckWinner(Label tile, bool @void)
    {
        int index = Board.Children.IndexOf(tile);
        return WinningIndex
            .Where(indexes => indexes.Contains(index))
            .Any(indexes => indexes.All(index => (Board.Children[index] as Label).Text == tile.Text));
        //return WinningIndex
        //    .Where(indexes => indexes.Contains(tile.Row * 3 + tile.Column))
        //    .Any(indexes => indexes.All(index => this[index].Value == tile.Value));
    }

    public bool CheckWinner(Label tile)
    {
        int index = Board.Children.IndexOf(tile);
        int row = index / MaxRow;
        int col = index % MaxRow;

        //hor || ver
        bool check = Check(tile, TileDirection.Horizontal) ||
                     Check(tile, TileDirection.Vertical);

        //Console.Write($"checking({check}");
        //tl 
        switch ((row, col))
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

    private bool Check(Label tile, TileDirection direction, int count = 0)
    {
        //int row = tile.Row;
        //int col = tile.Column;
        int index = Board.Children.IndexOf(tile);
        int row = index / MaxRow;
        int col = index % MaxRow;
        //bool current = tile.Text == Turn.ToString()[0];
        bool current = tile.Text == $"{Turn.ToString()[0]}";

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

        return current && Check(Board.Children[row * 3 + col] as Label, direction, count + 1);
    }

    private (int Index, int Row, int Col) GetTileDetail(IView tile)
    {
        int index = Board.Children.IndexOf(tile);
        int row = index / MaxRow;
        int col = index % MaxRow;
        return (index, row, col);
    }
}
