using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace BearSubPlayer
{
    public partial class MainWindow : Window
    {
        private void ElementContReq(Element elem, string str)
        {
            switch (elem)
            {
                case Element.TimeLabel:
                    TimeLbContent(str);
                    break;
                case Element.SubLabel:
                    SubLbContent(str);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void ElementValReq(Element elem, double val)
        {
            switch (elem)
            {
                case Element.TimeSlider:
                    if (MenuPanel.Visibility == Visibility.Visible) TimeSldValue(val);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void ElementStatReq(Element elem, bool option)
        {
            switch (elem)
            {
                case Element.PlayPanel:
                    PlayPanelIsEnabled(option);
                    break;
                case Element.SubLabel:
                    SubLbIsEnabled(option);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void CommandReq(Command cmd, bool option)
        {
            switch (cmd)
            {
                case Command.Initialize:
                    MainInitialize();
                    break;
                case Command.Reset:
                    MainReset(option);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void ApparenceReq(Apparence app, ApparenceArgs args)
        {
            switch (app)
            {
                case Apparence.Main:
                    MainBackground(args.FontBrush, args.Color, args.Opacity);
                    break;
                case Apparence.Font:
                    FontEffect(args.FontBrush, args.Color, args.Opacity, args.Softness, args.FontSize);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void MainBackground(Brush fontbrush, Color backgroundcolor, double opacity)
        {
            Background = new SolidColorBrush(backgroundcolor);

            this.InvokeIfNeeded(() =>
            {
                if (opacity <= 0.002)
                    Background.Opacity = 0.002;    // Almost transparent
                else
                    Background.Opacity = opacity;

                // Set all labels
                foreach (var child in MenuPanel.Children)
                {
                    if (child is Label label)
                        label.Foreground = fontbrush;
                }

                foreach (var child in PlayPanel.Children)
                {
                    if (child is Label label)
                        label.Foreground = fontbrush;
                }
            });
        }

        private void FontEffect(Brush fontbrush, Color shadowcolor, double opacity, double softness, double fontsize)
        {
            SubLabel.Foreground = fontbrush;

            var effect = new DropShadowEffect()
            {
                Color = shadowcolor,
                Direction = 320,
                ShadowDepth = 5,
                Opacity = opacity,
                BlurRadius = softness
            };

            this.InvokeIfNeeded(() =>
            {
                SubLabel.Effect = effect;
                SubLabel.FontSize = fontsize;
            });
        }

        private void MainInitialize()
        {
            var config = Config.GetConfig();
            if (config.MainCol == 0)  // White
                MainBackground(Brushes.Black, Colors.White, config.MainOp);
            else
                MainBackground(Brushes.White, Colors.Black, config.MainOp);

            if (config.FontCol == 0)  // White
                FontEffect(Brushes.White, Colors.White, config.FontOp, config.FontSn, config.FontSize);
            else
                FontEffect(Brushes.Black, Colors.Black, config.FontOp, config.FontSn, config.FontSize);
        }

        private void SubLbContent(string contents)
            => this.InvokeIfNeeded(() => SubLabel.Content = contents);

        private void SubLbIsEnabled(bool isenabled)
            => this.InvokeIfNeeded(() => SubLabel.IsEnabled = isenabled);

        private void TimeTBoxText(string time)
            => this.InvokeIfNeeded(() => TimeTBox.Text = time);

        private void TimeTBoxVisibility(bool visible)
            => this.InvokeIfNeeded(() =>
            {
                if (visible) TimeTBox.Visibility = Visibility.Visible;
                else TimeTBox.Visibility = Visibility.Hidden;
            });

        private void TimeLbContent(string time)
            => this.InvokeIfNeeded(() => TimeLb.Content = time);

        private void TimeSldValue(double value)
        {
            if (!TimeSld.IsMouseCaptureWithin)
                this.InvokeIfNeeded(() => TimeSld.Value = value);
        }

        private void PlayPanelIsEnabled(bool isenabled)
        {
            this.InvokeIfNeeded(() => PlayPanel.IsEnabled = isenabled);
            TimeTBoxVisibility(false);
        }

        private void MenuPanelVisibility(bool visible, bool ispartial = false)
        {
            this.InvokeIfNeeded(() =>
            {
                if (visible) MenuPanel.Visibility = Visibility.Visible;
                else MenuPanel.Visibility = Visibility.Hidden;
            });
            if (!ispartial) TimeTBoxVisibility(false);
        }

        private void MainReset(bool ispartial)
        {
            if (!ispartial)
                SubLbContent("Double click here to select a subtitle file");
            PlayPanelIsEnabled(false);
            TimeSldValue(0);
            TimeLbContent("00:00:00 / 00:00:00");
            SubLbIsEnabled(true);
        }
    }
}
