using System;
namespace TestMAUI.Models.EventHandlers;

public class TicTacToeTileClickedEventArg : EventArgs
{
	public TicTacToeTurn Turn { get; init; }
    public GameStatus GameStatus { get; init; }
	public int IndexSelected { get; init; }
	public TicTacToeTileClickedEventArg(
		TicTacToeTurn turn,
        GameStatus status,
		int indexSelected)
	{
		Turn = turn;
		GameStatus = status;
		IndexSelected = indexSelected;
    }
}

