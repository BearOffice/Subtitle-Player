namespace BearSubPlayer.SubReaders;

public record SubInfo
{
    public required TimeSpan TStart { get; init; }
    public required TimeSpan TEnd { get; init; }
    public required string Content { get; init; }
}
