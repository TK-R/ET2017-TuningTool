using System;
using SerialLibrary;
using RobotController.GameStrategy;
using RobotController.RobotStatus;
using RobotController.BlockArrange;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Mvvm;
using System.Threading;

namespace RobotController
{
    public class RobotControl : BindableBase
    {
        private bool _Runnnig;
        /// <summary>
        /// 周期処理実行中ならTrue
        /// </summary>
        public bool Running { set => SetProperty(ref _Runnnig, value); get => _Runnnig; }

        /// <summary>
        /// シリアル通信監理クラスへの参照
        /// </summary>
        public SerialManager Serial { set; get; }

        /// <summary>
        /// 現在の競技戦略
        /// </summary>
        private AbstractStrategy CurrentStrategy { set; get; }

        /// <summary>
        /// 走行体の周期動作を模擬スレッド
        /// </summary>
        private Thread CyclicThread { set; get; }

        private bool Killflag = false;
        public void ThreadStop()
        {
            Killflag = true;
        }

        public RobotControl()
        {
            // 起動時の競技戦略を宣言
            CurrentStrategy = new LineTraceStrategy();

            // 周期動作を定義
            CyclicThread = new Thread(_ =>
            {
                while (true)
                {
                    if (Killflag)
                        break;

                    // 20ms待って、すべての処理を繰り返す
                    Thread.Sleep(20);

                    // 実行中でなければ、次の周期へスキップ
                    if (!Running || Serial == null || !Serial.Runnning)
                        continue;
                    
                    // 入力動作
                    InputParameter.InputSignal = Serial.RecentInputSignal;

                    // 周期動作
                    CurrentStrategy.Run();

                    // 出力動作
                    Serial.WriteData(OutputParameter.OutputSignal);
                 }
            });

            // スレッドをスタート
            CyclicThread.Start();
            
        }
        
        public bool SetPIDParametor(List<PIDParametor> pid)
        {
            // 現在の戦略クラスがライントレースなら、PIDパラメータを更新
            if (CurrentStrategy is LineTraceStrategy lts)
            {
                lts.PIDParam = pid;
                return true;
            }

            return false;
        }

        ~RobotControl()
        {
            CyclicThread.Abort();
        }
    }
}
