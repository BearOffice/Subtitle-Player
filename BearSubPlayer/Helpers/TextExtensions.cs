using System.Text;

namespace BearSubPlayer.Helpers;

public static class TextExtensions
{
    public static string RepeatAndJoin(this string str, string join, int repeat)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < repeat; i++)
        {
            sb.Append(str);
            if (i < repeat - 1) sb.Append(join);
        }

        return sb.ToString();
    }
}
