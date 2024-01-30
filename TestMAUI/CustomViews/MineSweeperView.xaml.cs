namespace TestMAUI.CustomViews;

public partial class MineSweeperView : ContentView
{
    public static readonly BindableProperty RowsProperty = BindableProperty.Create(propertyName: nameof(Rows), returnType: typeof(int), declaringType: typeof(MineSweeperView), defaultValue: 10);
    public static readonly BindableProperty ColumnsProperty = BindableProperty.Create(propertyName: nameof(Columns), returnType: typeof(int), declaringType: typeof(MineSweeperView), defaultValue: 10);
    public static readonly BindableProperty NumberOfMineProperty = BindableProperty.Create(propertyName: nameof(NumberOfMine), returnType: typeof(int), declaringType: typeof(MineSweeperView), defaultValue: 10);
    public int NumberOfMine
    {
        get => (int)GetValue(NumberOfMineProperty);
        set => SetValue(NumberOfMineProperty, value);
    }
    public int Rows
    {
        get => (int)GetValue(RowsProperty);
        set => SetValue(RowsProperty, value);
    }
    public int Columns
    {
        get => (int)GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    public MineSweeperView()
	{
		InitializeComponent();
        Loaded += MineSweeperView_Loaded;
	}

    private void MineSweeperView_Loaded(object sender, EventArgs e)
    {

    }
}
