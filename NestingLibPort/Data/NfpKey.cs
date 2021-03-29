using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NestingLibPort.Data
{
    /// <summary>
    /// NFP键。保存NFP的一些数据，没有方法。
    /// </summary>
    public class NfpKey
    {
        /// <summary>
        /// 保存路径A的ID。
        /// </summary>
       public int A;
        /// <summary>
        /// 保存路径B的ID。
        /// </summary>
        public int B;
        /// <summary>
        /// 是否里面的。内。
        /// </summary>
        public bool inside;
        /// <summary>
        /// A旋转角度。
        /// </summary>
        public double Arotation;
        /// <summary>
        /// B旋转角度。
        /// </summary>
        public double Brotation;

        /// <summary>
        /// NFP键。保存NFP的一些数据，没有方法。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="inside">是否里面的。内。</param>
        /// <param name="arotation">A旋转</param>
        /// <param name="brotation">B旋转</param>
        public NfpKey(int a, int b, bool inside, double arotation, double brotation)
        {
            A = a;
            B = b;
            this.inside = inside;
            Arotation = arotation;
            Brotation = brotation;
        }

        public NfpKey()
        {
        }

        public int getA()
        {
            return A;
        }

        public void setA(int a)
        {
            A = a;
        }

        public int getB()
        {
            return B;
        }

        public void setB(int b)
        {
            B = b;
        }

        /// <summary>
        /// 是否里面的，是否其他多边形内的孔。
        /// 
        /// </summary>
        /// <returns></returns>
        public bool isInside()
        {
            return inside;
        }

        public void setInside(bool inside)
        {
            this.inside = inside;
        }

        public double getArotation()
        {
            return Arotation;
        }

        public void setArotation(double arotation)
        {
            Arotation = arotation;
        }

        public double getBrotation()
        {
            return Brotation;
        }

        public void setBrotation(double brotation)
        {
            Brotation = brotation;
        }

    }
}
