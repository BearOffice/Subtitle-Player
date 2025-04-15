using BearSubPlayer.SubReaders;
using System.IO;

namespace BearSubPlayer.Services;

public class SubReadService
{
    private readonly Dictionary<string, ISubReader> _subReaders;

    public SubReadService(Dictionary<string, ISubReader> subReaders)
    {
        _subReaders = subReaders;
    }

    public SubInfo[]? Read(string path)
    {
        var fileType = Path.GetExtension(path);
        if (fileType.Length > 1 && fileType[0] == '.') fileType = fileType[1..];
        if (!_subReaders.TryGetValue(fileType.ToLower(), out var subReader))
            return null;

        try
        {
            return subReader.Read(path);
        }
        catch
        {
            return null;
        }
    }
}
