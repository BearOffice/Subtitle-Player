using BearSubPlayer.Core.DataTags;
using BearSubPlayer.Helpers;
using BearSubPlayer.Services;
using BearSubPlayer.Stores;
using System.Windows;
using BearSubPlayer.Core.Actions;

namespace BearSubPlayer.Windows;

public partial class SettingWindow : Window
{
    private readonly DispatchCenter _dispatchCenter;
    private readonly SettingStore _store;

    public SettingWindow(DispatchCenter dispatchCenter, SettingStore store)
    {
        InitializeComponent();
        DataContext = this;

        _dispatchCenter = dispatchCenter;
        _store = store;

        Store_Changed(null, _store.GetData());
        _store.Changed += (sender, e) => this.InvokeIfNeeded(() => Store_Changed(sender, e)); ;
    }

    private void Store_Changed(object? sender, DataArgs e)
    {
        if (e.TryGetData(SettingTag.PanelOpacity, out var pnOpacity))
        {
            var item = (double)pnOpacity!;

            if (item <= 0.002)
                PNOpacitySlider.Value = 0.002;  // Almost transparent
            else
                PNOpacitySlider.Value = item;
        }

        if (e.TryGetData(SettingTag.PanelColor, out var pnColor))
        {
            if ((MonoColor)pnColor! is MonoColor.White)
                PNColorComboBox.SelectedIndex = 0;
            else
                PNColorComboBox.SelectedIndex = 1;
        }

        if (e.TryGetData(SettingTag.FontSize, out var fontSize))
        {
            FontSizeSlider.Value = (int)fontSize!;
        }

        if (e.TryGetData(SettingTag.FontColor, out var fontColor))
        {
            if ((MonoColor)fontColor! is MonoColor.White)
                FontColorComboBox.SelectedIndex = 0;
            else
                FontColorComboBox.SelectedIndex = 1;
        }

        if (e.TryGetData(SettingTag.ShadowOpacity, out var shadowOpacity))
        {
            ShadowOpacitySlider.Value = (double)shadowOpacity!;
        }

        if (e.TryGetData(SettingTag.ShadowSoftness, out var shadowSoftness))
        {
            ShadowSoftnessSlider.Value = (int)shadowSoftness!;
        }

        if (e.TryGetData(SettingTag.AutoPlayTrigger, out var triggerType))
        {
            var type = (AutoPlayTrigger)triggerType!;
            if (type is AutoPlayTrigger.None)
                AutoPlayTriggerComboBox.SelectedIndex = 0;
            else if (type is AutoPlayTrigger.MouseLeftClick)
                AutoPlayTriggerComboBox.SelectedIndex = 1;
            else
                AutoPlayTriggerComboBox.SelectedIndex = 2;
        }
    }

    private void FontSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        var args = new ActionArgs(SettingAction.Change);
        args.AddData(SettingTag.FontSize, (int)FontSizeSlider.Value);
        _dispatchCenter?.DispatchEvent(args);
    }

    private void PNColorComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        var args = new ActionArgs(SettingAction.Change);
        if (PNColorComboBox.SelectedIndex == 0)
            args.AddData(SettingTag.PanelColor, MonoColor.White);
        else
            args.AddData(SettingTag.PanelColor, MonoColor.Black);

        _dispatchCenter.DispatchEvent(args);
    }

    private void PNOpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        var args = new ActionArgs(SettingAction.Change);
        args.AddData(SettingTag.PanelOpacity, PNOpacitySlider.Value);
        _dispatchCenter?.DispatchEvent(args);
    }

    private void FontColorComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        var args = new ActionArgs(SettingAction.Change);
        if (FontColorComboBox.SelectedIndex == 0)
            args.AddData(SettingTag.FontColor, MonoColor.White);
        else
            args.AddData(SettingTag.FontColor, MonoColor.Black);

        _dispatchCenter.DispatchEvent(args);
    }

    private void ShadowOpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        var args = new ActionArgs(SettingAction.Change);
        args.AddData(SettingTag.ShadowOpacity, ShadowOpacitySlider.Value);
        _dispatchCenter?.DispatchEvent(args);
    }

    private void ShadowSoftnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        var args = new ActionArgs(SettingAction.Change);
        args.AddData(SettingTag.ShadowSoftness, (int)ShadowSoftnessSlider.Value);
        _dispatchCenter?.DispatchEvent(args);
    }

    private void AutoPlayTriggerComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        var args = new ActionArgs(SettingAction.Change);
        if (AutoPlayTriggerComboBox.SelectedIndex == 0)
            args.AddData(SettingTag.AutoPlayTrigger, AutoPlayTrigger.None);
        else if (AutoPlayTriggerComboBox.SelectedIndex == 1)
            args.AddData(SettingTag.AutoPlayTrigger, AutoPlayTrigger.MouseLeftClick);
        else
            args.AddData(SettingTag.AutoPlayTrigger, AutoPlayTrigger.SpaceKey);

        _dispatchCenter?.DispatchEvent(args);
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        _dispatchCenter.DispatchEvent(new ActionArgs(SettingAction.Exit));
    }

    private void SetDefaultButton_Click(object sender, RoutedEventArgs e)
    {
        _dispatchCenter.DispatchEvent(new ActionArgs(SettingAction.SetDefault), newThread: true);
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        _dispatchCenter.DispatchEvent(new ActionArgs(SettingAction.Save), newThread: true);
    }
}