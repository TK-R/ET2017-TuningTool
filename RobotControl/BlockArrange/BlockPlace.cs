using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RobotController.BlockArrange
{
    /// <summary>
    /// ブロック置き場ごとのステータスを示すクラス
    /// </summary>
    public class BlockPlace
    {
        private static Point[] PointArray = new Point[]
        {
            new Point(1, 9),
            new Point(82, 9),
            new Point(168, 9),
            new Point(254, 9),
            new Point(42, 34),
            new Point(126,34),
            new Point(210, 34),
            new Point(84, 58),
            new Point(169, 58),
            new Point(19, 77),
            new Point(236, 77),
            new Point(61, 100),
            new Point(108, 100),
            new Point(146, 100),
            new Point(192, 100)
        };

        /// <summary>
        /// 自身のフィールドをコピーしたブロック置き場ステータスを返す
        /// </summary>
        /// <returns>ブロック置き場</returns>
        public BlockPlace Clone()
        {
            return MemberwiseClone() as BlockPlace;
        }

        // 配置してあるブロックの情報
        public BlockColor OnBlockColor { set; get; }
        // 自身の番号
        public int No { set; get; }
        // 自身の色
        public BlockColor PlaceColor { set; get; }
        public override string ToString()
        {
            return "No:" + No + ",置き場色:" + PlaceColor.DisplayName() + ",ブロック:" + OnBlockColor.DisplayName();
        }

        /// <summary>
        /// 他のブロック置き場Noから自身との座標差を求めて返す
        /// </summary>
        /// <param name="no">フィールドNo</param>
        /// <returns>距離</returns>
        public double GetDistance(int no)
        {
            return GetDistance(PointArray[no]);
        }

        public Point GetPosition()
        {
            return PointArray[No];
        }

        /// <summary>
        /// あるポイントと自身のポイントとの距離を計算して返す
        /// </summary>
        /// <param name="dst">対象のポイント</param>
        /// <returns>距離</returns>
        public double GetDistance(Point dst)
        {
            var src = PointArray[No];

            return Math.Sqrt(Math.Pow(dst.X - src.X, 2) + Math.Pow(dst.Y - src.Y, 2));
        }
    }
}
