using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace BearSubPlayer
{
    public partial class MainWindow : Window
    {
        private SubPlayer _subPlayer;


        // ------ TimeTextbox func + show/hide ------
        private void TimeTBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return) return;
            TimeTBoxVisibility(false);

            if (!PlayPanel.IsEnabled) return;
            var result = TimeSpan.TryParse(TimeTBox.Text, out var time);
            if (result) _subPlayer.MoveTo(time);
        }

        private void TimeLb_MouseDown(object sender, MouseEventArgs e)
        {
            if (TimeTBox.Visibility == Visibility.Visible) return;

            var currenttime = TimeLb.Content.ToString().Split('/')[0].Trim();
            TimeTBoxText(currenttime);
            TimeTBoxVisibility(true);
        }

        // ------ Menu panel function ------
        private async void SubLabel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                DefaultExt = ".srt", // Default file extension
                Filter = "Sub File (*.srt, *ass)|*.srt;*.ass" // Filter files by extension
            };

            var result = dlg.ShowDialog();
            if (!(bool)result) return;

            MainReset(true);
            _subPlayer = await SubPlayer.CreateSubPlayerAsync(dlg.FileName);
        }

        private async void PlayLb_MouseDown(object sender, MouseButtonEventArgs e)
            => await _subPlayer.PlayAsync();

        private void TimeSld_MouseMove(object sender, MouseEventArgs e)
        {
            if (TimeSld.IsMouseCaptureWithin)
                _subPlayer.TimeSldChanged(TimeSld.Value);
        }

        private void BackWardLb_MouseDown(object sender, MouseButtonEventArgs e)
            => _subPlayer.Backward();

        private void ForWardLb_MouseDown(object sender, MouseButtonEventArgs e)
            => _subPlayer.Forward();

        private void PauseLb_MouseDown(object sender, MouseButtonEventArgs e)
            => _subPlayer.Pause();

        private void StopLb_MouseDown(object sender, MouseButtonEventArgs e)
            => _subPlayer.Stop();

        private void SettingLb_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SettingWindow.Self.Show();
        }
    }
}
