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
        /// 運搬元ブロック置き場までのアプローチ
        /// </summary>
        public Way[] ApproachWay { set; get; }

        /// <summary>
        /// ブロック運搬経路
        /// </summary>
        public Way[] BlockMoveWay { set; get; }

        /// <summary>
        /// ブロック運搬コマンドを電文フォーマットに合わせてシリアライズする
        /// </summary>
        /// <returns>電文フォーマットのbyte配列</returns>
        public byte[] Serialize()
        {
            var list = new List<byte> {
                (byte)TargetBlockColor,         // 運搬対象ブロック色
                (byte)SourceBlockPlaceNo,       // 運搬元ブロック置き場
                (byte)DestinationBlockPlaceNo,  // 運搬先ブロック置き場
                (byte)ApproachWay.Count()       // ブロック確保ウェイポイントの数
            };
            
            // ブロック確保ウェイポイントNoを追加
            list.AddRange(ApproachWay.Select(a => (byte)a.WayPointNo));

            // ブロック運搬経路が存在する場合（最終コマンドではない）場合
            if (BlockMoveWay != null)
            {
                // ブロック運搬ウェイポイントの数を追加
                list.Add((byte)BlockMoveWay.Count());

                // ブロック運搬ウェイポイントNoを追加
                list.AddRange(BlockMoveWay.Select(b => (byte)b.WayPointNo));
            }
            return list.ToArray();
        }
    }

    /// <summary>
    /// ブロック運搬経路の一区間を表すクラス
    /// </summary>
    public class Way
    {   
        /// <summary>
        /// ウェイポイント番号
        /// </summary>
        public int WayPointNo { set; get; }

        public override string ToString()
        {
            return "No: " + WayPointNo;
        }
    }
}
