using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class www_Default : System.Web.UI.Page
{
    public string Result = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        //隐马尔科夫模型的研究建模
        //http://www.zhihu.com/question/20962240
        var pS = new double[] { 1.0 / 3, 1.0 / 3, 1.0 / 3 };
        var p4N = new double[] { 1.0 / 4, 1.0 / 4, 1.0 / 4, 1.0 / 4 };
        var p6N = new double[] { 1.0 / 6, 1.0 / 6, 1.0 / 6, 1.0 / 6, 1.0 / 6, 1.0 / 6 };
        var p8N = new double[] { 1.0 / 8, 1.0 / 8, 1.0 / 8, 1.0 / 8, 1.0 / 8, 1.0 / 8, 1.0 / 8, 1.0 / 8 };
        var pN = new double[][] { p4N, p6N, p8N };

        //问题0
        //状态链
        var sn0 = new int[,] { { 1, 1 }, { 2, 6 }, { 2, 3 } };
        var pI = CountProbability(pS, pN, sn0);

        //问题1：看见不可见的，破解骰子序列（取3个序列,计算出隐式链中最大可能链与其出现概率,我这里只计算了概率，最大可能链的计算方法同理）
        //解法1：穷举所有可能的骰子序列
        var pList = new List<double>();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    //状态链
                    var sn1 = new int[,] { { i, 1 }, { j, 6 }, { k, 3 } };
                    var pI1 = CountProbability(pS, pN, sn1);

                    pList.Add(pI1);
                }
            }
        }

        //解法2：Viterbi algorithm (最大概率累加 1 2 3 .... n)
        var sn2 = new int[] { 1, 6, 3 };
        var pI2 = ViterbiAlgorithm(pS, pN, sn2);

        //问题2：谁动了我的骰子？(取3个序列,计算出显式链序列的出现概率)
        //解法1：穷举所有可能的骰子序列（上面就有了，所以不写了）

        //解法2：前向算法（forward algorithm）


        Result = string.Join("   ", pList.OrderByDescending(p => p)) + "---:" + pI2;
//        Result = total.ToString();
    }

    /// <summary>
    /// Viterbi algorithm算法计算可能的马尔科夫1阶序列最大概率
    /// </summary>
    /// <param name="pS"></param>
    /// <param name="pN"></param>
    /// <param name="sn"></param>
    /// <returns></returns>
    private static double ViterbiAlgorithm(double[] pS, double[][] pN, int[] sn)
    {
        var res = 1.0;
        foreach (var s in sn)
        {
            res *= CountMaxProbabilityFromSn(pS, pN, s);
        }
        return res;
    }

    private static double CountMaxProbabilityFromSn(double[] pS, double[][] pN, int oneSn)
    {
        var max = 0.0;
        var length = pS.Length;
        for (int i = 0; i < length; i++)
        {
            var sn = new int[,] { { i, oneSn } };
            var pi = CountProbability(pS, pN, sn);
            if (pi > max)
            {
                max = pi;
            }
        }
        return max;
    }

    /// <summary>
    /// 计算状态链的概率
    /// </summary>
    /// <param name="pS"></param>
    /// <param name="pN"></param>
    /// <param name="sn"></param>
    /// <param name="snlength"></param>
    /// <returns></returns>
    private static double CountProbability(double[] pS, double[][] pN, int[,] sn)
    {
        try
        {
            var snlength = sn.Length/2;
            var pI = 1.0;
            for (int i = 0; i < snlength; i++)
            {
                var ps = pS[sn[i, 0]];
                var pn = pN[sn[i, 0]][sn[i, 1] - 1];
                
                pI *= ps * pn;
            }
            return pI;
        }
        catch
        {
            return 0.0;
        }
    }
}