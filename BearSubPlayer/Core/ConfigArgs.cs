using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BearMLLib.Serialization;

namespace BearSubPlayer
{
    public record ConfigArgs
    {
        private double _mainOp = 0.5;
        private int _mainCol = 0;
        private int _fontSize = 32;
        private int _fontCol = 0;
        private double _fontOp = 0.5;
        private int _fontSn = 8;

        [Comment(" Main window opacity: 0.0 ~ 1.0")]
        public double MainOp { get => _mainOp; set => _mainOp = value >= 0.0 && value <= 1.0 ? value : _mainOp; }

        [Comment(" Main window color: white = 0, black = 1")]
        public int MainCol { get => _mainCol; set => _mainCol = value == 0 || value == 1 ? value : _mainCol; }

        [Comment(" Font size: 12 ~ 46")]
        public int FontSize { get => _fontSize; set => _fontSize = value >= 12 && value <= 46 ? value : _fontSize; }

        [Comment(" Font color: white = 0, black = 1")]
        public int FontCol { get => _fontCol; set => _fontCol = value == 0 || value == 1 ? value : _fontCol; }

        [Comment(" Font opacity: 0.0 ~ 1.0")]
        public double FontOp { get => _fontOp; set => _fontOp = value >= 0.0 && value <= 1.0 ? value : _fontOp; }

        [Comment(" Font shadow softness: 5 ~ 15")]
        public int FontSn { get => _fontSn; set => _fontSn = value >= 5 && value <= 15 ? value : _fontSn; }
    }
}
