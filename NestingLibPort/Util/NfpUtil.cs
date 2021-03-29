using NestingLibPort.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


//package com.qunhe.util.nest.util;

//import com.qunhe.util.nest.data.*;
//import java.util.List;
namespace NestingLibPort.Util
{
    /// <summary>
    /// NFP实用工具。
    /// </summary>
    public class NfpUtil
    { 
        /// <summary>
        /// NFP发生器。获取一对多边形，并生成nfp。
        /// </summary>
        /// <param name="pair">NFP对。包装二个排料路径和NFP煞。</param>
        /// <param name="config"></param>
        /// <returns>返回NFP对中二个路径的NFP轨迹。轨迹以路径表示。</returns>
        public static ParallelData nfpGenerator(NfpPair pair, Config config)
        { 
            bool searchEdges = config.isCONCAVE();//搜索边缘，是（支持）凹多边形返回TRUE。
            bool useHoles = config.isUSE_HOLE();//多边形内是否有孔。

            NestPath A = GeometryUtil.rotatePolygon2Polygon(pair.getA(), pair.getKey().getArotation());
            NestPath B = GeometryUtil.rotatePolygon2Polygon(pair.getB(), pair.getKey().getBrotation());

            List<NestPath> nfp;
            if (pair.getKey().isInside())
            {
                #region 如果是里面的。根据路径A是否为矩形生成NFP。根据NFP面积判断是否要反转多边形点顺序。
                 
                if (GeometryUtil.isRectangle(A, 0.001))//如果路径A是矩形。
                {
                    
                    nfp = GeometryUtil.noFitPolygonRectangle(A, B);
                    if (nfp == null)
                    {
                        Debug.WriteLine("error:nfp is null");
                    }
                }
                else
                {
                    nfp = GeometryUtil.noFitPolygon(A, B, true, searchEdges);
                }

                if (nfp != null && nfp.Count > 0)
                {
                    for (int i = 0; i < nfp.Count; i++)
                    {
                        if (GeometryUtil.polygonArea(nfp[i]) > 0)
                        {
                            nfp[i].reverse();
                        }
                    }
                }
                else
                {
                    throw (new Exception("内部NFP空警告"));
                    //Warning on null inner NFP 内部NFP空警告
                }
                #endregion

            }
            else
            {
                #region 根据是否有（支持）凹多边形构建NFP。完整性检查，根据面积反转多边形的点顺序。孔内放置。
                int count = 0;
                if (searchEdges)
                { 
                    // NFP Generator TODO  double scale contorl
                    // NFP生成器，双精度控制。
                    nfp = GeometryUtil.noFitPolygon(A, B, false, searchEdges);
                    if (nfp == null)
                    {

                    }
                }
                else
                {

                    nfp = GeometryUtil.minkowskiDifference(A, B);
                }
                // sanity check 完整性检查
                if (nfp == null || nfp.Count == 0)
                {

                    return null;
                }
                for (int i = 0; i < nfp.Count; i++)
                {
                    if (!searchEdges || i == 0)
                    {
                        if (Math.Abs(GeometryUtil.polygonArea(nfp[i])) < 
                            Math.Abs(GeometryUtil.polygonArea(A)))
                        {
                            nfp.RemoveAt(i);

                            return null;
                        }
                    }
                }
                if (nfp.Count == 0)
                {

                    return null;
                }

                // 根据面积的正负数反转多边形的点。
                for (int i = 0; i < nfp.Count; i++)
                {
                    if (GeometryUtil.polygonArea(nfp[i]) > 0)
                    {
                        nfp[i].reverse();
                    }

                    if (i > 0)
                    {
                        if ((bool)GeometryUtil.pointInPolygon(nfp[i].get(0), nfp[0]))
                        {
                            if (GeometryUtil.polygonArea(nfp[i]) < 0)
                            {
                                nfp[i].reverse();
                            }
                        }
                    }
                }

                //如果需要在多边形内的孔中放置钣件，且路径A有子路径（有孔）
                if (useHoles && A.getChildren().Count > 0)
                {
                    #region 在多边形内的孔中放置小钣件的处理。能不能放置仅根据长和宽比较。根据面积反转多边形点顺序。
                    Bound Bbounds = GeometryUtil.getPolygonBounds(B);
                    for (int i = 0; i < A.getChildren().Count; i++)
                    {
                        Bound Abounds = GeometryUtil.getPolygonBounds(A.getChildren()[i]);
                        // 如果路径A子路径中的长和宽都比路径B大，则可以放下。
                        // 这个判断比较宽泛，没有考虑路径B如果放置后能放下的可能性。
                        if (Abounds.width > Bbounds.width && Abounds.height > Bbounds.height)
                        { 
                            List<NestPath> cnfp = GeometryUtil.noFitPolygon(A.getChildren()[i], B, true, searchEdges);
                            // ensure all interior NFPs have the same winding direction
                            // 确保所有内部NFP的缠绕方向相同
                            if (cnfp != null && cnfp.Count > 0)
                            {

                                for (int j = 0; j < cnfp.Count; j++)
                                {
                                    if (GeometryUtil.polygonArea(cnfp[j]) < 0)
                                    {
                                        cnfp[j].reverse();
                                    }
                                    nfp.Add(cnfp[j]);
                                }
                            }

                        }
                    }
                    #endregion

                }
                #endregion

            }
            if (nfp == null)
            {
                Debug.WriteLine("error:nfp is null.");
            }
            Debug.WriteLine("nfp pair key:" + pair.getKey().ToString());
            return new ParallelData(pair.getKey(), nfp);
        }
   
    }

}
