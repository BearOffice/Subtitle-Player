using System;
using System.Windows;

namespace BearSubPlayer
{
    /// <summary>
    /// Interaction logic for Setting.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        private SettingWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            var config = Config.GetConfigArgs();

            OpacitySld.Value = config.MainOp;
            OpacityLb.Content = (int)(config.MainOp * 100) + "%";

            if (config.MainCol == 0)  // White
                WhiteRBtn.IsChecked = true;
            else
                BlackRBtn.IsChecked = true;

            FontSizeSld.Value = config.FontSize;
            FontSizeLb.Content = config.FontSize + "pt";

            if (config.FontCol == 0)  // White
                FontWhiteRBtn.IsChecked = true;
            else
                FontBlackRBtn.IsChecked = true;

            FontShadowOpacitySld.Value = config.FontOp;
            FontShadowOpacityLb.Content = (int)(config.FontOp * 100) + "%";
            FontShadowSoftnessSld.Value = config.FontSn;
            FontShadowSoftnessLb.Content = config.FontSn;
        }
    }
}