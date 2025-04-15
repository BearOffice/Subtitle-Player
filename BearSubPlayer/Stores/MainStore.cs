using BearSubPlayer.Core.Actions;
using BearSubPlayer.Core.DataTags;
using BearSubPlayer.Services;
using System.IO;

namespace BearSubPlayer.Stores;

public class MainStore : IStore
{
    public event EventHandler<DataArgs>? Changed;
    private readonly DispatchCenter _dispatchCenter;
    private readonly ConfigService _configService;
    private readonly SubReadService _subReadService;
    private readonly SubPlayerService _subPlayerService;

    public MainStore(DispatchCenter dispatchCenter, ConfigService configService,
        SubReadService subReadService, SubPlayerService subPlayerService)
    {
        _dispatchCenter = dispatchCenter;
        _dispatchCenter.AddListener(typeof(MainAction), ActionReceived);
        _dispatchCenter.AddListener(typeof(SettingAction), SettingChangeActionReceived);

        _configService = configService;
        _subReadService = subReadService;
        _subPlayerService = subPlayerService;
        _subPlayerService.SubChanged += SubPlayerService_SubChanged;
        _subPlayerService.TimeElapsed += SubPlayerService_TimeElapsed;
        _subPlayerService.InputFreezeNeeded += SubPlayerService_InputFreezeNeeded;
    }

    private void SubPlayerService_InputFreezeNeeded(bool obj)
    {
        var data = new DataArgs();
        data.AddData(MainTag.NeedFreezeInput, obj);
        Changed?.Invoke(this, data);
    }

    private void SubPlayerService_TimeElapsed((TimeSpan, TimeSpan) obj)
    {
        var data = new DataArgs();
        data.AddData(MainTag.CurrentTime, obj.Item1);
        data.AddData(MainTag.TotalTime, obj.Item2);
        Changed?.Invoke(this, data);
    }

    private void SubPlayerService_SubChanged(string obj)
    {
        var data = new DataArgs();
        data.AddData(MainTag.SubContent, obj);
        Changed?.Invoke(this, data);
    }

    private void ActionReceived(object? sender, ActionArgs e)
    {
        if (e.Type is MainAction.LoadFile)
        {
            var path = (string)e.GetAnonymousData()!;
            var subInfoArr = _subReadService.Read(path);
            var data = new DataArgs();

            if (subInfoArr is null)
            {
                data.AddData(MainTag.LoadFailureReasons, "Unable to load this file.");
                Changed?.Invoke(this, data);
                return;
            }

            _subPlayerService.LoadSubInfo(subInfoArr);
            data.AddData(MainTag.LoadSuccessInfo, Path.GetFileName(path));
            data.AddData(MainTag.CurrentTime, TimeSpan.Zero);
            data.AddData(MainTag.TotalTime, _subPlayerService.TotalSubTime!);
            Changed?.Invoke(this, data);
        }
        else if (e.Type is MainAction.MoveTime)
        {
            _subPlayerService.MoveTo((double)e.GetAnonymousData()!);
        }
        else if (e.Type is MainAction.MoveTimeByStr)
        {
            var str = (string)e.GetAnonymousData()!;
            if (TimeSpan.TryParse(str, out var time))
                _subPlayerService.MoveTo(time);
        }
        else if (e.Type is MainAction.Play)
        {
            _subPlayerService.Play();
        }
        else if (e.Type is MainAction.Pause)
        {
            _subPlayerService.Pause();
        }
        else if (e.Type is MainAction.Stop)
        {
            _subPlayerService.Stop();
        }
        else if (e.Type is MainAction.AdjustBackward)
        {
            _subPlayerService.Backward();
        }
        else if (e.Type is MainAction.AdjustForward)
        {
            _subPlayerService.Forward();
        }
        else if (e.Type is MainAction.ClearAdjustment)
        {
            _subPlayerService.ClearTimeAdjustment();
        }
    }

    private void SettingChangeActionReceived(object? sender, ActionArgs e)
    {
        if (e.Type is SettingAction.Change)
        {
            var data = new DataArgs();
            e.GetDataTags().ToList().ForEach(tag => data.AddData(tag, e.GetData(tag)!));
            Changed?.Invoke(this, data);
        }
    }

    public void Dispose()
    {
        _dispatchCenter.RemoveListener(ActionReceived);
        _dispatchCenter.RemoveListener(SettingChangeActionReceived);

        Changed.UnsubscribeAll();
        Changed = null;

        GC.SuppressFinalize(this);
    }

    public DataArgs GetData()
    {
        var data = new DataArgs();
        data.AddData(SettingTag.PanelOpacity, _configService.PanelOpacity);
        data.AddData(SettingTag.PanelColor, _configService.PanelColor);
        data.AddData(SettingTag.FontSize, _configService.FontSize);
        data.AddData(SettingTag.FontColor, _configService.FontColor);
        data.AddData(SettingTag.ShadowOpacity, _configService.ShadowOpacity);
        data.AddData(SettingTag.ShadowSoftness, _configService.ShadowSoftness);

        return data;
    }
}
