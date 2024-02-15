using System;
using TestMAUI.Models;

namespace TestMAUI.Services.Interfaces
{
	public interface ISwipeMergeService
	{
        void Reset();
        GameStatus GameStatus { get; }
        int Score { get; }
        bool IsPlaying();
        Task SwipeAsync(SwipeDirection direction);
    }
}

