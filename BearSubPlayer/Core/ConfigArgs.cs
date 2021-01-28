using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BearSubPlayer
{
    public record ConfigArgs
    {
        public double MainOp { get; set; }
        public int MainCol { get; set; }
        public int FontSize { get; set; }
        public int FontCol { get; set; }
        public double FontOp { get; set; }
        public int FontSn { get; set; }
    }
}
