using SerialLibrary;
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
            Brush.Color = Colors.Red;
            MaxValue = 100;
            MinValue = -100;
            Name = "モータ1出力";
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
            Brush.Color = Colors.Blue;
            MaxValue = 100;
            MinValue = -100;
            Name = "モータ2出力";
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
            Brush.Color = Colors.Yellow;
            MaxValue = 100;
            MinValue = -100;
            Name = "モータ3出力";
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
            Brush.Color = Colors.Green;
            MaxValue = 100;
            MinValue = -100;
            Name = "モータ4出力";
            Unit = "%";
        }

        internal override double GetOutputValue(OutputSignalData data)
        {
            return data.Motor4Power;
        }
    }

}
