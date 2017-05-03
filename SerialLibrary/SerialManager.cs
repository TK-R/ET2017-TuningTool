using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
//using PInvokeSerialPort;
using System.IO.Ports;
using System.Threading;

namespace SerialLibrary
{
    public class SerialManager
    {
        private static SerialPort Serial;

        public SerialState CurrentState;

        /// <summary>
        /// 最後に受信したの入力電文のデータ領域
        /// </summary>
        internal InputSignalData RecentInputSignal;

        /// <summary>
        /// 最後に受信した送信する出力電文のデータ領域
        /// </summary>
        internal OutputSignalData RecentOutputSignal;

        /// <summary>
        /// シリアル通信でデータを送受信中ならtrueを返す
        /// </summary>
        public bool Runnning { private set; get; }

        /// <summary>
        /// データを受信済ならtrue
        /// 読み込み処理を実施するとfalse
        /// </summary>
        public bool InputReceived { internal set; get; }

        /// <summary>
        /// データを受信済みならtrue
        /// 読み込み処理を実施するとfalse
        /// </summary>
        public bool OutputReceived { internal set; get; }

        /// <summary>
        /// 無限ループで動作する受信スレッド
        /// </summary>
        private Thread ReceiveThread;
        
        /// <summary>
        /// ポート名称を指定してシリアル通信を開始する
        /// 既に開始済みの場合にもtrueを返す
        /// </summary>
        /// <param name="portName">ポート名</param>
        /// <returns>成否</returns>
        public bool Start(string portName)
        {
            // 既に動作中なら何もしない
            if (Runnning)
                return true;

            CurrentState = new HeaderState(this);
            
            try
            {
                Serial = new SerialPort(portName, 115200, Parity.Odd, 8, StopBits.One);
                Serial.Open();

                // 受信スレッドを起動
                ReceiveThread = new Thread(Receive);
                ReceiveThread.Start();
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                return false;
            }

            Runnning = true;

            return true;

        }

        /// <summary>
        /// シリアル受信を停止して、受信スレッドも停止する
        /// </summary>
        public void Stop()
        {
            Runnning = false;
            ReceiveThread.Abort();
            InputReceived = false;
            OutputReceived = false;
            Serial.Close();
        }

        private void Receive()
        {
            var buff = new byte[1024];
            
            while (true)
            {
                try
                {
                    var readSize = Serial.Read(buff, 0, buff.Count());

                    for (int i = 0; i < readSize; i++)
                        CurrentState.Receive(buff[i]);
                }catch(TimeoutException)
                {
                }
            }
        }
        
  

        /// <summary>
        /// 電文送信処理
        /// 本メソッドの呼び出しで即時に呼ばれる
        /// </summary>
        /// <typeparam name="StructT"></typeparam>
        /// <param name="dataStruct"></param>
        /// <returns></returns>
        public bool WriteData<StructT>(StructT dataStruct)
        {
            if (!Runnning)
                return false;

            try
            {
                int size;
                COMMAND command;

                // データ領域の型に応じてヘッダを生成する
                if (dataStruct is InputSignalData)
                {
                    size = Marshal.SizeOf(typeof(InputSignalData));
                    command = COMMAND.INPUT_DATA_COMMAND;
                }
                else if (dataStruct is OutputSignalData)
                {
                    size = Marshal.SizeOf(typeof(OutputSignalData));
                    command = COMMAND.OUTPUT_DATA_COMMAND;
                }
                else
                {
                    throw new ApplicationException("データ領域型エラー");
                }

                var header = new Header { Head = 0xff, Command = (byte)command, Size = (ushort)size };
                
                // シリアル送信のため、バイナリにシリアライズ
                var headBin = DataTools.RawSerialize(header);
                var dataBin = DataTools.RawSerialize(dataStruct);

                // チェックサムの算出
                byte sum = (byte)(dataBin.Sum(s => s));

                // 送信データの結合
                List<byte> sendData = headBin.ToList();
                sendData.AddRange(dataBin);
                sendData.Add(sum);

                Serial.Write(sendData.ToArray(), 0, sendData.Count);

            }
            catch (IOException)
            {
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// 保持している最新の入力信号電文を返す
        /// </summary>
        /// <param name="input">入力信号電文のデータ領域</param>
        /// <returns>成否</returns>
        public bool ReadData(out InputSignalData input)
        {
            input = RecentInputSignal;
            // TODO 受信フラグの操作

            if (InputReceived)
            {
                InputReceived = false;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 保持している最新の出力信号電文を返す
        /// </summary>
        /// <param name="output">出力信号電文のデータ領域</param>
        /// <returns>成否</returns>
        public bool ReadData(out OutputSignalData output)
        {
            output = RecentOutputSignal;
            // TODO 受信フラグの操作

            if (OutputReceived)
            {
                OutputReceived = false;
                return true;
            }

            return false;

        }

    }

}
