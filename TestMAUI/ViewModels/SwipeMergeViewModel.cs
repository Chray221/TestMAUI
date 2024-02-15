using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using TestMAUI.Models;
using TestMAUI.Services.Interfaces;

namespace TestMAUI.ViewModels
{
    public partial class SwipeMergeViewModel : BaseViewModel
	{
        ISwipeMergeService _swipeMergeService;

        //[ObservableProperty] private SwipeMerge board;
        [ObservableProperty] private string message;
        public SwipeMergeViewModel(
            ILogger<SwipeMergeViewModel> logger,
            ISwipeMergeService swipeMergeService) : base(logger)
        {
            _swipeMergeService = swipeMergeService;
            //board ??= new SwipeMerge();
            //board.Start();
        }

        [RelayCommand]
        public void ReGenerate()
        {
            //MainThread.BeginInvokeOnMainThread(Board.Start);
            MainThread.BeginInvokeOnMainThread(_swipeMergeService.Reset);
            //IsEnded = false;
            SetMessage();
        }
        
        private void SetMessage()
        {
            //Message = $"Top Score: {Board.Max( tile => tile.Value)}";
            Message = $"Top Score: {_swipeMergeService.Score}";
        }

        [RelayCommand]
        public async void SwipeLeft()
        {
            //Board.SwipeLeft();
            await _swipeMergeService.SwipeAsync(SwipeDirection.Left);
            await CheckGame();
        }

        [RelayCommand]
        public async void SwipeRight()
        {
            //Board.SwipeRight();
            await _swipeMergeService.SwipeAsync(SwipeDirection.Right);
            await CheckGame();
        }

        [RelayCommand]
        public async void SwipeUp()
        {
            //Board.SwipeUp();
            await _swipeMergeService.SwipeAsync(SwipeDirection.Up);
            await CheckGame();
        }

        [RelayCommand]
        public async void SwipeDown()
        {
            //Board.SwipeDown();
            await _swipeMergeService.SwipeAsync(SwipeDirection.Down);
            await CheckGame();
        }

        private async Task CheckGame()
        {
            SetMessage();

            string endTitle = "";
            string endMessage = "";
            
            //switch (Board.GameStatus)
            switch (_swipeMergeService.GameStatus)
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

            if (!_swipeMergeService.IsPlaying())
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

