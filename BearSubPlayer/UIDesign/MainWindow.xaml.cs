using System;
using System.Windows;
using System.Windows.Media;
using TriggerLib;

namespace BearSubPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TriggerSource _triggerSource;

        public MainWindow()
        {
            InitializeComponent();

            // MainWindow size arrange
            Width = SystemParameters.PrimaryScreenWidth * 2 / 3;
            Top = SystemParameters.PrimaryScreenHeight * 5 / 6;
            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;

            SubLabel.Width = Width;
            var marginleft = Width - MenuPanel.Width - 10;
            MenuPanel.Margin = new Thickness(marginleft, 0, 0, 0);
            TimeTBox.Margin = new Thickness(marginleft, 0, 0, 0);

            MainInitialize();

            // Init TriggerSource
            _triggerSource = new TriggerSource(3000, () => MenuPanelVisibility(false), pullImmed: false);

            // Listen UIControl event
            UIControl.ElementContReq += ElementContReq;
            UIControl.ElementValReq += ElementValReq;
            UIControl.ElementStatReq += ElementStatReq;
            UIControl.CommandReq += CommandReq;
            UIControl.ApparenceReq += ApparenceReq;
        }
    }
}
