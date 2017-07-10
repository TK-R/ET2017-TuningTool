using RobotControl.RobotStatus;
using System;
using System.Collections.Generic;
using System.Text;

namespace RobotController.GameStrategy
{
    /// <summary>
    /// 戦略クラス
    /// </summary>
    public abstract class AbstractStrategy
    {
        /// <summary>
        /// 入力信号情報への参照
        /// </summary>
        public InputParameter InputData { private set; get; }

        /// <summary>
        /// 出力信号情報への参照
        /// </summary>
        public OutputParameter OutputData { private set; get; }

        /// <summary>
        /// 定周期で実行する処理
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// 戦略管理クラスへの参照
        /// </summary>
        public RobotController Context { set; get; }
    }
}
