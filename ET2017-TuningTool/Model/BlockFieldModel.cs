using Microsoft.Practices.Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET2017_TuningTool.Model
{
    public class BlockFieldModel : BindableBase
    {
        public class Position
        {
            public int Black { get; set; } = 1;
            public int Red { get; set; } = 1;
            public int Yellow { get; set; } = 1;
            public int Blue { get; set; } = 1;
            public int Green { get; set; } = 1;
        }

        private int _YellowPosition = 1;
        public int YelloPosition
        {
            get { return _YellowPosition; }
            set { SetProperty(ref _YellowPosition, value); }
        }

        private int _RedPosition = 1;
        public int RedPosition
        {
            get { return _RedPosition; }
            set { SetProperty(ref _RedPosition, value); }
        }

        private int _BlackPosition = 1;
        public int BlackPosition
        {
            get { return _BlackPosition; }
            set { SetProperty(ref _BlackPosition, value); }
        }

        private int _BluePosition = 1;
        public int BluePosition
        {
            get { return _BluePosition; }
            set { SetProperty(ref _BluePosition, value); }
        }

        private int _GreenPosition = 1;
        public int GreenPosition
        {
            get { return _GreenPosition; }
            set { SetProperty(ref _GreenPosition, value); }
        }

        /// <summary>
        /// 初期配置と緑ブロックの位置から、ブロック位置を変動する
        /// </summary>
        /// <param name="code"></param>
        /// <param name="greenPosition"></param>
        public void SetBlockPosition(int code, int greenPosition)
        {
            var pos = AdjustBlockPositionField(GetPositionFromCode(code));

            // 値の更新
            BlackPosition = pos.Black;
            BluePosition = pos.Blue;
            RedPosition = pos.Red;
            YelloPosition = pos.Yellow;
            GreenPosition = greenPosition;
        }

        /// <summary>
        /// あるブロック置き場の状態を示す
        /// </summary>
        public enum BlockPositionStatus
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

        /// <summary>
        /// ブロック配置場所のステータス
        /// </summary>
        private BlockPositionStatus[] BlockPositionField = new BlockPositionStatus[15];

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
                new int[] { 2,  1,  1 },
                new int[] { 3,  2,  3 },
                new int[] { 4,  4,  5 },
                new int[] { 5,  6,  6 },
                new int[] { 6,  7,  7 },
                new int[] { 9,  8,  8 },
                new int[] { 10, 9,  10 },
                new int[] { 11, 10, 11 },
                new int[] { 12, 11, 13 },
                new int[] { 13, 12, 14 },
                new int[] { 15, 14, 15}
            };

            return new Position {
                Black = pos.Black,            // 黒ブロックは値のアジャストなし
                Red = offsetTable[pos.Red - 1][0],
                Yellow = offsetTable[pos.Yellow -1][1],
                Blue = offsetTable[pos.Blue -1][2]
            };
        }
    }
}
