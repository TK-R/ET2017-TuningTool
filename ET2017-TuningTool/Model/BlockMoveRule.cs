using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ET2017_TuningTool.Model
{
    /// <summary>
    /// ブロック運搬ルール作成
    /// </summary>
    public class BlockMoveRule
    {
        /// <summary>
        /// ロボット上から【コピーした】座標
        /// </summary>
        Point RobotPosition { set; get; }

        /// <summary>
        /// フィールド情報から【コピーした】置き場情報の配列
        /// </summary>
        Place[] PlaceArray { get; set; }

        /// <summary>
        /// ブロック運搬コマンドのリスト
        /// </summary>
        List<BlockMoveCommand> BlockMoveCommandList = new List<BlockMoveCommand>();

        /// <summary>
        /// ウェイポイントの座標を保持する構造体
        /// </summary>
        private static readonly Point[] WayPointArray =
        {
            new Point(49,9),
            new Point(134,9),
            new Point(219,9),
            new Point(31,25),
            new Point(74,25),
            new Point(117,25),
            new Point(160,25),
            new Point(203,25),
            new Point(246,25),
            new Point(18,45),
            new Point(71,45),
            new Point(114,45),
            new Point(157,45),
            new Point(200,45),
            new Point(254,45),
            new Point(39,57),
            new Point(232,57),
            new Point(84,79),
            new Point(107,79),
            new Point(168,79),
            new Point(191,79),
            new Point(49,91),
            new Point(222,91),
            new Point(96,103),
            new Point(136,103),
            new Point(179,103)
        };
        
        /// <summary>
        /// 運搬先になりうるブロックのリスト
        /// </summary>
        int[] TargetNo { set; get; } = new int[] { 5, 7, 8, 12, 13 };


        public BlockMoveRule(EV3Model robot, BlockFieldModel field)
        {
            // ブロック置き場情報をコピーして保持
            // この配列への値操作はフィールドに影響を与えないため、計算に使用できる
            PlaceArray = field.PlaceArray.Select(p => p.Clone()).ToArray();

            RobotPosition = robot.Position;

            // ブロック運搬コマンドを生成する
            while (true)
            {
                var command = CalculateBlockMoveCommand();
                if (command == null)
                    break;

                BlockMoveCommandList.Add(command);
            }
        }

        /// <summary>
        /// ブロック運搬コマンドを生成して返す
        /// 完了済みの場合には、nullを返す
        /// </summary>
        /// <returns></returns>
        private BlockMoveCommand CalculateBlockMoveCommand()
        {
            // 移動していない（=ブロック色と置き場の色が異なる置き場を検出）
            var notMovedBlock = PlaceArray
                .Where(p => p.OnBlockColor != BlockColor.None)
                .Where(p => !TargetNo.Contains(p.No) || AvailableMoveBlock(p.OnBlockColor, p.PlaceColor));

            // 完了している場合
            if (notMovedBlock.Count() == 0)
                return null;

            // 運搬先となりうる（上にブロックが置かれていない）置き場を列挙する    
            var MoveAvailablePlace = PlaceArray
                .Where(p => TargetNo.Contains(p.No))
                .Where(p => p.OnBlockColor == BlockColor.None);

            // 運搬可能なブロックを列挙する
            var MoveAvailableBlock =
                notMovedBlock.Where(b =>
                    MoveAvailablePlace.Where(p => !AvailableMoveBlock(p.PlaceColor, b.OnBlockColor)).Count() != 0);

            Place srcPlace, dstPlace;
            if (MoveAvailableBlock.Count() == 0) // 運搬可能なブロックがなく、すでに埋まっているケース
            {
                // 赤ブロック置き場で、違うブロックが乗っている個所を取得
                var redP = PlaceArray
                                     .Where(p => p.PlaceColor == BlockColor.Red)
                                     .Where(p => p.OnBlockColor != BlockColor.Black)
                                     .FindMin(p => p.GetDistance(RobotPosition));
                if (redP != null)
                {
                    // 赤ブロック置き場を対象とする
                    srcPlace = redP;
                }
                else
                {
                    // 異なる色のブロックが乗っており、かつ最も近い個所を対象とする　
                    srcPlace = PlaceArray
                                         .Where(p => p.OnBlockColor != p.PlaceColor)
                                         .FindMin(p => p.GetDistance(RobotPosition));
                }

                // ブロックが置かれておらず、対象の置き場から最も近い場所を対象とする
                dstPlace = PlaceArray
                                  .Where(p => p.OnBlockColor == BlockColor.None)
                                  .FindMin(p => p.GetDistance(redP.No));

            }
            else // 運搬可能なブロックが存在するケース
            {
                // 移動していないブロックのうち、ロボットの距離が最も近いブロックを対象とする
                srcPlace = MoveAvailableBlock.FindMin(n => n.GetDistance(RobotPosition));
                dstPlace = MoveAvailablePlace.Where(p => TargetNo.Contains(p.No))
                                             .Where(p => !AvailableMoveBlock(p.PlaceColor, srcPlace.OnBlockColor))
                                             .FindMin(p => p.GetDistance(RobotPosition));
            }

            dstPlace.OnBlockColor = srcPlace.OnBlockColor;
            srcPlace.OnBlockColor = BlockColor.None;

            var command = new BlockMoveCommand {
                SourceBlockPlaceNo = srcPlace.No,
                DestinationBlockPlaceNo = dstPlace.No,
                TargetBlockColor = dstPlace.OnBlockColor,
                // 仮
                //                ApproachWay = Enumerable.Range(0, 26).Select(n => new Way { WayPointNo = n}).ToArray()
                ApproachWay = new Way[] {

                },

                BlockMoveWay = new Way[] {
                    new Way{ WayPointNo = 17 },

                }


            };
            
            return command;
        }

        /// <summary>
        /// 移動する必要があるor移動可能なブロックならTrueを返す
        /// </summary>
        /// <param name="block">ブロック色</param>
        /// <param name="place">移動先,または既に置かれている置き場色</param>
        /// <returns>移動可能ならTrue</returns>
        public bool AvailableMoveBlock(BlockColor block, BlockColor place)
        {
            // 置かれていない場合にはOK
            if (block == BlockColor.None)
                return false;

            // 一致している場合にはOK
            if (block == place)
                return false;
            if (block == BlockColor.Red && place == BlockColor.Black)
                return false;
            if (block == BlockColor.Black && place == BlockColor.Red)
                return false;

            return true;
        }


        public void Update(EV3Model robot, BlockFieldModel field)
        {
            // コマンドが既にない場合には何もしない
            if (BlockMoveCommandList.Count() == 0)
                return;

            var command = BlockMoveCommandList.First();
            var srcNo = command.SourceBlockPlaceNo;
            var dstNo = command.DestinationBlockPlaceNo;


            // 運搬元ブロックの座標を取得して、中心点に補正
            var srcPos = field.PlaceArray[srcNo].GetPosition();
            srcPos.X += 10;

            // 運搬先ブロックの座標を取得して、中心点に補正
            var dstPos = field.PlaceArray[dstNo].GetPosition();
            dstPos.X += 10;

            // 接近時のパスをアップデート
            var approach = command.ApproachWay.Select(t => WayPointArray[t.WayPointNo]).ToList();
            //ロボットの中心点座標を追加
            approach.Insert(0, new Point { X = robot.Position.X + 15, Y = robot.Position.Y + 15 });
            // 運搬元ブロック置き場の座標を追加
            approach.Add(srcPos);


            // 運搬時のパスをアップデート
            var moveBlock = command.BlockMoveWay.Select(t => WayPointArray[t.WayPointNo]).ToList();
            // 運搬元ブロック置場の座標を追加
            moveBlock.Insert(0, srcPos);
            // 運搬先ブロック置場の座標を追加
            moveBlock.Add(dstPos);


            // ブロック置き場情報を更新
            field.PlaceArray[dstNo].OnBlockColor = command.TargetBlockColor;
            field.PlaceArray[srcNo].OnBlockColor = BlockColor.None;
            field.UpdatePositionFromPlace();

            // 走行体情報を更新
            robot.Position = field.PlaceArray[dstNo].GetPosition();
            
            // 経路情報を更新
            field.ApproachWayPointArray = approach.ToArray();
            field.MoveBlockWayPointArray = moveBlock.ToArray();




            BlockMoveCommandList.Remove(command);
        }
    }
}
