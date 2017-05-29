using ET2017_TuningTool.Model;
using ET2017_TuningTool.Model.GraphModel;
using LiveCharts;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using SerialLibrary;
using System;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Media;

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
        public Point YellowPoint { get => _YellowPoins; set => SetProperty(ref _YellowPoins, value); }

        private Point _BlackPoint;
        public Point BlackPoint { get => _BlackPoint; set => SetProperty(ref _BlackPoint, value); }

        private Point _RedPoint;
        public Point RedPoint { get => _RedPoint; set => SetProperty(ref _RedPoint, value); }

        private Point _BluePoint;
        public Point BluePoint { get => _BluePoint; set => SetProperty(ref _BluePoint, value); }

        private Point _GreenPoint = new Point(126, 68);
        public Point GreenPoint { get => _GreenPoint; set => SetProperty(ref _GreenPoint, value); }

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

        private int _LeftMotorPower;
        /// <summary>
        /// 移動パワー（+で前進）
        /// </summary>
        public int LeftMotorPower
        {
            get => _LeftMotorPower;
            set => SetProperty(ref _LeftMotorPower, value);
        }

        private int _RightMotorPower;
        /// <summary>
        ///ステアリング（+の場合に右旋回）
        /// </summary>
        public int RightMotorPower
        {
            get => _RightMotorPower;
            set => SetProperty(ref _RightMotorPower, value);
        }

        private int _armPower;
        /// <summary>
        /// アームモータパワー
        /// </summary>
        public int ArmPower
        {
            get => _armPower;
            set => SetProperty(ref _armPower, value);
        }

        private int _tailPower;
        /// <summary>
        /// 尻尾モータパワー
        /// </summary>
        public int TailPower
        {
            get =>_tailPower;
            set => SetProperty(ref _tailPower, value); 
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

        #region グラフデータ
        public ChartValues<double> GraphValue1 { get; set; } = new ChartValues<double>();
        public ChartValues<double> GraphValue2 { get; set; } = new ChartValues<double>();
        public ChartValues<double> GraphValue3 { get; set; } = new ChartValues<double>();
        public ChartValues<double> GraphValue4 { get; set; } = new ChartValues<double>();
        public ChartValues<double> GraphValue5 { get; set; } = new ChartValues<double>();

        private double _GraphYMaxValue = 50;
        public double GraphYMaxValue { get => _GraphYMaxValue; set => SetProperty(ref _GraphYMaxValue, value); }

        private bool _AnimationEnable;
        public bool AnimationEnable { get => _AnimationEnable; set => SetProperty(ref _AnimationEnable, value); }
        #endregion

        private class ModelPair
        {
            internal AbstractGraphModel model;
            internal ChartValues<double> value;
        }

        public MainViewModel()
        {
            SerialPortNames =  SerialPort.GetPortNames();

            var ModelPairArray = new ModelPair[]
            {
                new ModelPair { model = new SonarGraphModel(), value = GraphValue1 },
                new ModelPair { model = new ReflectedLightGraphModel(), value = GraphValue2 },
                new ModelPair { model =  new TemparetureModel(), value = GraphValue3 },
                new ModelPair { model = new BatteryVoltageModel(), value = GraphValue4 },
                new ModelPair { model =  new BatteryCurrentModel(), value = GraphValue5 }
            };


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


                        AnimationEnable = false; // 画面要素をすべて更新するまでアニメーションOFF
                        var flag = (GraphValue1.Count > GraphYMaxValue);
                        foreach(var mp in ModelPairArray)
                        {
                            if (flag) mp.value.RemoveAt(0);
                            mp.value.Add(mp.model.GetValue(received));
                        }
                        AnimationEnable = true; // 画面要素をすべて更新したのでアニメーションON
                        
                    };

                    // 出力信号電文受信時に、対応するプロパティを更新する処理を登録
                    Serial.ReceiveOutputSignal = received =>
                    {
                        TailPower = received.Motor4Power;
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
