using System;

namespace BearSubPlayer.Sub
{
    public record SubInfo
    {
        public TimeSpan TStart { get; init; }
        public TimeSpan TEnd { get; init; }
        public string Contents { get; init; }
    }
}
