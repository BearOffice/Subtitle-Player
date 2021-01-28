using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BearSubPlayer.SubTitle
{
    public class AssReader : ISubReader
    {
        private int _tStartInd;
        private int _tEndInd;
        private int _contentsInd;
        private string _pattern;

        public List<SubInfo> Read(string path)
        {
            using var reader = new StreamReader(path);
            var sublist = new List<SubInfo>();
            var subblock = new List<string>();

            while (!reader.EndOfStream)
            {
                if (reader.ReadLine() == "[Events]")
                    break;
            }

            var rawformat = reader.ReadLine();
            var formats = rawformat.Split(':')[1].Split(',').Select(str => str.Trim()).ToList();

            GenerateInfoIndex(formats);
            GeneratePattern(formats.Count);

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var subinfo = AssInterpret(line);
                if (subinfo != null) sublist.Add(subinfo);
            }

            return sublist;
        }

        private void GenerateInfoIndex(List<string> formats)
        {
            _tStartInd = formats.FindIndex(str => str == "Start");
            _tEndInd = formats.FindIndex(str => str == "End");
            _contentsInd = formats.FindIndex(str => str == "Text");
        }

        private void GeneratePattern(int repeat)
        {
            _pattern = "Dialogue:";
            for (var i = 0; i < repeat; i++)
            {
                if (i < repeat - 1)
                    _pattern += "([^,]*),";
                else
                    _pattern += "(.*)";
            }
        }

        private SubInfo AssInterpret(string subblock)
        {
            var match = Regex.Match(subblock, _pattern);
            if (!match.Success) return null;

            var tstart = TimeSpan.Parse(match.Groups[_tStartInd + 1].Value.Trim());
            var tend = TimeSpan.Parse(match.Groups[_tEndInd + 1].Value.Trim());
            var rawcontents = match.Groups[_contentsInd + 1].Value.Replace(@"\N", "\n");
            var contents = Regex.Replace(rawcontents, @"<.*?>|{.*?}", "");

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