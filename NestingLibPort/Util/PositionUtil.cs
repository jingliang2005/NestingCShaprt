using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NestingLibPort.Data;


//package com.qunhe.util.nest.util;

//import com.qunhe.util.nest.data.NestPath;

//import java.util.List;
namespace NestingLibPort.Util
{
    /// <summary>
    /// 位置实用
    /// </summary>
    public class PositionUtil
    {
        /// <summary>
        /// 路径集合偏移，Y方向每个路径会增加10（固定的）。有问题。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static List<NestPath> positionTranslate4Path(double x, double y, List<NestPath> paths)
        {
            foreach (NestPath path in paths)
            {
                path.translate(x, y);
                y = path.getMaxY() + 10;
            }
            return paths;
        }
    }
}
