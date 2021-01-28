using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BearSubPlayer.SubTitle
{
    public class SrtReader : ISubReader
    {
        public List<SubInfo> Read(string path)
        {
            using var reader = new StreamReader(path);
            var sublist = new List<SubInfo>();
            var subblock = new List<string>();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line == "" && subblock.Count != 0)   // The end of one subtitle
                {
                    sublist.Add(SrtInterpret(subblock));
                    subblock.Clear();
                }
                else
                {
                    subblock.Add(line);
                }
            }

            return sublist;
        }

        private static SubInfo SrtInterpret(List<string> subblock)
        {
            var time = subblock[1].Replace(',', '.').Split(" --> ");    // Second line is time info
            var tstart = TimeSpan.Parse(time[0]);
            var tend = TimeSpan.Parse(time[1]);

            var contents = "";
            for (var i = 2; i < subblock.Count; i++)  // Third line to the last line is subtitle info
            {
                contents += Regex.Replace(subblock[i], @"<.*?>|{.*?}", "");  // Remove font info
                if (i < subblock.Count - 1)
                    contents += "\n";
            }

            return new SubInfo()
            {
                TStart = tstart,
                TEnd = tend,
                Contents = contents,
            };
        }

        public async Task<List<SubInfo>> ReadAsync(string path)
            => await Task.Run(() => Read(path));
    }
}