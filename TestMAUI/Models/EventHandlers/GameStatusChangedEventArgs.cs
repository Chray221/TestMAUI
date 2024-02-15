using System;
namespace TestMAUI.Models.EventHandlers
{
	public class GameStatusChangedEventArgs: EventArgs
	{
		public GameStatus Status { get; init; }
        public int MaxValue { get; init; }

        public GameStatusChangedEventArgs(GameStatus status, int maxValue)
        {
            Status = status;
            MaxValue = maxValue;
        }
    }
}

