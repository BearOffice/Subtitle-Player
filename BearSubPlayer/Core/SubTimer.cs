using System.Diagnostics;

using System.Timers;
using Timer = System.Timers.Timer;

namespace BearSubPlayer.Core;

public class SubTimer
{
    public event Action<SubTimer>? Elapsed;
    public bool IsEnded { get; private set; }
    public bool IsRunning => _timer.Enabled;
    public TimeSpan SubTotalTime => _baseTotalTime + _adjustedTime;
    public TimeSpan SubCurrentTime => _accumulatedTime + _stopwatch.Elapsed + _adjustedTime;
    public TimeSpan AdjustedTime => _adjustedTime;

    private readonly Stopwatch _stopwatch = new Stopwatch();
    private readonly Timer _timer = new Timer(50);
    private readonly TimeSpan _baseTotalTime;
    private TimeSpan _adjustedTime;
    private TimeSpan _accumulatedTime;

    public SubTimer(TimeSpan totalTime)
    {
        _baseTotalTime = totalTime;
        _timer.Elapsed += OnTimerElapsed;
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (SubCurrentTime >= SubTotalTime)
        {
            IsEnded = true;
            Stop();
        }

        Elapsed?.Invoke(this);
    }

    public void Start()
    {
        if (IsRunning) return;

        _stopwatch.Start();
        _timer.Start();
    }

    public void Pause()
    {
        if (!IsRunning) return;

        _accumulatedTime += _stopwatch.Elapsed;
        _stopwatch.Reset();
        _timer.Stop();
    }

    public void Stop()
    {
        _stopwatch.Stop();
        _timer.Stop();
    }

    public void MoveTo(TimeSpan time)
    {
        // Since SubTimer only has properties that show the time after adjustment,
        // we need to apply the internal adjustment to the input time.
        _accumulatedTime = time - _adjustedTime;

        if (IsRunning)
            _stopwatch.Restart();
    }

    public void AdjustTime(TimeSpan timeAdjustment)
    {
        _adjustedTime += timeAdjustment;
    }

    public void ClearTimeAdjustment()
    {
        _adjustedTime = TimeSpan.Zero;
    }
}