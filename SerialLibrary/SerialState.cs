using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SerialLibrary
{

    public abstract class SerialState
    {
        internal SerialManager Context;
        internal List<byte> buff = new List<byte>();

        internal SerialState(SerialManager manager)
        {
            Context = manager;
        }

        internal abstract void Receive(byte data);


    }

    public class HeaderState : SerialState
    {
        internal HeaderState(SerialManager manager) : base(manager) {}

        internal override void Receive(byte data)
        {
            if(data == 0xff) Context.CurrentState = new HeaderDataState(Context);
        }

        public override string ToString()
        {
            return "ヘッダ受信中";
        }
    }

    public class HeaderDataState : SerialState
    {
        internal HeaderDataState(SerialManager manager) : base(manager) { buff.Add(0xff); }
        internal override void Receive(byte data)
        {
            buff.Add(data);
            if (buff.Count < Marshal.SizeOf(typeof(Header))) return;

            // ヘッダデータ受信完了のため、構造体を構築
            var header = DataTools.RawDeserialize<Header>(buff.ToArray(), 0);

            switch ((COMMAND)header.Command)
            {
                case COMMAND.INPUT_DATA_COMMAND:
                    Context.CurrentState = new InputDataState(Context);
                    break;
                case COMMAND.OUTPUT_DATA_COMMAND:
                    Context.CurrentState = new OutputDataState(Context);
                    break;
                case COMMAND.PID_DATA_COMMAND:
                    Context.CurrentState = new PIDDataState(Context);
                    break;
                case COMMAND.SELF_POSITION_DATA_COMMAND:
                    Context.CurrentState = new SelfPositionDataState(Context);
                    break;
                case COMMAND.HSL_COLOR_DATA_COMMAND:
                    Context.CurrentState = new HSLDataState(Context);
                    break;
                default:
                    Context.CurrentState = new HeaderState(Context);
                    break;
            }
        }

        public override string ToString()
        {
            return "ヘッダデータ受信中";
        }
    }

    internal class OutputDataState : SerialState
    {
        internal OutputDataState(SerialManager manager) : base(manager) { }

        internal override void Receive(byte data)
        {
            buff.Add(data);
            if (buff.Count < Marshal.SizeOf(typeof(OutputSignalData)) + 1) return;

            if (buff.Last() == (byte)buff.Take(Marshal.SizeOf(typeof(OutputSignalData))).Sum(t => t))
            {
                // チェックサムが一致したら、構造体を構築
                Context.RecentOutputSignal = DataTools.RawDeserialize<OutputSignalData>(buff.ToArray(), 0);
                Context.OutputReceivedFlag = true;
            }

            Context.CurrentState = new HeaderState(Context);
        }
    }

    internal class InputDataState : SerialState
    {
        internal InputDataState(SerialManager manager) : base(manager) { }
        internal override void Receive(byte data)
        {
            buff.Add(data);
            if (buff.Count < Marshal.SizeOf(typeof(InputSignalData)) + 1) return;

            if (buff.Last() == (byte)buff.Take(Marshal.SizeOf(typeof(InputSignalData))).Sum(t => t))
            {
                // チェックサムが一致したら、構造体を構築
                Context.RecentInputSignal = DataTools.RawDeserialize<InputSignalData>(buff.ToArray(), 0);
                Context.InputReceivedFlag = true;
            }

            Context.CurrentState = new HeaderState(Context);
        }
    }

    internal class PIDDataState : SerialState
    {
        internal PIDDataState(SerialManager manager) : base(manager) { }
        internal override void Receive(byte data)
        {
            buff.Add(data);
            if (buff.Count < Marshal.SizeOf(typeof(PIDData)) + 1) return;

            if (buff.Last() == (byte)buff.Take(Marshal.SizeOf(typeof(PIDData))).Sum(t => t))
            {
                // チェックサムが一致したら、構造体を構築
                Context.RecentPIDData = DataTools.RawDeserialize<PIDData>(buff.ToArray(), 0);
            }

            Context.CurrentState = new HeaderState(Context);
        }
    }


    internal class SelfPositionDataState : SerialState
    {
        internal SelfPositionDataState(SerialManager manager) : base(manager) { }
        internal override void Receive(byte data)
        {
            buff.Add(data);
            if (buff.Count < Marshal.SizeOf(typeof(SelfPositionData)) + 1) return;

            if (buff.Last() == (byte)buff.Take(Marshal.SizeOf(typeof(SelfPositionData))).Sum(t => t))
            {
                // チェックサムが一致したら、構造体を構築
                Context.RecentPositionData = DataTools.RawDeserialize<SelfPositionData>(buff.ToArray(), 0);
            }

            Context.CurrentState = new HeaderState(Context);
        }
    }


    internal class HSLDataState : SerialState
    {
        internal HSLDataState(SerialManager manager) : base(manager) { }
        internal override void Receive(byte data)
        {
            buff.Add(data);
            if (buff.Count < Marshal.SizeOf(typeof(HSLColorData)) + 1) return;

            if (buff.Last() == (byte)buff.Take(Marshal.SizeOf(typeof(HSLColorData))).Sum(t => t))
            {
                // チェックサムが一致したら、構造体を構築
                Context.RecentHSLColorData = DataTools.RawDeserialize<HSLColorData>(buff.ToArray(), 0);
            }

            Context.CurrentState = new HeaderState(Context);
        }
    }

}
