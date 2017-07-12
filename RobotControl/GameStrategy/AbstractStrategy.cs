using RobotController.RobotStatus;
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
        /// 定周期で実行する処理
        /// </summary>
        public abstract void Run();
        
        /// <summary>
        /// 戦略管理クラスへの参照
        /// </summary>
        public RobotControl Context { set; get; }
    }
}
