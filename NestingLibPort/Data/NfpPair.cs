using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NestingLibPort.Data
{
    /// <summary>
    /// NFP对。代表二个路径和一个NPF键。
    /// </summary>
    public class NfpPair
    {
        /// <summary>
        /// 路径A。
        /// </summary>
        NestPath A;
        /// <summary>
        /// 路径B。
        /// </summary>
        NestPath B;
        /// <summary>
        /// NFP键。
        /// </summary>
        NfpKey key;


        public NfpPair(NestPath a, NestPath b, NfpKey key)
        {
            A = a;
            B = b;
            this.key = key;
        }

        public NestPath getA()
        {
            return A;
        }

        public void setA(NestPath a)
        {
            A = a;
        }

        public NestPath getB()
        {
            return B;
        }

        public void setB(NestPath b)
        {
            B = b;
        }

        public NfpKey getKey()
        {
            return key;
        }

        public void setKey(NfpKey key)
        {
            this.key = key;
        }
    }

}
