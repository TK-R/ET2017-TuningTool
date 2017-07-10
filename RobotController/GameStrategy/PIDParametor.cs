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
        int StateNo { get; set; }

        /// <summary>
        /// 比例ゲイン
        /// </summary>
        float PGain { get; set; }

        /// <summary>
        /// 積分ゲイン
        /// </summary>
        float IGain { set; get; }

        /// <summary>
        ///  微分ゲイン
        /// </summary>
        float DGain { set; get; }


    }
}
