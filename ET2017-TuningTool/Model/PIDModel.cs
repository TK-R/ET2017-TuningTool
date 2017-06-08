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
        /// 受信した電文データを元に保持していうｒ情報を更新する
        /// </summary>
        /// <param name="p"></param>
        public void UpdatePID(PIDData p)
        {
            Power = p.BasePower;
            PGain = p.PGain;
            IGain = p.IGain;
            DGain = p.DGain;
        }
    }
}
