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
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;
using System.Reactive.Subjects;
using System.Reactive;

namespace ET2017_TuningTool
{
    public enum PIDStateNo
    {
        LineTraceStraight = 1,
        pR_B,
        pR_C,
        pR_D,
        pR_E,
        pL_B,
        pL_C,
        pL_D,
        pL_E,
        pL_F,
        pL_G,
        LastStraight,
        BlockMovePIDState = 30,
        BlockMoveHighPIDState,
        ETSumoPIDState,
        ETSumoHighPIDState,
        ETTrainSlow,
        ETTrainHigh,
        ForwardPID = 99,
    }


    public class MainViewModel : BindableBase, IDisposable
    {
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

        /// <summary>
        /// Bluetooth送信開始コマンド
        /// </summary>
        public ReactiveCommand BluetoothONCommand { get; private set; }

        /// <summary>
        /// Bluetooth送信停止コマンド
        /// </summary>
        public ReactiveCommand BluetoothOFFCommnad { get; private set; }

        /// <summary>
        /// 運搬ルール送信コマンド
        /// </summary>
        public ReactiveCommand SendRuleCommand { get; private set; }

        /// <summary>
        /// PID情報送信コマンド
        /// </summary>
        public ReactiveCommand PIDSendCommand { get; private set; }

        /// <summary>
        /// PID情報コピーコマンド
        /// </summary>
        public ReactiveCommand PIDCopyCommand { get; private set; }

        #endregion

        #region シリアルポート関係

        SerialModel Serial = new SerialModel();

        public ReactiveProperty<string> SelectPortName { get; set; } = new ReactiveProperty<string>();

        public ReadOnlyReactiveProperty<string[]> SerialPortNames { get; set; }

        public ReactiveProperty<bool> SerialConnected { get; }
        #endregion

        #region 自己位置推定データ
        /// <summary>
        /// 自己位置推定結果の描画に用いる走行体情報クラス
        /// </summary>
        public EV3Model SelfPositionRobotModel { get; set; } = new EV3Model();

        /// <summary>
        /// 自己位置推定結果におけるロボットの位置情報(描画用)
        /// </summary>
        public ReactiveProperty<Point> SelfPositionRobotPos { get; }

        /// <summary>
        /// 自己位置推定結果におけるロボットの位置情報
        /// </summary>
        public ReactiveProperty<Point> SelfPositionRobotPosRaw { get; }

        /// <summary>
        /// 自己位置推定結果におけるロボットの角度情報(描画用)
        /// </summary>
        public ReactiveProperty<int> SelfPositionRobotAngle { get; }

        /// <summary>
        /// 自己位置推定結果によるロボットの角度情報
        /// </summary>
        public ReactiveProperty<int> SelfPositionRobotAngleRaw { get; }

        /// <summary>
        /// 自己位置推定結果によるロボットの総走行距離
        /// </summary>
        public ReactiveProperty<uint> SelfPositionRobotDistanceRaw { get; }

        #endregion

        #region ブロック並べ攻略データ

        /// <summary>
        /// 初期配置コード
        /// </summary>
        public ReactiveProperty<int> InitPostionCode { get; set; } = new ReactiveProperty<int>(12085);

        public ReactiveProperty<int> GreenBlockPos { get; set; } = new ReactiveProperty<int>(8);
        /// <summary>
        /// フィールドのブロック情報管理クラス
        /// </summary>
        BlockFieldModel BlockField = new BlockFieldModel();
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
        /// ブロック並べフィールドにおけるロボットの位置情報
        /// </summary>
        public ReactiveProperty<Point> BlockRobotPos { get; }
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
        /// センサカラー値から生成したブラシ
        /// </summary>
        public ReadOnlyReactiveProperty<SolidColorBrush> SensorColor { get; }
        /// <summary>
        /// HSLカラー情報(色相)データ
        /// </summary>
        public ReadOnlyReactiveProperty<float> HueData { get; }
        /// <summary>
        /// HSLカラー情報(彩度)データ
        /// </summary>
        public ReadOnlyReactiveProperty<float> SaturationData { get; }
        /// <summary>
        /// HSLカラー情報(輝度)データ
        /// </summary>
        public ReadOnlyReactiveProperty<float> LuminosityData { get; }
        /// <summary>
        /// HSLカラー情報種別データ
        /// </summary>
        public ReadOnlyReactiveProperty<string> HSLKindString { get; }
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

        #region 走行体制御

        public ReactiveProperty<PIDStateNo> SelectedStateNo { get; } = new ReactiveProperty<PIDStateNo>(PIDStateNo.LineTraceStraight);

        public ReactiveProperty<PIDListModel> PIDList { get; } = new ReactiveProperty<PIDListModel>(PIDListModel.LoadFromFile(Environment.CurrentDirectory + "\\recent.xml"));

        public ReactiveProperty<PIDModel> PIDData { get; } = new ReactiveProperty<PIDModel>(new PIDModel { PGain = 1.0f });
        
        /// <summary>
        /// 変更通知送信する際の目標座標（X）
        /// </summary>
        public ReactiveProperty<uint> TargetPositionX { get; } = new ReactiveProperty<uint>(5190);
        /// <summary>
        /// 変更通知送信する際の目標座標（Y）
        /// </summary>
        public ReactiveProperty<uint> TargetPositionY { get; } = new ReactiveProperty<uint>(430);
        /// <summary>
        /// 変更通知送信する際の目標角度
        /// </summary>
        public ReactiveProperty<ushort> TargetAngle { get; } = new ReactiveProperty<ushort>(270);
        /// <summary>
        /// 変更通知送信する際の目標総距離(mm)
        /// </summary>
        public ReactiveProperty<uint> TargetDistance { get; } = new ReactiveProperty<uint>(0);
        /// <summary>
        /// 自己位置再設定コマンド送信処理
        /// </summary>
        public ReactiveCommand PositionResetCommand { get; private set; }
        /// <summary>
        /// 画面にバインドされる画像名称
        /// </summary>
        public ReactiveProperty<string> ImageName { get; set; }
        /// <summary>
        /// コースを強調するか否か
        /// </summary>
        public ReactiveProperty<bool> EmphasizesCourse { get; set; } = new ReactiveProperty<bool>(true);
        /// <summary>
        /// 画像ソースのヘッダ文字列
        /// </summary>
        private readonly string NameHeader = "/ET2017-TuningTool;component/Image/Field";
        #endregion

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
            SensorColor = Serial.ObserveProperty(s => s.RecentInputSignalData)
                                .ObserveOnDispatcher() // UIスレッドに戻す
                                .Select(v => new SolidColorBrush(Color.FromArgb(255, (byte)v.ColorR, (byte)v.ColorG, (byte)v.ColorB)))
                                .ToReadOnlyReactiveProperty().AddTo(this.Disposable);

            // HSL情報の表示
            HueData = Serial.ObserveProperty(s => s.RecentHSLColorData).Select(v => v.Hue).ToReadOnlyReactiveProperty().AddTo(this.Disposable);
            SaturationData = Serial.ObserveProperty(s => s.RecentHSLColorData).Select(v => v.Saturation).ToReadOnlyReactiveProperty().AddTo(this.Disposable);
            LuminosityData = Serial.ObserveProperty(s => s.RecentHSLColorData).Select(v => v.Luminosity).ToReadOnlyReactiveProperty().AddTo(this.Disposable);
            HSLKindString = Serial.ObserveProperty(s => s.RecentHSLColorData)
                                  .Select(v => Enum.GetName(typeof(HSLKindEnum), v.HSLKind)).ToReadOnlyReactiveProperty().AddTo(this.Disposable);

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

            // 自己位置情報のログ出力の登録
            Serial.ObserveProperty(s => s.RecentSelfPositionData)
                  .Subscribe(r => LogWriteModel.Write(r));

            // PIDの更新を登録
            Serial.ObserveProperty(s => s.RecentPIDData)
                  .Subscribe(r =>
            {
                PIDData.Value.UpdatePID(r);
            });

            //自己位置情報の更新を登録（描画用）
            SelfPositionRobotPos = Serial.ObserveProperty(s => s.RecentSelfPositionData)
                                         .Select(t => new Point((t.PositionX - 240) / 5510.0 * 451.0, (t.PositionY - 100) / 3722.0 * 296.0))
                                         .ToReactiveProperty().AddTo(this.Disposable);

            SelfPositionRobotAngle = Serial.ObserveProperty(s => s.RecentSelfPositionData)
                                         .Select(t => (int)t.Angle * -1)
                                         .ToReactiveProperty().AddTo(this.Disposable);

            //自己位置情報の更新を登録（生データ）
            SelfPositionRobotPosRaw = Serial.ObserveProperty(s => s.RecentSelfPositionData)
                                         .Select(t => new Point(t.PositionX, t.PositionY))
                                         .ToReactiveProperty().AddTo(this.Disposable);
            SelfPositionRobotAngleRaw = Serial.ObserveProperty(s => s.RecentSelfPositionData)
                             .Select(t => (int)t.Angle)
                             .ToReactiveProperty().AddTo(this.Disposable);
            SelfPositionRobotDistanceRaw = Serial.ObserveProperty(s => s.RecentSelfPositionData)
                                                 .Select(t => (uint)t.Distance)
                                                 .ToReactiveProperty().AddTo(this.Disposable);


            // 接続コマンド押下イベントを定義
            ConnectCommand = SerialConnected
                .CombineLatest(SelectPortName, (connected, selectPort) =>
                 !connected && !string.IsNullOrEmpty(selectPort)) // 未接続かつシリアルポート選択済み
                .ToReactiveCommand().AddTo(this.Disposable);
            ConnectCommand.Subscribe(_ =>
            {
                Serial.StartSerial(SelectPortName.Value);
            });

            // 切断コマンドはシリアル通信が接続中のみ実行可能なコマンドとして定義する
            DisconnectCommand = SerialConnected.ToReactiveCommand().AddTo(this.Disposable);
            DisconnectCommand.Subscribe(_ =>
            {
                Serial.StopSerial();
            });

            var rule = new BlockMoveRule(BlockRobotModel, BlockField);

            // 初期位置コードを求める
            DecodeCommand = InitPostionCode.Select(c => c < 99999)
                .ToReactiveCommand().AddTo(this.Disposable);
            DecodeCommand.Subscribe(_ =>
           {
                // 運搬経路は初期化
                BlockField.ApproachWayPointArray = new Point[0];
               BlockField.MoveBlockWayPointArray = new Point[0];

               BlockField.SetBlockPosition(InitPostionCode.Value, GreenBlockPos.Value);
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
                // 運搬経路は初期化
                BlockField.ApproachWayPointArray = new Point[0];
                BlockField.MoveBlockWayPointArray = new Point[0];

                BlockField.SetBlockPosition(InitPostionCode.Value, GreenBlockPos.Value);
                BlockRobotModel.ResetPosition();
                rule = new BlockMoveRule(BlockRobotModel, BlockField);

                var data = rule.Serialize();
                Serial.WriteByteData(COMMAND.BLOCK_MOVE_RULE_COMMNAD, data);
            });

            // PIDパラメータ送信コマンドを定義
            PIDSendCommand = SerialConnected.ToReactiveCommand().AddTo(this.Disposable);
            PIDSendCommand.Subscribe(_ =>
            {
                // 接続中以外は何もしない
                if (!Serial.Connected)
                    return;

                Serial.WriteData(new PIDData
                {
                    BasePower = PIDData.Value.Power,
                    PGain = PIDData.Value.PGain,
                    IGain = PIDData.Value.IGain,
                    DGain = PIDData.Value.DGain,
                    Steering = PIDData.Value.Steering,
                    State = PIDData.Value.StateNo

                });
            });

            // PID情報の入れ替え処理
            SelectedStateNo.Subscribe(no => PIDData.Value = PIDList.Value.PIDPrametorArray.Where(p => p.StateNo == (int)no).First());

            // PID情報をクリップボードへコピーする処理を定義
            PIDCopyCommand = new ReactiveCommand();
            PIDCopyCommand.Subscribe(_ =>
                Clipboard.SetText(PIDData.Value.GetHeaderText())
            );

            // 自己位置推定値のリセットコマンドを定義
            PositionResetCommand = SerialConnected.ToReactiveCommand().AddTo(this.Disposable);
            PositionResetCommand.Subscribe(_ =>
            {
                Serial.WriteData(new SelfPositionData
                {
                    PositionX = TargetPositionX.Value,
                    PositionY = TargetPositionY.Value,
                    Angle = TargetAngle.Value,
                    Distance = TargetDistance.Value
                });
            });

            // 送信停止コマンド
            BluetoothOFFCommnad = SerialConnected.ToReactiveCommand().AddTo(this.Disposable);
            BluetoothOFFCommnad.Subscribe(_ =>
                Serial.WriteData(new BluetoothControl { SendON = 0 }));

            // 送信開始コマンド
            BluetoothONCommand = SerialConnected.ToReactiveCommand().AddTo(this.Disposable);
            BluetoothONCommand.Subscribe(_ =>
                Serial.WriteData(new BluetoothControl { SendON = 1 }));

            // 画像再描画
            ImageName = SelectedStateNo.CombineLatest(EmphasizesCourse, (No, flag) =>
            {
                // 強調未指定時または難所選択時には通常画像を表示
                if (flag == false || No > PIDStateNo.LastStraight)
                    return NameHeader + ".png";
                else
                    return NameHeader + "_" + No.ToString() + ".png";

            }).ToReactiveProperty().AddTo(this.Disposable);
        }


        /// <summary>
        /// オブジェクト解放時には、シリアルポートを停止する
        /// </summary>
        public void  Dispose()
        {
            // PIDパラメータを保存する
            PIDList.Value.SaveAsFile(Environment.CurrentDirectory + "\\recent.xml");


            this.Disposable.Dispose();

            if (Serial != null)
                Serial.StopSerial();

        }

    }
}
