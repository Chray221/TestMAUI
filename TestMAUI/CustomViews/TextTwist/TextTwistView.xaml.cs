#nullable enable

using System.Linq;
using TestMAUI.Helpers;
using TestMAUI.Helpers.Extensions;
using TestMAUI.Models;
using TestMAUI.Services;
using TestMAUI.Services.Interfaces;

namespace TestMAUI.CustomViews.TextTwist;

public partial class TextTwistView : StackLayout , ITextTwistService
{
    #region BindableProperties
    public static readonly BindableProperty ScoreProperty = BindableProperty.Create(propertyName: nameof(Score), returnType: typeof(int), declaringType: typeof(TextTwistView), defaultValue: 0);
    public static readonly BindableProperty RoundProperty = BindableProperty.Create(propertyName: nameof(Round), returnType: typeof(int), declaringType: typeof(TextTwistView), defaultValue: 0);
    public static readonly BindableProperty TimerProperty = BindableProperty.Create(propertyName: nameof(Timer), returnType: typeof(TimeSpan), declaringType: typeof(TextTwistView), defaultValue: TimeSpan.FromMinutes(2), propertyChanged: OnTimerPropertyChanged);
    public static readonly BindableProperty GameStatusProperty = BindableProperty.Create(propertyName: nameof(GameStatus), returnType: typeof(GameStatus), declaringType: typeof(TextTwistView), defaultValue: GameStatus.Paused, defaultBindingMode: BindingMode.OneWayToSource);
    #endregion

    #region Properties
    public int Score
    {
        get => (int)GetValue(ScoreProperty);
        set => SetValue(ScoreProperty, value);
    }
    public int Round
    {
        get => (int)GetValue(RoundProperty);
        set => SetValue(RoundProperty, value);
    }
    public TimeSpan Timer
    {
        get => (TimeSpan)GetValue(TimerProperty);
        set => SetValue(TimerProperty, value);
    }
    public GameStatus GameStatus
    {
        get => (GameStatus)GetValue(GameStatusProperty);
        set => SetValue(GameStatusProperty, value);
    }

    #endregion

    #region _fields
    private IEnumerable<KeyView> _keys;
    private IEnumerable<KeyView> _lastEnteredKeys;
    private IEnumerable<WordTilesView> _wordsView;

    private bool _isShuffling = false;
    private bool _isChecking = false;

    IDispatcherTimer _timer;
    #endregion

    public TextTwistView()
	{
		InitializeComponent();
        TextTwistService.Instance.SetService(this);

        if (_timer is null)
        {
            _timer ??= Application.Current?.Dispatcher.CreateTimer() ?? Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += GameTimer_Tick;
        }
    }

    #region Events
    async void KeyButton_Clicked(object? sender, System.EventArgs e)
    {
        if (GameStatus is GameStatus.Stopped or GameStatus.Paused) return;

        if (sender is KeyView key)
            await MoveToIndex(key);
    }

    async void Button_Clicked(object? sender, System.EventArgs e)
    {
        if (GameStatus is GameStatus.Stopped or GameStatus.Paused) return;

        if (sender is Button button)
        {
            switch (button.Text)
            {
                case "TWIST": await ShuffleKeys(); break;
                case "LAST": await EnterLastEnteredKeysAsync(); break;
                case "ENTER": await CheckEnteredWordAsync(); break;
                case "CLEAR": await ClearKeysAsync(); break;
                case "DELETE": await DeleteEnteredKeyAsync(); break;
            }
        }
    }

    private static void OnTimerPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        TextTwistView view = (TextTwistView)bindable;
        if(newValue is TimeSpan newTimer)
        {
            view.timerView.Text = $"TIME: {newTimer.Minutes:00}:{newTimer.Seconds:00}";
        }
    }

    private void GameTimer_Tick(object? sender, EventArgs e)
    {
        Timer = Timer.Subtract(_timer.Interval);
        if(Timer.TotalSeconds <= 0)
        {
            SetGameEnded();
            //StopTimer();
            PauseTimer();
        }
    }
    #endregion

    #region Public Methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="words">should be 6 letter word or up</param>
    public async Task SetWordsAsync(List<string>? words)
    {
        Round++;
        words ??= new List<string>()
        {
            "EON", "MEN", "MET", "MOM", "MOT", "NET", "NOT", "ONE", "TEN", "TOE",
            "TON", "MEMO", "MOTE", "NOTE", "OMEN", "TOME", "TONE", "MOMENT"
        };

        string word = (words.MaxBy(w => w.Length) ?? string.Empty).ToUpper();

        BindableLayout.SetItemsSource(enteredKeyList, word.ToCharArray());
        for (int index = 0; index < word.Length; index++)
        {
            KeyView newKey = new() { Text = "" + word[index] };
            newKey.Clicked += KeyButton_Clicked;
            inputContainerView.Add(newKey, index, 2);
        }

        _keys = inputContainerView.OfType<KeyView>().ToList();

        wordList.ItemsSource = words.OrderBy(word => word.Length).ThenBy(word => word);
        _wordsView = wordList.OfType<WordTilesView>().ToList();
        await ShuffleKeys();
    }

    public void StartTimer()
    {
        _timer.Start();
        GameStatus = GameStatus.Playing;
    }

    public void PauseTimer()
    {
        _timer.Stop();
        GameStatus = GameStatus.Paused;
    }

    public void StopTimer()
    {
        Timer = TimeSpan.FromMinutes(2);
        _timer.Stop();
        GameStatus = GameStatus.Stopped;
    }

    public async Task ResetGameAsync()
    {
        Round = 0;
        GameStatus = GameStatus.Playing;
        Timer = TimeSpan.FromMinutes(2);
        await ClearKeysAsync();
    }

    public async Task ClearKeysAsync()
    {
        List<Task> tasks = new();
        foreach (KeyView key in _keys)
            tasks.Add(key.ResetAsync());
        
        await Task.WhenAll(tasks);
    }

    public async Task ShuffleKeys()
    {
        System.Diagnostics.Debug.WriteLine("===SWAPING===");
        if (!_isShuffling)
        {
            _isShuffling = true;
            List<Task> tasks = new();
            List<KeyView> swappedViews = new List<KeyView>();

            var availableKeys = _keys.Where(key => !key.IsEntered()).ToList();
            int availableKeysCount = availableKeys.Count;
            if (availableKeysCount > 1)
            {
                int maxShuffleCount = Random.Shared.Next(1, availableKeysCount);

                for (int shuffleCount = 0; shuffleCount < maxShuffleCount; shuffleCount++)
                {
                SHUFFLE:
                    int index = Random.Shared.Next(0, availableKeysCount);
                    int toIndex = Random.Shared.Next(0, availableKeysCount);
                    if (index == toIndex)
                        goto SHUFFLE;
                    KeyView fromKey = availableKeys[index];
                    KeyView toKey = availableKeys[toIndex];
                    //await fromKey.SwapAsync(toKey);
                    fromKey.SwapPending(toKey);
                    tasks.Add(fromKey.SwapAsync(toKey));
                }
            }

            if (tasks.Any())
            {
                await Task.WhenAll(tasks);
            }

            foreach(var key in _keys)
            {
                key.ClearPending();
            }

            _isShuffling = false;
        }
    }

    public async Task CheckEnteredWordAsync()
    {
        if (_isChecking) return;

        _isChecking = true;
        if (GetEnteredWordView() is WordTilesView wordView)
        {
            _lastEnteredKeys = GetEnteredKeys().ToList();
            if (!wordView.IsCorrect)
            {
                wordView.IsCorrect = wordView.IsShow = true;
                Score += wordView.Word.Length * 500;
                await ClearKeysAsync();
                return;
            }
            // notif for already entered word
            //return;
        }

        await Parallel.ForEachAsync(
            source: GetEnteredKeys(),
            body: async (key, Token) => await key.ShakeAsync());
        _isChecking = false;
    }

    public async Task DeleteEnteredKeyAsync()
    {
        if(GetEnteredKeys()?.LastOrDefault() is KeyView key)
            await key.ResetAsync();
    }

    public async Task EnterLastEnteredKeysAsync()
    {
        if (_lastEnteredKeys == null || !_lastEnteredKeys.Any()) return;

        List<Task> tasks = new ();
        int count = 0;
        //await ClearAsync();
        foreach (KeyView key in _lastEnteredKeys)
            tasks.Add(MoveToIndex(key, count++, true));
        
        await Task.WhenAll(tasks);
    }

    public void ShowAllWords()
    {
        _wordsView.ForEach((index, word) => word.IsShow = true);
    }

    #endregion

    private void SetGameEnded()
    {
        if (Timer.TotalSeconds <= 0 &&
            _wordsView.LastOrDefault() is WordTilesView word)
            GameStatus = word.IsCorrect ? GameStatus.Winner: GameStatus.GameOver;
        if (GameStatus is GameStatus.GameOver or GameStatus.Winner)
            ShowAllWords();
    }


    #region Utilities
    WordTilesView? GetEnteredWordView()
    {
        string enteredWord = string.Join("", GetEnteredKeys().Select(key => key.Text));
        return _wordsView.FirstOrDefault(word => word.Word == enteredWord);
    }

    IEnumerable<KeyView> GetEnteredKeys() => _keys.Where(key => key.IsEntered()).OrderBy(key => key.ClassId);

    async Task MoveToIndex(KeyView button, int? index = null, bool isForce = false)
    {
        if (!isForce && button.IsEntered())
        {
            await button.ResetAsync();
        }
        // if key
        else
        {
            int enteredKeysCount = index ?? _keys.Count(key => key.IsEntered());
            View moveToView = (enteredKeyList[enteredKeysCount] as View)!;
            button.ClassId = "" + (enteredKeysCount);
            double offset = ((button.HeightRequest - moveToView.HeightRequest) / 2);
            double nextButtonX = moveToView.X - button.X - offset;
            double nextButtonY = enteredKeyList.Y - button.Y - offset;
            await button.MoveKeyAsync(nextButtonX, nextButtonY);
        }
    }
    #endregion
}
