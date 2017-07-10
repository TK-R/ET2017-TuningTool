using RobotController.BlockArrange;
using System;
using System.Collections.Generic;
using System.Text;

namespace RobotController.GameStrategy
{
    /// <summary>
    /// ブロック並べ戦略クラス
    /// </summary>
    public class ArrangeBlockStrategy : AbstractStrategy
    { 
        /// <summary>
        /// ブロック並べコマンドのリスト
        /// </summary>
        public List<BlockMoveCommand> CommandList { set; get; }


        public override void Run()
        {

        }
    }
}
