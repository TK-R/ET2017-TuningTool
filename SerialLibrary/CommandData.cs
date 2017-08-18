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
        /// <summary>
        /// ブロック運搬ルール電文
        /// </summary>
        BLOCK_MOVE_RULE_COMMNAD = 0x20, 
        /// <summary>
        /// 自己位置情報電文
        /// </summary>
        SELF_POSITION_DATA_COMMAND = 0x30,
        /// <summary>
        /// HSL情報電文
        /// </summary>
        HSL_COLOR_DATA_COMMAND = 0x31,

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
    [StructLayout(LayoutKind.Explicit, Size = 29, Pack = 1)]
    public struct InputSignalData
    {
        /// <summary>
        /// モータ1の角位置
        /// </summary>
        [FieldOffset(0)]
        public sbyte Motor1Radian;
        /// <summary>
        /// モータ2の角位置
        /// </summary>
        [FieldOffset(1)]
        public sbyte Motor2Radian;
        /// <summary>
        /// モータ3の角位置
        /// </summary>
        [FieldOffset(2)]
        public sbyte Motor3Radian;
        /// <summary>
        /// モータ4の角位置
        /// </summary>
        [FieldOffset(3)]
        public sbyte Motor4Radian;
        /// <summary>
        /// タッチセンサから 0:OFF 1:ON
        /// </summary>
        [FieldOffset(4)]
        public byte TouchSenser;
        /// <summary>
        /// 超音波センサから取得した距離（cm)
        /// </summary>
        [FieldOffset(5)]
        public ushort SonicDistance;
        /// <summary>
        /// カラーセンサから取得したRGB値(R)
        /// </summary>
        [FieldOffset(7)]
        public ushort ColorR;
        /// <summary>
        /// カラーセンサから取得したRGB値(R)
        /// </summary>
        [FieldOffset(9)]
        public ushort ColorG;
        /// <summary>
        /// カラーセンサから取得したRGB値(R)
        /// </summary>
        [FieldOffset(11)]
        public ushort ColorB;
        /// <summary>
        /// 環境光
        /// </summary>
        [FieldOffset(13)]
        public byte EnvironmentalLight;
        /// <summary>
        /// 反射光
        /// </summary>
        [FieldOffset(14)]
        public byte ReflectedLight;
        /// <summary>
        /// 角速度センサから取得した角位置
        /// </summary>
        [FieldOffset(15)]
        public short Angle;
        /// <summary>
        /// 角速度センサから取得した角速度
        /// </summary>
        [FieldOffset(17)]
        public short AnglarVelocity;
        /// <summary>
        /// 予備1
        /// </summary>
        [FieldOffset(19)]
        public ushort reserved1;
        /// <summary>
        /// 予備2
        /// </summary>
        [FieldOffset(21)]
        public float reserved2;
        /// <summary>
        /// バッテリ電流(mA)
        /// </summary>
        [FieldOffset(25)]
        public ushort BatteryCurrent;
        /// <summary>
        /// バッテリ電圧(V)
        /// </summary>
        [FieldOffset(27)]
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
    [StructLayout(LayoutKind.Explicit, Size = 20, Pack = 1)]
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

        /// <summary>
        /// ステート番号
        /// </summary>
        [FieldOffset(16)]
        public int State;
    }

    [StructLayout(LayoutKind.Explicit, Size = 14, Pack = 1)]
    public struct SelfPositionData
    {
        /// <summary>
        /// X座標(mm)
        /// </summary>
        [FieldOffset(0)]
        public uint PositionX;

        /// <summary>
        /// Y座標(mm)
        /// </summary>
        [FieldOffset(4)]
        public uint PositionY;

        /// <summary>
        /// 角度
        /// </summary>
        [FieldOffset(8)]
        public ushort Angle;

        /// <summary>
        /// 総走行距離
        /// </summary>
        [FieldOffset(10)]
        public uint Distance;
    }

    [StructLayout(LayoutKind.Explicit, Size = 13, Pack = 1)]
    public struct HSLColorData
    {
        /// <summary>
        /// 色相
        /// </summary>
        [FieldOffset(0)]
        public float Hue;

        /// <summary>
        /// 彩度
        /// </summary>
        [FieldOffset(4)]
        public float Saturation;

        /// <summary>
        /// 輝度
        /// </summary>
        [FieldOffset(8)]
        public float Luminosity;

        /// <summary>
        /// 色種別
        /// </summary>
        [FieldOffset(12)]
        public byte HSLKind;
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
