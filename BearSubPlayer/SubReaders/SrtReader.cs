using System.IO;
using System.Text.RegularExpressions;

namespace BearSubPlayer.SubReaders;

public class SrtReader : ISubReader
{
    public SubInfo[] Read(string path)
    {
        using var reader = new StreamReader(path);
        var infoList = new List<SubInfo>();
        var subBlock = new List<string>();

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine() ?? throw new IOException("Unable to read file.");

            if (string.IsNullOrWhiteSpace(line) && subBlock.Count != 0)  // The end of one subtitle
            {
                infoList.Add(SrtInterpret(subBlock));
                subBlock.Clear();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(line) && subBlock.Count == 0) continue;  // Ignore empty line at head
                subBlock.Add(line);
            }
        }

        return [.. infoList];
    }

    private static SubInfo SrtInterpret(List<string> subBlock)
    {
        var time = subBlock[1].Replace(',', '.').Split(" --> ");  // Second line is time info
        var tStart = TimeSpan.Parse(time[0]);
        var tEnd = TimeSpan.Parse(time[1]);

        var content = "";
        for (var i = 2; i < subBlock.Count; i++)  // Third line to the last line is subtitle info
        {
            content += Regex.Replace(subBlock[i], @"<.*?>|{.*?}", "");  // Remove font info
            if (i < subBlock.Count - 1)
                content += "\n";
        }

        return new SubInfo()
        {
            TStart = tStart,
            TEnd = tEnd,
            Content = content,
        };
    }
}