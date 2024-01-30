using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using TestMAUI.Helpers;
using TestMAUI.Models;
using TestMAUI.Services.Interfaces;

namespace TestMAUI.ViewModels
{
    [QueryProperty(nameof(RowNum), nameof(RowNum))]
    [QueryProperty(nameof(ColNum), nameof(ColNum))]
    public partial class MineSweeperViewModel : BaseViewModel, IQueryAttributable , IPageLoadedLifeCycle
	{

        [ObservableProperty] private int rowNum = 10;
        [ObservableProperty] private int colNum = 10;

        [ObservableProperty] private RowDefinitionCollection rowDefinitions = new RowDefinitionCollection();
        [ObservableProperty] private ColumnDefinitionCollection columnDefinations = new ColumnDefinitionCollection();
        [ObservableProperty] private MineSweeper tiles;

        [ObservableProperty] private bool isEnded;
        [ObservableProperty] private string message;
        [ObservableProperty] private bool isFlagging;
        [ObservableProperty] private int tileFlaggedCount;

        public MineSweeperViewModel(ILogger<MineSweeperViewModel> logger)
            : base(logger)
        {
            
        }

        public override void OnNavigatedTo()
        {
            PageLog();
            Tiles = new MineSweeper(RowNum, ColNum, (RowNum + ColNum) / 2);
            Generate();
            base.OnNavigatedTo();
        }

        public void OnPageLoaded()
        {
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            query.TryGetRefValue(nameof(RowNum), ref rowNum);
            query.TryGetRefValue(nameof(ColNum), ref colNum);
        }

        public override void OnNavigatingTo()
        {
            base.OnNavigatingTo();
            //Tiles = new MineSweeper(RowNum, ColNum, (RowNum + ColNum) / 2);
            //Generate();
        }

        private void Generate()
        {
            Tiles.Clear();
            Tiles.Generate();
            for (int rowDef = 0; rowDef < Tiles.TotalRows; rowDef++)
            {
                RowDefinitions.Add(new RowDefinition(GridLength.Star));
            }
            for (int colDef = 0; colDef < Tiles.TotalColumns; colDef++)
            {
                ColumnDefinations.Add(new ColumnDefinition(GridLength.Star));
            }
        }

        [RelayCommand]
        public void ReGenerate()
        {
            MainThread.BeginInvokeOnMainThread(Tiles.ReGenerate);
            IsEnded = false;
        }

        [RelayCommand]
        public void SetFlagging()
        {
            IsFlagging = !IsFlagging;
        }

        [RelayCommand]
        public async Task ClickTile(MineSweeperTile tile)
        {
            if (IsFlagging)
            {
                tile.IsFlagged = !tile.IsFlagged;
                TileFlaggedCount = Tiles.TotalFlags;
                //IsFlagging = false;
                return;
            }

            Tiles.OpenTile(tile);

            string endTitle = "";
            string endMessage = "";
            switch (Tiles.GameStatus)
            {
                case GameStatus.GameOver:

                    endTitle = "Kabooooom!";
                    endMessage = "Game Over!!, \n\nYou hit a bomb,\n\n Do you want to continue?";
                    Message = "GAME OVER";
                    break;
                case GameStatus.Winner:
                    endTitle = "Winner";
                    endMessage = "You win the game,\n Do you want to continue?";
                    Message = "!!! WINNER !!!";
                    break;
            }
            if(Tiles.IsGameEnded())
            {
                //Toast.Make(Message).Show();
                bool answer = await DisplayAlertAsync(endTitle, endMessage, "Yes", "No");
                if (answer)
                {
                    ReGenerate();
                }
            }
        }
    }
}

