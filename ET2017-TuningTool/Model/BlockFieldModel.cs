using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Windows;
using System.Linq;

namespace ET2017_TuningTool.Model
{
    public class BlockFieldModel : BindableBase
    {
        #region ブロック位置
        private int _YellowPosition = 0;
        /// <summary>
        /// 黄色ブロックの位置
        /// </summary>
        public int YellowPosition
        {
            get { return _YellowPosition; }
            set { SetProperty(ref _YellowPosition, value); }
        }

        private int _RedPosition = 1;
        /// <summary>
        /// 赤ブロックの位置
        /// </summary>
        public int RedPosition
        {
            get { return _RedPosition; }
            set { SetProperty(ref _RedPosition, value); }
        }

        private int _BlackPosition = 2;
        /// <summary>
        /// 黒ブロックの位置
        /// </summary>
        public int BlackPosition
        {
            get { return _BlackPosition; }
            set { SetProperty(ref _BlackPosition, value); }
        }

        private int _BluePosition = 3;
        /// <summary>
        /// 青ブロックの位置
        /// </summary>
        public int BluePosition
        {
            get { return _BluePosition; }
            set { SetProperty(ref _BluePosition, value); }
        }

        private int _GreenPosition = 4;
        /// <summary>
        /// 緑ブロックの位置
        /// </summary>
        public int GreenPosition
        {
            get { return _GreenPosition; }
            set { SetProperty(ref _GreenPosition, value); }
        }

        private Point[] _ApproachWayPointArray = new Point[0];
        public Point[] ApproachWayPointArray
        {
            get { return _ApproachWayPointArray; }
            set { SetProperty(ref _ApproachWayPointArray, value); }
        }

        private Point[] _MoveBlockWayArray = new Point[0];
        public Point[] MoveBlockWayPointArray
        {
            get { return _ApproachWayPointArray; }
            set { SetProperty(ref _ApproachWayPointArray, value); }
        }


        #endregion

        public BlockPlace[] PlaceArray { private set; get; }

        public BlockFieldModel()
        {
            // フィールド情報体を初期化
            PlaceArray = Enumerable.Range(0, 15).Select(r => new BlockPlace { No = r }).ToArray();

            PlaceArray = new BlockPlace[]
            {
                new BlockPlace{ No = 0, PlaceColor = BlockColor.Red },
                new BlockPlace{ No = 1, PlaceColor = BlockColor.Blue },
                new BlockPlace{ No = 2, PlaceColor = BlockColor.Yellow },
                new BlockPlace{ No = 3, PlaceColor = BlockColor.Green },
                new BlockPlace{ No = 4, PlaceColor = BlockColor.Yellow },
                new BlockPlace{ No = 5, PlaceColor = BlockColor.Green },
                new BlockPlace{ No = 6, PlaceColor = BlockColor.Red},
                new BlockPlace{ No = 7, PlaceColor = BlockColor.Red },
                new BlockPlace{ No = 8, PlaceColor = BlockColor.Blue},
                new BlockPlace{ No = 9, PlaceColor = BlockColor.Green },
                new BlockPlace{ No = 10, PlaceColor = BlockColor.Green},
                new BlockPlace{ No = 11, PlaceColor = BlockColor.Green},
                new BlockPlace{ No = 12, PlaceColor = BlockColor.Yellow},
                new BlockPlace{ No = 13, PlaceColor = BlockColor.Red},
                new BlockPlace{ No = 14, PlaceColor = BlockColor.Yellow },
            };
        }

        /// <summary>
        /// 初期配置と緑ブロックの位置から、ブロック位置を変動する
        /// </summary>
        /// <param name="code"></param>
        /// <param name="greenPosition"></param>
        public void SetBlockPosition(int code, int greenPosition)
        {
            var pos = AdjustBlockPositionField(GetPositionFromCode(code));
            pos.Green = greenPosition;

            // 値の更新
            BlackPosition = pos.Black;
            BluePosition = pos.Blue;
            RedPosition = pos.Red;
            YellowPosition = pos.Yellow;
            GreenPosition = pos.Green;

            foreach (var p in PlaceArray)
                p.OnBlockColor = BlockColor.None;

            PlaceArray[BlackPosition].OnBlockColor = BlockColor.Black;
            PlaceArray[BluePosition].OnBlockColor = BlockColor.Blue;
            PlaceArray[RedPosition].OnBlockColor = BlockColor.Red;
            PlaceArray[YellowPosition].OnBlockColor = BlockColor.Yellow;
            PlaceArray[GreenPosition].OnBlockColor = BlockColor.Green;
        }


        public void UpdatePositionFromPlace()
        {
            BlackPosition = PlaceArray.Where(p => p.OnBlockColor == BlockColor.Black).FirstOrDefault().No;
            BluePosition = PlaceArray.Where(p => p.OnBlockColor == BlockColor.Blue).FirstOrDefault().No;
            RedPosition = PlaceArray.Where(p => p.OnBlockColor == BlockColor.Red).FirstOrDefault().No;
            YellowPosition = PlaceArray.Where(p => p.OnBlockColor == BlockColor.Yellow).FirstOrDefault().No;
            GreenPosition = PlaceArray.Where(p => p.OnBlockColor == BlockColor.Green).FirstOrDefault().No;
        }
    
        /// <summary>
        /// 与えられた初期位置コードから、黒・赤・黄・青の値を求める
        /// </summary>
        /// <param name="code">初期位置コード</param>
        /// <return>ポジションクラスのインスタンス</return>
        public static Position GetPositionFromCode(int code)
        {
            // ブロックの個数繰り返し
            const int state = 3;
            // 初期位置コードのエンコード/デコードに使用する係数
            const int k = 11;

            int[] tmpArray = new int[4];

            // 再帰呼び出しにより初期位置コードを減算しながらそれぞれの項の値を一時配列に格納する
            void func(int c, int s)
            {
                // 与えられたコードより、予想された配置場所のほうが大きくなる数の最小値を取得する。
                var pos = Enumerable.Range(1, 15).Where(t => c < t * Math.Pow(k, s)).Min();
                tmpArray[state - s] = pos;

                // 最終項なら終了、そうでなければ初期位置コードを減算して次の値を格納する
                if (s == 0) return;
                else func(c - (int)((pos - 1) * Math.Pow(k, s)), --s);

            };

            func(code, state);

            return new Position
            {
                Black = tmpArray[0],
                Red = tmpArray[1],
                Yellow = tmpArray[2],
                Blue = tmpArray[3],
            };
        }

        /// <summary>
        /// ブロックの位置にオフセット値を加えた値を返す
        /// </summary>
        /// <param name="pos">オフセットを加味しないポジション</param>
        /// <returns>オフセットを加味したポジション</returns>
        public static Position AdjustBlockPositionField(Position pos)
        {
            int[][] offsetTable = new int[][] {
                //         赤, 黄, 青
                new int[] { 1,  0,  0 },
                new int[] { 2,  1,  2 },
                new int[] { 3,  3,  4 },
                new int[] { 4,  5,  5 },
                new int[] { 5,  6,  6 },
                new int[] { 8,  7,  7 },
                new int[] { 9,  8,  9 },
                new int[] { 10, 9,  10 },
                new int[] { 11, 10, 12 },
                new int[] { 12, 11, 13 },
                new int[] { 14, 13, 14 }
            };

            return new Position {
                Black = pos.Black - 1,            // 黒ブロックは一律-1
                Red = offsetTable[pos.Red - 1][0],
                Yellow = offsetTable[pos.Yellow -1][1],
                Blue = offsetTable[pos.Blue -1][2]
            };
        }

    }

    /// <summary>
    /// ブロック配置場所を示すクラス
    /// </summary>
    public class Position
    {
        public int Black { get; set; } = 1;
        public int Red { get; set; } = 1;
        public int Yellow { get; set; } = 1;
        public int Blue { get; set; } = 1;
        public int Green { get; set; } = 1;
    }

    /// <summary>
    /// あるブロック置き場の状態を示す列挙体
    /// </summary>
    public enum BlockColor
    {
        /// <summary>
        /// 配置なし
        /// </summary>
        None = 0,
        /// <summary>
        /// 黒ブロックが配置
        /// </summary>
        Black,
        /// <summary>
        /// 赤ブロックが配置
        /// </summary>
        Red,
        /// <summary>
        /// 黄ブロックが配置
        /// </summary>
        Yellow,
        /// <summary>
        /// 青ブロックが配置
        /// </summary>
        Blue,
        /// <summary>
        /// 緑ブロックが配置
        /// </summary>
        Green,
    };

    static class EnumHelper
    {
        public static string DisplayName(this BlockColor c)
        {
            string[] names = { "なし", "黒", "赤", "黄", "青", "緑" };
            return names[(int)c];
        }
    }
}
