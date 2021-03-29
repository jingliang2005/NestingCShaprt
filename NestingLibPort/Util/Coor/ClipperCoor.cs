using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NestingLibPort.Util.Coor
{
    /// <summary>
    /// 剪辑库Clipper的坐标类。
    /// </summary>
    public class ClipperCoor
    {
        long x;
        long y;

        public ClipperCoor()
        {
        }

        public ClipperCoor(long x, long y)
        {
            this.x = x;
            this.y = y;
        }

        public long getX()
        {
            return x;
        }

        public void setX(long x)
        {
            this.x = x;
        }

        public long getY()
        {
            return y;
        }

        public void setY(long y)
        {
            this.y = y;
        }
    }
}
