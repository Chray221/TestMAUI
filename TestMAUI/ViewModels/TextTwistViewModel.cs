using System;
using Microsoft.Extensions.Logging;
using TestMAUI.Models;
using TestMAUI.Repository.IRepository;
using TestMAUI.Services.Interfaces;

namespace TestMAUI.ViewModels
{
	public class TextTwistViewModel : BaseViewModel
	{
        private GameStatus _gameStatus;
        public GameStatus GameStatus
        {
            get { return _gameStatus; }
            set { SetProperty(ref _gameStatus, value); GameStatusChanged(); }
        }

        readonly ITextTwistService _textTwistService;
        readonly IDictionaryRepository _dictionaryRepository;
        public TextTwistViewModel(
            ILogger<TextTwistViewModel> logger,
            ITextTwistService textTwistService,
            IDictionaryRepository dictionaryRepository) : base(logger)
        {
            _textTwistService = textTwistService;
            _dictionaryRepository = dictionaryRepository;
        }

        public override async void OnNavigatedTo()
        {
            base.OnNavigatedTo();
            await Generate();
        }

        public async Task Generate(bool isRestart = true)
        {
            var words = await _dictionaryRepository.GetTextTwistWordsAsync();
            await _textTwistService.SetWordsAsync(words.Select(word => word.Word).ToList());
            if (isRestart)
            {
                _textTwistService.StopTimer();
            }
            _textTwistService.StartTimer();
        }

        private async void GameStatusChanged()
        {
            switch (GameStatus)
            {
                case GameStatus.Paused:
                    // show paused game;
                    await DisplayAlertAsync("PAUSED", "The Game is currently paused, would you like to play now?", "YES");
                    _textTwistService.StartTimer();
                    break;
                case GameStatus.Stopped:
                    // this is impossible;
                    break;
                case GameStatus.Playing:
                    break;
                case GameStatus.GameOver:
                    // show game over
                    bool playAgain = await DisplayAlertAsync("GAME OVER", "You didn't make it, better luck next time, would you like to play another game?", "YES", "NO");
                    if (playAgain)
                    {
                        await _textTwistService.ResetGameAsync();
                        await Generate(false);
                    }
                    else
                        await GoBackAsync();
                    break;
                case GameStatus.Winner:
                    // show winner view
                    bool proceed = await DisplayAlertAsync("WINNER", "You make it, would you like to proceed to the next round?", "YES", "NO");
                    if (proceed)
                        await Generate(true);
                    else
                        await GoBackAsync();
                    break;
            }
        }
    }
}

