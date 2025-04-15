namespace BearSubPlayer.SubReaders;

public interface ISubReader
{
    public SubInfo[] Read(string path);
}