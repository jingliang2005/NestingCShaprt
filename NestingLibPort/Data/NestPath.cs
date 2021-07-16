using ClipperLib;
using NestingLibPort.Util;
using System;
using System.Collections.Generic;
using System.Linq;


namespace NestingLibPort.Data
{

    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    /// <summary>
    /// 排料路径。
    /// </summary>
    public class NestPath : IComparable<NestPath>
    {
        /// <summary>
        /// 段的集合。
        /// </summary>
        private List<Segment> segments;
        /// <summary>
        /// 子路径集合。
        /// </summary>
        private List<NestPath> children;
        /// <summary>
        /// 父路径。
        /// </summary>
        private NestPath parent;
        /// <summary>
        /// X方向偏移。
        /// </summary>
        public double offsetX;
        /// <summary>
        /// Y方向偏移。
        /// </summary>
        public double offsetY;

        private int id;
        /// <summary>
        /// 来源。
        /// </summary>
        private int source;
        /// <summary>
        /// 旋转
        /// </summary>
        private double rotation;
        
        /// <summary>
        /// 面积。
        /// </summary>
        public double area;
        /// <summary>
        /// 出价。
        /// </summary>
        public int bid;

        /// <summary>
        /// 配置。
        /// </summary>
        public NestingLibPort.Util.Config config;

        /// <summary>
        /// 增加一段。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void add(double x, double y)
        {
            this.add(new Segment(x, y));
        }

        /// <summary>
        /// 是否相等。通过比较段的数量，段比较，子路径数量，子路径比较来确定是否相等。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(Object obj)
        {
            NestPath nestPath = (NestPath)obj;
            if (segments.Count != nestPath.size())
            {
                return false;
            }
            for (int i = 0; i < segments.Count; i++)
            {
                if (!segments[i].Equals(nestPath.get(i)))
                {
                    return false;
                }
            }
            if (children.Count != nestPath.getChildren().Count)
            {
                return false;
            }
            for (int i = 0; i < children.Count; i++)
            {
                if (!children[i].Equals(nestPath.getChildren()[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 获取配置。
        /// </summary>
        /// <returns></returns>
        public Config getConfig()
        {
            return config;
        }
        /// <summary>
        /// 设置配置。
        /// </summary>
        /// <param name="config"></param>
        public void setConfig(Config config)
        {
            this.config = config;
        }

        /// <summary>
        /// 丢弃最后一个segment，
        /// </summary>
        public void pop()
        {
            segments.RemoveAt(segments.Count - 1);
        }

        /// <summary>
        /// 反转。就是将点的顺序反过来。
        /// </summary>
        public void reverse()
        {
            List<Segment> rever = new List<Segment>();
            for (int i = segments.Count - 1; i >= 0; i--)
            {
                rever.Add(segments[i]);
            }
            segments.Clear();
            foreach (Segment s in rever)
            {
                segments.Add(s);
            }
        }

        /// <summary>
        /// 获取指定索引的段。
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Segment get(int i)
        {
            return segments[i];
        }
        /// <summary>
        /// 获取父路径。
        /// </summary>
        /// <returns></returns>
        public NestPath getParent()
        {
            return parent;
        }
        /// <summary>
        /// 设置父路径。
        /// </summary>
        /// <param name="parent"></param>
        public void setParent(NestPath parent)
        {
            this.parent = parent;
        }
        /// <summary>
        /// 增加子路径。
        /// </summary>
        /// <param name="nestPath"></param>
        public void addChildren(NestPath nestPath)
        {
            children.Add(nestPath);
            nestPath.setParent(this);
        }

        /// <summary>
        /// 输出字符串。
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            String res = "";
            res += "id = " + id + " , source = " + source + " , rotation = " + rotation + "\n";
            int count = 0;
            foreach (Segment s in segments)
            {
                res += "Segment " + count + "\n";
                count++;
                res += s.ToString() + "\n";
            }
            count = 0;
            foreach (NestPath nestPath in children)
            {
                res += "children " + count + "\n";
                count++;
                res += nestPath.ToString();
            }
            return res;
        }

        /// <summary>
        /// 获取子路径集合。
        /// </summary>
        /// <returns></returns>
        public List<NestPath> getChildren()
        {
            return children;
        }
        /// <summary>
        /// 设置子路径集合。
        /// </summary>
        /// <param name="children"></param>
        public void setChildren(List<NestPath> children)
        {
            this.children = children;
        }
        /// <summary>
        /// 获取旋转角度。
        /// </summary>
        /// <returns></returns>
        public double getRotation()
        {
            return rotation;
        }
        /// <summary>
        /// 设置旋转角度。
        /// </summary>
        /// <param name="rotation"></param>
        public void setRotation(double rotation)
        {
            this.rotation = rotation;
        }
        /// <summary>
        /// 设置段集合。
        /// </summary>
        /// <param name="segments"></param>
        public void setSegments(List<Segment> segments)
        {
            this.segments = segments;
        }
        /// <summary>
        /// 获取来源。
        /// </summary>
        /// <returns></returns>
        public int getSource()
        {
            return source;
        }
      
        /// <summary>
        /// 设置来源。
        /// </summary>
        /// <param name="source"></param>
        public void setSource(int source)
        {
            this.source = source;
        }
       
        /// <summary>
        /// 构造器。排料路径。
        /// </summary>
        public NestPath()
        {
            offsetX = 0;
            offsetY = 0;
            parent = null;
            children = new List<NestPath>();
            segments = new List<Segment>();
            area = 0;
            config = new NestingLibPort.Util.Config();
        }


        public NestPath(Config config)
        {
            offsetX = 0;
            offsetY = 0;
            children = new List<NestPath>();
            segments = new List<Segment>();
            area = 0;
            this.config = config;
        }

        public NestPath(NestPath srcNestPath)
        {
            segments = new List<Segment>();
            foreach (Segment segment in srcNestPath.getSegments())
            {
                segments.Add(new Segment(segment));
            }

            this.id = srcNestPath.id;
            this.rotation = srcNestPath.rotation;
            this.source = srcNestPath.source;
            this.offsetX = srcNestPath.offsetX;
            this.offsetY = srcNestPath.offsetY;
            this.bid = srcNestPath.bid;
            this.area = srcNestPath.area;
            children = new List<NestPath>();

            foreach (NestPath nestPath in srcNestPath.getChildren())
            {
                NestPath child = new NestPath(nestPath);
                child.setParent(this);
                children.Add(child);
            }
        }

        /// <summary>
        /// 清理路径，当二个点过于接近时，删除第二个点，（或许这二个点都在一个像素上）。
        /// （也许对于高精度，或者大图形这可能要慎重），通过Clipper库的路径清理方法清理。
        /// 先转换成Clipper库的路径，清理，清理完成后转换回排料路径。
        /// </summary>
        /// <param name="srcPath"></param>
        /// <returns></returns>
        public static NestPath cleanNestPath(NestPath srcPath)
        {
            /**
             * Convert NestPath 2 Clipper
             * 转换NestPath 2 Clipper
             */
            Path path = CommonUtil.NestPath2Path(srcPath);
            Paths simple = Clipper.SimplifyPolygon(path, PolyFillType.pftEvenOdd);
            if (simple.Count == 0)
            {
                return null;
            }
            Path biggest = simple[0];
            double biggestArea = Math.Abs(Clipper.Area(biggest));
            for (int i = 0; i < simple.Count; i++)
            {
                double area = Math.Abs(Clipper.Area(simple[i]));
                if (area > biggestArea)
                {
                    biggest = simple[i];
                    biggestArea = area;
                }
            }
            //Path clean = biggest.Cleaned(Config.CURVE_TOLERANCE * Config.CLIIPER_SCALE);
            //路径清理= great.Cleaned（Config.CURVE_TOLERANCE * Config.CLIIPER_SCALE）;
            Path clean = Clipper.CleanPolygon(biggest, Config.CURVE_TOLERANCE * Config.CLIIPER_SCALE);

            if (clean.Count == 0)
            {
                return null;
            }

            /**
             *  Convert Clipper 2 NestPath
             */
            NestPath cleanPath = CommonUtil.Path2NestPath(clean);
            cleanPath.bid = srcPath.bid;
            cleanPath.setRotation(srcPath.rotation);
            return cleanPath;
        }

        /// <summary>
        /// 清零。左下角移动到0点。
        /// 通过平移将NestPath的最低x坐标，y坐标的值必定都是0，
        /// </summary>
        public void Zerolize()
        {
            ZeroX(); ZeroY();
        }
        /// <summary>
        /// X方向的最小值设定为0.X方向移动到0。
        /// </summary>
        private void ZeroX()
        {
            double xMin = Double.MaxValue;
            foreach (Segment s in segments)
            {
                if (xMin > s.getX())
                {
                    xMin = s.getX();
                }
            }
            foreach (Segment s in segments)
            {
                s.setX(s.getX() - xMin);
            }
        }
        /// <summary>
        /// Y方向的最小值设定为0。Y方向移动到0。
        /// </summary>
        private void ZeroY()
        {
            double yMin = Double.MaxValue;
            foreach (Segment s in segments)
            {
                if (yMin > s.getY())
                {
                    yMin = s.getY();
                }
            }
            foreach (Segment s in segments)
            {
                s.setY(s.getY() - yMin);
            }
        }

        /// <summary>
        /// 清空段的集合。segments.Clear();
        /// </summary>
        public void clear()
        {
            segments.Clear();
        }

        /// <summary>
        /// 获取段的数量。
        /// </summary>
        /// <returns></returns>
        public int size()
        {
            return segments.Count;
        }

        /// <summary>
        /// 增加一段。
        /// </summary>
        /// <param name="s"></param>
        public void add(Segment s)
        {
            segments.Add(s);
        }
        /// <summary>
        /// 获取段的集合。
        /// </summary>
        /// <returns></returns>
        public List<Segment> getSegments()
        {
            return segments;
        }

        /// <summary>
        /// 获取编号。ID通常用于标识路径。
        /// </summary>
        /// <returns></returns>
        public int getId()
        { 
            return id;
        }
        /// <summary>
        /// 设置编号。
        /// </summary>
        /// <param name="id"></param>
        public void setId(int id)
        {
            this.id = id;
        }
        /// <summary>
        /// 获取X方向的偏移。
        /// </summary>
        /// <returns></returns>
        public double getOffsetX()
        {
            return offsetX;
        }
        /// <summary>
        /// 设置X方向的偏移。
        /// </summary>
        /// <param name="offsetX"></param>
        public void setOffsetX(double offsetX)
        {
            this.offsetX = offsetX;
        }
        /// <summary>
        /// 获取Y方向的偏移。
        /// </summary>
        /// <returns></returns>
        public double getOffsetY()
        {
            return offsetY;
        }
        /// <summary>
        /// 设置Y方向的偏移。
        /// </summary>
        /// <param name="offsetY"></param>
        public void setOffsetY(double offsetY)
        {
            this.offsetY = offsetY;
        }

        /// <summary>
        /// 按面积排序，排序顺序的比较，在之前，之后，相同。通过比较面积。
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public int CompareTo(NestPath o)
        {
            double area0 = this.area;
            double area1 = o.area;
            if (area0 > area1)
            {
                return 1;
            }
            else if (area0 == area1)
            {
                return 0;
            }
            return -1;
        }

        /// <summary>
        /// 获取Y方向的最大值。
        /// </summary>
        /// <returns></returns>
        public double getMaxY()
        {
            double MaxY = Double.MinValue;
            foreach (Segment s in segments)
            {
                if (MaxY < s.getY())
                {
                    MaxY = s.getY();
                }
            }
            return MaxY;
        }

        /// <summary>
        /// 偏移。将全部线段平衡（X和Y方向同时移动）
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void translate(double x, double y)
        {
            foreach (Segment s in segments)
            {
                s.setX(s.getX() + x);
                s.setY(s.getY() + y);
            }
        }

    }
}
