using RobotController.ColorUtil;
using RobotController.RobotStatus;
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
        public List<PIDParametor> PIDParamList { set; get; }

        public override void Run()
        {
            double pk = PIDParamList[0].PGain, pd = PIDParamList[0].DGain;
            int center = 90;
            var power = 50;

            int prevDef = 0;
            var input = InputParameter.InputSignal;
            var output = OutputParameter.OutputSignal;

            var light = input.ReflectedLight;

            var diff = center - (int)light;
            var steering = pk * diff + (diff - prevDef) * pd;
            


            if (light == 100)
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

            OutputParameter.OutputSignal = output;
            prevDef = diff;
        }
    }

}
