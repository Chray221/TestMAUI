using System.Collections;
using System.Collections.Specialized;
using Microsoft.Maui.Layouts;

namespace TestMAUI.CustomViews;

public partial class MyFlexLayout : Grid
{
    #region BindableProperties
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(propertyName: nameof(ItemsSource), returnType: typeof(IEnumerable), declaringType: typeof(MyFlexLayout), defaultValue: null, propertyChanged: OnItemsSourceChanged);
    public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(propertyName: nameof(ItemTemplate), returnType: typeof(DataTemplate), declaringType: typeof(MyFlexLayout), defaultValue: null);
    public static readonly BindableProperty DirectionProperty = BindableProperty.Create(propertyName: nameof(Direction), returnType: typeof(FlexDirection), declaringType: typeof(MyFlexLayout), defaultValue: FlexDirection.Row);
    public static readonly BindableProperty MaxRowProperty = BindableProperty.Create(propertyName: nameof(MaxRow), returnType: typeof(int), declaringType: typeof(MyFlexLayout), defaultValue: null, propertyChanged: (bindable, oldValue,newValue) => OnGridSizeChanged(FlexDirection.Row,bindable, oldValue, newValue));
    public static readonly BindableProperty MaxColumnProperty = BindableProperty.Create(propertyName: nameof(MaxColumn), returnType: typeof(int), declaringType: typeof(MyFlexLayout), defaultValue: null,  propertyChanged: (bindable, oldValue, newValue) => OnGridSizeChanged(FlexDirection.Row, bindable, oldValue, newValue));
    public static readonly BindableProperty WrapProperty = BindableProperty.Create(propertyName: nameof(Wrap), returnType: typeof(FlexWrap), declaringType: typeof(MyFlexLayout), defaultValue: FlexWrap.Wrap);
    #endregion

    #region Properties
    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }
    public DataTemplate ItemTemplate
    {
        get => (DataTemplate)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }
    public FlexDirection Direction
    {
        get => (FlexDirection)GetValue(DirectionProperty);
        set => SetValue(DirectionProperty, value);
    }
    public int MaxRow
    {
        get => (int)GetValue(MaxRowProperty);
        set => SetValue(MaxRowProperty, value);
    }
    public int MaxColumn
    {
        get => (int)GetValue(MaxColumnProperty);
        set => SetValue(MaxColumnProperty, value);
    }
    public FlexWrap Wrap
    {
        get => (FlexWrap)GetValue(WrapProperty);
        set => SetValue(WrapProperty, value);
    }
    #endregion

    #region _fields
    int _maxRow=0;
    int _maxColumn=0;
    #endregion

    public MyFlexLayout()
	{
		InitializeComponent();
        Clear();
    }

    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
		MyFlexLayout view = (MyFlexLayout)bindable;
        if(newValue is IEnumerable items)
            view.InsertViews(items);
        if (newValue is INotifyCollectionChanged collectionChanged)
		{
            collectionChanged.CollectionChanged += view.ItemsSource_CollectionChanged;
		}
    }

    private static void OnGridSizeChanged(FlexDirection direction, BindableObject bindable, object oldValue, object newValue)
    {
        MyFlexLayout view = (MyFlexLayout)bindable;
        if (newValue is int newIntValue && oldValue is int oldIntValue)
        {
            int nextValue = newIntValue - oldIntValue;
            for (int index = 0; index < +nextValue; index++)
            {
                switch (direction)
                {
                    case FlexDirection.Column:  case FlexDirection.ColumnReverse:
                        view._maxColumn = newIntValue;
                        if (nextValue > 0)
                            view.AddColumn();
                        else
                            view.ColumnDefinitions.RemoveAt(view.ColumnDefinitions.Count - 1);
                        break;
                    case FlexDirection.Row: case FlexDirection.RowReverse:
                        view._maxRow = newIntValue;
                        if (nextValue > 0)
                            view.AddRow();
                        else
                            view.RowDefinitions.RemoveAt(view.RowDefinitions.Count - 1);
                        break;
                }
            }
        }
    }

    void AddRow()
    {
        switch (Wrap)
        {
            case FlexWrap.NoWrap:
                AddRowDefinition(new RowDefinition(GridLength.Star));
                break;
            case FlexWrap.Wrap:
                AddRowDefinition(new RowDefinition(GridLength.Auto));
                break;
            case FlexWrap.Reverse:
                AddRowDefinition(new RowDefinition(GridLength.Auto));
                break;
        }
    }

    void AddColumn()
    {
        switch (Wrap)
        {
            case FlexWrap.NoWrap:
                AddColumnDefinition(new ColumnDefinition(GridLength.Star));
                break;
            case FlexWrap.Wrap:
                AddColumnDefinition(new ColumnDefinition(GridLength.Auto));
                break;
            case FlexWrap.Reverse:
                AddColumnDefinition(new ColumnDefinition(GridLength.Auto));
                break;
        }
    }

    private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                InsertViews(e.NewItems,e.NewStartingIndex);
                break;
            case NotifyCollectionChangedAction.Remove:
                break;
            case NotifyCollectionChangedAction.Replace:
                break;
            case NotifyCollectionChangedAction.Move:
                break;
            case NotifyCollectionChangedAction.Reset:
                break;
        }
    }

    private int GetRow(int index) => index / (_maxColumn);

    private int GetColumn(int index) => index % (_maxColumn - 1);

    private int GetRowDirection(int index)
    {
        return Direction switch
        {
            FlexDirection.Column or FlexDirection.ColumnReverse => index % (_maxRow),
            _ => index / (_maxColumn),
        };
    }

    private int GetColumnDirection(int index)
    {
        return Direction switch
        {
            FlexDirection.Column or FlexDirection.ColumnReverse => index / (_maxRow),
            _ => index % (_maxColumn - 1),
        };
    }

    private void InsertViews(IEnumerable items, int startingIndex = 0)
    {
        if (ItemTemplate == null) return;

        int row = 0;
        int column = 0;
        foreach (var item in items)
        {
            row = GetRowDirection(startingIndex);
            column = GetColumnDirection(startingIndex);

            // add new column;
            if(column >= _maxColumn) AddColumn();
            
            if (ItemTemplate.CreateContent() is View newView)
            {
                newView.BindingContext = item;
                Add(newView);
                SetRow((IView)newView, row);
                SetColumn((IView)newView, column);
            }
            startingIndex++;
        }
    }
}
