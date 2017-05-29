using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SerialLibrary;

namespace ET2017_TuningTool.Model.GraphModel
{
    public abstract class AbstractGraphModel
    {
        /// <summary>
        /// 要素の名称
        /// </summary>
        public string Name { internal set; get; }
        
        /// <summary>
        /// 単位
        /// </summary>
        public string Unit { internal set; get; }

        /// <summary>
        /// 最大値
        /// </summary>
        public double MaxValue { internal set; get; }
            
        /// <summary>
        /// 最小値
        /// </summary>
        public double MinValue { internal set; get; }

        /// <summary>
        /// 受信した値から、最小値と最大値の範囲で0-100のグラフ値に変換した値を返す
        /// </summary>
        /// <typeparam name="StructT">構造体の型</typeparam>
        /// <param name="dataStruct">受信したデータ領域の構造体</param>
        /// <returns></returns>
        public double GetValue<StructT>(StructT dataStruct)
        {
            // グラフ上の最大と最小値を定義
            const int gMax = 100, gMin = 0;

            // 電文の生データを取得
            var rawValue = GetStructValue(dataStruct);

            // (グラフ最大値 - グラフ最小値) ÷（最大値 - 最小値）*（物理量 - 最小値）
            return  (gMax - gMin) / (MaxValue - MinValue) * (rawValue - MinValue);
        }

        /// <summary>
        /// 構造体から任意のメンバの値を返す。このメソッドをサブクラスが実装すること。
        /// </summary>
        /// <typeparam name="StructT">構造体の型</typeparam>
        /// <param name="dataStruct">受信したデータ領域の構造体</param>
        /// <returns></returns>
        internal abstract double GetStructValue(object dataStruct); 
    }
}
