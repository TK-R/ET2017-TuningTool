using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET2017_TuningTool.Model
{
    public class BlockMoveRule
    {
        EV3Model RobotModel { get; set; }

        BlockFieldModel FieldModel { get; set; }
        
        public BlockMoveRule(EV3Model robot, BlockFieldModel field)
        {
            RobotModel = robot;
            FieldModel = field;
        }

        /// <summary>
        /// ロボット位置から、次のブロックの位置を求める
        /// </summary>
        /// <param name="RobotPosition"></param>
        public void ChangeNextPosition()
        {
            // 移動していない（=ブロック色と置き場の色が異なる置き場を検出）
            var notMovedBlock = FieldModel. PlaceArray.Where(p => FieldModel.IsToMoveBlock(p.OnBlockColor, p.PlaceColor));

            if (notMovedBlock.Count() == 0)
                return;

            // 移動していないブロックのうち、ロボットの距離が最も近いブロックを対象とする
            var srcPlace = notMovedBlock.FindMin(n => n.GetDistance(RobotModel.Position));

            int dstNo = 0;
            switch (srcPlace.OnBlockColor)
            {
                case BlockColor.Black:
                    dstNo = 7;
                    break;
                case BlockColor.Blue:
                    dstNo = 8;
                    break;
                case BlockColor.Red:
                    dstNo = 13;
                    break;
                case BlockColor.Yellow:
                    dstNo = 12;
                    break;
                case BlockColor.Green:
                    dstNo = 5;
                    break;
                default:
                    return;
            }
            var dstPlace = FieldModel.PlaceArray.Where(p => p.OnBlockColor == BlockColor.None)
                                     .Where(p => p.No == dstNo).First();

            dstPlace.OnBlockColor = srcPlace.OnBlockColor;
            srcPlace.OnBlockColor = BlockColor.None;

            FieldModel.UpdatePositionFromPlace();
            RobotModel.Position = dstPlace.GetPosition();
        }

    }
}
