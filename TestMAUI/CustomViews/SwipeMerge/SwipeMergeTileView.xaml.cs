#nullable enable

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TestMAUI.CustomViews.SwipeMerge;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public partial class SwipeMergeTileView : ContentView
{
	public static readonly BindableProperty ValueProperty = BindableProperty.Create(propertyName: nameof(Value), returnType: typeof(int), declaringType: typeof(SwipeMergeTileView), defaultValue: 0,  propertyChanged: (bind,old,@new) => OnPropertyChanged(bind,nameof(Value),old,@new));
    public static readonly BindableProperty RowProperty = BindableProperty.Create(propertyName: nameof(Row), returnType: typeof(int), declaringType: typeof(SwipeMergeTileView), defaultValue: 0, propertyChanged: (bind, old, @new) => OnPropertyChanged(bind, nameof(Row), old, @new));
    public static readonly BindableProperty ColumnProperty = BindableProperty.Create(propertyName: nameof(Column), returnType: typeof(int), declaringType: typeof(SwipeMergeTileView), defaultValue: 0, propertyChanged: (bind, old, @new) => OnPropertyChanged(bind, nameof(Column), old, @new));

    public static readonly BindableProperty LeftTileProperty = BindableProperty.Create(propertyName: nameof(LeftTile), returnType: typeof(SwipeMergeTileView), declaringType: typeof(SwipeMergeTileView), defaultValue: null);
    public static readonly BindableProperty RightTileProperty = BindableProperty.Create(propertyName: nameof(RightTile), returnType: typeof(SwipeMergeTileView), declaringType: typeof(SwipeMergeTileView), defaultValue: null);
    public static readonly BindableProperty UpTileProperty = BindableProperty.Create(propertyName: nameof(UpTile), returnType: typeof(SwipeMergeTileView), declaringType: typeof(SwipeMergeTileView), defaultValue: null);
    public static readonly BindableProperty DownTileProperty = BindableProperty.Create(propertyName: nameof(DownTile), returnType: typeof(SwipeMergeTileView), declaringType: typeof(SwipeMergeTileView), defaultValue: null);

    public int Column
    {
        get => (int)GetValue(ColumnProperty);
        set => SetValue(ColumnProperty, value);
    }
    public int Value
	{
		get => (int)GetValue(ValueProperty);
		set => SetValue(ValueProperty, value);
    }
    public int Row
    {
        get => (int)GetValue(RowProperty);
        set => SetValue(RowProperty, value);
    }

    public SwipeMergeTileView LeftTile
    {
        get => (SwipeMergeTileView)GetValue(LeftTileProperty);
        set => SetValue(LeftTileProperty, value);
    }
    public SwipeMergeTileView RightTile
    {
        get => (SwipeMergeTileView)GetValue(RightTileProperty);
        set => SetValue(RightTileProperty, value);
    }
    public SwipeMergeTileView UpTile
    {
        get => (SwipeMergeTileView)GetValue(UpTileProperty);
        set => SetValue(UpTileProperty, value);
    }
    public SwipeMergeTileView DownTile
    {
        get => (SwipeMergeTileView)GetValue(DownTileProperty);
        set => SetValue(DownTileProperty, value);
    }

    private string DebuggerDisplay { get { return $"Value: {Value}, ValueText: {ValueText}, Row: {Row}, Column: {Column}"; } }
    public string ValueText { get { return Value == 0 ? "" : $"{Value}"; } }

    int _pendingValue;
    double? _height;
    double? _width;
    readonly uint _moveAnimSpeed = 50;
    readonly uint _scaleAnimSpeed = 50;
    readonly Easing _animEasing = Easing.BounceIn;
    TaskCompletionSource? _moveAnimation;

    public SwipeMergeTileView()
	{
		InitializeComponent();
        Grid.SetRow(this, Row);
        Grid.SetColumn(this, Column);
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        switch(propertyName)
        {
            case nameof(Height):
                _height ??= Height;
                break;
            case nameof(HeightRequest):
                _height ??= HeightRequest;
                break;
            case nameof(Width):
                _width ??= Width;
                break;
            case nameof(WidthRequest):
                _width ??= WidthRequest;
                break;
        }
    }

    public async void Clear(bool isForce = false)
    {
        _pendingValue = 0;

        if(!isForce)
            await Task.Delay((int)(_moveAnimSpeed + _scaleAnimSpeed));

        Value = 0;
    }

    public bool IsEmpty()
    {
        //return Value == 0;
        return _pendingValue == 0;
    }

    public bool IsSameValue(SwipeMergeTileView fromTile)
    {
        return _pendingValue == fromTile?._pendingValue;
    }

    private void ApplyPendingMerge()
    {
        Value = _pendingValue;
        TranslationX = 0;
        TranslationY = 0;
    }

    #region Merge To
    SwipeMergeTileView? _pendingToTile;
    bool _isMerge = false;
    public void AddMergeQueueTo(SwipeMergeTileView toTile)
    {
        _pendingToTile?.ApplyPendingMerge();

        _pendingToTile = toTile;
        if (_pendingToTile.IsEmpty() || _pendingToTile.IsSameValue(this))
        {
            _isMerge = _pendingToTile.IsSameValue(this);
            _pendingToTile._pendingValue += _pendingValue;
            _pendingValue = 0;
            //fromTile.Clear();
        }
    }

    public async Task AnimatePendingMergeToAsync()
    {
        _moveAnimation = new TaskCompletionSource();
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (_pendingToTile != null)
            {
                await this.TranslateTo(
                    x: (_pendingToTile.Column - Column) * _height.Value,
                    y: (_pendingToTile.Row - Row) * _width.Value,
                    length: _moveAnimSpeed,
                    easing: _animEasing);
                if (_isMerge)
                {
                    Scale = .5;
                    ZIndex = this.ZIndex + 1;
                    await this.ScaleTo(
                        scale: 1,
                        length: _scaleAnimSpeed,
                        easing: _animEasing);
                }

                _pendingToTile.ApplyPendingMerge();

                ApplyPendingMerge();
                _pendingToTile = null;
            }
            _moveAnimation?.TrySetResult();
            _moveAnimation = null;            
        });
        await (_moveAnimation?.Task ?? Task.CompletedTask);
    }

    public async Task MergeToAsync(SwipeMergeTileView toTile)
    {
        AddMergeQueueTo(toTile);
        await AnimatePendingMergeToAsync();
    }
    #endregion

    #region Merge Using Direction

    public async Task MergeAsync(SwipeDirection direction)
    {
        if(!IsEmpty() && FindNextTile(this, direction) is SwipeMergeTileView nextTile)
        {
            await MergeToAsync(nextTile);
        }
    }
    
    private SwipeMergeTileView? FindNextTile(SwipeMergeTileView? currentTile, SwipeDirection direction)
    {
        //currentTile ??= this;
        if (currentTile == null) return null;

        SwipeMergeTileView? nextTile = direction switch
        {
            SwipeDirection.Right => currentTile.RightTile,
            SwipeDirection.Left => currentTile.LeftTile,
            SwipeDirection.Up => currentTile.UpTile,
            _ or SwipeDirection.Down => currentTile.DownTile,
        };
        if (currentTile != this)
        {
            if (nextTile == null || currentTile.IsSameValue(this) ||
                (currentTile.IsEmpty() && !nextTile.IsSameValue(this) && !nextTile.IsEmpty()))
                return currentTile;

            if (currentTile.IsEmpty() && nextTile.IsSameValue(this))
                return nextTile;
        }
        if (nextTile?.IsSameValue(this) == false && nextTile?.IsEmpty() == false)
            return null;
        return FindNextTile(nextTile,direction);
    }
    #endregion

    private static void OnPropertyChanged(BindableObject bindable, string propertyName, object oldValue, object newValue)
    {
        if (bindable is SwipeMergeTileView view )
        {
            switch(propertyName)
            {
                case nameof(Value) when newValue is int intNewValue:
                    //view.valueHolder.Text = intNewValue == 0 ? "" : $"{intNewValue}";
                    if(oldValue is int intOldValue &&
                        intOldValue == 0)
                    {
                        view._pendingValue = intNewValue;
                    }
                    view.valueHolder.Text = view.ValueText;
                    view.BackgroundColor = intNewValue == 0
                                                ? Colors.Transparent
                                                : Color.FromArgb("#C8C8C8");
                    
                    break;
                case nameof(Row) when newValue is int newRow:
                    Grid.SetRow(view, newRow);
                    break;
                case nameof(Column) when newValue is int newColumn:
                    Grid.SetColumn(view, newColumn);
                    break;
            }
        }
    }

    public void Log()
    {
        Debug.WriteLine(DebuggerDisplay);
    }


}
