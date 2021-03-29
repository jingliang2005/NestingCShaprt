using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NestingLibPort.Data
{
    /// <summary>
    /// 边框。
    /// </summary>
    public class Bound
    {
        /// <summary>
        /// X方向的最小位置。
        /// </summary>
        public double xmin;

        /// <summary>
        /// Y方向的最小位置。
        /// </summary>
        public double ymin;

        /// <summary>
        /// 宽度。
        /// </summary>
        public double width;

        /// <summary>
        /// 高度。
        /// </summary>
        public double height;

        /// <summary>
        /// 边界构造器。
        /// </summary>
        /// <param name="xmin">X方向的最小坐标。</param>
        /// <param name="ymin">Y方向的最小坐标。</param>
        /// <param name="width">宽度。</param>
        /// <param name="height">高度。</param>
        public Bound(double xmin, double ymin, double width, double height)
        {
            this.xmin = xmin;
            this.ymin = ymin;
            this.width = width;
            this.height = height;
        }
        /// <summary>
        /// 边界构造器。
        /// </summary>
        public Bound()
        {
        }

        public double getXmin()
        {
            return xmin;
        }

        public void setXmin(double xmin)
        {
            this.xmin = xmin;
        }

        public double getYmin()
        {
            return ymin;
        }

        public void setYmin(double ymin)
        {
            this.ymin = ymin;
        }

        public double getWidth()
        {
            return width;
        }

        public void setWidth(double width)
        {
            this.width = width;
        }

        public double getHeight()
        {
            return height;
        }

        public void setHeight(double height)
        {
            this.height = height;
        }

       /// <summary>
       /// 输出字符串。
       /// </summary>
       /// <returns></returns>
        public override String ToString()
        {
            return "xmin = " + xmin + " , ymin = " + ymin + " , width = " + width + ", height = " + height;
        }
    }

}
