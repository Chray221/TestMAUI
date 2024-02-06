using System;
using TestMAUI.Models;

namespace TestMAUI.Services.Interfaces;

public interface ITicTacToeService
{    
    bool IsGameEnded();
    GameStatus GameStatus { get; }
}

