using System;
using SerialLibrary;
using System.Windows.Media;

namespace ET2017_TuningTool.Model.GraphModel
{
    public abstract class InputValueModel : AbstractValueModel
    {
        // この順番を変えることで、グラフとリストの表示項目を変更できる
        // 上から5つがグラフに表示される
        public static Type[] InputValueType = new Type[]
        {
            typeof(SonarGraphModel), // 超音波センサ
            typeof(LeftMotorAngle), // 左モータ角度
            typeof(RightMotorAngle), // 右モータ角度
            typeof(ReflectedLightGraphModel), // 反射
            typeof(ControlMillSecond),  // 処理にかかった時間
            typeof(BatteryVoltageModel), // バッテリ電圧
            typeof(BatteryCurrentModel), // バッテリ電流
            typeof(AngularVelocity), // 角速度
            typeof(ColorRValue), // カラーセンサのR
            typeof(ColorGValue), // カラーセンサのG
            typeof(ColorBValue), // カラーセンサのB
        };


        internal override double GetStructValue(object dataStruct)
        {
            if (dataStruct is InputSignalData data)
                return GetInputValue(data);
            else
                throw new ApplicationException("型が不一致です");
        }

        internal abstract double GetInputValue(InputSignalData data);
    }

    /// <summary>
    /// 超音波センサ
    /// </summary>
    public class SonarGraphModel : InputValueModel
    {
        public SonarGraphModel()
        {
            Brush.Color = Colors.Red;
            MaxValue = 255; 
            MinValue = 0;
            Name = "超音波センサ";
            Unit = "cm";
        }

        internal override double GetInputValue(InputSignalData data)
        {
            return data.SonicDistance;
        }
    }

    /// <summary>
    /// 反射光センサ
    /// </summary>
    public class ReflectedLightGraphModel : InputValueModel
    {
        public ReflectedLightGraphModel()
        {
            Brush.Color = (Color)ColorConverter.ConvertFromString("#1abc9c");
            MaxValue = 100;
            MinValue = 0;
            Name = "反射光";
            Unit = "%";
        }

        internal override double GetInputValue(InputSignalData data)
        {
            return data.ReflectedLight;
        }
    }

    /// <summary>
    /// 周期処理にかかった時間
    /// </summary>
    public class ControlMillSecond : InputValueModel
    {
        public ControlMillSecond()
        {
            Brush.Color = (Color)ColorConverter.ConvertFromString("#9b59b6");
            Name = "処理時間";
            MaxValue = 50;
            MinValue = 0;
            Unit = "msec";
        }

        internal override double GetInputValue(InputSignalData data)
        {
            return data.ConrolMillSec;
        }
    }

    /// <summary>
    /// 角速度センサから取得した角位置
    /// </summary>
    public class Angle : InputValueModel
    {
        public Angle()
        {
            Brush.Color = Colors.Transparent;

            Name = "角位置";
            MaxValue = 30;
            MinValue = -30;
            Unit = "deg";
        }

        internal override double GetInputValue(InputSignalData data)
        {
            return data.Angle;
        }
    }
    

    /// <summary>
    /// 角速度センサから取得した角速度
    /// </summary>
    public class AngularVelocity : InputValueModel
    {
        public AngularVelocity()
        {
            Brush.Color = Colors.Transparent;

            Name = "角速度";
            MaxValue = 360;
            MinValue = -360;
            Unit = "deg/s";
        }

        internal override double GetInputValue(InputSignalData data)
        {
            return data.AnglarVelocity;
        }
    }


    /// <summary>
    /// バッテリ電圧
    /// </summary>
    public class BatteryVoltageModel : InputValueModel
    {
        public BatteryVoltageModel()
        {
            Brush.Color = Colors.Transparent;
            MaxValue = 10000;
            MinValue = 0;
            Name = "バッテリ電圧";
            Unit = "mV";
        }

        internal override double GetInputValue(InputSignalData data)
        {
            return data.BatteryVoltage;
        }
    }

    /// <summary>
    /// バッテリ電圧
    /// </summary>
    public class BatteryCurrentModel : InputValueModel
    {
        public BatteryCurrentModel()
        {
            Brush.Color = Colors.Transparent;
            MaxValue = 10000;
            MinValue = 0;
            Name = "バッテリ電流";
            Unit = "mA";
        }

        internal override double GetInputValue(InputSignalData data)
        {
            return data.BatteryCurrent;
        }
    }


    /// <summary>
    /// 左モータの前回からの差分値
    /// </summary>
    public class LeftMotorAngle : InputValueModel
    {
        public LeftMotorAngle()
        {
            Brush.Color = (Color)ColorConverter.ConvertFromString("#e74c3c");
            MaxValue = 50;
            MinValue = -50;
            Name = "左モータ角度";
            Unit = "°";
        }

        internal override double GetInputValue(InputSignalData data)
        {
            return data.Motor1Radian;
        }
    }

    /// <summary>
    /// 右モータの前回からの差分値
    /// </summary>
    public class RightMotorAngle : InputValueModel
    {
        public RightMotorAngle()
        {
            Brush.Color = (Color)ColorConverter.ConvertFromString("#7f8c8d");
            MaxValue = 50; 
            MinValue = -50; 
            Name = "右モータ角度";
            Unit = "°";
        }

        internal override double GetInputValue(InputSignalData data)
        {
            return data.Motor2Radian;
        }
    }
    
    /// <summary>
    /// カラーセンサの赤
    /// </summary>
    public class ColorRValue : InputValueModel
    {
        public ColorRValue()
        {
            Brush.Color = Colors.Transparent;
            MaxValue = 255;
            MinValue = 0;
            Name = "Color-R";
            Unit = "";
        }

        internal override double GetInputValue(InputSignalData data)
        {
            return data.ColorR;
        }
    }

    /// <summary>
    /// カラーセンサの緑
    /// </summary>
    public class ColorGValue : InputValueModel
    {
        public ColorGValue()
        {
            Brush.Color = Colors.Transparent;
            MaxValue = 255;
            MinValue = 0;
            Name = "Color-G";
            Unit = "";
        }

        internal override double GetInputValue(InputSignalData data)
        {
            return data.ColorG;
        }
    }

    /// <summary>
    /// カラーセンサの青
    /// </summary>
    public class ColorBValue : InputValueModel
    {
        public ColorBValue()
        {
            Brush.Color = Colors.Transparent;
            MaxValue = 255;
            MinValue = 0;
            Name = "Color-B";
            Unit = "";
        }

        internal override double GetInputValue(InputSignalData data)
        {
            return data.ColorB;
        }
    }

}
