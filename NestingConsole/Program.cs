
using NestingLibPort;
using NestingLibPort.Data;
using NestingLibPort.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NestingConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test_GU_noFitPolygonRectangle();
            Console.WriteLine("请按回车键开始演示！" );
            Console.ReadLine();
            NestPath bin = new NestPath();
            double binWidth =500;
            double binHeight = 300;
            bin.add(0, 0); 
            bin.add(binWidth, 0);
            bin.add(binWidth, binHeight);
            // bin.add(400, 400);
            // bin.add(200, 400);
            bin.add(0, binHeight);
            Console.WriteLine("材料大小 : 宽度 = " + binWidth + " 高度 =" + binHeight);
            var nestPaths = SvgUtil.transferSvgIntoPolygons("test.xml");
            Console.WriteLine("读取文件 = test.xml");
            Console.WriteLine("零件数 = " + nestPaths.Count);
            Config config = new Config();
            Console.WriteLine("配置套料");
            Nest nest = new Nest(bin, nestPaths, config, 10);
            Console.WriteLine("演示套料");
            // 开始排料，并获取放置结果。
            List<List<Placement>> appliedPlacement = nest.startNest();
            Console.WriteLine("套料完成");
            var svgPolygons =  SvgUtil.svgGenerator(nestPaths, appliedPlacement, binWidth, binHeight);
            Console.WriteLine("转换为SVG格式");
            SvgUtil.saveSvgFile(svgPolygons, "output.svg");
            Console.WriteLine("保存SVG文件..打开文件");
            Process.Start("output.svg");
            Console.ReadLine();
        }

        static void Test_GU_noFitPolygonRectangle()
        {
            Console.WriteLine("测试矩形NFP...");
            Config config = new Config();
            NestPath RectPath = new NestPath(config);
            RectPath.add(0, 0);
            RectPath.add(210, 0);
            RectPath.add(210, 210);
            RectPath.add(150, 150);
            RectPath.add(0, 210);

            NestPath pathB = new NestPath(config);
            pathB.add(40.5, 20.5);
            pathB.add(60.5,30.5);
            pathB.add(80.5, 60.5);
            pathB.add(50.5, 80.5);
            pathB.add(30.5, 20.5);
            pathB.setRotation(5);
            pathB.bid = 1;
            List<NestPath> path_list = new List<NestPath>();
            path_list.Add(pathB);
            NestPath pathC = new NestPath(pathB);
            pathC.setRotation(5);
            pathC.bid = 2;
            path_list.Add(pathC);
            Nest nest = new Nest(RectPath, path_list, config, 2);
            Console.WriteLine("演示套料");
            // 开始排料，并获取放置结果。
            List<List<Placement>> appliedPlacement = nest.startNest();
            Console.WriteLine("套料完成");
            var svgPolygons = SvgUtil.svgGenerator(path_list, appliedPlacement, 120, 120);

            /*
            List<NestPath> r =   GeometryUtil.noFitPolygonRectangle(RectPath, pathB); 
            r.Add(pathB);
            r.Insert(0,RectPath);

            foreach (NestPath np in r)
            {
                Console.WriteLine("测试矩形NFP..." + np.ToString());
                //for (int i = 0; i < np.size(); i++)
                //{
                //    Console.WriteLine("x:" + np.get(i).x.ToString());
                //    Console.WriteLine("y:" + np.get(i).y.ToString()); 
                //} 
            }*/
             
            //var svgPolygons = SvgUtil.svgGenerator(r, 200, 200);
            SvgUtil.saveSvgFile(svgPolygons, "output.svg");
            Console.WriteLine("保存SVG文件..打开文件");
            Process.Start("output.svg");
        }



    }
}
