using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET2017_TuningTool
{
    public class EV3Model : BindableBase
    {

        public Point _Position = new Point(5, 90);
        public Point Position { set => SetProperty(ref _Position, value); get => _Position; }

        public void ResetPosition()
        {
            Position = new Point(0, 90);
        }

        /// <summary>
        /// 画像のサイズを加味した、中心点の座標を返す
        /// </summary>
        /// <returns></returns>
        public Point GetPosition()
        {
            return new Point { X = Position.X + 15, Y = Position.Y + 15 };
        }
    }
}
