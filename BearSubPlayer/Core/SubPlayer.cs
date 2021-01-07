using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using BearSubPlayer.Sub;

namespace BearSubPlayer.Core
{
    public class SubPlayer
    {
        private readonly List<SubInfo> _subList;
        private readonly SubTimer _subTimer;
        private string _currentContents;

        private SubPlayer(List<SubInfo> sublist, string filename)
        {
            _subList = sublist;
            var totaltime = _subList.Max(info => info.TEnd);
            _subTimer = new SubTimer(totaltime);
            _subTimer.Elapsed += Display;

            ShowLoadedNotice(filename, totaltime);
            UIControl.Request(Element.PlayPanel, true);
        }

        private static void ShowLoadedNotice(string filename, TimeSpan totaltime)
        {
            if (filename.Length > 35)
                UIControl.Request(Element.SubLabel, filename.Substring(0, 35) + "... is loaded");
            else
                UIControl.Request(Element.SubLabel, filename + " is loaded");

            UIControl.Request(Element.TimeLabel, $"00:00:00 / {totaltime:hh\\:mm\\:ss}");
        }

        public async Task PlayAsync()
        {
            if (_subTimer.IsRunning) return;

            UIControl.Request(Element.SubLabel, false);  // The file can be loaded only the player isn't playing
            UIControl.Request(Element.PlayPanel, false);  // Lock the play widget

            for (var i = 6; i >= 1; i--)
            {
                UIControl.Request(Element.SubLabel, i % 2 == 0 ? Repeat("=", i / 2) : $"- {i / 2 + 1} -");
                await Task.Delay(500);
            }

            UIControl.Request(Element.PlayPanel, true);
            UIControl.Request(Element.SubLabel, _currentContents);

            _subTimer.Start();

            static string Repeat(string str, int repeat)
            {
                var newstr = "";
                for (var i = 0; i < repeat; i++)
                {
                    newstr += str;
                    if (i < repeat - 1) newstr += " ";
                }
                return newstr;
            }
        }

        public void TimeSldChanged(double timesldvalue)
        {
            var time = (int)(timesldvalue * _subTimer.TotalTime.TotalMilliseconds / 100);
            _subTimer.MoveTo(new TimeSpan(0, 0, 0, 0, time));
            Display();
        }

        public bool MoveTo(TimeSpan time)
        {
            if (time < new TimeSpan(0, 0, 0, 0, 0) || time > _subTimer.TotalTime) return false;
            _subTimer.MoveTo(time);
            Display();
            return true;
        }

        public void Backward()
        {
            _subTimer.AdjustTime(new TimeSpan(0, 0, 0, 0, -50));
            if (!_subTimer.IsRunning)
                Display();
        }

        public void Forward()
        {
            _subTimer.AdjustTime(new TimeSpan(0, 0, 0, 0, 50));
            if (!_subTimer.IsRunning)
                Display();
        }

        public void Pause()
        {
            _subTimer.Pause();
            UIControl.Request(Element.SubLabel, true);
        }

        public void Stop()
        {
            _subTimer.Stop();
            UIControl.Request(Command.Reset, false);
            UIControl.Request(Element.SubLabel, true);
        }

        private void Display()
        {
            SubDisplay();
            TimeDisplay();
            if (_subTimer.IsEnded) Stop();
        }

        private void SubDisplay()
        {
            var sub = _subList.FirstOrDefault(x => x.TStart <= _subTimer.CurrentTime && _subTimer.CurrentTime <= x.TEnd);
            var contents = "";
            if (sub != null) contents = sub.Contents;

            if (_currentContents != contents)
            {
                UIControl.Request(Element.SubLabel, contents);
                _currentContents = contents;
            }
        }

        private void TimeDisplay()
        {
            var elapsedtime = _subTimer.ElapsedTime;
            var totaltime = _subTimer.TotalTime;
            UIControl.Request(Element.TimeLabel, $"{elapsedtime:hh\\:mm\\:ss} / {totaltime:hh\\:mm\\:ss}");
            UIControl.Request(Element.TimeSlider, elapsedtime.TotalMilliseconds / totaltime.TotalMilliseconds * 100);
        }

        public static async Task<SubPlayer> CreateSubPlayerAsync(string path)
        {
            try
            {
                var subreader = GetSubReader(Path.GetExtension(path));
                var sublist = await subreader.ReadAsync(path);
                var filename = Path.GetFileName(path);

                return new SubPlayer(sublist, filename);
            }
            catch
            {
                UIControl.Request(Element.SubLabel, "An invalid subtitle file, please try again");
                return null;
            }
        }

        private static ISubReader GetSubReader(string ext)
        {
            return ext switch
            {
                ".srt" => new SrtReader(),
                ".ass" => new AssReader(),
                _ => throw new Exception("Unknown exception")
            };
        }
    }
}