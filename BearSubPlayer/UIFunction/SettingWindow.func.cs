using System;
using System.Windows;
using System.Windows.Media;

namespace BearSubPlayer
{
    public partial class SettingWindow : Window
    {
        public static SettingWindow Self { get; private set; } = new SettingWindow();
        private bool _exitsign = false;

        private void Main_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
                    => Main_Changed();

        private void Main_Changed(object sender, RoutedEventArgs e)
            => Main_Changed();

        private void Main_Changed()
        {
            if (!IsInitialized) return;

            OpacityLb.Content = (int)(OpacitySld.Value * 100) + "%";  // Change float to %
            if ((bool)WhiteRBtn.IsChecked)
                UIControl.Request(Apparence.Main, new ApparenceArgs()
                {
                    FontBrush = Brushes.Black,
                    Color = Colors.White,
                    Opacity = OpacitySld.Value
                });
            else
                UIControl.Request(Apparence.Main, new ApparenceArgs()
                {
                    FontBrush = Brushes.White,
                    Color = Colors.Black,
                    Opacity = OpacitySld.Value
                });
        }

        private void Font_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
            => Font_Changed();

        private void Font_Changed(object sender, RoutedEventArgs e)
            => Font_Changed();

        private void Font_Changed()
        {
            if (!IsInitialized) return;

            FontSizeLb.Content = (int)FontSizeSld.Value + "pt";
            FontShadowOpacityLb.Content = (int)(FontShadowOpacitySld.Value * 100) + "%"; // Change float to %
            FontShadowSoftnessLb.Content = (int)FontShadowSoftnessSld.Value;

            if ((bool)FontWhiteRBtn.IsChecked)
                UIControl.Request(Apparence.Font, new ApparenceArgs()
                {
                    FontBrush = Brushes.White,
                    Color = Colors.White,
                    Opacity = FontShadowOpacitySld.Value,
                    Softness = FontShadowSoftnessSld.Value,
                    FontSize = FontSizeSld.Value
                });
            else
                UIControl.Request(Apparence.Font, new ApparenceArgs()
                {
                    FontBrush = Brushes.Black,
                    Color = Colors.Black,
                    Opacity = FontShadowOpacitySld.Value,
                    Softness = FontShadowSoftnessSld.Value,
                    FontSize = FontSizeSld.Value
                });
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            _exitsign = true;
            Application.Current.Shutdown();
        }

        private void SetDefaultBtn_Click(object sender, RoutedEventArgs e)
        {
            Config.SetDefault();
            UIControl.Request(Command.Initialize);
            Initialize();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
            => Close();

        private void SettingWindow_Closed(object sender, EventArgs e)
        {
            Config.SaveConfig(new ConfigArgs
            {
                MainOp = Math.Round(OpacitySld.Value * 100) / 100,  // Round the number
                FontSize = (int)FontSizeSld.Value,
                FontOp = Math.Round(FontShadowOpacitySld.Value * 100) / 100,
                FontSn = (int)FontShadowSoftnessSld.Value,
                MainCol = (bool)WhiteRBtn.IsChecked ? 0 : 1,
                FontCol = (bool)FontWhiteRBtn.IsChecked ? 0 : 1,
            });

            if (!_exitsign) Self = new SettingWindow();  // Create settingwindow's instance will interrupt the exiting procedure
        }
    }
}
