using BearSubPlayer.Core.Actions;
using BearSubPlayer.Core.DataTags;
using BearSubPlayer.Services;
using System.Windows;

namespace BearSubPlayer.Stores;

public class SettingStore : IStore
{
    public event EventHandler<DataArgs>? Changed;
    private readonly DispatchCenter _dispatchCenter;
    private readonly ConfigService _configService;

    public SettingStore(DispatchCenter dispatchCenter, ConfigService configService)
    {
        _dispatchCenter = dispatchCenter;
        _dispatchCenter.AddListener(typeof(SettingAction), ActionReceived);

        _configService = configService;
    }

    private void ActionReceived(object? sender, ActionArgs e)
    {
        if (e.Type is SettingAction.Save)
        {
            _configService.SaveChanges();
        }
        else if (e.Type is SettingAction.Exit)
        {
            Application.Current.Shutdown();
        }
        else if (e.Type is SettingAction.SetDefault)
        {
            _configService.SetDefault();
            Changed?.Invoke(this, GetData());
        }
        else if (e.Type is SettingAction.Change)
        {
            foreach (var tag in e.GetDataTags())
            {
                switch (tag)
                {
                    case SettingTag.PanelOpacity:
                        _configService.PanelOpacity = (double)e.GetData(tag)!;
                        break;
                    case SettingTag.PanelColor:
                        _configService.PanelColor = (MonoColor)e.GetData(tag)!;
                        break;
                    case SettingTag.FontSize:
                        _configService.FontSize = (int)e.GetData(tag)!;
                        break;
                    case SettingTag.FontColor:
                        _configService.FontColor = (MonoColor)e.GetData(tag)!;
                        break;
                    case SettingTag.ShadowOpacity:
                        _configService.ShadowOpacity = (double)e.GetData(tag)!;
                        break;
                    case SettingTag.ShadowSoftness:
                        _configService.ShadowSoftness = (int)e.GetData(tag)!;
                        break;
                    case SettingTag.AutoPlayTrigger:
                        _configService.AutoPlayTrigger = (AutoPlayTrigger)e.GetData(tag)!;
                        break;
                }
            }
        }
    }


    public void Dispose()
    {
        _dispatchCenter.RemoveListener(ActionReceived);

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
        data.AddData(SettingTag.AutoPlayTrigger, _configService.AutoPlayTrigger);

        return data;
    }
}
