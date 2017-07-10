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
        /// 定周期で実行される
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// 戦略管理クラスへの参照
        /// </summary>
        public RobotController Context { set; get; }
    }
}
