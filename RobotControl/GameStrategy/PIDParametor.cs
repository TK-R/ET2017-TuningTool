using System;
using System.Collections.Generic;
using System.Text;

namespace RobotController.GameStrategy
{
    public class PIDParametor
    {
        /// <summary>
        /// ステートNo
        /// </summary>
        public int StateNo { get; set; }

        /// <summary>
        /// 比例ゲイン
        /// </summary>
        public float PGain { get; set; }

        /// <summary>
        /// 積分ゲイン
        /// </summary>
        public float IGain { set; get; }

        /// <summary>
        ///  微分ゲイン
        /// </summary>
        public float DGain { set; get; }


    }
}
