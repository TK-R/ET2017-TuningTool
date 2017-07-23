using NLog;
using SerialLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET2017_TuningTool.Model
{
    public static class LogWriteModel
    {
        /// <summary>
        /// 書き込み待ちの入力信号電文のリスト
        /// </summary>
        static List<InputSignalData> InputData = new List<InputSignalData>();

        /// <summary>
        /// 書き込み待ちの出力信号電文のリスト
        /// </summary>
        static List<OutputSignalData> OutputData = new List<OutputSignalData>();

        private static Logger NLogger = LogManager.GetCurrentClassLogger();

        public static void Write(OutputSignalData output)
        {
            if(InputData.Count() == 0)
            {
                OutputData.Add(output);
                return;
            }

            // 既に受信した入力信号電文があれば、受信した出力信号電文と一緒に出力
            var input = InputData.FirstOrDefault();
            InputData.Remove(input);

            Write(input, output);

        }

        public static void Write(InputSignalData input)
        {
            if (OutputData.Count() == 0)
            {
                InputData.Add(input);
                return;
            }

            // 既に受信した入力信号電文があれば、受信した出力信号電文と一緒に出力
            var output = OutputData.FirstOrDefault();
            OutputData.Remove(output);

            Write(input, output);


        }

        static int No = 1;
        private static void Write(InputSignalData input, OutputSignalData output)
        {
            StringBuilder sb = new StringBuilder();
            

            // 文字列連結
            sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18}",
                No,input.Motor1Radian, input.Motor2Radian, input.Motor3Radian, input.Motor4Radian,
                input.TouchSenser, input.SonicDistance, input.ColorR, input.ColorG, input.ColorB,
                input.ReflectedLight, input.Angle, input.AnglarVelocity, input.BatteryCurrent, input.BatteryVoltage,
                output.Motor1Power, output.Motor2Power, output.Motor3Power, output.Motor4Power);

            No++;

            NLogger.Info(sb.ToString());
        }
    }
}
