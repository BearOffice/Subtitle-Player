using System;
using ConfigReadingLib;

namespace BearSubPlayer
{
    public static class Config
    {
        private static readonly ConfigInfo[] _defaultConfigs = new[]
        {
            new ConfigInfo { Key = "mainop", Value = "0.5", Comment = "value=\"0-1\"" },
            new ConfigInfo { Key = "maincol", Value = "0", Comment = "value=\"white=0, black=1\"" },
            new ConfigInfo { Key = "fontsize", Value = "32", Comment = "value=\"12-46\"" },
            new ConfigInfo { Key = "fontcol", Value = "0", Comment = "value=\"white=0, black=1\"" },
            new ConfigInfo { Key = "fontop", Value = "0.5", Comment = "value=\"0-1\"" },
            new ConfigInfo { Key = "fontsn", Value = "8", Comment = "value=\"5-15\"" }
        };
        private static readonly ParseToString _parseToString = new ParseToString
        {
            [typeof(double)] = x => x.ToString(),
        };
        private static readonly ParseFromString _parseFromString = new ParseFromString
        {
            [typeof(double)] = x => double.Parse(x),
        };

        public static ConfigArgs GetConfig()
        {
            var configargs = GetArgs();
            if (configargs == null)
            {
                SetDefault();
                configargs = GetArgs();
            }

            return configargs;
        }

        private static ConfigArgs GetArgs()
        {
            ConfigReader reader;
            try
            {
                reader = new ConfigReader("config.conf", strict: true);
            }
            catch
            {
                return null;
            }

            var configargs = new ConfigArgs();
            reader.SetPropertiesFromKeys(configargs, _parseFromString, exactmatch: true);

            if (!(0 <= configargs.MainOp && configargs.MainOp <= 1)) return null;
            if (!(0 <= configargs.MainCol && configargs.MainCol <= 1)) return null;
            if (!(12 <= configargs.FontSize && configargs.FontSize <= 46)) return null;
            if (!(0 <= configargs.FontCol && configargs.FontCol <= 1)) return null;
            if (!(0 <= configargs.FontOp && configargs.FontOp <= 1)) return null;
            if (!(5 <= configargs.FontSn && configargs.FontSn <= 15)) return null;

            return configargs;
        }

        public static void SetDefault()
        {
            ConfigReader.Create(_defaultConfigs, "config.conf");
        }

        public static void SaveConfig(ConfigArgs configargs)
        {
            try
            {
                var reader = new ConfigReader("config.conf", strict: true);
                reader.SavePropertiesToKeys(configargs, _parseToString, exactmatch: true);
            }
            catch
            {
                SetDefault();
            }
        }
    }
}
