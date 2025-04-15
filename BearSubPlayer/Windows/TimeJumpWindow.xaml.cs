using System.Windows;
using System.Windows.Input;

namespace BearSubPlayer.Windows;

public partial class TimeJumpWindow : Window
{
    public bool NeedAction { get; private set; }

    public TimeJumpWindow()
    {
        InitializeComponent();
    }

    private void TimeTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            NeedAction = true;
            Close();
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        NeedAction = true;
        Close();
    }
}
