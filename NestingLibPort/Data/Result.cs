using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NestingLibPort.Data
{
    /// <summary>
    /// 结果类。
    /// </summary>
    public class Result
    { 
        /// <summary>
        /// 放置集合。保存放置好的路径列表。
        /// </summary>
        public List<List<Vector>> placements;

        /// <summary>
        /// 适合度。数值越小越好。
        /// 
        /// </summary>
        public double fitness;

        /// <summary>
        /// 路径集合。这里保存的是没有放置的路径。
        /// </summary>
        public List<NestPath> paths;

        /// <summary>
        /// 面积。保存的是版面的余料面积。
        /// </summary>
        public double area;

        public Result(List<List<Vector>> placements, double fitness, List<NestPath> paths, double area)
        {
            this.placements = placements;
            this.fitness = fitness;
            this.paths = paths;
            this.area = area;
        }

        public Result()
        {
        }
    }
}
