
namespace TestMAUI.Services.Interfaces
{
	public interface ITextTwistService
	{
        Task SetWords(List<string> words);
        void StartTimer();
        void PauseTimer();
        void StopTimer();
        Task ClearAsync();
        Task ShuffleKeys();
        Task CheckEnteredWordAsync();
        Task DeleteEnteredKeyAsync();
        Task EnterLastEnteredKeysAsync();
    }
}

