using System;
using System.Windows;
using System.Windows.Media;
using TriggerLib;
using BearSubPlayer.Core;

namespace BearSubPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SubPlayer _subPlayer;
        private readonly TriggerSource _triggerSource;
        private Brush _currentBrush;

        public MainWindow()
        {
            InitializeComponent();

            // MainWindow size arrange
            this.Width = SystemParameters.PrimaryScreenWidth * 2 / 3;
            Top = SystemParameters.PrimaryScreenHeight * 5 / 6;
            Left = (SystemParameters.PrimaryScreenWidth - this.Width) / 2;

            SubLabel.Width = this.Width;
            var marginleft = this.Width - MenuPanel.Width - 10;
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
