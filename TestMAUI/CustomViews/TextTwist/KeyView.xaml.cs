#nullable enable

namespace TestMAUI.CustomViews.TextTwist;

public partial class KeyView : Button
{
    private readonly uint _animationSpeed = 120;
    private readonly uint _shakeOffset = 4;
    private KeyView? _pendingToKey = null;

    public KeyView()
	{
		InitializeComponent();
	}

    public async Task MoveKeyAsync(double newX = 0, double newY = 0)
    {
        if (TranslationX != newX || TranslationY != newY)
        {
            await Task.WhenAll(
                this.TranslateTo(
                    x: newX,
                    y: newY,
                    length: _animationSpeed),
                this.RotateTo(720, _animationSpeed)
                );
            TranslationX = newX;
            TranslationY = newY;
            Rotation = 0;
        }
    }

    public async Task ShakeAsync()
    {
        double oldTranslationX = TranslationX;
        uint shakeAnimationSpeed = _animationSpeed / 5;
        await this.TranslateTo(
            x: oldTranslationX - _shakeOffset, y: TranslationY, length: shakeAnimationSpeed);
        await this.TranslateTo(
            x: oldTranslationX + _shakeOffset, y: TranslationY, length: shakeAnimationSpeed);
        await this.TranslateTo(
            x: oldTranslationX - _shakeOffset, y: TranslationY, length: shakeAnimationSpeed);
        await this.TranslateTo(
            x: oldTranslationX + _shakeOffset, y: TranslationY, length: shakeAnimationSpeed);
        await this.TranslateTo(
            x: oldTranslationX , y: TranslationY, length: shakeAnimationSpeed);
        TranslationX = oldTranslationX;
        Rotation = 0;
    }

    public bool IsEntered()
    {
        //return (key.ClassId ?? string.Empty).StartsWith("EK");
        return !string.IsNullOrEmpty(ClassId);
    }

    public async Task ResetAsync()
    {
        if (IsEntered())
        {
            //ClassId = ClassId?.Remove(0, 1);
            ClassId = null;
            await MoveKeyAsync();
        }
    }

    public KeyView? GetLastPending()
    {
        KeyView? pendingToKey = _pendingToKey;
        while (pendingToKey is not null &&
              pendingToKey != this &&
              pendingToKey._pendingToKey != this)
        {
            pendingToKey = pendingToKey._pendingToKey;
        }

        return pendingToKey;
    }

    public async Task SwapAsync(KeyView toKey)
    {
        toKey = _pendingToKey ?? toKey;
        if (!IsEntered() || !toKey.IsEntered())
        {
            await Task.WhenAll(
                MoveKeyAsync(toKey.X - X, toKey.Y - Y),
                toKey.MoveKeyAsync(X - toKey.X, Y - toKey.Y));

            (toKey.Text, Text) = (Text, toKey.Text);

            TranslationX = 0;
            TranslationY = 0;
            toKey.TranslationX = 0;
            toKey.TranslationY = 0;
        }
        ClearPending();
    }

    public void SwapPending(KeyView toKey)
    {
        if (toKey is null) return;

        int cases = 0;
#if DEBUG
        KeyView? lastPending = null;
        KeyView? lastToPending = null;
#endif
        // case 1 both not swaped
        if (_pendingToKey is null && toKey._pendingToKey is null)
        {
            _pendingToKey = toKey;
            toKey._pendingToKey = this;
            cases = 1;
        }
        // case 2 to is swapped
        else if (_pendingToKey is null &&
                 toKey.GetLastPending() is KeyView case2LastToPending)
        {
#if DEBUG
            lastToPending = case2LastToPending;
#endif

            _pendingToKey = toKey;
            case2LastToPending._pendingToKey = this;
            cases = 2;
        }
        // case 3 from is swapped
        else if (GetLastPending() is KeyView case3LastFromPending && toKey.GetLastPending() is null)
        {
#if DEBUG
            lastPending = case3LastFromPending;
#endif

            case3LastFromPending._pendingToKey = toKey;
            toKey._pendingToKey = this;
            cases = 3;
        }
        // case 4 both is swapped
        else if (GetLastPending() is KeyView case4LastFromPending &&
                 toKey.GetLastPending() is KeyView case4LastToPending)
        {
#if DEBUG
            lastPending = case4LastFromPending;
            lastToPending = case4LastToPending;
#endif

            case4LastFromPending._pendingToKey = toKey;
            case4LastToPending._pendingToKey = this;
            cases = 4;
        }

#if DEBUG
        System.Diagnostics.Debug.WriteLine($"FROM:({Text} LP:{lastPending?.Text}) TO:({toKey.Text} LP:{lastToPending?.Text}) CASE: {cases}");
#endif
        
    }

    public void ClearPending()
    {
        if (_pendingToKey is not null)
        {
            _pendingToKey._pendingToKey = null;
            _pendingToKey = null;
        }
    }
}
