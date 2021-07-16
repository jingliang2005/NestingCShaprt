using NestingLibPort.Data;
using NestingLibPort.Util;
using System;
using System.Collections.Generic;


namespace NestingLibPort.Algorithm
{
    /// <summary>
    /// 遗传算法。
    /// </summary>
    public class GeneticAlgorithm
    {
        /// <summary>
        /// 最初的。
        /// </summary>
        public List<NestPath> adam;

        /// <summary>
        /// 版面。板材，表示要在上面切割的地方。（不一定的矩形，binBounds则是外接矩形。）
        /// </summary>
        public NestPath bin;

        /// <summary>
        /// 版面边框。封闭X/Y方向的最小坐标，宽度和高度。代表矩形边界。
        /// 如果要排料的是矩形，则和bin是一样的，如果要排料的不是矩形，则为外接矩形。
        /// </summary>
        public Bound binBounds;

        /// <summary>
        /// 角度集合。NFP的角度。
        /// </summary>
        public List<double> angles;

        /// <summary>
        /// 人口，簇群。
        /// </summary>
        public List<Individual> population;

        /// <summary>
        /// 配置。曲线公差，样片两两之间的距离，个体数量，变异几率等待参数。
        /// </summary>
        public Config config;

        /// <summary>
        /// 遗传算法构造器。
        /// </summary>
        /// <param name="adam">最初的，没有优化过的初始状态。</param>
        /// <param name="bin">板材，要排料的材料</param>
        /// <param name="config">配置参数。</param>
        public GeneticAlgorithm(List<NestPath> adam, NestPath bin, Config config)
        {
            this.adam = adam;
            this.bin = bin;
            this.config = config;
            this.binBounds = GeometryUtil.getPolygonBounds(bin);
            population = new List<Individual>();
            init();
        }

        /// <summary>
        /// 一代，世代。选择一个本代最优的人，循环以下步骤直到簇群大小：
        /// 并随机选择二个人交配出一个小孩，小孩再变异
        /// </summary>
        public void generation()
        {
            List<Individual> newpopulation = new List<Individual>();//新的人群，簇群。
            population.Sort();//人群排序。根据适应度排序。
            //新人群增加上一代的优化的人。（根据适应度排序后，最佳的是第一个。
            newpopulation.Add(population[0]);
            while (newpopulation.Count < config.POPULATION_SIZE)
            {
                // male 男 female 女 children 孩子们
                Individual male = randomWeightedIndividual(null);
                Individual female = randomWeightedIndividual(male);
                List<Individual> children = mate(male, female);
                newpopulation.Add(mutate(children[0]));
                if (newpopulation.Count < population.Count)
                {
                    newpopulation.Add(mutate(children[1]));
                }
            }
            population = newpopulation;
        }

        /// <summary>
        /// 伴侣，交配。从男人和女人中复制部分数据到二个小孩的公共部分，（复制放置集合和角度集合的随机个项）
        /// 其中一个小孩用男人的不重复数据填充，别一个小孩用女人的不重复数据填充。
        /// </summary>
        /// <param name="male">男人</param>
        /// <param name="female">女人</param>
        /// <returns></returns>
        public List<Individual> mate(Individual male, Individual female)
        {
            List<Individual> children = new List<Individual>();

            long cutpoint = (long)Math.Round(Math.Min(Math.Max(new Random().NextDouble(), 0.1), 0.9)
                * (male.placement.Count - 1));

            List<NestPath> gene1 = new List<NestPath>();//基因1。
            List<double> rot1 = new List<double>();//角度1。
            List<NestPath> gene2 = new List<NestPath>();//基因2。
            List<double> rot2 = new List<double>();//角度2。
            //将男人和女人的放置集合和角度集合复制一部分出来。
            for (int i = 0; i < cutpoint; i++)
            {
                gene1.Add(new NestPath(male.placement[i]));
                rot1.Add(male.getRotation()[i]);
                gene2.Add(new NestPath(female.placement[i]));
                rot2.Add(female.getRotation()[i]);
            }
            // 从女人的放置集合和角度集合复制数据，将基因1集合填充完整。
            for (int i = 0; i < female.placement.Count; i++)
            {
                if (!contains(gene1, female.placement[i].getId()))
                {
                    gene1.Add(female.placement[i]);
                    rot1.Add(female.rotation[i]);
                }
            }
            // 从男人的放置集合和角度集合复制数据，将基因2集合填充完整。
            for (int i = 0; i < male.placement.Count; i++)
            {
                if (!contains(gene2, male.placement[i].getId()))
                {
                    gene2.Add(male.placement[i]);
                    rot2.Add(male.rotation[i]);
                }
            }
            Individual individual1 = new Individual(gene1, rot1);
            Individual individual2 = new Individual(gene2, rot2);

            checkAndUpdate(individual1);
            checkAndUpdate(individual2);

            children.Add(individual1);
            children.Add(individual2);
            return children;
        }

        /// <summary>
        /// 包含，是否包含指定ID的排料路径。
        /// </summary>
        /// <param name="gene"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool contains(List<NestPath> gene, int id)
        {
            for (int i = 0; i < gene.Count; i++)
            {
                if (gene[i].getId() == id)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 随机加权个人，从簇群中移除一个要排除的人，并返回一个随机的人，或返回最优的人（集合索引0）。
        /// </summary>
        /// <param name="exclude">排除的个人</param>
        /// <returns></returns>
        private Individual randomWeightedIndividual(Individual exclude)
        {
            List<Individual> pop = new List<Individual>();
            for (int i = 0; i < population.Count; i++)
            {
                Individual individual = population[i];
                Individual clone = new Individual(individual);
                pop.Add(clone);
            }
            if (exclude != null)
            {
                int index = pop.IndexOf(exclude);
                if (index >= 0)
                {
                    pop.RemoveAt(index);
                }
            }
            double rand = new Random().NextDouble();
            double lower = 0;
            double weight = 1 / pop.Count;
            double upper = weight;

            for (int i = 0; i < pop.Count; i++)
            {
                if (rand > lower && rand < upper)
                {
                    return pop[i];
                }
                lower = upper;
                upper += 2 * weight * ((pop.Count - i) / pop.Count);
            }
            return pop[0];
        }

        /// <summary>
        /// 初始化。生成一组随机角度，并创建最初的人，采用最初的人突变（变异）出簇群。
        /// </summary>
        private void init()
        {
            angles = new List<double>();
            for (int i = 0; i < adam.Count; i++)
            {
                double angle = randomAngle(adam[i]);//产生随机角度。同时按此返回角度旋转路径。
                angles.Add(angle);
            }
            population.Add(new Individual(adam, angles));
            while (population.Count < config.POPULATION_SIZE)
            {
                Individual mutant = mutate(population[0]);//突变体，变体。
                population.Add(mutant);
            }
        }

        /// <summary>
        /// 突变体，变体。随机交换个体中放置集合中项的位置，重新生成一组随机角度更新角度集合。
        /// </summary>
        /// <param name="individual"></param>
        /// <returns></returns>
        private Individual mutate(Individual individual)
        {

            Individual clone = new Individual(individual);
            for (int i = 0; i < clone.placement.Count; i++)
            {
                double random = new Random().NextDouble();
                if (random < 0.01 * config.MUTATION_RATE)
                {
                    int j = i + 1;
                    if (j < clone.placement.Count)
                    {
                        var placement = clone.getPlacement();
                        placement.Swap(i, j);
                        //Collections.swap(clone.getPlacement(), i, j);
                    }
                }
                random = new Random().NextDouble();
                if (random < 0.01 * config.MUTATION_RATE)
                {
                    clone.getRotation()[i] = randomAngle(clone.placement[i]);
                    //   clone.getRotation().set(i, randomAngle(clone.placement.get(i)));
                }
            }
            checkAndUpdate(clone);
            return clone;
        }

        /// <summary>
        /// 随机角度，
        /// 为一个polygon 返回一个角度
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        private double randomAngle(NestPath part)
        {
            List<double> angleList = new List<double>();
            double rotate = Math.Max(1, part.getRotation());
            if (rotate == 0)
            {
                angleList.Add(0);
            }
            else
            {
                for (int i = 0; i < rotate; i++)
                {
                    angleList.Add((360 / rotate) * i);
                }
            }
            angleList.Shuffle();//将集合中的角度按随机排序。
            //Collections.shuffle(angleList);
            for (int i = 0; i < angleList.Count; i++)
            {
                Bound rotatedPart = GeometryUtil.rotatePolygon(part, angleList[i]);

                if (rotatedPart.getWidth() < binBounds.getWidth() &&
                    rotatedPart.getHeight() < binBounds.getHeight())
                {
                    return angleList[i];
                }
            }
            /**
             * 没有找到合法的角度
             */
            return -1;
        }
        /// <summary>
        /// 获取最初的人。
        /// </summary>
        /// <returns></returns>
        public List<NestPath> getAdam()
        {
            return adam;
        }
        /// <summary>
        /// 设置最初的人。
        /// </summary>
        /// <param name="adam"></param>
        public void setAdam(List<NestPath> adam)
        {
            this.adam = adam;
        }
        /// <summary>
        /// 获取版面。
        /// </summary>
        /// <returns></returns>
        public NestPath getBin()
        {
            return bin;
        }

        /// <summary>
        /// 设置版面。
        /// </summary>
        /// <param name="bin"></param>
        public void setBin(NestPath bin)
        {
            this.bin = bin;
        }

        /// <summary>
        /// 检查并更新，确认并更新。确认可以放置（长度比较），如不能放置则重新生成一个随机角度。
        /// </summary>
        /// <param name="individual"></param>
        public void checkAndUpdate(Individual individual)
        {
            for (int i = 0; i < individual.placement.Count; i++)
            {
                double angle = individual.getRotation()[i];
                NestPath nestPath = individual.getPlacement()[i];
                Bound rotateBound = GeometryUtil.rotatePolygon(nestPath, angle);
                if (rotateBound.width < binBounds.width &&
                    rotateBound.height < binBounds.height)
                {
                    continue;
                }
                else
                {
                    double safeAngle = randomAngle(nestPath);
                    individual.getRotation()[i] = safeAngle;
                }
            }
        }
    }
}




