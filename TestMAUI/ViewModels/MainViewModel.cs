using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using TestMAUI.Views;

namespace TestMAUI.ViewModels
{
	public partial class MainViewModel : BaseViewModel
    {
        int count = 0;

        [ObservableProperty] private bool mineSweeperExpanded = false;
        [ObservableProperty] private string countText = "Click me";
        [ObservableProperty] private List<string> mineSweepers = new List<string>()
        {
            "3x3",
            "6x6",
            "8x8",
            "10x10",
            "13x10",
            "15x10",
            "20x10",
            "30x10",
            "40x10"
        };

        public MainViewModel(ILogger<MainViewModel> logger)
            : base(logger)
        {
        }

        [RelayCommand]
        public void CounterClicked()
        {
            //count++;
            CountText = $"Clicked {++count} time{(count > 1 ? "s" : "")}";
            SemanticScreenReader.Announce(CountText);
        }

        [RelayCommand]
        public async Task ToMineSweeper(string dimen)
        {
            int rowNum = 10;
            int colNum = 10;
            string[] dimens = dimen.Split('x');
            if (dimens.Length >= 2)
            {
                _ = int.TryParse(dimens[0], out rowNum);
                _ = int.TryParse(dimens[1], out colNum);
            }

            await NavigateToAsync<MineSwepperPage>(
                new KeyValuePair<string, object>(nameof(MineSweeperViewModel.RowNum), rowNum),
                new KeyValuePair<string, object>(nameof(MineSweeperViewModel.ColNum), colNum));
            
        }

        [RelayCommand]
        public async Task ToTicTacToe()
        {
            await NavigateToAsync<TicTacToePage>();
        }

        [RelayCommand]
        public async Task ToSwipeMerge()
        {
            await NavigateToAsync<SwipeMergePage>();
        }

        [RelayCommand]
        public void ExpandMineSweeper()
        {
            MineSweeperExpanded = !MineSweeperExpanded;
        }
    }
}

