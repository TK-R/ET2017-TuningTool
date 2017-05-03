using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET2017_TuningTool
{
    public class EV3Model : BindableBase
    {
        private int _state;
        public int State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }
    }
}
