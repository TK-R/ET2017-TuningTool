using System;
using System.Collections.Generic;
using System.Text;

namespace RobotController.GameStrategy
{
    /// <summary>
    /// ライントレース攻略用戦略クラス
    /// </summary>
    public class LineTraceStrategy : AbstractStrategy
    {
        public List<PIDParametor> PIDParam { set; get; }

        public override void Run()
        {
            double pk = 0.5, pd = 4;
            var power = 100;

            int prevDef = 0;
            var input = InputData.InputData;
            var output = OutputData.OutputData;

            var diff = 50 - input.ReflectedLight;
            var steering = pk * diff + (diff - prevDef) * pd;

            if (input.ReflectedLight == 100)
            {
                output.Motor1Power = 0;
                output.Motor2Power = 0;
            }
            else if (steering > 0)
            {
                output.Motor1Power = (sbyte)(power + steering < 100 ? power + steering : 100);
                output.Motor2Power = (sbyte)(power - steering > 0 ? power - steering : 0);
            }
            else
            {
                output.Motor1Power = (sbyte)(power + steering > 0 ? power + steering : 0);
                output.Motor2Power = (sbyte)(power - steering < 100 ? power - steering : 100);
            }

            prevDef = diff;
        }
    }

}
