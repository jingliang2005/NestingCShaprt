using ClipperLib;
using NestingLibPort.Data;
using NestingLibPort.Util.Coor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Diagnostics;//诊断程序。

namespace NestingLibPort.Util
{
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    /// <summary>
    /// 放置工作。
    /// </summary>
    public class PlacementWorker
    {
        /// <summary>
        /// 版面多边形。
        /// </summary>
        public NestPath binPolygon;
        /// <summary>
        /// 配置。
        /// </summary>
        public Config config;
        /// <summary>
        /// NFP不规则排料缓存。键值集合，键为字符串，值为排料路径的集合。
        /// </summary>
        public Dictionary<String, List<NestPath>> nfpCache;
        //private static Gson gson = new GsonBuilder().create();

        /// <summary>
        /// 放置工作。
        /// </summary>
        /// <param name="binPolygon"></param>
        /// <param name="config"></param>
        /// <param name="nfpCache"></param>
        public PlacementWorker(NestPath binPolygon, Config config, Dictionary<String, List<NestPath>> nfpCache)
        {
            this.binPolygon = binPolygon;
            this.config = config;
            this.nfpCache = nfpCache;
        }

        /// <summary>
        /// 放置路径
        /// 根据板件列表与旋转角列表，通过nfp,计算板件在底板上的位置，并返回这个种群的fitness
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public Result placePaths(List<NestPath> paths)
        {
            #region 处理旋转。
            List<NestPath> rotated = new List<NestPath>();
            for (int i = 0; i < paths.Count; i++)
            {
                NestPath r = GeometryUtil.rotatePolygon2Polygon(paths[i], paths[i].getRotation());
                r.setRotation(paths[i].getRotation());
                r.setSource(paths[i].getSource());
                r.setId(paths[i].getId());
                rotated.Add(r);
            }
            paths = rotated;

            #endregion

            List<List<Vector>> allplacements = new List<List<Vector>>();//全部放置顶点。
            double fitness = 0;//适合度。
            double binarea = Math.Abs(GeometryUtil.polygonArea(this.binPolygon));//版面面积。
            String key = null;
            List<NestPath> nfp = null;

            while (paths.Count > 0)
            {
                #region 放置钣件，放置好的从集合中移除。调整适合度。
                List<NestPath> placed = new List<NestPath>();//当前放置集合。
                List<Vector> placements = new List<Vector>();//放置矢量集合

                //fitness += 1;//这行不合理。没有任何影响适合度的因素。
                double minwidth = Double.MaxValue;//最小宽度。
                for (int i = 0; i < paths.Count; i++)
                {
                    #region 循环放置。
                    #region 通过NFP KEY从NFP CACHE集合中获取NFP路径。如果没有则继续下一个。
                    NestPath path = paths[i];

                    //inner NFP 内部NFP
                    key = new JavaScriptSerializer().Serialize(new NfpKey(-1, path.getId(), true, 0, path.getRotation()));
                    //key = gson.toJson(new NfpKey(-1, path.getId(), true, 0, path.getRotation()));

                    if (!nfpCache.ContainsKey(key))
                    {
                        continue;
                    }

                    List<NestPath> binNfp = nfpCache[key];

                    #endregion

                    #region ensure exists 确保存在，如果不存在则继续下一个。
                    // ensure exists 确保存在
                    bool error = false;
                    for (int j = 0; j < placed.Count; j++)
                    {
                        key = new JavaScriptSerializer().Serialize(new NfpKey(placed[j].getId(), path.getId(), false, placed[j].getRotation(), path.getRotation()));
                        // key = gson.toJson(new NfpKey(placed[j].getId(), path.getId(), false, placed[j].getRotation(), path.getRotation()));
                        if (nfpCache.ContainsKey(key))
                            nfp = nfpCache[key];
                        else
                        {
                            error = true;
                            break;
                        }
                    }

                    if (error)
                    {
                        continue;
                    } 
                    #endregion

                    #region 第一名，放在左边，然后继续下一个循环。
                    Vector position = null;//位置
                    if (placed.Count == 0)
                    {
                        // first placement , put it on the lefth
                        // 第一名，放在左边
                        for (int j = 0; j < binNfp.Count; j++)
                        {
                            for (int k = 0; k < binNfp[j].size(); k++)
                            {
                                if (position == null || binNfp[j].get(k).x - path.get(0).x < position.x)
                                {
                                    position = new Vector(
                                            binNfp[j].get(k).x - path.get(0).x,
                                            binNfp[j].get(k).y - path.get(0).y,
                                            path.getId(),
                                            path.getRotation()
                                    );
                                }
                            }
                        }
                        placements.Add(position);
                        placed.Add(path);
                        continue;
                    }
                    #endregion

                    #region 将坐标转换成剪辑工具的坐标，并用剪辑工具的联合比较。比较失败则继续下一个循环。
                    Paths clipperBinNfp = new Paths();//using Paths = List<List<IntPoint>>;以剪辑工具数据表示的NFP

                    for (int j = 0; j < binNfp.Count; j++)
                    {
                        NestPath binNfpj = binNfp[j];
                        clipperBinNfp.Add(scaleUp2ClipperCoordinates(binNfpj));
                    }

                    Clipper clipper = new Clipper();//剪辑工具。
                    Paths combinedNfp = new Paths();


                    for (int j = 0; j < placed.Count; j++)
                    {
                        #region 将放置集合中的路径转换成剪辑工具的坐标类型，并增加到剪辑工具中。
                        key = new JavaScriptSerializer().Serialize(new NfpKey(placed[j].getId(), path.getId(), false, placed[j].getRotation(), path.getRotation()));
                        //key = gson.toJson(new NfpKey(placed[j].getId(), path.getId(), false, placed[j].getRotation(), path.getRotation()));
                        nfp = nfpCache[key];
                        if (nfp == null)
                        {
                            continue;
                        }

                        for (int k = 0; k < nfp.Count; k++)
                        {
                            #region 将NFP路径转换成剪辑工具的坐标类型，并增加到剪辑工具中。
                            Path clone = PlacementWorker.scaleUp2ClipperCoordinates(nfp[k]);
                            for (int m = 0; m < clone.Count; m++)
                            {
                                long clx = (long)clone[m].X;
                                long cly = (long)clone[m].Y;
                                IntPoint intPoint = clone[m];
                                intPoint.X = (clx + (long)(placements[j].x * Config.CLIIPER_SCALE));
                                intPoint.Y = (cly + (long)(placements[j].y * Config.CLIIPER_SCALE));
                                clone[m] = intPoint;
                            }
                            //clone = clone.Cleaned(0.0001 * Config.CLIIPER_SCALE);
                            clone = Clipper.CleanPolygon(clone, 0.0001 * Config.CLIIPER_SCALE);
                            double areaPoly = Math.Abs(Clipper.Area(clone));
                            if (clone.Count > 2 && areaPoly > 0.1 * Config.CLIIPER_SCALE * Config.CLIIPER_SCALE)
                            {
                                clipper.AddPath(clone, PolyType.ptSubject, true);
                            }
                            #endregion
                        }
                        #endregion
                    }

                    if (!clipper.Execute(ClipType.ctUnion,
                                                 combinedNfp,
                                                 PolyFillType.pftNonZero,
                                                 PolyFillType.pftNonZero))
                    {
                        continue;
                    }

                    #endregion

                    #region difference with bin polygon 与版面多边形的相差比较。失败则继续下一个循环。
                    Paths finalNfp = new Paths();//最终Nfp
                    clipper = new Clipper();

                    clipper.AddPaths(combinedNfp, PolyType.ptClip, true);
                    clipper.AddPaths(clipperBinNfp, PolyType.ptSubject, true);

                    if (!clipper.Execute(ClipType.ctDifference, finalNfp,
                        PolyFillType.pftNonZero, PolyFillType.pftNonZero))
                    {
                        continue;
                    }

                    #endregion

                    #region 最终NFP处理，移除过于密集的点，坐标从剪辑工个转换回来。
                    // finalNfp = finalNfp.Cleaned(0.0001 * Config.CLIIPER_SCALE); 
                    finalNfp = Clipper.CleanPolygons(finalNfp, 0.0001 * Config.CLIIPER_SCALE);
                    //移除finalNfp（最终NFP）中过于密集的点。
                    for (int j = 0; j < finalNfp.Count(); j++)
                    {
                        //double areaPoly = Math.Abs(finalNfp[j].Area);
                        double areaPoly = Math.Abs(Clipper.Area(finalNfp[j]));
                        if (finalNfp[j].Count < 3 || areaPoly < 0.1 * Config.CLIIPER_SCALE * Config.CLIIPER_SCALE)
                        {
                            finalNfp.RemoveAt(j);
                            j--;
                        }
                    }

                    if (finalNfp == null || finalNfp.Count == 0)
                    {
                        continue;
                    }

                    List<NestPath> f = new List<NestPath>();
                    for (int j = 0; j < finalNfp.Count; j++)
                    {
                        f.Add(toNestCoordinates(finalNfp[j]));
                    }

                    #endregion

                    #region 计算面积及其他参数。计算应和适合度评估有关。
                    List<NestPath> finalNfpf = f;
                    double minarea = Double.MinValue;
                    double minX = Double.MaxValue;
                    NestPath nf = null;
                    double area = Double.MinValue;
                    Vector shifvector = null;
                    for (int j = 0; j < finalNfpf.Count; j++)
                    {
                        #region 如果面积过小则继续下一个，否则计算面积并累加。
                        nf = finalNfpf[j];
                        if (Math.Abs(GeometryUtil.polygonArea(nf)) < 2)
                        {
                            continue;
                        }
                        for (int k = 0; k < nf.size(); k++)
                        {
                            NestPath allpoints = new NestPath();
                            for (int m = 0; m < placed.Count; m++)
                            {
                                for (int n = 0; n < placed[m].size(); n++)
                                {
                                    allpoints.add(new Segment(placed[m].get(n).x + placements[m].x,
                                                                placed[m].get(n).y + placements[m].y));
                                }
                            }
                            shifvector = new Vector(nf.get(k).x - path.get(0).x, nf.get(k).y - path.get(0).y, path.getId(), path.getRotation(), combinedNfp);
                            for (int m = 0; m < path.size(); m++)
                            {
                                allpoints.add(new Segment(path.get(m).x + shifvector.x, path.get(m).y + shifvector.y));
                            }
                            Bound rectBounds = GeometryUtil.getPolygonBounds(allpoints);

                            area = rectBounds.getWidth() * 2 + rectBounds.getHeight();
                            if (minarea == Double.MinValue
                                    || area < minarea
                                    || (GeometryUtil.almostEqual(minarea, area)
                                    && (minX == Double.MinValue || shifvector.x < minX)))
                            {
                                minarea = area;
                                minwidth = rectBounds.getWidth();
                                position = shifvector;
                                minX = shifvector.x;
                            }
                        }
                        #endregion
                    }

                    #endregion

                    // 如果不为空则增加到放置列表。
                    if (position != null)
                    { 
                        placed.Add(path);
                        placements.Add(position);
                    }
                    #endregion

                }
                 
                //调整适合度，适合度加上一个数。
                if (minwidth != Double.MinValue)
                {
                    fitness += minwidth / binarea;
                }

                // 移除已放置的路径。
                for (int i = 0; i < placed.Count; i++)
                {
                    int index = paths.IndexOf(placed[i]);
                    if (index >= 0)
                    {
                        paths.RemoveAt(index);
                    }
                }

                // 如果放置结果不为空，并且数量大于0。则增加到全部放置集合中。否则不能放置了退出。结束放置。
                if (placements != null && placements.Count > 0)
                {
                    allplacements.Add(placements);
                }
                else
                {
                    Debug.WriteLine("error:(placements != null && placements.Count > 0)");
                    //throw (new Exception("error:(placements != null && placements.Count > 0)"));
                    break; // something went wrong 出问题了
                } 
                #endregion 
            }

            // there were paths that couldn't be placed
            // 有无法放置的路径
            fitness += (double)(2 * paths.Count);
            return new Result(allplacements, fitness, paths, binarea);
        }

        /// <summary>
        /// 坐标转换，与clipper库交互必须坐标转换
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static Path scaleUp2ClipperCoordinates(NestPath polygon)
        {
            Path p = new Path();
            foreach (Segment s in polygon.getSegments())
            {
                ClipperCoor cc = CommonUtil.toClipperCoor(s.x, s.y);
                p.Add(new IntPoint(cc.getX(), cc.getY()));
            }
            return p;
        }
        /// <summary>
        /// 坐标转换，与clipper库交互必须坐标转换
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static NestPath toNestCoordinates(Path polygon)
        {
            NestPath clone = new NestPath();
            for (int i = 0; i < polygon.Count; i++)
            {
                Segment s = new Segment((double)polygon[i].X / Config.CLIIPER_SCALE, (double)polygon[i].Y / Config.CLIIPER_SCALE);
                clone.add(s);
            }
            return clone;
        }

    }
}
