using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BearSubPlayer
{
    public partial class MainWindow : Window
    {
        // ------ Main window Drag func + show/hide ------
        private void Main_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
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
                MenuPanelVisibility(true, ispartial: true);
            }
        }

        // ------ Highlight animation ------
        private void Label_MouseMove(object sender, MouseEventArgs e)
        {
            var label = (Label)sender;
            if (label.Foreground == Brushes.White)
            {
                _currentBrush = Brushes.White;
                label.Foreground = Brushes.DimGray;
            }
            else if (label.Foreground == Brushes.Black)
            {
                _currentBrush = Brushes.Black;
                label.Foreground = Brushes.LightGray;
            }
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            var label = (Label)sender;
            label.Foreground = _currentBrush;
        }
    }
}
