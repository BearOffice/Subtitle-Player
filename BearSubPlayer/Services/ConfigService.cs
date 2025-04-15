using BearMarkupLanguage;
using System.IO;
using System.Numerics;

namespace BearSubPlayer.Services;

public class ConfigService
{
    public double PanelOpacity
    {
        get => _panelOpacityFitter.Fit(_ml.GetValue<double>("pn_opacity"), f => _ml.ChangeValue("pn_opacity", f.Default));
        set => _ml.ChangeValue("pn_opacity", _panelOpacityFitter.Fit(value));
    }

    public MonoColor PanelColor
    {
        get => _ml.GetValue<MonoColor>("pn_color");
        set => _ml.ChangeValue("pn_color", value);
    }

    public int FontSize
    {
        get => _fontSizeFitter.Fit(_ml.GetValue<int>("font_size"), f => _ml.ChangeValue("font_size", f.Default));
        set => _ml.ChangeValue("font_size", _fontSizeFitter.Fit(value));
    }

    public MonoColor FontColor
    {
        get => _ml.GetValue<MonoColor>("font_color");
        set => _ml.ChangeValue("font_color", value);
    }

    public double ShadowOpacity
    {
        get => _shadowOpacityFitter.Fit(
            _ml.GetValue<double>("shadow_opacity"), f => _ml.ChangeValue("shadow_opacity", f.Default));
        set => _ml.ChangeValue("shadow_opacity", _shadowOpacityFitter.Fit(value));
    }

    public int ShadowSoftness
    {
        get => _shadowSoftnessFitter.Fit(
            _ml.GetValue<int>("shadow_softness"), f => _ml.ChangeValue("shadow_softness", f.Default));
        set => _ml.ChangeValue("shadow_softness", _shadowSoftnessFitter.Fit(value));
    }

    public AutoPlayTrigger AutoPlayTrigger
    {
        get => _ml.GetValue<AutoPlayTrigger>("trigger");
        set => _ml.ChangeValue("trigger", value);
    }

    public string BasePath { get; }

    private static readonly RangedNumberFitter<double> _panelOpacityFitter = new(0.0, 1.0, 0.5);
    private static readonly RangedNumberFitter<int> _fontSizeFitter = new(12, 46, 32);
    private static readonly RangedNumberFitter<double> _shadowOpacityFitter = new(0.0, 1.0, 0.5);
    private static readonly RangedNumberFitter<int> _shadowSoftnessFitter = new(5, 15, 8);

    private readonly BearML _ml;

    public ConfigService(string basePath)
    {
        BasePath = basePath;

        var configPath = Path.Combine(BasePath, "config.txt");
        if (!File.Exists(configPath))
        {
            _ml = new BearML(configPath);
            SetDefault();
        }
        else
        {
            _ml = new BearML(configPath);
        }

        _ml.DelayedSave = true;
    }

    public void SetDefault()
    {
        // remove current values
        _ml.GetAllKeys().ToList().ForEach(_ml.RemoveKey);
        _ml.GetAllSubBlockNames().ToList().ForEach(_ml.RemoveBlock);

        _ml.AddKeyValue("main panel opacity", _panelOpacityFitter.Default);
        _ml.ChangeKeyAliases("main panel opacity", ["pn_opacity"]);

        _ml.AddKeyValue("main panel color", MonoColor.White);
        _ml.ChangeKeyAliases("main panel color", ["pn_color"]);

        _ml.AddKeyValue("font size", _fontSizeFitter.Default);
        _ml.ChangeKeyAliases("font size", ["font_size"]);

        _ml.AddKeyValue("font color", MonoColor.Black);
        _ml.ChangeKeyAliases("font color", ["font_color"]);

        _ml.AddKeyValue("font shadow opacity", _shadowOpacityFitter.Default);
        _ml.ChangeKeyAliases("font shadow opacity", ["shadow_opacity"]);

        _ml.AddKeyValue("font shadow softness", _shadowSoftnessFitter.Default);
        _ml.ChangeKeyAliases("font shadow softness", ["shadow_softness"]);

        _ml.AddKeyValue("auto play trigger", AutoPlayTrigger.SpaceKey);
        _ml.ChangeKeyAliases("auto play trigger", ["trigger"]);
    }

    public void SaveChanges()
    {
        _ml.Save();
    }
}

public class RangedNumberFitter<T> where T : INumber<T>
{
    public T Default { get => _defaultValue; }

    private readonly T _min;
    private readonly T _max;
    private readonly T _defaultValue;

    public RangedNumberFitter(T min, T max, T defaultValue)
    {
        _min = min;
        _max = max;
        _defaultValue = defaultValue;
    }

    public T Fit(T value, Action<RangedNumberFitter<T>>? action = null)
    {
        if (value >= _min && value <= _max)
        {
            return value;
        }

        action?.Invoke(this);
        return _defaultValue;
    }
}

public enum MonoColor
{
    White,
    Black
}

public enum AutoPlayTrigger
{
    None,
    MouseLeftClick,
    SpaceKey,
} 