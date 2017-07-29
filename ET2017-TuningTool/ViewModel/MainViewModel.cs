using ET2017_TuningTool.Model;
using ET2017_TuningTool.Model.GraphModel;
using LiveCharts;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SerialLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;
using RobotController;
using System.Reactive.Subjects;
using System.Reactive;

namespace ET2017_TuningTool
{

    public class MainViewModel : BindableBase, IDisposable
    {

        private Subject<Unit> CommitTrigger { get; } = new Subject<Unit>();

        private IObservable<Unit> CommitAsObservable => this.CommitTrigger;


        /// <summary>
        /// Rpの一斉解放用のオブジェクト
        /// </summary>
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

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

        /// <summary>
        /// 次の配置場所を求めるコマンド
        /// </summary>
        public ReactiveCommand NextPositionCommand { get; private set; }
        
        public ReactiveCommand SendRuleCommand { get; private set; }

        #endregion

        #region シリアルポート関係

        SerialModel Serial = new SerialModel();

        public ReactiveProperty<string> SelectPortName { get; set; } = new ReactiveProperty<string>();

        public ReadOnlyReactiveProperty<string[]> SerialPortNames { get; set; }

        public ReactiveProperty<bool> SerialConnected { get; }
        #endregion

        #region 座標データ

        /// <summary>
        /// 初期配置コード
        /// </summary>
        public ReactiveProperty<int> InitPostionCode { get; set; } = new ReactiveProperty<int>(12008);
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
        /// 赤ブロックの位置情報
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
        /// ブロック並べフィールドの描画に用いる走行体情報クラス
        /// </summary>
        public EV3Model BlockRobotModel { get; set; } = new EV3Model();

        /// <summary>
        /// 自己位置推定結果の描画に用いる走行体情報クラス
        /// </summary>
        public EV3Model SelfPositionRobotModel { get; set; } = new EV3Model();

        /// <summary>
        /// ブロック並べフィールドにおけるロボットの位置情報
        /// </summary>
        public ReactiveProperty<Point> BlockRobotPos { get; }

        /// <summary>
        /// 自己位置推定結果におけるロボットの位置情報(描画用)
        /// </summary>
        public ReactiveProperty<Point> SelfPositionRobotPos { get; }

        /// <summary>
        /// 事項指定結果におけるロボットの位置情報
        /// </summary>
        public ReactiveProperty<Point> SelfPositionRobotPosRaw { get; }

        /// <summary>
        /// 自己位置推定結果におけるロボットの角度情報
        /// </summary>
        public ReactiveProperty<int> SelfPositionRobotAngle { get; }

        /// <summary>
        /// 運搬対象ブロックに接近する際の運搬経路のパス
        /// </summary>
        public ReactiveProperty<Point[]> ApproachWay { get; }

        /// <summary>
        /// ブロックを運搬先ブロック置き場に運搬する際のパス
        /// </summary>
        public ReactiveProperty<Point[]> MoveBlockWay { get; }

        /// <summary>
        /// ブロックの座標を保持する構造体
        /// </summary>
        private static readonly Point[] BlockPositionArray =
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

        /// <summary>
        /// ウェイポイントの座標を保持する構造体
        /// </summary>
        private static readonly Point[] WayPointArray =
        {
            new Point(49,9),
            new Point(134,9),
            new Point(219,9),
            new Point(35,25),
            new Point(74,25),
            new Point(117,25),
            new Point(160,25),
            new Point(203,25),
            new Point(239,25),
            new Point(18,45),
            new Point(71,45),
            new Point(114,45),
            new Point(157,45),
            new Point(200,45),
            new Point(254,45),
            new Point(39,57),
            new Point(232,57),
            new Point(84,79),
            new Point(107,79),
            new Point(168,79),
            new Point(191,79),
            new Point(55,91),
            new Point(222,91),
            new Point(96,100),
            new Point(136,100),
            new Point(179,100)
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

        public ReactiveProperty<bool> PCControlRobot { get; set; }

        public RobotControl RobotController { get; set; }

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
            Yellow = BlockField.ObserveProperty(x => x.YellowPosition)
                        .Select(p => BlockPositionArray[p])
                        .ToReactiveProperty().AddTo(this.Disposable);
            Red = BlockField.ObserveProperty(x => x.RedPosition)
                     .Select(p => BlockPositionArray[p])
                     .ToReactiveProperty().AddTo(this.Disposable);
            Black = BlockField.ObserveProperty(x => x.BlackPosition)
                       .Select(p => BlockPositionArray[p])
                       .ToReactiveProperty().AddTo(this.Disposable);
            Blue = BlockField.ObserveProperty(x => x.BluePosition)
                      .Select(p => BlockPositionArray[p])
                      .ToReactiveProperty().AddTo(this.Disposable);
            Green = BlockField.ObserveProperty(x => x.GreenPosition)
                       .Select(p => BlockPositionArray[p])
                       .ToReactiveProperty().AddTo(this.Disposable);
            BlockRobotPos = BlockRobotModel.ObserveProperty(r => r.Position)
                              .ToReactiveProperty().AddTo(this.Disposable);

            // 入力値モデルを生成
            foreach (var t in InputValueModel.InputValueType)
            {
                var model = Activator.CreateInstance(t) as InputValueModel;
                InputModels.Add(model);
                var prop = model.ObserveProperty(m => m.GraphValue).ToReadOnlyReactiveProperty().AddTo(this.Disposable);
                InputGraphValueList.Add(prop);
            }

            // 出力値モデルを生成
            foreach (var t in OutputValueModel.OutputValueType)
            {
                var model = Activator.CreateInstance(t) as OutputValueModel;
                OutputModels.Add(model);
                var prop = model.ObserveProperty(m => m.GraphValue).ToReadOnlyReactiveProperty().AddTo(this.Disposable);
                OutputGraphValueList.Add(prop);
            }
            
            // グラフ点数を定義
            GraphYMaxCount = InputModels.First().ObserveProperty(m => m.GraphMaxCount).ToReadOnlyReactiveProperty().AddTo(this.Disposable);

            // ポート名称一覧
            SerialPortNames = Serial.ObserveProperty(s => s.SerialPortNames).ToReadOnlyReactiveProperty().AddTo(this.Disposable);
            // 接続状態
            SerialConnected = Serial.ObserveProperty(s => s.Connected).Delay(TimeSpan.FromMilliseconds(500)).ToReactiveProperty().AddTo(this.Disposable);

            // センサカラーの表示
            //SensorColor = Serial.ObserveProperty(s => s.RecentInputSignalData)
            //                    .ObserveOnDispatcher() // UIスレッドに戻す
            //                    .Select(v => new SolidColorBrush(Color.FromArgb(255, v.ColorR, v.ColorG, v.ColorB)))
            //                    .ToReadOnlyReactiveProperty().AddTo(this.Disposable);

            // 入力値の更新を登録
            Serial.ObserveProperty(s => s.RecentInputSignalData)
                  .SkipTime(TimeSpan.FromMilliseconds(200))
                  .Subscribe(r =>
            {
                foreach (var m in InputModels) m.UpdateValue(r);
            });

            // ログ出力の登録
            Serial.ObserveProperty(s => s.RecentInputSignalData)
                  .Subscribe(r => LogWriteModel.Write(r));    

            // 出力値の更新を登録
            Serial.ObserveProperty(s => s.RecentOutputSignalData)
                  .SkipTime(TimeSpan.FromMilliseconds(200))
                  .Subscribe(r =>
            {
                foreach (var m in OutputModels) m.UpdateValue(r);
            });
            
            // ログ出力の登録
            Serial.ObserveProperty(s => s.RecentOutputSignalData)
                  .Subscribe(r => LogWriteModel.Write(r));

            // PIDの更新を登録
            Serial.ObserveProperty(s => s.RecentPIDData)
                  .Subscribe(r =>
            {
                PID.UpdatePID(r);
            });

            //自己位置情報の更新を登録（描画用）
            SelfPositionRobotPos = Serial.ObserveProperty(s => s.RecentSelfPositionData)
                                         .Select(t => new Point(t.PositionX / 5510.0 * 451.0, t.PositionY / 3722.0 * 296.0))
                                         .ToReactiveProperty().AddTo(this.Disposable);
        
            SelfPositionRobotAngle = Serial.ObserveProperty(s => s.RecentSelfPositionData)
                                         .Select(t => (int)t.Angle)
                                         .ToReactiveProperty().AddTo(this.Disposable);

            //自己位置情報の更新を登録（生データ）
            SelfPositionRobotPosRaw = Serial.ObserveProperty(s => s.RecentSelfPositionData)
                                         .Select(t => new Point(t.PositionX , t.PositionY))
                                         .ToReactiveProperty().AddTo(this.Disposable);


            // 接続コマンド押下イベントを定義
            ConnectCommand = SerialConnected
                .CombineLatest(SelectPortName, (connected, selectPort) =>
                 !connected && !string.IsNullOrEmpty(selectPort)) // 未接続かつシリアルポート選択済み
                .ToReactiveCommand().AddTo(this.Disposable);
            ConnectCommand.Subscribe(_ =>
            {
                Serial.StartSerial(SelectPortName.Value);
                RobotController.Serial = Serial.Serial;
            });
            
            // 切断コマンドはシリアル通信が接続中のみ実行可能なコマンドとして定義する
            DisconnectCommand = SerialConnected.ToReactiveCommand().AddTo(this.Disposable);
            DisconnectCommand.Subscribe(_ => {
               Serial.StopSerial();
            });


            var rule = new BlockMoveRule(BlockRobotModel, BlockField);
            
            // 初期位置コードを求める
            DecodeCommand = InitPostionCode.Select(c =>  c < 99999)
                .ToReactiveCommand().AddTo(this.Disposable);
            DecodeCommand.Subscribe( _ =>
            {
                // 運搬経路は初期化
                BlockField.ApproachWayPointArray = new Point[0];
                BlockField.MoveBlockWayPointArray = new Point[0];

                BlockField.SetBlockPosition(InitPostionCode.Value, 0);
                BlockRobotModel.ResetPosition();
                rule = new BlockMoveRule(BlockRobotModel, BlockField);
            });

            // 運搬コマンドの経路を登録する
            ApproachWay = BlockField.ObserveProperty(b => b.ApproachWayPointArray)
                             .ToReactiveProperty().AddTo(this.Disposable);

            MoveBlockWay = BlockField.ObserveProperty(b => b.MoveBlockWayPointArray)
                             .ToReactiveProperty().AddTo(this.Disposable);

            // ブロック情報更新
            NextPositionCommand = InitPostionCode.Select(c => c < 99999)
                                                 .ToReactiveCommand().AddTo(this.Disposable);

            NextPositionCommand.Subscribe(_ => rule.Update(BlockRobotModel, BlockField));

            // ブロック運搬ルール送信
            SendRuleCommand = SerialConnected.ToReactiveCommand().AddTo(this.Disposable);
            SendRuleCommand.Subscribe(_ =>
            {
                var data = rule.Serialize();
                Serial.WriteByteData(COMMAND.BLOCK_MOVE_RULE_COMMNAD, data);
            });


            // PIDゲインデータの通信を登録
            PIDPowerData = PID.ObserveProperty(p => p.Power).ToReactiveProperty().AddTo(this.Disposable);
            PIDPGainData = PID.ObserveProperty(p => p.PGain).ToReactiveProperty().AddTo(this.Disposable);
            PIDIGainData = PID.ObserveProperty(p => p.IGain).ToReactiveProperty().AddTo(this.Disposable);
            PIDDGainData = PID.ObserveProperty(p => p.DGain).ToReactiveProperty().AddTo(this.Disposable);

            // 初期値を格納
            PID.Power =15;
            PID.PGain = 0.14f;
            PID.DGain = 0.1f;

            // 200ms値が確定したら、データを送信
            void sendData()
            {
                // 接続中以外は何もしない
                if (!Serial.Connected)
                    return;

                // PC制御中なら、内部のコントローラにPIDパラメータを反映
                SetControllerPID();

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


            // ロボット制御クラスを初期化する
            RobotController = new RobotControl();
            PCControlRobot = RobotController.ToReactivePropertyAsSynchronized(r => r.Running)
                                       .AddTo(this.Disposable);

            SetControllerPID();

        }

        /// <summary>
        /// 自身の保持するロボット制御コントローラにPID値をセットする
        /// </summary>
        private void SetControllerPID()
        {
            var pidList = new List<RobotController.GameStrategy.PIDParametor>()
                    {
                        new RobotController.GameStrategy.PIDParametor { StateNo = 0,
                                                                        Power = PIDPowerData.Value,
                                                                        PGain = PIDPGainData.Value,
                                                                        IGain = PIDIGainData.Value,
                                                                        DGain = PIDDGainData.Value }
                    };
            RobotController.SetPIDParametor(pidList);
            
        }

        /// <summary>
        /// オブジェクト解放時には、シリアルポートを停止する
        /// </summary>
        public void  Dispose()
        {
            this.Disposable.Dispose();

            if (Serial != null)
                Serial.StopSerial();

            RobotController.ThreadStop();
        }

    }
}
