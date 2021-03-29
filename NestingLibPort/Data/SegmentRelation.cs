using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NestingLibPort.Data
{
    /// <summary>
    /// 段关联。
    /// </summary>
    public class SegmentRelation
    {
        /// <summary>
        /// 类型。0。1。2。
        /// </summary>
        public int type;
        public int A;
        public int B;

        /// <summary>
        /// 构建段关联类。类型，A ID 和 B ID。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public SegmentRelation(int type, int a, int b)
        {
            this.type = type;
            A = a;
            B = b;
        }

        /// <summary>
        /// 段关联类。
        /// </summary>
        public SegmentRelation()
        {
        }
   
    }

}
