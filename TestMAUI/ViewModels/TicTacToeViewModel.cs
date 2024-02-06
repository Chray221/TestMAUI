using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using TestMAUI.Models;
using TestMAUI.Models.EventHandlers;

namespace TestMAUI.ViewModels
{
	public partial class TicTacToeViewModel : BaseViewModel
	{
        [ObservableProperty] public TicTacToe board;
        [ObservableProperty] public string message;
        [ObservableProperty] public string turnMessage;
        [ObservableProperty] private bool isEnded;

        [ObservableProperty] private RowDefinitionCollection rowDefinitions = new RowDefinitionCollection();
        [ObservableProperty] private ColumnDefinitionCollection columnDefinations = new ColumnDefinitionCollection();

        public TicTacToeViewModel(ILogger<TicTacToeViewModel> logger) : base(logger)
        {
            int size = 100;
            for (int rowDef = 0; rowDef < 3; rowDef++)
            {
                //RowDefinitions.Add(new RowDefinition(GridLength.Star));
                RowDefinitions.Add(new RowDefinition(size));
            }
            for (int colDef = 0; colDef < 3; colDef++)
            {
                //ColumnDefinations.Add(new ColumnDefinition(GridLength.Star));
                ColumnDefinations.Add(new ColumnDefinition(size));
            }

            Board = board ?? new TicTacToe();
            Board.Start();
            SetTurnMessage();
        }

        public override void OnNavigatedTo()
        {
            base.OnNavigatedTo();
            //Board = board ?? new TicTacToe();
            //Board.Start();
            //SetTurnMessage();
        }

        [RelayCommand]
        public async Task ClickTile(TicTacToeTile tile)
        {
            Board.SetTile(tile);

            string endTitle = "";
            string endMessage = "";
            switch (Board.Status)
            {
                case GameStatus.GameOver:
                    endTitle = "GAME OVER!";
                    endMessage = "Game Over!!, \n\nYou hit a bomb,\n\n Do you want to continue?";
                    Message = "GAME OVER";
                    break;
                case GameStatus.Winner:
                    endTitle = "WINNER";
                    endMessage = "You win the game,\n Do you want to continue?";
                    Message = "!!! WINNER !!!";
                    break;
                case GameStatus.Draw:
                    endTitle = "DRAW";
                    endMessage = "The Game is a Draw,\n Do you want to continue?";
                    Message = "!!! DRAW !!!";
                    break;
            }
            if (Board.IsGameEnded())
            {
                //Toast.Make(Message).Show();
                bool answer = await DisplayAlertAsync(endTitle, endMessage, "Yes", "No");
                if (answer)
                {
                    ReGenerate();
                }
            }
            SetTurnMessage();
        }

        [RelayCommand]
        public void ReGenerate()
        {
            MainThread.BeginInvokeOnMainThread(Board.Start);
            IsEnded = false;
            SetTurnMessage();
        }

        [RelayCommand]
        public void TileSelected(TicTacToeTileClickedEventArg arg)
        {
            TurnMessage = $"Turn of {arg.Turn}";
            //SetTurnMessage();
        }

        private void SetTurnMessage()
        {
            TurnMessage = $"Turn of {Board.Turn}";
        }
    }
}