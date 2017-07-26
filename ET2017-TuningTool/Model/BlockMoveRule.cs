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
        BlockPlace[] PlaceArray { get; set; }

        /// <summary>
        /// ブロック運搬コマンドのリスト
        /// </summary>
        List<BlockMoveCommand> BlockMoveCommandList = new List<BlockMoveCommand>();

        /// <summary>
        /// ライン情報の配列
        /// </summary>
        public static Line[] LineArray { get; set; } =
        {
            new Line{ No = 0, StartPlaceNo = 0, EndPlaceNo = 1, WayPoint = new Point(49,9), NearLineNo = new int[] {3, 4} },
            new Line{ No = 1, StartPlaceNo = 1, EndPlaceNo = 2, WayPoint = new Point(134,9), NearLineNo = new int[] {5, 6} },
            new Line{ No = 2, StartPlaceNo = 2, EndPlaceNo = 3, WayPoint = new Point(219,9), NearLineNo = new int[] {7, 8} },
            new Line{ No = 3, StartPlaceNo = 0, EndPlaceNo = 4, WayPoint = new Point(31,25), NearLineNo = new int[] {0, 4, 9} },
            new Line{ No = 4, StartPlaceNo = 1, EndPlaceNo = 4, WayPoint = new Point(74,25), NearLineNo = new int[] {0, 3, 5, 10, 11} },
            new Line{ No = 5, StartPlaceNo = 1, EndPlaceNo = 5, WayPoint = new Point(117,25), NearLineNo = new int[] {1, 4, 6, 10, 11} },
            new Line{ No = 6, StartPlaceNo = 2, EndPlaceNo = 5, WayPoint = new Point(160,25), NearLineNo = new int[] {1, 5, 7, 12, 13} },
            new Line{ No = 7, StartPlaceNo = 2, EndPlaceNo = 6, WayPoint = new Point(203,25), NearLineNo = new int[] {2, 6, 8, 12, 13} },
            new Line{ No = 8, StartPlaceNo = 3, EndPlaceNo = 6, WayPoint = new Point(246,25), NearLineNo = new int[] {2, 7,14 } },
            new Line{ No = 9, StartPlaceNo = 0, EndPlaceNo = 9, WayPoint = new Point(18,45) ,NearLineNo = new int[] {3, 15} },
            new Line{ No = 10, StartPlaceNo = 4, EndPlaceNo = 7, WayPoint = new Point(71,45) ,NearLineNo = new int[] {4, 5, 9, 11, 15, 17} },
            new Line{ No = 11, StartPlaceNo = 5, EndPlaceNo = 7, WayPoint = new Point(114,45) ,NearLineNo = new int[] {4, 5, 10, 12, 18, 19, 24} },
            new Line{ No = 12, StartPlaceNo = 5, EndPlaceNo = 8, WayPoint = new Point(157,45) ,NearLineNo = new int[] {6, 7, 11, 13, 18, 19, 24} },
            new Line{ No = 13, StartPlaceNo = 6, EndPlaceNo = 8, WayPoint = new Point(200,45) ,NearLineNo = new int[] {6, 7, 12, 14, 16, 20, 22} },
            new Line{ No = 14, StartPlaceNo = 3, EndPlaceNo = 10, WayPoint = new Point(254,45) ,NearLineNo = new int[] {8, 16} },
            new Line{ No = 15, StartPlaceNo = 4, EndPlaceNo = 9, WayPoint = new Point(39,57) ,NearLineNo = new int[] {3, 9, 10, 17, 21} },
            new Line{ No = 16, StartPlaceNo = 6, EndPlaceNo = 10, WayPoint = new Point(232,57) ,NearLineNo = new int[] {8, 13, 14, 20, 22} },
            new Line{ No = 17, StartPlaceNo = 7, EndPlaceNo = 11, WayPoint = new Point(84,79) ,NearLineNo = new int[] {10, 15, 18, 21, 23} },
            new Line{ No = 18, StartPlaceNo = 7, EndPlaceNo = 12, WayPoint = new Point(107,79) ,NearLineNo = new int[] {11, 12, 17, 19, 23, 24} },
            new Line{ No = 19, StartPlaceNo = 8, EndPlaceNo = 13, WayPoint = new Point(168,79) ,NearLineNo = new int[] {11, 12, 18, 20, 24, 25} },
            new Line{ No = 20, StartPlaceNo = 8, EndPlaceNo = 14, WayPoint = new Point(191,79) ,NearLineNo = new int[] {13, 16, 19, 12, 25} },
            new Line{ No = 21, StartPlaceNo = 9, EndPlaceNo = 11, WayPoint = new Point(49,91) ,NearLineNo = new int[] {10, 15, 17} },
            new Line{ No = 22, StartPlaceNo = 10, EndPlaceNo = 14, WayPoint = new Point(222,91), NearLineNo = new int[] {13, 16, 20} },
            new Line{ No = 23, StartPlaceNo = 11, EndPlaceNo = 12, WayPoint = new Point(96,103), NearLineNo = new int[] {17,18} },
            new Line{ No = 24, StartPlaceNo = 12, EndPlaceNo = 13, WayPoint = new Point(136,103), NearLineNo = new int[] {11, 12, 18, 19} },
            new Line{ No = 25, StartPlaceNo = 13, EndPlaceNo = 14, WayPoint = new Point(179,103), NearLineNo = new int[] {19, 20}},
            new Line{ No = 26, StartPlaceNo = 9, EndPlaceNo = 9, WayPoint = new Point (18,90), NearLineNo = new int[] { 9,21}}
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

            RobotPosition = robot.GetPosition();

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

            BlockPlace srcPlace, dstPlace;
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

            var di = new Dijkstra(LineArray);


            var startWayPoint = LineArray.FindMin(l => l.GetDistance(RobotPosition) + l.GetDistance(srcPlace.GetPosition())).No;   // ロボット位置+始点の位置から一番近いウェイポイント

            var approachWayPoint = LineArray.Where(l => l.StartPlaceNo == srcPlace.No || // 始点か終点が運搬開始ブロック置き場に接している
                                                   l.EndPlaceNo == srcPlace.No)
                                       .FindMin(l => l.GetDistance(RobotPosition)).No;     // そのうち、最もロボットに近い点

            var moveStartWayPoint = LineArray.Where(l => l.StartPlaceNo == srcPlace.No || // 始点か終点が運搬開始ブロック置き場に接している
                                                   l.EndPlaceNo == srcPlace.No) 
                                             .FindMin(l => l.GetDistance(dstPlace.GetPosition())).No; // そのうち、最も終点に近い点

            var dstWayPoint = LineArray.Where(l => l.StartPlaceNo == dstPlace.No || // 始点か終点が運搬開始ブロック置き場に接している
                                                   l.EndPlaceNo == dstPlace.No).First().No;

            
            var approach = di.GetRouteNodeNo(startWayPoint, approachWayPoint);
            var blockMove = di.GetRouteNodeNo(moveStartWayPoint, dstWayPoint);
            
            var command = new BlockMoveCommand {
                SourceBlockPlaceNo = srcPlace.No,
                DestinationBlockPlaceNo = dstPlace.No,
                TargetBlockColor = dstPlace.OnBlockColor,
                ApproachWay = approach.Select(a => new Way { WayPointNo = a }).ToArray(),
                BlockMoveWay = blockMove.Select(a => new Way { WayPointNo = a }).ToArray(),
                
            };

            // 運搬後のポジションに更新
            RobotPosition = dstPlace.GetPosition();

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
        
        /// <summary>
        /// 情報更新処理
        /// </summary>
        /// <param name="robot">走行体</param>
        /// <param name="field">フィールド情報</param>
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
            var approach = command.ApproachWay.Select(t => LineArray[t.WayPointNo].WayPoint).ToList();
            //ロボットの中心点座標を追加
            approach.Insert(0, robot.GetPosition());
            // 運搬元ブロック置き場の座標を追加
            approach.Add(srcPos);
            
            // 運搬時のパスをアップデート
            var moveBlock = command.BlockMoveWay.Select(t => LineArray[t.WayPointNo].WayPoint).ToList();
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

        public byte[] Serialize()
        {
            // 初期値としてブロック運搬コマンド数を格納
            var list = new List<byte> { (byte)BlockMoveCommandList.Count() };

            // 順番にブロック運搬コマンドをシリアライズして格納
            foreach(var command in BlockMoveCommandList)
            {
                list.AddRange(command.Serialize());
            }

            return list.ToArray();
        }
    }
}
