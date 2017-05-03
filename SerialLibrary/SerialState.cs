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
                default:
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
                Context.OutputReceived = true;
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
                Context.InputReceived = true;
            }

            Context.CurrentState = new HeaderState(Context);
        }
    }

}
