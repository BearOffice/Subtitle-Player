using System;
using BearMLLib;

namespace BearSubPlayer
{
    public static class Config
    {
        public static ConfigArgs GetConfigArgs()
        {
            try
            {
                var reader = new BearML("config.txt");
                return reader.DeserializeObjectGroup<ConfigArgs>("configs");
            }
            catch 
            {
                SetDefault();
                return new ConfigArgs();
            }
        }

        public static void SetDefault()
        {
            var reader = new BearML();
            reader.AddObjectGroup("configs", new ConfigArgs());
            reader.SaveCopyTo("config.txt");
        }

        public static void SaveConfig(ConfigArgs configargs)
        {
            try
            {
                var reader = new BearML("config.txt");
                reader.ChangeObjectGroup("configs", configargs);
            }
            catch 
            {
                SetDefault();
            }
        }
    }
}
