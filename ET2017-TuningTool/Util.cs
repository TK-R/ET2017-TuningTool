using System;
using System.Linq;
using System.Reactive.Linq;
using System.Collections.Generic;

namespace ET2017_TuningTool
{
    public static class Util
    {
        public static IObservable<T> SkipTime<T>(this IObservable<T> source, TimeSpan interval) => source
                           .Publish(x => x.Window(() => x.Take(1).Delay(interval)))
                           .SelectMany(x => x.Take(1));

    }
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// 最小値を持つ要素を返します
        /// </summary>
        public static TSource FindMin<TSource, TResult>(
            this IEnumerable<TSource> self,
            Func<TSource, TResult> selector)
        {
            return self.First(c => selector(c).Equals(self.Min(selector)));
        }

        /// <summary>
        /// 最大値を持つ要素を返します
        /// </summary>
        public static TSource FindMax<TSource, TResult>(
            this IEnumerable<TSource> self,
            Func<TSource, TResult> selector)
        {
            return self.First(c => selector(c).Equals(self.Max(selector)));
        }
    }
}
