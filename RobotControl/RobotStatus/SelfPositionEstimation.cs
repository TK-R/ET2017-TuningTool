using System;
using System.Windows;
using System.Linq;


namespace RobotController.RobotStatus
{
    /// <summary>
    /// 自己位置推定クラス
    /// </summary>
    public class SelfPositionEstimation
    {
        /// <summary>
        /// タイヤの直径（mm）
        /// </summary>
        public readonly double TireDiameter = 80.0;

        /// <summary>
        /// フィールド上辺を0度とした、時計回りのロボットの角度
        /// </summary>
        public double RobotAngle { private set; get; }

        /// <summary>
        /// ロボットの位置座標（mm）
        /// </summary>
        public Point RobotPosition { private set; get; }
        
        /// <summary>
        /// 初期座標を指定して、ロボットの位置座標を更新する
        /// </summary>
        /// <param name="p"></param>
        public SelfPositionEstimation(Point p)
        {

        }
        
        /// <summary>
        /// エンコーダの値をもとに自己位置と角度を更新する
        /// </summary>
        /// <param name="leftAngle"></param>
        /// <param name="rightAngle"></param>
        public void Update (int leftAngle, int rightAngle)
        {

        }
    }
}
