using System;
using SerialLibrary;
using System.Windows.Media;

namespace ET2017_TuningTool.Model.GraphModel
{
    public abstract class InputValueModel : AbstractValueModel
    {
        // この順番を変えることで、グラフとリストの表示項目を変更できる
        public static Type[] InputValueType = new Type[]
        {
            //         typeof(SonarGraphModel), // 超音波センサ
            typeof(LeftMotorAngle), // 左モータ角度
            typeof(RightMotorAngle), // 右モータ角度
            typeof(ReflectedLightGraphModel), // 反射光
            typeof(BatteryVoltageModel), // バッテリ電圧
            typeof(BatteryCurrentModel), // バッテリ電流
       //     typeof(TemparetureModel), // 温度
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
            MaxValue = 3000; // 仮
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
    /// バッテリ電圧
    /// </summary>
    public class BatteryVoltageModel : InputValueModel
    {
        public BatteryVoltageModel()
        {
            Brush.Color = (Color)ColorConverter.ConvertFromString("#3498db");
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
            Brush.Color = (Color)ColorConverter.ConvertFromString("#9b59b6");
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
    /// 温度
    /// </summary>
    public class TemparetureModel : InputValueModel
    {
        public TemparetureModel()
        {
            Brush.Color = (Color)ColorConverter.ConvertFromString("#95a5a6");
            MaxValue = 50;
            MinValue = 0; 
             Name = "温度";
            Unit = "℃";
        }

        internal override double GetInputValue(InputSignalData data)
        {
            return data.Temperature;
        }
    }

    public class LeftMotorAngle : InputValueModel
    {
        public LeftMotorAngle()
        {
            Brush.Color = (Color)ColorConverter.ConvertFromString("#e74c3c");
            MaxValue = 50;
            MinValue = 0;
            Name = "左モータ角度";
            Unit = "°";
        }

        internal override double GetInputValue(InputSignalData data)
        {
            return data.Motor1Radian;
        }
    }

    public class RightMotorAngle : InputValueModel
    {
        public RightMotorAngle()
        {
            Brush.Color = (Color)ColorConverter.ConvertFromString("#7f8c8d");
            MaxValue = 50; 
             MinValue = 0; 
             Name = "右モータ角度";
            Unit = "°";
        }

        internal override double GetInputValue(InputSignalData data)
        {
            return data.Motor2Radian;
        }
    }



}
