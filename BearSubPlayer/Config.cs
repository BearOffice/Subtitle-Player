using System;
using ConfReaderLib;

namespace BearSubPlayer
{
    public class Config
    {
        public double MainOp { get; set; }
        public int MainCol { get; set; }
        public int FontSize { get; set; }
        public int FontCol { get; set; }
        public double FontOp { get; set; }
        public int FontSn { get; set; }

        private static readonly (string, string, string)[] _defaultconfig = new[] {
            ("mainop", "0.5", "value=\"0-1\"") ,
            ("maincol", "0", "value=\"white=0, black=1\""),
            ("fontsize", "32", "value=\"12-46\""),
            ("fontcol", "0", "value=\"white=0, black=1\""),
            ("fontop", "0.5", "value=\"0-1\""),
            ("fontsn", "8", "value=\"5-15\"")
        };

        public Config()
        {
            try
            {
                Load();

                // Check Value
                if (!(0 <= MainOp && MainOp <= 1)) throw new Exception();
                if (!(0 <= MainCol && MainCol <= 1)) throw new Exception();
                if (!(12 <= FontSize && FontSize <= 46)) throw new Exception();
                if (!(0 <= FontCol && FontCol <= 1)) throw new Exception();
                if (!(0 <= FontOp && FontOp <= 1)) throw new Exception();
                if (!(5 <= FontSn && FontSn <= 15)) throw new Exception();
            }
            catch
            {
                SetDefault();
                Load();
            }
        }

        public static void SetDefault()
        {
            ConfReader.Create(_defaultconfig, "config.conf");
        }

        private void Load()
        {
            var reader = new ConfReader("config.conf", strict: true);
            var rule = new ParseFromString()
            {
                [typeof(double)] = x => double.Parse(x),
            };
            reader.SetProperties(this, rule, strict: true);
        }

        public void Save()
        {
            try
            {
                var reader = new ConfReader("config.conf", strict: true);
                var rule = new ParseToString()
                {
                    [typeof(double)] = x => x.ToString(),
                };
                reader.SaveProperties(this, rule, strict: true);
            }
            catch
            {
                SetDefault();
            }
        }
    }
}
