using System;
using Splash;

namespace SplashCheck
{
    class TestHMMCS
    {
        enum Weather { Sunny, Cloudy, Rainy };  // 隐藏状态（天气）
        enum Seaweed { Dry, Dryish, Damp, Soggy };  // 观察状态（海藻湿度）

        static void Main(string[] args)
        {
            // 测试前向算法和后向算法
            CheckForwardAndBackward();
            Console.WriteLine();

            // 测试维特比算法
            CheckViterbi();
            Console.WriteLine();

            // 测试HMM学习算法
            CheckBaumWelch();
            Console.ReadLine();
        }

        // 测试前向算法和后向算法
        static void CheckForwardAndBackward()
        {
            // 状态转移矩阵
            Double[,] A = {
                {0.500, 0.375, 0.125},
                {0.250, 0.125, 0.625},
                {0.250, 0.375, 0.375}
            };

            // 混淆矩阵
            Double[,] B = 
            {
                {0.60, 0.20, 0.15, 0.05},
                {0.25, 0.25, 0.25, 0.25},
                {0.05, 0.10, 0.35, 0.50}
            };

            // 初始概率向量
            Double[] PI = { 0.63, 0.17, 0.20 };

            // 观察序列
            Int32[] OB = { (Int32)Seaweed.Dry, (Int32)Seaweed.Damp, (Int32)Seaweed.Soggy };

            // 初始化HMM模型
            HMM hmm = new HMM(A.GetLength(0), B.GetLength(1));
            hmm.A = A;
            hmm.B = B;
            hmm.PI = PI;

            // 观察序列的概率
            Console.WriteLine("------------前向算法：双精度运算-----------------");
            Double Probability = hmm.Forward(OB);
            Console.WriteLine("Probability =" + Probability.ToString("0.###E+0"));
            Console.WriteLine();

            // 观察序列的概率
            Console.WriteLine("------------后向算法：双精度运算-----------------");
            Probability = hmm.Backward(OB);
            Console.WriteLine("Probability =" + Probability.ToString("0.###E+0"));
        }

        // 测试维特比算法
        static void CheckViterbi()
        {
            // 状态转移矩阵
            Double[,] A = {
                {0.500, 0.250, 0.250},
                {0.375, 0.125, 0.375},
                {0.125, 0.675, 0.375}
            };

            // 混淆矩阵
            Double[,] B = 
            {
                {0.60, 0.20, 0.15, 0.05},
                {0.25, 0.25, 0.25, 0.25},
                {0.05, 0.10, 0.35, 0.50}
            };

            // 初始概率向量
            Double[] PI = { 0.63, 0.17, 0.20 };

            // 观察序列
            Int32[] OB = { (Int32)Seaweed.Dry, (Int32)Seaweed.Damp, (Int32)Seaweed.Soggy, (Int32)Seaweed.Dryish, (Int32)Seaweed.Dry };

            // 初始化HMM模型
            HMM hmm = new HMM(A.GetLength(0), B.GetLength(1));
            hmm.A = A;
            hmm.B = B;
            hmm.PI = PI;

            // 找出最有可能的隐藏状态序列
            Double Probability;

            Console.WriteLine("------------维特比算法：双精度运算-----------------");
            Int32[] Q = hmm.Viterbi(OB, out Probability);
            Console.WriteLine("Probability =" + Probability.ToString("0.###E+0"));
            foreach (Int32 Value in Q)
            {
                Console.WriteLine(((Weather)Value).ToString());
            }
            Console.WriteLine();

            Console.WriteLine("------------维特比算法：对数运算-----------------");
            Q = hmm.ViterbiLog(OB, out Probability);
            Console.WriteLine("Probability =" + Probability.ToString("0.###E+0"));
            foreach (Int32 Value in Q)
            {
                Console.WriteLine(((Weather)Value).ToString());
            }
        }

        static void CheckBaumWelch()
        {
            // 状态转移矩阵
            Double[,] A = {
                {0.500, 0.250, 0.250},
                {0.375, 0.125, 0.375},
                {0.125, 0.675, 0.375}
            };

            // 混淆矩阵
            Double[,] B = 
            {
                {0.60, 0.20, 0.15, 0.05},
                {0.25, 0.25, 0.25, 0.25},
                {0.05, 0.10, 0.35, 0.50}
            };

            // 初始概率向量
            Double[] PI = { 0.63, 0.17, 0.20 };

            // 观察序列
            Int32[] OB = { (Int32)Seaweed.Dry, (Int32)Seaweed.Damp, (Int32)Seaweed.Soggy, (Int32)Seaweed.Dryish, (Int32)Seaweed.Dry };

            // 初始化HMM模型
            HMM hmm = new HMM(A.GetLength(0), B.GetLength(1));

            // 数组克隆，避免损坏原始数据
            hmm.A = (Double[,])A.Clone();
            hmm.B = (Double[,])B.Clone();
            hmm.PI = (Double[])PI.Clone();

            // 前向-后向算法
            Console.WriteLine("------------Baum-Welch算法-----------------");
            Double LogProbInit, LogProbFinal;
            Int32 Iterations = hmm.BaumWelch(OB, out LogProbInit, out LogProbFinal);
            Console.WriteLine("迭代次数 = {0}", Iterations);
            Console.WriteLine("初始概率 = {0}", Math.Exp(LogProbInit));
            Console.WriteLine("最终概率 = {0}", Math.Exp(LogProbFinal));
            Console.WriteLine();

            // 打印学习后的模型参数
            Console.WriteLine("新的模型参数：");
            Console.WriteLine("PI");
            for (Int32 i = 0; i < hmm.N; i++)
            {
                if (i == 0)
                    Console.Write(hmm.PI[i].ToString("0.000"));
                else
                    Console.Write(" " + hmm.PI[i].ToString("0.000"));
            }
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("A");
            for (Int32 i = 0; i < hmm.N; i++)
            {
                for (Int32 j = 0; j < hmm.N; j++)
                {
                    if (j == 0)
                        Console.Write(hmm.A[i, j].ToString("0.000"));
                    else
                        Console.Write(" " + hmm.A[i, j].ToString("0.000"));
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            Console.WriteLine("B");
            for (Int32 i = 0; i < hmm.N; i++)
            {
                for (Int32 j = 0; j < hmm.M; j++)
                {
                    if (j == 0)
                        Console.Write(hmm.B[i, j].ToString("0.000"));
                    else
                        Console.Write(" " + hmm.B[i, j].ToString("0.000"));
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
