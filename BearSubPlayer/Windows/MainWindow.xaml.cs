using BearSubPlayer.Core;
using BearSubPlayer.Core.Actions;
using BearSubPlayer.Core.DataTags;
using BearSubPlayer.Helpers;
using BearSubPlayer.Services;
using BearSubPlayer.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using TriggerLib;

namespace BearSubPlayer.Windows;

public partial class MainWindow : Window
{
    private readonly IServiceProvider _serviceProvider;
    private readonly DispatchCenter _dispatchCenter;
    private readonly MainStore _store;
    private readonly TriggerSource _triggerSource;
    private SettingWindow? _settingWindow;

    public MainWindow(IServiceProvider serviceProvider, DispatchCenter dispatchCenter, MainStore store)
    {
        InitializeComponent();
        DataContext = this;

        // Arrange MainWindow size 
        Width = SystemParameters.PrimaryScreenWidth * 2 / 3;
        Top = SystemParameters.PrimaryScreenHeight * 5 / 6;
        Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;

        _serviceProvider = serviceProvider;
        _dispatchCenter = dispatchCenter;
        _store = store;

        Store_Changed(null, _store.GetData());
        _store.Changed += (sender, e) => this.InvokeIfNeeded(() => Store_Changed(sender, e));

        // Init TriggerSource
        _triggerSource = new TriggerSource(3000,
            () => this.InvokeIfNeeded(() => MenuPanel.Visibility = Visibility.Hidden),
            pullImmed: false);
    }

    private void Store_Changed(object? sender, DataArgs e)
    {
        var count = e.GetDataTags().Length;

        // MainTag
        if (e.TryGetData(MainTag.LoadSuccessInfo, out var fileName))
        {
            var name = (string)fileName!;
            if (name.Length > 35)
                ResetMainPanelContents(true, string.Concat(name.AsSpan(0, 35), "... is loaded"));
            else
                ResetMainPanelContents(true, string.Concat(name, " is loaded"));

            ChangeMainPanelLockStatus(MainPanelLocalStatus.SubLoaded);
            if (count == 1) return; else count--;
        }

        if (e.TryGetData(MainTag.LoadFailureReasons, out var failureReasons))
        {
            ResetMainPanelContents(true, (string)failureReasons!);
            ChangeMainPanelLockStatus(MainPanelLocalStatus.Idling);

            if (count == 1) return; else count--;
        }

        if (e.TryGetData(MainTag.NeedFreezeInput, out var needFreeze))
        {
            if ((bool)needFreeze!)
                ChangeMainPanelLockStatus(MainPanelLocalStatus.Triggering);
            else
                ChangeMainPanelLockStatus(MainPanelLocalStatus.Playing);  // Trigger action finished and start playing

            if (count == 1) return; else count--;
        }

        if (e.TryGetData(MainTag.CurrentTime, out var currentTime))
        {
            var item = (TimeSpan)currentTime!;
            var totalTime = (TimeSpan)e.GetData(MainTag.TotalTime)!;
            TimeTB.Text = $"{item:hh\\:mm\\:ss} / {totalTime:hh\\:mm\\:ss}";
            TimeSlider.Value = item.TotalMilliseconds / totalTime.TotalMilliseconds * TimeSlider.Maximum;

            if (count == 1) return; else count--;
        }

        if (e.TryGetData(MainTag.SubContent, out var subContent))
        {
            SubLabel.Content = (string)subContent!;
            if (count == 1) return; else count--;
        }

        // SettingTag
        if (e.TryGetData(SettingTag.PanelOpacity, out var pnOpacity))
        {
            var item = (double)pnOpacity!;

            if (item <= 0.002)
                Background.Opacity = 0.002;  // Almost transparent
            else
                Background.Opacity = item;

            if (count == 1) return; else count--;
        }

        if (e.TryGetData(SettingTag.PanelColor, out var pnColor))
        {
            if ((MonoColor)pnColor! is MonoColor.White)
                ChangeMainPanelColor(Colors.Black, Colors.White);
            else
                ChangeMainPanelColor(Colors.White, Colors.Black);

            if (count == 1) return; else count--;
        }

        if (e.TryGetData(SettingTag.FontSize, out var fontSize))
        {
            SubLabel.FontSize = (int)fontSize!;
            if (count == 1) return; else count--;
        }

        if (e.TryGetData(SettingTag.FontColor, out var fontColor))
        {
            if ((MonoColor)fontColor! is MonoColor.White)
            {
                SubLabel.Foreground = Brushes.White;
                ((DropShadowEffect)SubLabel.Effect).Color = Colors.White;
            }
            else
            {
                SubLabel.Foreground = Brushes.Black;
                ((DropShadowEffect)SubLabel.Effect).Color = Colors.Black;
            }

            if (count == 1) return; else count--;
        }

        if (e.TryGetData(SettingTag.ShadowOpacity, out var shadowOpacity))
        {
            ((DropShadowEffect)SubLabel.Effect).Opacity = (double)shadowOpacity!;
            if (count == 1) return;
        }

        if (e.TryGetData(SettingTag.ShadowSoftness, out var shadowSoftness))
        {
            ((DropShadowEffect)SubLabel.Effect).BlurRadius = (int)shadowSoftness!;
        }
    }

    private void ChangeMainPanelColor(Color fontColor, Color bgColor)
    {
        var bgBrush = new SolidColorBrush(bgColor)
        {
            Opacity = Background.Opacity
        };
        Background = bgBrush;

        var fontBrush = new SolidColorBrush(fontColor);

        foreach (var child in PlayControlsPanel.Children)
        {
            if (child is TextBlock tb)
                tb.Foreground = fontBrush;
        }

        TimeTB.Foreground = fontBrush;
        SettingTB.Foreground = fontBrush;
    }

    private void SubLabel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            DefaultExt = ".srt",
            Filter = "Subtitle File (*.srt, *ass)|*.srt;*.ass"
        };

        var result = dialog.ShowDialog();
        if (result is null || result is false) return;

        _dispatchCenter.DispatchEvent(new ActionArgs(MainAction.LoadFile, dialog.FileName), newThread: true);
    }

    private void ResetMainPanelContents(bool resetSubLabel = true, string? subLabelContent = null)
    {
        if (resetSubLabel)
            SubLabel.Content = subLabelContent is null ?
                "Double click here to select a subtitle file" : subLabelContent;
        TimeSlider.Value = 0;
        TimeTB.Text = "00:00:00 / 00:00:00";
    }

    private void ChangeMainPanelLockStatus(MainPanelLocalStatus status)
    {
        switch (status)
        {
            case MainPanelLocalStatus.Idling:
                SubLabel.IsEnabled = true;
                PlayPanel.IsEnabled = false;
                break;
            case MainPanelLocalStatus.Triggering:
                SubLabel.IsEnabled = false;
                PlayPanel.IsEnabled = false;
                break;
            case MainPanelLocalStatus.SubLoaded:
                SubLabel.IsEnabled = true;
                PlayPanel.IsEnabled = true;
                break;
            case MainPanelLocalStatus.Playing:
                SubLabel.IsEnabled = false;
                PlayPanel.IsEnabled = true;
                break;
        }
    }

    private void TimeSlider_MouseMove(object sender, MouseEventArgs e)
    {
        if (TimeSlider.IsMouseCaptureWithin)
        {
            var timePosition = TimeSlider.Value / TimeSlider.Maximum;
            _dispatchCenter.DispatchEvent(new ActionArgs(MainAction.MoveTime, timePosition), newThread: true);
        }
    }

    private void TimeTB_MouseDown(object sender, MouseButtonEventArgs e)
    {
        var timeJumpWindow = _serviceProvider.GetRequiredService<TimeJumpWindow>();
        timeJumpWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        timeJumpWindow.ShowDialog();

        if (timeJumpWindow.NeedAction)
        {
            _dispatchCenter.DispatchEvent(
                new ActionArgs(MainAction.MoveTimeByStr, timeJumpWindow.TimeTextBox.Text), newThread: true);
        }
    }

    private void PlayTB_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _dispatchCenter.DispatchEvent(new ActionArgs(MainAction.Play), newThread: true);
    }

    private void PauseTB_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _dispatchCenter.DispatchEvent(new ActionArgs(MainAction.Pause), newThread: true);
    }

    private void ADBackwardTB_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _dispatchCenter.DispatchEvent(new ActionArgs(MainAction.AdjustBackward), newThread: true);
    }

    private void ADForwardTB_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _dispatchCenter.DispatchEvent(new ActionArgs(MainAction.AdjustForward), newThread: true);
    }

    private void ClearADTB_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _dispatchCenter.DispatchEvent(new ActionArgs(MainAction.ClearAdjustment), newThread: true);
    }

    private void StopTB_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _dispatchCenter.DispatchEvent(new ActionArgs(MainAction.Stop), newThread: true);
        ChangeMainPanelLockStatus(MainPanelLocalStatus.Idling);
        ResetMainPanelContents();
    }

    private void SettingTB_MouseDown(object sender, MouseButtonEventArgs e)
    {
        ShowSettingWindow();
    }

    private void ShowSettingWindow()
    {
        if (_settingWindow is not null)
        {
            BringWindowToFront(_settingWindow);
            return;
        }

        _settingWindow = _serviceProvider.GetRequiredService<SettingWindow>();
        _settingWindow.Closed += (sender, args) =>
        {
            _settingWindow = null;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive);
        };

        _settingWindow.Show();
        _settingWindow.Activate();
    }

    private static void BringWindowToFront(Window window)
    {
        if (window.WindowState == WindowState.Minimized)
            window.WindowState = WindowState.Normal;
        window.Activate();
    }

    // ===== Window drag and move =====
    private void Main_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed) DragMove();
    }

    private void Main_MouseLeave(object sender, MouseEventArgs e)
    {
        _triggerSource.Pull();
    }

    private void Main_MouseMove(object sender, MouseEventArgs e)
    {
        if (_triggerSource.Trigger.IsPulled)
        {
            _triggerSource.ResetTrigger(pullImmed: false);
            MenuPanel.Visibility = Visibility.Visible;
        }
    }
}

internal enum MainPanelLocalStatus
{
    Idling,
    Triggering,
    SubLoaded,
    Playing,
}