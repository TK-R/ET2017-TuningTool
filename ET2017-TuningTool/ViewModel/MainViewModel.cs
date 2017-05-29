﻿using ET2017_TuningTool.Model;
using ET2017_TuningTool.Model.GraphModel;
using LiveCharts;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SerialLibrary;
using System;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;

namespace ET2017_TuningTool
{
    public class MainViewModel : BindableBase
    {
        public DelegateCommand ConnectCommand { get; }
        public DelegateCommand DisconnectCommand { get; }
        public ReactiveCommand DecodeCommand { get; private set; }

        #region シリアルポート関係
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

        #endregion

        public ReactiveProperty<int> InitPostionCode { get; set; }

        #region ブロックの座標データ
        /// <summary>
        /// フィールドのブロック情報管理クラス
        /// </summary>
        BlockFieldModel BlockField =  new BlockFieldModel();
        /// <summary>
        /// 黄色ブロックの位置情報
        /// </summary>
        public ReactiveProperty<Point> Yellow { get; }
        /// <summary>
        /// 黒ブロックの位置情報
        /// </summary>
        public ReactiveProperty<Point> Black { get; }
        /// <summary>
        /// 垢ブロックの位置情報
        /// </summary>
        public ReactiveProperty<Point> Red { get; }
        /// <summary>
        /// 青ブロックの位置情報
        /// </summary>
        public ReactiveProperty<Point> Blue { get; }
        /// <summary>
        /// 緑ブロックの位置情報
        /// </summary>
        public ReactiveProperty<Point> Green { get; }

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

            // ブロックの配置情報を登録
            BlockField = new BlockFieldModel();
            Yellow = BlockField.ObserveProperty(x => x.YelloPosition)
                        .Select(p => BlockPositionArray[p - 1])
                        .ToReactiveProperty();
            Red = BlockField.ObserveProperty(x => x.RedPosition)
                     .Select(p => BlockPositionArray[p - 1])
                     .ToReactiveProperty();
            Black = BlockField.ObserveProperty(x => x.BlackPosition)
                       .Select(p => BlockPositionArray[p - 1])
                       .ToReactiveProperty();
            Blue = BlockField.ObserveProperty(x => x.BluePosition)
                      .Select(p => BlockPositionArray[p - 1])
                      .ToReactiveProperty();
            Green = BlockField.ObserveProperty(x => x.GreenPosition)
                       .Select(p => BlockPositionArray[p - 1])
                       .ToReactiveProperty();
            
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
                    };

                    if (!Serial.Start(SelectedPortName))
                        Console.WriteLine("hogeeeee");
                    
                });

            DisconnectCommand = new DelegateCommand(
                () =>
                {
                    Serial.Stop();
                });


            InitPostionCode = new ReactiveProperty<int>(0);
            DecodeCommand = InitPostionCode.Select(i => i < 99999).ToReactiveCommand();

            DecodeCommand.Subscribe(
                () =>
                {
                    // 初期位置コードを求める。
                    BlockField.SetBlockPosition(InitPostionCode.Value, 1);
                });
        }

    }
}
