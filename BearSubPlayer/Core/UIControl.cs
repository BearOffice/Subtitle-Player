using System;
using System.Windows.Media;

namespace BearSubPlayer
{
    public static class UIControl
    {
        public static event Action<Element, string> ElementContReq;
        public static event Action<Element, double> ElementValReq;
        public static event Action<Element, bool> ElementStatReq;
        public static event Action<Command, bool> CommandReq;
        public static event Action<Apparence, ApparenceArgs> ApparenceReq;

        public static void Request(Element elem, string str) => ElementContReq?.Invoke(elem, str);
        public static void Request(Element elem, double val) => ElementValReq?.Invoke(elem, val);
        public static void Request(Element elem, bool option) => ElementStatReq?.Invoke(elem, option);
        public static void Request(Command cmd, bool option = false) => CommandReq?.Invoke(cmd, option);
        public static void Request(Apparence app, ApparenceArgs args) => ApparenceReq?.Invoke(app, args);
    }

    public record ApparenceArgs
    {
        public Brush FontBrush { get; init; }
        public Color Color { get; init; }
        public double Opacity { get; init; }
        public double Softness { get; init; }
        public double FontSize { get; init; }
    }

    public enum Element
    {
        PlayPanel,
        TimeLabel,
        TimeSlider,
        SubLabel,
    }

    public enum Command
    {
        Initialize,
        Reset
    }

    public enum Apparence
    {
        Main,
        Font
    }
}
