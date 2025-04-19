using BearSubPlayer.Core;
using BearSubPlayer.SubReaders;

namespace BearSubPlayer.Services;

public class SubPlayerService
{
    public event Action<bool>? InputFreezeNeeded;
    public event Action<string>? SubChanged;
    public event Action<(TimeSpan, TimeSpan)>? TimeElapsed;
    public bool IsFreezeNeeded { get; private set; }
    public TimeSpan? TotalSubTime { get => _subTimer?.SubTotalTime; }

    private readonly ConfigService _configService;
    private readonly InputSimulationService _inputSimService;
    private SubInfo[]? _subInfoArr;
    private SubTimer? _subTimer;
    private string? _currentContents;

    public SubPlayerService(ConfigService configService, InputSimulationService inputSimService)
    {
        _configService = configService;
        _inputSimService = inputSimService;
    }

    public void LoadSubInfo(SubInfo[] subInfoArr)
    {
        _subInfoArr = subInfoArr;
        var totalTime = subInfoArr.Max(info => info.TEnd);

        _subTimer = new SubTimer(totalTime);
        _subTimer.Elapsed += SubTimer_Elapsed;
    }

    private void SubTimer_Elapsed(SubTimer obj)
    {
        TimeElapsed?.Invoke((obj.SubCurrentTime, obj.SubTotalTime));

        var sub = _subInfoArr?.FirstOrDefault(x => x.TStart <= obj.SubCurrentTime && obj.SubCurrentTime <= x.TEnd);
        var contents = "";
        if (sub is not null) contents = sub.Content;

        if (_currentContents is null || _currentContents != contents)
        {
            _currentContents = contents;
            SubChanged?.Invoke(_currentContents);
        }
    }

    public void UnloadSubInfo()
    {
        _subInfoArr = null;
        _subTimer!.Elapsed -= SubTimer_Elapsed;
        _subTimer = null;
    }

    public void Play()
    {
        IsFreezeNeeded = true;
        InputFreezeNeeded?.Invoke(true);

        var triggerType = _configService.AutoPlayTrigger;
        for (var i = 3; i >= 1; i--)
        {
            SubChanged?.Invoke($"Auto trigger: {triggerType}, {i}s left...");
            Task.Delay(1000).Wait();
        }

        // Replace the notice message above
        SubChanged?.Invoke(_currentContents is null ? "" : _currentContents);

        switch (triggerType)
        {
            case AutoPlayTrigger.MouseLeftClick:
                _inputSimService.MouseLeftClick(0);
                break;
            case AutoPlayTrigger.SpaceKey:
                _inputSimService.SpaceKey(0);
                break;
            case AutoPlayTrigger.None:
                break;
        }

        _subTimer!.Start();

        IsFreezeNeeded = false;
        InputFreezeNeeded?.Invoke(false);
    }

    public void ChangeTime(double timePercentage)
    {
        var time = (int)(timePercentage * _subTimer!.SubTotalTime.TotalMilliseconds / 100);
        _subTimer.MoveTo(new TimeSpan(0, 0, 0, 0, time));
    }

    public bool MoveTo(double position)
    {
        var time = position * _subTimer!.SubTotalTime.TotalMilliseconds;
        return MoveTo(new TimeSpan(0, 0, 0, 0, (int)time));
    }

    public bool MoveTo(TimeSpan time)
    {
        if (time < new TimeSpan(0, 0, 0, 0, 0) || time > _subTimer!.SubTotalTime) return false;
        _subTimer.MoveTo(time);
        SubTimer_Elapsed(_subTimer);
        return true;
    }

    public void Backward()
    {
        _subTimer!.AdjustTime(new TimeSpan(0, 0, 0, 0, -60));
        if (!_subTimer.IsRunning) SubTimer_Elapsed(_subTimer);
    }

    public void Forward()
    {
        _subTimer!.AdjustTime(new TimeSpan(0, 0, 0, 0, 60));
        if (!_subTimer.IsRunning) SubTimer_Elapsed(_subTimer);
    }

    public void ClearTimeAdjustment()
    {
        _subTimer!.ClearTimeAdjustment();
        if (!_subTimer.IsRunning) SubTimer_Elapsed(_subTimer);
    }

    public TimeSpan GetTimeAdjustment()
    {
        return _subTimer!.AdjustedTime;
    }

    public void Pause()
    {
        _subTimer!.Pause();
    }

    public void Stop()
    {
        _subTimer!.Stop();
    }
}
