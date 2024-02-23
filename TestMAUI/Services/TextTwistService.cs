using System;
using TestMAUI.CustomViews.TextTwist;
using TestMAUI.Services.Interfaces;

namespace TestMAUI.Services
{
	public class TextTwistService : ITextTwistService
    {
        static TextTwistService _instance;
        public static TextTwistService Instance { get { return _instance ??= new TextTwistService(); } }

        TextTwistView _view;
        private TextTwistService()
        {
        }

        public void SetService(TextTwistView view)
        {
            _view = view;
        }

        public Task SetWordsAsync(List<string> words) => _view.SetWordsAsync(words);
        public void StartTimer() => _view.StartTimer();
        public void PauseTimer() => _view.PauseTimer();
        public void StopTimer() => _view.StopTimer();
        public Task ClearKeysAsync() => _view.ClearKeysAsync();
        public Task ShuffleKeys() => _view.ShuffleKeys();
        public Task CheckEnteredWordAsync() => _view.CheckEnteredWordAsync();
        public Task DeleteEnteredKeyAsync() => _view.DeleteEnteredKeyAsync();
        public Task EnterLastEnteredKeysAsync() => _view.EnterLastEnteredKeysAsync();
        public Task ResetGameAsync() => _view.ResetGameAsync();

        public void ShowAllWords() => _view.ShowAllWords();
    }
}

