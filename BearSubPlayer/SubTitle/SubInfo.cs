using System;

namespace BearSubPlayer.SubTitle
{
    public record SubInfo
    {
        public TimeSpan TStart { get; init; }
        public TimeSpan TEnd { get; init; }
        public string Contents { get; init; }
    }
}
