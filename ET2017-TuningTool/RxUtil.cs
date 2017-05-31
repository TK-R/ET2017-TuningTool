using System;
using System.Linq;
using System.Reactive.Linq;

namespace ET2017_TuningTool
{
    public static class RxUtil
    {
        public static IObservable<T> SkipTime<T>(this IObservable<T> source, TimeSpan interval) => source
                           .Publish(x => x.Window(() => x.Take(1).Delay(interval)))
                           .SelectMany(x => x.Take(1));

    }
}
