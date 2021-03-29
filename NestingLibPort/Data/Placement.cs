using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NestingLibPort.Data
{
    /// <summary>
    /// 放置，代表排料后的放置结果。
    /// </summary>
    public class Placement
    {
        /// <summary>
        /// 投标，出价的意思 。
        /// </summary>
        public int bid;

        /// <summary>
        /// 翻译，偏移的意思 。偏移后的段。
        /// </summary>
        public Segment translate;

        /// <summary>
        /// 旋转角度。
        /// </summary>
        public double rotate;

        /// <summary>
        /// 放置结果。
        /// </summary>
        /// <param name="bid"></param>
        /// <param name="translate"></param>
        /// <param name="rotate"></param>
        public Placement(int bid, Segment translate, double rotate)
        {
            this.bid = bid;
            this.translate = translate;
            this.rotate = rotate;
        }

        public Placement()
        {
        }
    }

}
