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
        public List<PIDParametor> PIDParam { set; get; }

        public override void Run()
        {
            double pk = 0.5, pd = 4;
            var power = 50;

            int prevDef = 0;
            var input = InputParameter.InputSignal;
            var output = OutputParameter.OutputSignal;

            var light = HSLColor.FromRGB(input.ColorR, input.ColorG, input.ColorB).Luminosity * 100;

            var diff = 50 - (int)light;
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
