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
using System.Windows.Controls;
using System.Globalization;
using ET2017_TuningTool.Model;

namespace ET2017_TuningTool
{
    public class MainViewModel : BindableBase
    {
        public DelegateCommand ConnectCommand { get; }
        public DelegateCommand DisconnectCommand { get; }
        public DelegateCommand DecodeCommand { get; set; }

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

        private int _InitialPositionCode = 0;
        /// <summary>
        /// 入力された初期位置コード
        /// </summary>
        public int InitialPositionCode
        {
            get => _InitialPositionCode;
            set => SetProperty(ref _InitialPositionCode, value);
        }

        #region ブロックの座標データ
        private Point _YellowPoins;
        public Point YellowPoint { get { return _YellowPoins; } set { SetProperty(ref _YellowPoins, value); } }

        private Point _BlackPoint;
        public Point BlackPoint { get { return _BlackPoint; } set { SetProperty(ref _BlackPoint, value); } }

        private Point _RedPoint;
        public Point RedPoint { get { return _RedPoint; } set { SetProperty(ref _RedPoint, value); } }

        private Point _BluePoint;
        public Point BluePoint { get { return _BluePoint; } set { SetProperty(ref _BluePoint, value); } }

        private Point _GreenPoint = new Point(126, 68);
        public Point GreenPoint { get { return _GreenPoint; } set { SetProperty(ref _GreenPoint, value); } }

        /// <summary>
        /// ブロックの座標を保持する構造体
        /// </summary>
        static readonly Point[] BlockPositionArray =
        {
            new Point(1, 9),
            new Point(82, 9),
            new Point(168, 9),
            new Point(254, 9),
            new Point(42, 34),
            new Point(126,34),
            new Point(210, 34),
            new Point(84, 58),
            new Point(169, 58),
            new Point(19, 77),
            new Point(236, 77),
            new Point(61, 100),
            new Point(108, 100),
            new Point(146, 100),
            new Point(192, 100),
        };
        #endregion
        
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

            // 接続コマンド押下イベントを定義
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

            DecodeCommand = new DelegateCommand(
                () =>
                {
                    // 初期位置コードを求める。
                    var pos = BlockFieldModel.AdjustBlockPositionField(BlockFieldModel.GetPositionFromCode(InitialPositionCode));

                    // 座標データを代入
                    BlackPoint = BlockPositionArray[pos.Black - 1];
                    RedPoint = BlockPositionArray[pos.Red - 1];
                    YellowPoint = BlockPositionArray[pos.Yellow - 1];
                    BluePoint = BlockPositionArray[pos.Blue - 1];
                   
                });
        }

    }
}
