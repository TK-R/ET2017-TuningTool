using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ET2017_TuningTool.Model
{
    /// <summary>
    /// 一回のブロック運搬を定義するクラス
    /// </summary>
    public class BlockMoveCommand
    {
        /// <summary>
        /// 運搬対象ブロックの色
        /// </summary>
        public BlockColor TargetBlockColor { set; get; }

        /// <summary>
        /// 運搬元ポリゴンブロック置き場
        /// </summary>
        public int SourceBlockPlaceNo { set; get; }
        
        /// <summary>
        /// 運搬先ポリゴンブロック置き場
        /// </summary>
        public int DestinationBlockPlaceNo { set; get; }

        /// <summary>
        /// ブロック運搬経路
        /// </summary>
        public Way[] BlockMoveWay { set; get; }

        /// <summary>
        /// 運搬元ブロック置き場までのアプローチ
        /// </summary>
        public Way[] ApproachWay { set; get; }

    }

    /// <summary>
    /// ブロック運搬経路の一区間を表すクラス
    /// </summary>
    public class Way
    { 

        /// <summary>
        /// ウェイポイントの経由のみならTrue,以降ライントレースならFalse
        /// </summary>
        public bool IsWayPoint { set; get; }
        
        /// <summary>
        /// ウェイポイントの座標
        /// </summary>
        public int WayPointNo { set; get; }

        public override string ToString()
        {
            return "No: " + WayPointNo;
        }
    }
}
