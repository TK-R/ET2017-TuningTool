using Microsoft.Practices.Prism.Mvvm;
using SerialLibrary;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET2017_TuningTool.Model
{
    public class SerialModel : BindableBase
    {
        /// <summary>
        /// シリアル監視クラスへの参照
        /// </summary>
        private SerialManager Serial;

        private string[] _SerialPortNames;
        /// <summary>
        /// シリアルポート一覧
        /// </summary>
        public string[] SerialPortNames
        {
            get => _SerialPortNames;
            set => SetProperty(ref _SerialPortNames, value);
        }

        private bool _Connected;
        /// <summary>
        /// シリアル接続中フラグ
        /// </summary>
        public bool Connected
        {
            get { return _Connected; }
            set { SetProperty(ref _Connected, value); }
        }

        public SerialModel()
        {
            // ポート一覧を取得して、プロパティに保持する
            SerialPortNames = SerialPort.GetPortNames();
        }

        /// <summary>
        /// シリアル通信を開始する
        /// </summary>
        /// <param name="portName">ポート名称</param>
        public void StartSerial(string portName)
        {
            // 既に接続済みの場合には何もしない
            if (Connected)
                return;

            Serial = new SerialManager();

            // 入力信号電文受信時に、対応するプロパティを更新する処理を登録
            Serial.ReceiveInputSignal = received => RecentInputSignalData = received;

            // 出力信号電文受信時に、対応するプロパティを更新する処理を登録
            Serial.ReceiveOutputSignal = received => RecentOutputSignalData = received;
            
            // シリアル通信を開始
            Connected = Serial.Start(portName);

        }

        public void StopSerial()
        {

            // 接続済みでなければ何もしない
            if (!Connected)
                return;

            Serial.Stop();
            Connected = false;
        }

        private InputSignalData _RecentInputSignalData;
        /// <summary>
        /// 直近に受信した入力信号電文
        /// </summary>
        public InputSignalData RecentInputSignalData
        {
            get { return _RecentInputSignalData; }
            set { SetProperty(ref _RecentInputSignalData, value); }
        }
        
        private OutputSignalData _RecentOutputSignalData;
        /// <summary>
        /// 直近に受信した出力信号電文
        /// </summary>
        public OutputSignalData RecentOutputSignalData
        {
            get { return _RecentOutputSignalData; }
            set { SetProperty(ref _RecentOutputSignalData, value); }
        }
    }
}

