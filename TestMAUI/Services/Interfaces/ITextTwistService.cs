
namespace TestMAUI.Services.Interfaces
{
	public interface ITextTwistService
	{
        Task SetWordsAsync(List<string> words);
        void StartTimer();
        void PauseTimer();
        void StopTimer();
        void ShowAllWords();
        Task ResetGameAsync();
        Task ClearKeysAsync();
        Task ShuffleKeys();
        Task CheckEnteredWordAsync();
        Task DeleteEnteredKeyAsync();
        Task EnterLastEnteredKeysAsync();
    }
}

