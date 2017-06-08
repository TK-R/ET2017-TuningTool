using System.Runtime.InteropServices;

namespace SerialLibrary
{
    /// <summary>
    /// 電文コード
    /// </summary>
    public enum COMMAND {
        /// <summary>
        /// 入力信号電文
        /// </summary>
        INPUT_DATA_COMMAND = 0x01, 
        /// <summary>
        /// 出力信号電文
        /// </summary>
        OUTPUT_DATA_COMMAND = 0x02,
        /// <summary>
        /// PID電文
        /// </summary>
        PID_DATA_COMMAND = 0x10,
    };


    /// <summary>
    /// 電文共通ヘッダ領域
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 4, Pack = 1)]
    public struct Header
    {
        /// <summary>
        /// ヘッダコード = 0xFF固定
        /// </summary>
        [FieldOffset(0)]
        public byte Head;
        /// <summary>
        /// 電文サイズ（ヘッダを除く）
        /// </summary>
        [FieldOffset(1)]
        public ushort Size;
        /// <summary>
        /// 電文コード
        /// </summary>
        [FieldOffset(3)]
        public byte Command;
    }

    /// <summary>
    /// 入力信号電文データ領域
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 30, Pack = 1)]
    public struct InputSignalData
    {
        /// <summary>
        /// モータ1の角位置
        /// </summary>
        [FieldOffset(0)]
        public short Motor1Radian;
        /// <summary>
        /// モータ2の角位置
        /// </summary>
        [FieldOffset(2)]
        public short Motor2Radian;
        /// <summary>
        /// モータ3の角位置
        /// </summary>
        [FieldOffset(4)]
        public short Motor3Radian;
        /// <summary>
        /// モータ4の角位置
        /// </summary>
        [FieldOffset(6)]
        public short Motor4Radian;
        /// <summary>
        /// タッチセンサから 0:OFF 1:ON
        /// </summary>
        [FieldOffset(8)]
        public byte TouchSenser;
        /// <summary>
        /// 超音波センサから取得した距離（cm)
        /// </summary>
        [FieldOffset(9)]
        public ushort SonicDistance;
        /// <summary>
        /// カラーセンサから取得したRGB値(R)
        /// </summary>
        [FieldOffset(11)]
        public byte ColorR;
        /// <summary>
        /// カラーセンサから取得したRGB値(R)
        /// </summary>
        [FieldOffset(12)]
        public byte ColorG;
        /// <summary>
        /// カラーセンサから取得したRGB値(R)
        /// </summary>
        [FieldOffset(13)]
        public byte ColorB;
        /// <summary>
        /// 環境光
        /// </summary>
        [FieldOffset(14)]
        public byte EnvironmentalLight;
        /// <summary>
        /// 反射光
        /// </summary>
        [FieldOffset(15)]
        public byte ReflectedLight;
        /// <summary>
        /// /加速度センサから取得した加速度値(X)
        /// </summary>
        [FieldOffset(16)]
        public ushort AxesX;
        /// <summary>
        /// /加速度センサから取得した加速度値(Y)
        /// </summary>
        [FieldOffset(18)]
        public ushort AxesY;
        /// <summary>
        /// /加速度センサから取得した加速度値(Z)
        /// </summary>
        [FieldOffset(20)]
        public ushort AxesZ;
        /// <summary>
        /// 温度センサから取得した温度(摂氏)
        /// </summary>
        [FieldOffset(22)]
        public float Temperature;
        /// <summary>
        /// バッテリ電流(mA)
        /// </summary>
        [FieldOffset(26)]
        public ushort BatteryCurrent;
        /// <summary>
        /// バッテリ電圧(V)
        /// </summary>
        [FieldOffset(28)]
        public ushort BatteryVoltage;
    }

    /// <summary>
    /// 出力信号電文データ領域
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 4, Pack = 1)]
    public struct OutputSignalData
    {
        /// <summary>
        /// 移動用モータのパワー（-100-100%)
        /// </summary>
        [FieldOffset(0)]
        public sbyte Motor1Power;
        /// <summary>
        /// ステアリング +側で右旋回（-100-100%)
        /// </summary>
        [FieldOffset(1)]
        public sbyte Motor2Power;
        /// <summary>
        /// モータ3のパワー（-100-100%)
        /// </summary>
        [FieldOffset(2)]
        public sbyte Motor3Power;
        /// <summary>
        /// モータ4のパワー
        /// </summary>
        [FieldOffset(3)]
        public sbyte Motor4Power;
    }

    /// <summary>
    /// PIDゲインデータ
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 16, Pack = 1)]
    public struct PIDData
    {
        /// <summary>
        /// 基準パワー
        /// </summary>
        [FieldOffset(0)]
        public float BasePower;

        /// <summary>
        /// 比例ゲイン
        /// </summary>
        [FieldOffset(4)]
        public float PGain;

        /// <summary>
        /// 積分ゲイン
        /// </summary>
        [FieldOffset(8)]
        public float IGain;

        /// <summary>
        /// 微分ゲイン
        /// </summary>
        [FieldOffset(12)]
        public float DGain;
    }

    
    [StructLayout(LayoutKind.Explicit, Size = 2, Pack = 1)]
    public struct ApplicationData
    {
        /// <summary>
        /// 現在のステートID
        /// </summary>
        [FieldOffset(0)]
        public ushort State;
    }
}
