using System.ComponentModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TestMAUI.Models
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial class SwipeMergeTile : BaseTile
    {
        [ObservableProperty] private int value;

        public string ValueText { get { return Value == 0 ? "" : $"{Value}"; } }

        public SwipeMergeTile(int row, int column)
            : base(row, column)
        {
        }

        public void Clear()
        {
            Value = 0;
        }

        public bool IsEmpty()
        {
            return Value == 0;
        }

        public void Add(SwipeMergeTile tile)
        {
            Value += tile.Value;
        }

        //public static SwipeMergeTile operator +(SwipeMergeTile left, SwipeMergeTile right)
        //=> new SwipeMergeTile(left.Row, left.Column) { Value = left.Value + right.Value };        

        //public static SwipeMergeTile operator -(SwipeMergeTile left, SwipeMergeTile right)
        //=> new SwipeMergeTile(left.Row, left.Column) { Value = left.Value - right.Value };

        public static SwipeMergeTile operator +(SwipeMergeTile left, SwipeMergeTile right)
        {
            left.Value += right.Value;
            return left;
        }

        public static SwipeMergeTile operator -(SwipeMergeTile left, SwipeMergeTile right)
        {
            left.Value -= right.Value;
            return left;
        }

        public static bool operator ==(SwipeMergeTile left, SwipeMergeTile right)
        {            
            return left.Value == right.Value;
        }

        public static bool operator !=(SwipeMergeTile left, SwipeMergeTile right)
        {
            return left.Value != right.Value;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            switch(e.PropertyName)
            {
                case nameof(Value):
                    OnPropertyChanged(nameof(ValueText));
                    break;
            }
        }

        private string DebuggerDisplay { get { return $"Value: {Value}, ValueText: {ValueText}"; } }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }
            if(obj is SwipeMergeTile tile)
            {
                return Value == tile.Value;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

