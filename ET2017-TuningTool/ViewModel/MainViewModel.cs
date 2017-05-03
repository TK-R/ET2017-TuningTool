using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;


namespace ET2017_TuningTool
{
    public class MainViewModel : BindableBase
    {
        public ReactiveCommand command { get; }

        private string[] _SerialPortNames;
        public string[] SerialPortNames
        {
            get => _SerialPortNames;
            set => SetProperty(ref _SerialPortNames, value);
        }

        private string _SelectedPortName;
        public string SelectedPortName
        {
            get => _SelectedPortName;
            set => SetProperty(ref _SelectedPortName, value);
        }

        public MainViewModel()
        {
            SerialPortNames =  SerialPort.GetPortNames();
            

        }

    }
}
