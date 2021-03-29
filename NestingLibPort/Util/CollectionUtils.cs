using System;
using System.Collections.Generic;

namespace NestingLibPort.Util
{
    /// <summary>
    /// 集合实用程序。
    /// </summary>
    public static class CollectionUtils
    {
        /// <summary>
        /// 伪随机数发生器。
        /// </summary>
        private static Random rng = new Random();
        /// <summary>
        /// 交换。交换集合中二个索引处的值。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="indexA"></param>
        /// <param name="indexB"></param>
        public static void Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        /// <summary>
        /// 随机播放。将集合中项的顺序随机交换。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
