using System;
using SerialLibrary;
using RobotController.GameStrategy;
using RobotController.RobotStatus;
using RobotController.BlockArrange;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Mvvm;

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
        private SerialManager Serial { set; get; }

        private AbstractStrategy CurrentStrategy { set; get; }

        public RobotControl(SerialManager serial)
        {
            Serial = serial;    
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
    }
}
