using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using SerialLibrary;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows;

namespace ET2017_TuningTool
{
    public class MainViewModel : BindableBase
    {
        public DelegateCommand ConnectCommand { get; }
        public DelegateCommand DisconnectCommand { get; }

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

        private string _SelectedPortName;
        /// <summary>
        /// 選択されたシリアルポート名
        /// </summary>
        public string SelectedPortName
        {
            get => _SelectedPortName;
            set => SetProperty(ref _SelectedPortName, value);
        }

        #region 出力信号電文情報

        private int _movePower;
        /// <summary>
        /// 移動パワー（+で前進）
        /// </summary>
        public int MovePower
        {
            get => _movePower;
            set => SetProperty(ref _movePower, value);
        }

        private int _steering;
        /// <summary>
        ///ステアリング（+の場合に右旋回）
        /// </summary>
        public int Steering
        {
            get => _steering;
            set => SetProperty(ref _steering, value);
        }

        private int _armPower;
        /// <summary>
        /// モータ3(ARM）パワー
        /// </summary>
        public int ArmPower
        {
            get => _armPower;
            set => SetProperty(ref _armPower, value);
        }

        #endregion

        #region 入力信号電文情報

        private SolidColorBrush _SensorColor;
        /// <summary>
        /// カラーセンサの色情報
        /// </summary>
        public SolidColorBrush SensorColor
        {
            get => _SensorColor;
            set => SetProperty(ref _SensorColor, value);
        }

        private int _reflectedLight;
        /// <summary>
        /// 反射光
        /// </summary>
        public int ReflectedLight
        {
            get => _reflectedLight;
            set => SetProperty(ref _reflectedLight, value);
        }

        #endregion

        public MainViewModel()
        {
            SerialPortNames =  SerialPort.GetPortNames();
            ConnectCommand = new DelegateCommand(
                () => 
                {
                    Serial = new SerialManager();
                    
                    // 入力信号電文受信時に、対応するプロパティを更新する処理を登録
                    Serial.ReceiveInputSignal = received => {
                        ReflectedLight = received.ReflectedLight;

                        // UI要素なのでUIスレッドで動作すること。
                        Application.Current.Dispatcher.Invoke(new Action(() => {
                            SensorColor = new SolidColorBrush(Color.FromArgb(255, received.ColorR, received.ColorG, received.ColorB));
                        }));

                    };

                    // 出力信号電文受信時に、対応するプロパティを更新する処理を登録
                    Serial.ReceiveOutputSignal = received =>
                    {
                        MovePower = received.MovePower;
                        Steering = received.Steering;
                        ArmPower = received.Motor3Power;
                    };

                    if (!Serial.Start(SelectedPortName))
                        Console.WriteLine("hogeeeee");
                    
                });

            DisconnectCommand = new DelegateCommand(
                () =>
                {
                    Serial.Stop();
                });
        }

    }
}
