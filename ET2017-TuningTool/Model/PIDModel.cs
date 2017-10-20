using Microsoft.Practices.Prism.Mvvm;
using SerialLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET2017_TuningTool.Model
{
    public class PIDModel : BindableBase
    {
        /// <summary>
        /// PID制御に用いるベース出力
        /// </summary>
        private float _power;
        public float Power
        {
            get { return _power; }
            set { SetProperty(ref _power, value); }
        }

        /// <summary>
        /// PID制御に用いる比例ゲイン
        /// </summary>
        private float _pGain;
        public float PGain
        {
            get { return _pGain; }
            set { SetProperty(ref _pGain, value); }
        }

        /// <summary>
        /// PID制御に用いる積分ゲイン
        /// </summary>
        private float _iGain;
        public float IGain
        {
            get { return _iGain; }
            set { SetProperty(ref _iGain, value); }
        }

        /// <summary>
        /// PID制御に用いる微分ゲイン
        /// </summary>
        private float _dGain;
        public float DGain
        {
            get { return _dGain; }
            set { SetProperty(ref _dGain, value); }
        }

        /// <summary>
        /// PID制御に用いるステアリング値
        /// </summary>
        private sbyte _steering;
        public sbyte Steering
        {
            get { return _steering; }
            set { SetProperty(ref _steering, value); }
        }

        /// <summary>
        /// どの状態で用いるPIDゲインかを示すステート番号
        /// </summary>
        private int _stateNo;
        public int StateNo
        {
            get { return _stateNo; }
            set { SetProperty(ref _stateNo, value); }
        }

        private string _stateName;
        /// <summary>
        /// 度の状態で用いるPIDゲインかを示す名称
        /// </summary>
        public string StateName
        {
            get { return _stateName; }
            set { SetProperty(ref _stateName, value); }
        }

        /// <summary>
        /// 受信した電文データを元に保持している情報を更新する
        /// </summary>
        /// <param name="p"></param>
        public void UpdatePID(PIDData p)
        {
            Power = p.BasePower;
            PGain = p.PGain;
            IGain = p.IGain;
            DGain = p.DGain;
            Steering = p.Steering;
            StateNo = p.State;
        }

        /// <summary>
        /// ソースコードに記載するための文字列を返す
        /// </summary>
        /// <returns></returns>
        public string GetHeaderText()
        {
            return "PIDData line" + StateName + " = {" +
                Power + ", " + PGain + ", " + IGain + ", " + DGain + ", " + StateName + ", " + Steering + "};" + Environment.NewLine +
                "SetPIDData(line" + StateName + ");";
        }
    }
}
