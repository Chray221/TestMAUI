namespace TestMAUI.CustomViews.TextTwist;

public partial class WordTilesView : HorizontalStackLayout
{
    #region BindableProperty
    public static readonly BindableProperty WordProperty = BindableProperty.Create(propertyName: nameof(Word), returnType: typeof(string), declaringType: typeof(WordTilesView), defaultValue: null, propertyChanged: OnWordChanged);
    public static readonly BindableProperty IsShowProperty = BindableProperty.Create(propertyName: nameof(IsShow), returnType: typeof(bool), declaringType: typeof(WordTilesView), defaultValue: false);
    public static readonly BindableProperty IsCorrectProperty = BindableProperty.Create(propertyName: nameof(IsCorrect), returnType: typeof(bool), declaringType: typeof(WordTilesView), defaultValue: false);
    #endregion

    #region Property
    public string Word
    {
        get => (string)GetValue(WordProperty);
        set => SetValue(WordProperty, value);
    }
    public bool IsShow
    {
        get => (bool)GetValue(IsShowProperty);
        set => SetValue(IsShowProperty, value);
    }
    public bool IsCorrect
    {
        get => (bool)GetValue(IsCorrectProperty);
        set => SetValue(IsCorrectProperty, value);
    }
    #endregion

    public WordTilesView()
	{
		InitializeComponent();
    }

    private static void OnWordChanged(BindableObject bindable, object oldValue, object newValue)
    {
		WordTilesView view = (WordTilesView)bindable;
		if(newValue is string newWordValue)
		{
			if (view.Any()) view.Clear();
			BindableLayout.SetItemsSource(view, newWordValue.ToCharArray());
		}
    }
}
