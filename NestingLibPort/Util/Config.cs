using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//package com.qunhe.util.nest.util;
namespace NestingLibPort.Util
{
    /// <summary>
    /// 排料参数配置。
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 剪切器大小
        /// </summary>
        public static int CLIIPER_SCALE = 10000;

        /// <summary>
        /// 曲线公差
        /// </summary>
        public static double CURVE_TOLERANCE = 0.02;

        /// <summary>
        /// 在套料过程中，所有板件两两之间的距离
        /// </summary>
        public double SPACING;

        /// <summary>
        /// 利用遗传算法时所生成的族群个体数量
        /// </summary>
        public int POPULATION_SIZE;

        /// <summary>
        /// 利用遗传算法时，套料顺序的变异几率
        /// </summary>
        public int MUTATION_RATE;

        /// <summary>
        /// 凹
        /// </summary>
        public bool CONCAVE;

        /// <summary>
        /// 当板件中存在空心板件时，是否允许将板件放在空心板件当中
        /// </summary>
        public bool USE_HOLE;


        public Config()
        {
            CLIIPER_SCALE = 10000;
            CURVE_TOLERANCE = 0.5;
            SPACING = 5;
            POPULATION_SIZE = 20;
            MUTATION_RATE = 10;
            CONCAVE =  false;
            USE_HOLE =  false;
        }

        /// <summary>
        /// 是否是凹多边形。？是否支持凹多边形。凹（五笔：MMGD）。
        /// </summary>
        /// <returns></returns>
        public bool isCONCAVE()
        {
            return CONCAVE;
        }

        /// <summary>
        /// 是否有孔，是否支持将钣件放置在多边形内的孔中。
        /// </summary>
        /// <returns></returns>
        public bool isUSE_HOLE()
        {
            return USE_HOLE;
        }
    }
}
