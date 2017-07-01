using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET2017_TuningTool.Model
{
    /// <summary>
    /// ブロック運搬ルール作成
    /// </summary>
    public class BlockMoveRule
    {
        EV3Model RobotModel { get; set; }

        BlockFieldModel FieldModel { get; set; }
        
        int[] TargetNo { set; get; }

        public BlockMoveRule(EV3Model robot, BlockFieldModel field)
        {
            RobotModel = robot;
            FieldModel = field;

            // 運搬先になりうるブロックのリスト
            TargetNo = new int[]
            {
                5,
                7,
                8,
                12,
                13,
            };

        }

        /// <summary>
        /// ロボット位置から、次のブロックの位置を求める
        /// </summary>
        /// <param name="RobotPosition"></param>
        public void ChangeNextPosition()
        {
            // 移動していない（=ブロック色と置き場の色が異なる置き場を検出）
            var notMovedBlock = FieldModel.PlaceArray
                .Where(p => p.OnBlockColor != BlockColor.None)
                .Where(p => !TargetNo.Contains(p.No) || FieldModel.AvailableMoveBlock(p.OnBlockColor, p.PlaceColor));
            
            // 完了している場合
            if (notMovedBlock.Count() == 0)
                return;

            // 運搬先となりうる（上にブロックが置かれていない）置き場を列挙する    
            var MoveAvailablePlace = FieldModel.PlaceArray
                .Where(p => TargetNo.Contains(p.No))
                .Where(p => p.OnBlockColor == BlockColor.None);

            // 運搬可能なブロックを列挙する
            var MoveAvailableBlock =
                notMovedBlock.Where(b => 
                    MoveAvailablePlace.Where(p => !FieldModel.AvailableMoveBlock(p.PlaceColor,b.OnBlockColor)).Count() != 0);
            
            Place srcPlace ,dstPlace;
            if (MoveAvailableBlock.Count() == 0) // 運搬可能なブロックがなく、すでに埋まっているケース
            {
                // 赤ブロック置き場で、違うブロックが乗っている個所を取得
                var redP = FieldModel.PlaceArray
                                     .Where(p => p.PlaceColor == BlockColor.Red)
                                     .Where(p => p.OnBlockColor != BlockColor.Black)
                                     .FindMin(p => p.GetDistance(RobotModel.Position));
                if (redP != null)
                {
                    // 赤ブロック置き場を対象とする
                    srcPlace = redP;
                }
                else
                {
                    // 異なる色のブロックが乗っており、かつ最も近い個所を対象とする　
                    srcPlace = FieldModel.PlaceArray
                                         .Where(p => p.OnBlockColor != p.PlaceColor)
                                         .FindMin(p => p.GetDistance(RobotModel.Position));
                }

                // ブロックが置かれておらず、対象の置き場から最も近い場所を対象とする
                dstPlace = FieldModel.PlaceArray
                                  .Where(p => p.OnBlockColor == BlockColor.None)
                                  .FindMin(p => p.GetDistance(redP.No));

            }
            else // 運搬可能なブロックが存在するケース
            {
                // 移動していないブロックのうち、ロボットの距離が最も近いブロックを対象とする
                srcPlace = MoveAvailableBlock.FindMin(n => n.GetDistance(RobotModel.Position));
                dstPlace = MoveAvailablePlace.Where(p => TargetNo.Contains(p.No))
                                             .Where(p => !FieldModel.AvailableMoveBlock(p.PlaceColor, srcPlace.OnBlockColor))
                                             .FindMin(p => p.GetDistance(RobotModel.Position));
            }


            dstPlace.OnBlockColor = srcPlace.OnBlockColor;
            srcPlace.OnBlockColor = BlockColor.None;

            FieldModel.UpdatePositionFromPlace();
            RobotModel.Position = dstPlace.GetPosition();
        }

    }
}
