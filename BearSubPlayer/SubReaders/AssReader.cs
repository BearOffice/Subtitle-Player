using System.IO;
using System.Text.RegularExpressions;

namespace BearSubPlayer.SubReaders;

public class AssReader : ISubReader
{
    public SubInfo[] Read(string path)
    {
        using var reader = new StreamReader(path);
        var infoList = new List<SubInfo>();
        var subBlock = new List<string>();

        while (!reader.EndOfStream)
        {
            if (reader.ReadLine() == "[Events]")
                break;
        }

        var rawFormat = reader.ReadLine() ?? throw new IOException("Unable to read file.");
        var formats = rawFormat.Split(':')[1].Split(',').Select(str => str.Trim()).ToArray();

        var infoIndex = GenerateInfoIndex(formats);
        var pattern = GeneratePattern(formats.Length);

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine() ?? throw new IOException("Unable to read file.");
            var subInfo = AssInterpret(line, infoIndex, pattern);
            if (subInfo != null) infoList.Add(subInfo);
        }

        return [.. infoList];
    }

    private static AssSubInfoIndex GenerateInfoIndex(string[] formats)
    {
        var formatList = formats.ToList();
        return new AssSubInfoIndex
        {
            TStart = formatList.FindIndex(str => str == "Start"),
            TEnd = formatList.FindIndex(str => str == "End"),
            Content = formatList.FindIndex(str => str == "Text")
        };
    }

    private static Regex GeneratePattern(int repeat)
    {
        var pattern = "Dialogue:";
        for (var i = 0; i < repeat; i++)
        {
            if (i < repeat - 1)
                pattern += "([^,]*),";
            else
                pattern += "(.*)";
        }

        return new Regex(pattern);
    }

    private static SubInfo? AssInterpret(string subBlock, AssSubInfoIndex infoIndex, Regex pattern)
    {
        var match = pattern.Match(subBlock);
        if (!match.Success) return null;

        var tStart = TimeSpan.Parse(match.Groups[infoIndex.TStart + 1].Value.Trim());
        var tend = TimeSpan.Parse(match.Groups[infoIndex.TEnd + 1].Value.Trim());
        var rawContent = match.Groups[infoIndex.Content + 1].Value.Replace(@"\N", "\n");
        var content = Regex.Replace(rawContent, @"<.*?>|{.*?}", "");

        return new SubInfo()
        {
            TStart = tStart,
            TEnd = tend,
            Content = content,
        };
    }
}


public record AssSubInfoIndex
{
    public required int TStart { get; init; }
    public required int TEnd { get; init; }
    public required int Content { get; init; }
}