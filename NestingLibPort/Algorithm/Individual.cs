using NestingLibPort.Data;
using System;
using System.Collections.Generic;

//package com.qunhe.util.nest.algorithm;

//import com.qunhe.util.nest.data.NestPath;

//import java.util.ArrayList;
//import java.util.List;


namespace NestingLibPort.Algorithm
{
    /// <summary>
    /// 个人，有个性的人。
    /// </summary>
    public class Individual : IComparable<Individual>
    {
        /// <summary>
        /// 放置的排料路径集合。
        /// </summary>
        public List<NestPath> placement;

        /// <summary>
        /// 旋转角度的集合。
        /// </summary>
        public List<double> rotation;

        /// <summary>
        /// 适合度。数值小代表更合适。
        /// </summary>
        public double fitness;

        /// <summary>
        /// 构造器。
        /// </summary>
        /// <param name="individual"></param>
        public Individual(Individual individual)
        {
            fitness = individual.fitness;
            placement = new List<NestPath>();
            rotation = new List<double>();
            for (int i = 0; i < individual.placement.Count; i++)
            {
                NestPath cloneNestPath = new NestPath(individual.placement[i]);
                placement.Add(cloneNestPath);
            }
            for (int i = 0; i < individual.rotation.Count; i++)
            {
                double rotationAngle = individual.getRotation()[i];
                rotation.Add(rotationAngle);
            }
        }

        /// <summary>
        /// 构造器。
        /// </summary>
        public Individual()
        {
            fitness = -1;
            placement = new List<NestPath>();
            rotation = new List<double>();
        }

        /// <summary>
        /// 构造器。
        /// </summary>
        /// <param name="placement"></param>
        /// <param name="rotation"></param>
        public Individual(List<NestPath> placement, List<double> rotation)
        {
            fitness = -1;
            this.placement = placement;
            this.rotation = rotation;
        }

        /// <summary>
        /// 返回旋转路径集合的数量。
        /// </summary>
        /// <returns></returns>
        public int size()
        {
            return placement.Count;
        }

        /// <summary>
        /// 获取放置路径的集合。
        /// </summary>
        /// <returns></returns>
        public List<NestPath> getPlacement()
        {
            return placement;
        }

        /// <summary>
        /// 设置放置路径集合。
        /// </summary>
        /// <param name="placement"></param>
        public void setPlacement(List<NestPath> placement)
        {
            this.placement = placement;
        }

        /// <summary>
        /// 获取旋转角度的集合。
        /// </summary>
        /// <returns></returns>
        public List<double> getRotation()
        {
            return rotation;
        }

        /// <summary>
        /// 设置旋转角度的集合。
        /// </summary>
        /// <param name="rotation"></param>
        public void setRotation(List<double> rotation)
        {
            this.rotation = rotation;
        }

        /// <summary>
        /// 比较方法。根据适合度比较。
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public int CompareTo(Individual o)
        {
            if (fitness > o.fitness)
            {
                return 1;
            }
            else if (fitness == o.fitness)
            {
                return 0;
            }
            return -1;
        }

        /// <summary>
        /// 是否相等。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(Object obj)
        {
            Individual individual = (Individual)obj;
            if (placement.Count != individual.size())
            {
                return false;
            }
            for (int i = 0; i < placement.Count; i++)
            {
                if (!placement[i].Equals(individual.getPlacement()[i]))
                {
                    return false;
                }
            }
            if (rotation.Count != individual.getRotation().Count)
            {
                return false;
            }
            for (int i = 0; i < rotation.Count; i++)
            {
                if (rotation[i] != individual.getRotation()[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 输出字符串。
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            String res = "";
            int count = 0;
            for (int i = 0; i < placement.Count; i++)
            {
                res += "NestPath " + count + "\n";
                count++;
                res += placement[i].ToString() + "\n";
            }
            res += "rotation \n";
            foreach (int r in rotation)
            {
                res += r + " ";
            }
            res += "\n";

            return res;
        }

        /// <summary>
        /// 获取适合度。
        /// </summary>
        /// <returns></returns>
        public double getFitness()
        {
            return fitness;
        }

        /// <summary>
        /// 设置适合度。
        /// </summary>
        /// <param name="fitness"></param>
        public void setFitness(double fitness)
        {
            this.fitness = fitness;
        }


    }
}
