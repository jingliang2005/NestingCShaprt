
using ClipperLib;
using System;
using System.Collections.Generic;

namespace NestingLibPort.Data
{
    using Paths = List<List<IntPoint>>;

    /// <summary>
    /// 向量。保存X坐标，Y坐标，ID,旋转角度。NFP路径，（IntPoint）集合的集合表示。
    /// </summary>
    public class Vector
    {
        public double x;
        public double y;
        public int id;
        public double rotation;
        public Paths nfp;

        public Vector(double x, double y, int id, double rotation)
        {
            this.x = x;
            this.y = y;
            this.id = id;
            this.rotation = rotation;
            this.nfp = new Paths();
        }

        public Vector(double x, double y, int id, double rotation, Paths nfp)
        {
            this.x = x;
            this.y = y;
            this.id = id;
            this.rotation = rotation;
            this.nfp = nfp;
        }

        public Vector()
        {
            nfp = new Paths();
        }

        
        public override String ToString()
        {
            return "x = " + x + " , y = " + y;
        }
    }

}
