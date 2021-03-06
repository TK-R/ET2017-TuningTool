﻿using SerialLibrary;
using System;
using System.Windows.Media;
namespace ET2017_TuningTool.Model.GraphModel
{
    public abstract class OutputValueModel : AbstractValueModel
    {
        /// <summary>
        /// 出力値モデルの一覧
        /// </summary>
        public static Type[] OutputValueType = new Type[]
        {
            typeof(Motor1Power),
            typeof(Motor2Power),
            typeof(Motor3Power),
            typeof(Motor4Power),
        };

        internal override double GetStructValue(object dataStruct)
        {
            if (dataStruct is  OutputSignalData data)
                return GetOutputValue(data);
            else
                throw new ApplicationException("型が不一致です");
        }

        internal abstract double GetOutputValue(OutputSignalData data);
    }

    /// <summary>
    /// モータ1パワー
    /// </summary>
    public class Motor1Power : OutputValueModel
    {
        public Motor1Power()
        {
            Brush.Color = (Color)ColorConverter.ConvertFromString("#E91E63");
            MaxValue = 100;
            MinValue = -100;
            Name = "左モータ出力";
            Unit = "%";
        }

        internal override double GetOutputValue(OutputSignalData data)
        {
            return data.Motor1Power;
        }
    }

    /// <summary>
    /// モータ2パワー
    /// </summary>
    public class Motor2Power : OutputValueModel
    {
        public Motor2Power()
        {
            Brush.Color = (Color)ColorConverter.ConvertFromString("#795548");
            MaxValue = 100;
            MinValue = -100;
            Name = "右モータ出力";
            Unit = "%";
        }

        internal override double GetOutputValue(OutputSignalData data)
        {
            return data.Motor2Power;
        }
    }
    /// <summary>
    /// モータ1パワー
    /// </summary>
    public class Motor3Power : OutputValueModel
    {
        public Motor3Power()
        {
            Brush.Color = (Color)ColorConverter.ConvertFromString("#34495e");
            MaxValue = 100;
            MinValue = -100;
            Name = "アームモータ出力";
            Unit = "%";
        }

        internal override double GetOutputValue(OutputSignalData data)
        {
            return data.Motor3Power;
        }
    }
    /// <summary>
    /// モータ1パワー
    /// </summary>
    public class Motor4Power : OutputValueModel
    {
        public Motor4Power()
        {
            Brush.Color = (Color)ColorConverter.ConvertFromString("#f39c12");
            MaxValue = 100;
            MinValue = -100;
            Name = "尻尾モータ出力"; 
            Unit = "%";
        }

        internal override double GetOutputValue(OutputSignalData data)
        {
            return data.Motor4Power;
        }
    }

}
