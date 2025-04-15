using System.Windows.Threading;

namespace BearSubPlayer.Helpers;

public static class XAMLExtensions
{
    public static void InvokeIfNeeded(this DispatcherObject dispatcher, Action action)
    {
        if (Thread.CurrentThread != dispatcher.Dispatcher.Thread)
            dispatcher.Dispatcher.Invoke(action);
        else
            action();
    }
}
