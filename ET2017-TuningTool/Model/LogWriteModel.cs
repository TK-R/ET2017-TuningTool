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

        private static void Write(InputSignalData input, OutputSignalData output)
        {
            NLogger.Info(input.BatteryCurrent+ "mA," + input.BatteryVoltage + "mV");
        }
    }
}
