using System;
using System.Diagnostics;
using System.Timers;

namespace BearSubPlayer
{
    public class SubTimer
    {
        private readonly Stopwatch _stopWatch = new Stopwatch();
        private readonly Timer _timer = new Timer(50);
        private readonly TimeSpan _totalTime;
        private TimeSpan _adjustTime = new TimeSpan();
        private TimeSpan _previousTime = new TimeSpan();
        public event Action Elapsed;
        public bool IsEnded { get; private set; } = false;
        public bool IsRunning
        {
            get => _timer.Enabled;
        }
        public TimeSpan TotalTime
        {
            get => _totalTime + _adjustTime;
        }
        public TimeSpan ElapsedTime
        {
            get => _previousTime + _stopWatch.Elapsed;
        }
        public TimeSpan CurrentTime
        {
            get => ElapsedTime + _adjustTime;
        }

        public SubTimer(TimeSpan totalTime)
        {
            _totalTime = totalTime;
            _timer.Elapsed += PublishEvent;
        }

        private void PublishEvent(object sender, ElapsedEventArgs e)
        {
            if (CurrentTime >= TotalTime)
            {
                IsEnded = true;
                Stop();
            }
            Elapsed?.Invoke();
        }

        public void Start()
        {
            if (IsRunning) return;
            _timer.Start();
            _stopWatch.Start();
        }

        public void Pause()
        {
            if (!IsRunning) return;
            _previousTime += _stopWatch.Elapsed;
            _timer.Stop();
            _stopWatch.Reset();
        }

        public void Stop()
        {
            _timer.Stop();
            _stopWatch.Stop();
        }

        public void MoveTo(TimeSpan time)
        {
            _previousTime = time;
            if (IsRunning)
                _stopWatch.Restart();
        }

        public void AdjustTime(TimeSpan time)
        {
            _adjustTime += time;
        }
    }
}
