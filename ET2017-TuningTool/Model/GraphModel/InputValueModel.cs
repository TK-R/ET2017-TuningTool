﻿using System;
using SerialLibrary;
using System.Windows.Media;

namespace ET2017_TuningTool.Model.GraphModel
{
    public abstract class InputValueModel : AbstractValueModel
    {
        public static Type[] InputValueType = new Type[]
        {
            typeof(SonarGraphModel), // 超音波センサ
            typeof(ReflectedLightGraphModel), // 反射光
            typeof(BatteryVoltageModel), // バッテリ電圧
            typeof(BatteryCurrentModel), // バッテリ電流
            typeof(TemparetureModel), // 温度
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
            Brush.Color = Colors.Blue;
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
            Brush.Color = Colors.Yellow;
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
            Brush.Color = Colors.Green;
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
            Brush.Color = Colors.Purple;
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
