using ET2017_TuningTool.Model;
using ET2017_TuningTool.Model.GraphModel;
using LiveCharts;
using Microsoft.Practices.Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SerialLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;

namespace ET2017_TuningTool
{
    public class MainViewModel : BindableBase, IDisposable
    {
        #region コマンド
        /// <summary>
        /// シリアルポート接続コマンド
        /// </summary>
        public ReactiveCommand ConnectCommand { get; private set; }
        /// <summary>
        /// シリアルポート切断コマンド
        /// </summary>
        public ReactiveCommand DisconnectCommand { get; private set; }
        /// <summary>
        /// 初期位置コードのデコードコマンド
        /// </summary>
        public ReactiveCommand DecodeCommand { get; private set; }
        #endregion

        #region シリアルポート関係

        SerialModel Serial = new SerialModel();

        public ReactiveProperty<string> SelectPortName { get; set; } = new ReactiveProperty<string>();

        public ReadOnlyReactiveProperty<string[]> SerialPortNames { get; set; }

        public ReactiveProperty<bool> SerialConnected { get; }
        #endregion

        #region ブロックの座標データ

        /// <summary>
        /// 初期配置コード
        /// </summary>
        public ReactiveProperty<int> InitPostionCode { get; set; } = new ReactiveProperty<int>(0);
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
        /// <summary>
        /// センサカラー情報
        /// </summary>
        public ReadOnlyReactiveProperty<SolidColorBrush> SensorColor { get; }
        #endregion

        #region グラフ情報
        /// <summary>
        /// グラフに表示する入力値データを格納する
        /// </summary>
        public List<ReadOnlyReactiveProperty<ChartValues<double>>> InputGraphValueList { get; set; } 
            = new List<ReadOnlyReactiveProperty<ChartValues<double>>>();

        /// <summary>
        /// グラフに表示する出力値データを格納する
        /// </summary>
        public List<ReadOnlyReactiveProperty<ChartValues<double>>> OutputGraphValueList { get; set; }
            = new List<ReadOnlyReactiveProperty<ChartValues<double>>>();

        /// <summary>
        /// グラフのY軸の個数を規定する
        /// </summary>
        public ReadOnlyReactiveProperty<int> GraphYMaxCount { get; }
        #endregion

        public ReactiveProperty<float> PIDPowerData { get; }
        public ReactiveProperty<float> PIDPGainData { get; }
        public ReactiveProperty<float> PIDIGainData { get; }
        public ReactiveProperty<float> PIDDGainData { get; }

        public PIDModel PID { get; set; } = new PIDModel();

        /// <summary>
        /// 入力値モデルを格納するリスト
        /// </summary>
        public List<InputValueModel> InputModels { get; set; } = new List<InputValueModel>();

        /// <summary>
        /// 出力値モデルを格納するリスト
        /// </summary>
        public List<OutputValueModel> OutputModels { get; set; } = new List<OutputValueModel>();

        public MainViewModel()
        {
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

            
            // 入力値モデルを生成
            foreach (var t in InputValueModel.InputValueType)
            {
                var model = Activator.CreateInstance(t) as InputValueModel;
                InputModels.Add(model);
                var prop = model.ObserveProperty(m => m.GraphValue).ToReadOnlyReactiveProperty();
                InputGraphValueList.Add(prop);
            }

            // 出力値モデルを生成
            foreach (var t in OutputValueModel.OutputValueType)
            {
                var model = Activator.CreateInstance(t) as OutputValueModel;
                OutputModels.Add(model);
                var prop = model.ObserveProperty(m => m.GraphValue).ToReadOnlyReactiveProperty();
                OutputGraphValueList.Add(prop);
            }
            
            // グラフ点数を定義
            GraphYMaxCount = InputModels.First().ObserveProperty(m => m.GraphMaxCount).ToReadOnlyReactiveProperty();

            // ポート名称一覧
            SerialPortNames = Serial.ObserveProperty(s => s.SerialPortNames).ToReadOnlyReactiveProperty();
            // 接続状態
            SerialConnected = Serial.ObserveProperty(s => s.Connected).Delay(TimeSpan.FromMilliseconds(500)).ToReactiveProperty();
            
            // センサカラーの表示
            SensorColor = Serial.ObserveProperty(s => s.RecentInputSignalData)
                                .ObserveOnDispatcher() // UIスレッドに戻す
                                .Select(v => new SolidColorBrush(Color.FromArgb(255, v.ColorR, v.ColorG, v.ColorB)))
                                .ToReadOnlyReactiveProperty();

            // 入力値の更新を登録
            Serial.ObserveProperty(s => s.RecentInputSignalData)
                  .Subscribe(r => 
            {
                foreach ( var m in InputModels)
                    m.UpdateValue(r);
            });
            // 出力値の更新を登録
            Serial.ObserveProperty(s => s.RecentOutputSignalData)
                  .Subscribe(r =>
            {
                foreach (var m in OutputModels) m.UpdateValue(r);
            });
            // PIDの更新を登録
            Serial.ObserveProperty(s => s.RecentPIDData)
                  .Subscribe(r =>
            {
                PID.UpdatePID(r);
            });
            

            // 接続コマンド押下イベントを定義
            ConnectCommand = SerialConnected
                .CombineLatest(SelectPortName, (connected, selectPort) =>
                 !connected && !string.IsNullOrEmpty(selectPort)) // 未接続かつシリアルポート選択済み
                .ToReactiveCommand();
            ConnectCommand.Subscribe(_ => Serial.StartSerial(SelectPortName.Value));
            
            // 切断コマンドはシリアル通信が接続中のみ実行可能なコマンドとして定義する
            DisconnectCommand = SerialConnected.ToReactiveCommand();
            DisconnectCommand.Subscribe(_ => Serial.StopSerial());

            // 初期位置コードを求める。    
            DecodeCommand = InitPostionCode
                .CombineLatest(SerialConnected, (i, c)  => c && i < 99999)
                .ToReactiveCommand();
            DecodeCommand.Subscribe( _ => BlockField.SetBlockPosition(InitPostionCode.Value, 1));

            // PIDゲインデータの通信を登録
            PIDPowerData = PID.ObserveProperty(p => p.Power).ToReactiveProperty();
            PIDPGainData = PID.ObserveProperty(p => p.PGain).ToReactiveProperty();
            PIDIGainData = PID.ObserveProperty(p => p.IGain).ToReactiveProperty();
            PIDDGainData = PID.ObserveProperty(p => p.DGain).ToReactiveProperty();

            // 200ms値が確定したら、データを送信
            void sendData()
            {
                // 接続中以外は何もしない
                if (!Serial.Connected)
                    return;

                Serial.WriteData(new PIDData
                {
                    BasePower = PIDPowerData.Value,
                    PGain = PIDPGainData.Value,
                    IGain = PIDIGainData.Value,
                    DGain = PIDDGainData.Value
                });
            }
            var pidWait = TimeSpan.FromMilliseconds(500);
            PIDPowerData.Throttle(pidWait).Subscribe(_ => sendData());
            PIDPGainData.Throttle(pidWait).Subscribe(_ => sendData());
            PIDIGainData.Throttle(pidWait).Subscribe(_ => sendData());
            PIDDGainData.Throttle(pidWait).Subscribe(_ => sendData());

        }

        /// <summary>
        /// オブジェクト解放時には、シリアルポートを停止する
        /// </summary>
        public void  Dispose()
        {
            if (Serial != null)
                Serial.StopSerial();
            
        }

    }
}
