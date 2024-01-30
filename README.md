# Maui.Toolbox

Maui.Toolbox is a collection of useful utilities used in various Xamarin.Forms projects.

## Features

  - Auto scale views based on device screen resolution.

## Installation
Install the nuget package to Maui project

add in MauiProgram.cs
```csharp
	builder
		.UseMauiApp<App>()
		.UseMauiToolkit("Sample")

```

### Android Project

```csharp
    protected override void OnCreate(Bundle savedInstanceState)
    {
        ...
        Xamarin.Forms.Forms.Init(this, savedInstanceState);
        Xamarin.Essentials.Platform.Init(this, savedInstanceState);
        Maui.Toolbox.Platform.Init([KEY], this, savedInstanceState);
        ...
    }
```

### iOS Project

```csharp
    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
        ...
        Xamarin.Forms.Forms.Init();
        Maui.Toolbox.Platform.Init([KEY]);
        ...
    }
```

## Scale
### Public Properties
| DataType | Property | Description |
| --- |--- | --- |
| float | ScreenWidth | The screen width available for the app to use. |
| float | ScreenHeight | The screen height available for the app to use. |
| float | AdjustedHeight | The new screen height after adjusting base on 16/9 ratio. |
| float | AdjustedWidth |  The new screen width after adjusting base on 16/9 ratio. |
| float | OrigScreenWidth | The original screen height before adjusting base on 16/9 ratio. |
| float | OrigScreenHeight | The original screen width before adjusting base on 16/9 ratio. |
| float | AppScale | The screen scale for iOS and screen density for Android. |
| float | StatusBarHeight | The height of the status bar. |
| float | TopInsets | The height of the status bar and notch height |
| float | BottomInsets | The height covered by the bezel, navigation bar. Useful for bottom aligned contents. |

Sample:
```csharp
using Maui.Toolbox;
...
    var screenWidth = Scaler.ScreenWidth;
...
```
### Extensions
| Return Type | Method 
| --- |--- |
| float | ScaleHeight | 
| float | ScaleWidth | 
| double | ScaleFont | 
| Thickness | ScaleThickness | 
| Thickness | ScaleThicknessWidth | 
| Thickness | ScaleThicknessHeight | 
| CornerRadius | ScaleCornerRadius | 
| CornerRadius | ScaleCornerRadiusWidth | 
| double | GetMinSize | 

Sample:
```csharp
using Maui.Toolbox;
...
    var width = 50.ScaleWidth();
    var height = 50.ScaleHeight(40); // 50 for Android, 40 for iOS
...
```
### MarkupExtensions
| Return Type | Method |
| --- |--- |
| int/double/single | ScaleHeight |
| int | ScaleHeightInt | 
| double | ScaleHeightDouble |
| float | ScaleHeightFloat |  
| int/double/single | ScaleWidth |
| int | ScaleWidthInt | 
| double | ScaleWidthDouble | 
| float | ScaleWidthFloat | 
| GridLength | ScaleGridHeight | 
| GridLength | ScaleGridWidth | 
| Thickness | ScaleThickness |
| Thickness | ScaleThicknessWidth | 
| Thickness | ScaleThicknessHeight |
| double | ScaleFontSize | 
| CornerRadius | ScaleCornerRadius |
| CornerRadius | ScaleCornerRadiusWidth |

Sample: Same value for both platforms.
```xml
<?xml version="1.0" encoding="UTF-8"?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="ToolboxDemo.ScalePage"
             xmlns:toolbox="clr-namespace:Maui.Toolbox;assembly=Maui.Toolbox"
             Title="Scale Test">

    <StackLayout Spacing="0" Orientation="Vertical">
        <BoxView HorizontalOptions="Start"
                 WidthRequest="{toolbox:ScaleWidth Value=320}"
                 HeightRequest="{toolbox:ScaleHeight Value=20}"
                 Margin="{toolbox:ScaleThickness Value='5,5,5,5'}">
        </BoxView>
    </StackLayout>
</ContentPage>
```
Sample: Different values for the two platforms.
```xml
<?xml version="1.0" encoding="UTF-8"?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="ToolboxDemo.ScalePage"
             xmlns:toolbox="clr-namespace:Maui.Toolbox;assembly=Maui.Toolbox"
             Title="Scale Test">

    <StackLayout Spacing="0" Orientation="Vertical">
        <BoxView HorizontalOptions="Start"
                 WidthRequest="{toolbox:ScaleWidth Value=320, Android=160}"
                 HeightRequest="{toolbox:ScaleHeight Value=20, Android=40}"
                 Margin="{toolbox:ScaleThickness Value='5,5,5,5', Android='10,10,10,10'}">
        </BoxView>
    </StackLayout>
</ContentPage>
```