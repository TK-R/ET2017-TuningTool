﻿using System;
using SerialLibrary;

namespace RobotController
{
    public class RobotController
    {
        /// <summary>
        /// 周期処理実行中ならTrue
        /// </summary>
        private bool Running { set; get; }

        /// <summary>
        /// シリアル通信監理クラスへの参照
        /// </summary>
        private SerialManager Serial { set; get; }

        public RobotController(SerialManager serial)
        {
            Serial = serial;    
        }

        
        public void Start()
        {
            Running = true;
        }

        public void Stop()
        {
            Running = false;
        }

    }
}
