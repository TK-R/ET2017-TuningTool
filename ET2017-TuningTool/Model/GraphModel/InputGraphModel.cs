using System;
using SerialLibrary;

namespace ET2017_TuningTool.Model.GraphModel
{
    public abstract class InputGraphModel : AbstractGraphModel
    {
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
    public class SonarGraphModel : InputGraphModel
    {
        public SonarGraphModel() {
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
    public class ReflectedLightGraphModel : InputGraphModel
    {
    public ReflectedLightGraphModel()
        {
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
    public class BatteryVoltageModel : InputGraphModel
    {
        public BatteryVoltageModel()
        {
            MaxValue = 10000;
            MinValue = 5000;
            Name = "バッテリ電圧";
            Unit = "V";
        }

        internal override double GetInputValue(InputSignalData data)
        {
            return data.BatteryVoltage;
        }
    }

    /// <summary>
    /// バッテリ電圧
    /// </summary>
    public class BatteryCurrentModel : InputGraphModel
    {
        public BatteryCurrentModel()
        {
            MaxValue = 10000;
            MinValue = 0;
            Name = "バッテリ電流";
            Unit = "A";
        }

        internal override double GetInputValue(InputSignalData data)
        {
            return data.BatteryCurrent;
        }
    }

    /// <summary>
    /// 温度
    /// </summary>
    public class TemparetureModel : InputGraphModel
    {
        public TemparetureModel()
        {
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
}
