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
        int prevDiff = 0;
        public override void Run()
        {
            var pid = PIDParamList[0];
            double pk = pid.PGain, pd = pid.DGain, power = pid.Power;
            int center = 45;
          
            var input = InputParameter.InputSignal;
            var output = OutputParameter.OutputSignal;

            var light = input.ReflectedLight;

            var diff = center - (int)light;
            var steering = pk * diff + (diff - prevDiff) * pd;
            
        
            if (steering > 0)
            {
                double rate = (100 - steering) > 0 ? 100 - steering : 0; 

                output.Motor1Power = (sbyte)power;
                output.Motor2Power = (sbyte)(power * (rate / 100.0));
            }
            else
            {
                double rate = (100 - Math.Abs(steering)) > 0 ? 100 - steering : 0;
                output.Motor1Power = (sbyte)(power * (rate / 100.0));
                output.Motor2Power = (sbyte)pid.Power;
            }


            Console.WriteLine("L:" + output.Motor1Power + ", R:" +  output.Motor2Power);

            OutputParameter.OutputSignal = output;
            prevDiff = diff;
        }
    }

}
