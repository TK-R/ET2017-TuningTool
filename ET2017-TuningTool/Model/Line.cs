using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ET2017_TuningTool.Model
{
    public class Line
    {
        /// <summary>
        /// No（0始まり）
        /// </summary>
        public int No { set; get; }

        /// <summary>
        /// 始点ブロック置き場No
        /// </summary>
        public int StartPlaceNo { set; get; }

        /// <summary>
        /// 終点ブロック置き場No
        /// </summary>
        public int EndPlaceNo { set; get; }

        /// <summary>
        /// ラインの中点であるウェイポイントの座標
        /// </summary>
        public Point WayPoint { set; get; }

        /// <summary>
        /// あるラインのウェイポイントとの座標差を求める
        /// </summary>
        /// <param name="dst"></param>
        /// <returns></returns>
        public double GetDistance(Line dstLine)
        {
            var src = this.WayPoint;
            var dst = dstLine.WayPoint;

            return Math.Sqrt(Math.Pow(dst.X - src.X, 2) + Math.Pow(dst.Y - src.Y, 2));
        }

        /// <summary>
        /// 隣接する（通過WP対象となる）ラインNo
        /// </summary>
        public int[] NearLineNo { set; get; }
    }
}
