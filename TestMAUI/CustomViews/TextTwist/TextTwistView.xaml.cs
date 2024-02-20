#nullable enable

using Microsoft.Maui.Controls;
using TestMAUI.Helpers;

namespace TestMAUI.CustomViews.TextTwist;

public partial class TextTwistView : StackLayout
{
    #region BindableProperties
    public static readonly BindableProperty ScoreProperty = BindableProperty.Create(propertyName: nameof(Score), returnType: typeof(int), declaringType: typeof(TextTwistView), defaultValue: 0);
    public static readonly BindableProperty RoundProperty = BindableProperty.Create(propertyName: nameof(Round), returnType: typeof(int), declaringType: typeof(TextTwistView), defaultValue: 1);
    public static readonly BindableProperty TimerProperty = BindableProperty.Create(propertyName: nameof(Timer), returnType: typeof(TimeSpan), declaringType: typeof(TextTwistView), defaultValue: TimeSpan.FromMinutes(2), propertyChanged: OnTimerPropertyChanged);
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

    #endregion

    #region _fields
    private readonly uint _animationSpeed = 120;

    private string _word;
    private List<string> _words;
    private IEnumerable<Button> _keys;
    private IEnumerable<Button> _lastEnteredKeys;
    private IEnumerable<WordTilesView> _wordsView;
    private bool _isPaused = false;
    IDispatcherTimer _timer;
    #endregion

    public TextTwistView()
	{
		InitializeComponent();
		_= SetWords(null);
    }

	/// <summary>
	/// 
	/// </summary>
	/// <param name="words">should be 6 letter word or up</param>
	public async Task SetWords(List<string> strings)
    {
        _words = strings ?? new List<string>()
        {
            "EON",
            "MEN",
            "MET",
            "MOM",
            "MOT",
            "NET",
            "NOT",
            "ONE",
            "TEN",
            "TOE",
            "TON",
            "MEMO",
            "MOTE",
            "NOTE",
            "OMEN",
            "TOME",
            "TONE",
            "MOMENT"
        };

        _word = _words.MaxBy(w => w.Length) ?? string.Empty;

        BindableLayout.SetItemsSource(enteredKeyList, _word.ToCharArray());

        //BindableLayout.SetItemsSource(keyList, _word.ToCharArray());
        if (Resources.TryGetValue("KeyButton", out Style keyStyle))
        {
            for (int index = 0; index < _word.Length; index++)
            {
                Button newKey = new Button() { Style = keyStyle, Text = ""+_word[index] };
                newKey.Clicked += KeyButton_Clicked;
                inputContainerView.Add(newKey, index, 2);
            }
        }
        _keys = inputContainerView.OfType<Button>();

        wordList.ItemsSource = _words.OrderBy( word => word.Length).ThenBy(word => word);
        _wordsView = wordList.OfType<WordTilesView>();
        await ShuffleKeys();
        StartTimer();
    }

    public void StartTimer()
    {
        if(_timer is null)
        {
            _timer ??= Application.Current?.Dispatcher.CreateTimer() ?? Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += _timer_Tick;
        }

        if(!_isPaused)
            StopTimer();
        _timer.Start();
        _isPaused = true;
    }

    public void PauseTimer()
    {
        _isPaused = true;
        _timer.Stop();
    }

    public void StopTimer()
    {
        Timer = TimeSpan.FromMinutes(2);
        _timer.Stop();
    }

    private void _timer_Tick(object? sender, EventArgs e)
    {
        Timer = Timer.Subtract(_timer.Interval);
    }

    #region Events
    async void KeyButton_Clicked(object? sender, System.EventArgs e)
    {
        if (sender is Button key)
        {
            await MoveToIndex(key);
        }
    }

    async void Button_Clicked(object? sender, System.EventArgs e)
    {
        if (sender is Button button)
        {
            switch (button.Text)
            {
                case "TWIST": await ShuffleKeys(); break;
                case "LAST": await EnterLastEnteredKeysAsync(); break;
                case "ENTER": await CheckEnteredWordAsync(); break;
                case "CLEAR": await ClearAsync(); break;
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
    #endregion

    #region Public Methods
    public async Task ClearAsync()
    {
        List<Task> tasks = new();
        foreach (Button key in _keys)
        {
            tasks.Add(ResetKeyAsync(key));
        }
        await Task.WhenAll(tasks);
    }

    private bool isShuffling = false;
    public async Task ShuffleKeys()
    {
        if (!isShuffling)
        {
            isShuffling = true;
            List<Task> tasks = new();
            var availableKeys = _keys.Where(key => !IsKeyEntered(key));
            int availableKeysCount = availableKeys.Count();
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
                    await SwapKey(availableKeys.ElementAt(index), availableKeys.ElementAt(toIndex));
                    //tasks.Add(SwapKey(availableKeys.ElementAt(index), availableKeys.ElementAt(toIndex)));
                }
            }

            if (tasks.Any())
            {
                await Task.WhenAll(tasks);
            }
            isShuffling = false;
        }
    }

    public async Task CheckEnteredWordAsync()
    {
        if(GetEnteredWordView() is WordTilesView wordView)
        {
            _lastEnteredKeys = GetEnteredKeys().ToList();
            if (!wordView.IsCorrect)
            {
                wordView.IsCorrect = wordView.IsShow = true;
                Score += wordView.Word.Length * 500;
                await ClearAsync();
                return;
            }
            // notif for already entered word
            //return;
        }
    }

    public async Task DeleteEnteredKeyAsync()
    {
        if(GetEnteredKeys()?.LastOrDefault() is Button key)
            await ResetKeyAsync(key);
    }

    public async Task EnterLastEnteredKeysAsync()
    {
        if (_lastEnteredKeys == null || !_lastEnteredKeys.Any()) return;

        List<Task> tasks = new List<Task>();
        int count = 0;
        foreach(Button key in _lastEnteredKeys)
        {
            tasks.Add(MoveToIndex(key, count++));
        }
        await Task.WhenAll(tasks);
    }

    #endregion

    #region Utilities
    WordTilesView? GetEnteredWordView()
    {
        string enteredWord = string.Join("", GetEnteredKeys().Select(key => key.Text));
        return _wordsView.FirstOrDefault(word => word.Word == enteredWord);
    }

    IEnumerable<Button> GetEnteredKeys() =>
        _keys.Where(IsKeyEntered).OrderBy(key => key.ClassId);    

    bool IsKeyEntered(Button key)
    {
        //return (key.ClassId ?? string.Empty).StartsWith("EK");
        return !string.IsNullOrEmpty(key.ClassId);
    }

    async Task MoveToIndex(Button button, int? index = null)
    {
        // if entered
        if (IsKeyEntered(button))
        {
            //button.ClassId = button.ClassId.Remove(0, 1);
            button.ClassId = string.Empty;
            await MoveKeyAsync(button);
        }
        // if key
        else
        {
            int enteredKeysCount = index ?? _keys.Count(IsKeyEntered);
            View moveToView = (enteredKeyList[enteredKeysCount] as View)!;
            //button.ClassId = "EK" + (enteredKeysCount);
            button.ClassId = "" + (enteredKeysCount);
            double offset = ((button.HeightRequest - moveToView.HeightRequest) / 2);
            double nextButtonX = moveToView.X - button.X - offset;
            //double nextButtonY = enteredKeyList.Y - keyList.Y - offset;
            double nextButtonY = enteredKeyList.Y - button.Y - offset;
            await MoveKeyAsync(button, nextButtonX, nextButtonY);
        }
    }

    async Task MoveKeyAsync(Button key, double newX = 0, double newY = 0)
    {
        if (key.TranslationX != newX || key.TranslationY != newY)
        {
            await Task.WhenAll(
                key.TranslateTo(
                    x: newX,
                    y: newY,
                    length: _animationSpeed),
                key.RotateTo(720, _animationSpeed)
                );
            key.TranslationX = newX;
            key.TranslationY = newY;
            key.Rotation = 0;
        }
    }

    async Task SwapKey(Button fromKey, Button toKey)
    {
        if (!IsKeyEntered(fromKey) || !IsKeyEntered(toKey))
        {
            string fromKeyText = fromKey.Text;

            double newFromKeyX = toKey.X - fromKey.X;
            double newFromKeyY = toKey.Y - fromKey.Y;
            double newToKeyX = fromKey.X - toKey.X;
            double newToKeyY = fromKey.Y - toKey.Y;
            await Task.WhenAll(
                MoveKeyAsync(fromKey, newFromKeyX, newFromKeyY),
                MoveKeyAsync(toKey, newToKeyX, newToKeyY));

            fromKey.Text = toKey.Text;
            toKey.Text = fromKeyText;

            fromKey.TranslationX = 0;
            fromKey.TranslationY = 0;
            toKey.TranslationX = 0;
            toKey.TranslationY = 0;
        }
    }

    async Task ResetKeyAsync(Button key)
    {
        if (IsKeyEntered(key))
        {
            key.ClassId = key.ClassId?.Remove(0, 1);
            await MoveKeyAsync(key);
        }
    }
    #endregion
}
