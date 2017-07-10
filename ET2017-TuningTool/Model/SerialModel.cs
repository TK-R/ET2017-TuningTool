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
        public SerialManager Serial { private set; get; }

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
            SerialPortNames = SerialPort.GetPortNames().Distinct().ToArray();
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

            Serial = new SerialManager()
            {

                // 入力信号電文受信時に、対応するプロパティを更新する処理を登録
                ReceiveInputSignal = received => RecentInputSignalData = received,

                // 出力信号電文受信時に、対応するプロパティを更新する処理を登録
                ReceiveOutputSignal = received => RecentOutputSignalData = received
            };

            // シリアル通信を開始
            Connected = Serial.Start(portName);

        }
        
        public bool WriteData<StructT>(StructT dataStruct)
        {
            return Serial.WriteData(dataStruct);
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
            set {
                // 同一の値を受信しても通知処理が実施されるように、SetProperyは使わない。
                _RecentInputSignalData = value;
                OnPropertyChanged("RecentInputSignalData");
            }
        }
        
        private OutputSignalData _RecentOutputSignalData;
        /// <summary>
        /// 直近に受信した出力信号電文
        /// </summary>
        public OutputSignalData RecentOutputSignalData
        {
            get { return _RecentOutputSignalData; }
            set {
                // 同一の値を受信しても通知処理が実施されるように、SetProperyは使わない。
                _RecentOutputSignalData = value;
                OnPropertyChanged("RecentOutputSignalData");
            }
        }

        private PIDData _RecentPIDData;
        public PIDData RecentPIDData
        {
            get { return _RecentPIDData; }
            set { SetProperty(ref _RecentPIDData, value); }
        }
    }
}

