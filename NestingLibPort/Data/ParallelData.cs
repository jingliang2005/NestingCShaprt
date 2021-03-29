using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//package com.qunhe.util.nest.data;

//import java.util.ArrayList;
//import java.util.List;
namespace NestingLibPort.Data
{
    /// <summary>
    /// 平行数据。NFP轨迹数据，从二个路径生成。封装一个NFP KEY和路径集合。
    /// </summary>
    public class ParallelData
    {
        /// <summary>
        /// NFP键。
        /// </summary>
        public NfpKey key;

        /// <summary>
        /// 路径集合。
        /// </summary>
        public List<NestPath> value;

        public ParallelData()
        {
            value = new List<NestPath>();
        }

        public ParallelData(NfpKey key, List<NestPath> value)
        {
            this.key = key;
            this.value = value;
        }

        public NfpKey getKey()
        {
            return key;
        }

        public void setKey(NfpKey key)
        {
            this.key = key;
        }

        public List<NestPath> getValue()
        {
            return value;
        }

        public void setValue(List<NestPath> value)
        {
            this.value = value;
        }
    }

}
