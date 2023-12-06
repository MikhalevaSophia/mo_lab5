using System;
using System.Linq;

class P
{
    const double alpha = 0.5;

    static void Main()
    {
        double[,] matrix = { { 4, 18, 5, 3 }, { 1, 10, 5, 15 }, { 8, 6, 15, 15 }, { 15, 10, 8, 12 }, { 19, 12, 1, 0 } };

        ChooseBestStrategy(matrix);
        Console.WriteLine();
        Console.ReadLine();
    }

    static void OutMatrix(double[,] matrix, double[] aV = null, string aL = "")
    {
        Console.WriteLine("S\t       " + string.Join("\t        ", Enumerable.Range(1, matrix.GetLength(1)).Select(j => $" b{j}")) + (aV != null ? $"\t      {aL}" : ""));

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            Console.Write($"a{i + 1}\t");

            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                Console.Write($"  {matrix[i, j].ToString().PadLeft(8)}\t");
            }

            if (aV != null)
            {
                Console.Write($"{aV[i].ToString().PadLeft(8)}");
            }

            Console.WriteLine();
        }
    }

    static string BernulliCriteria(double[,] matrix)
    {
        double[] cS = new double[matrix.GetLength(0)];
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            cS[i] = 0;
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                cS[i] += matrix[i, j];
            }
            cS[i] /= matrix.GetLength(1);
        }

        OutMatrix(matrix, cS, "Ψᵢ");

        int oSIndex = Array.IndexOf(cS, cS.Max()) + 1;
        Console.WriteLine($"1) Критерий недостаточного основания Бернулли.\n" +
                          $"Если пользоваться критерием Бернулли, то следует руководствоваться стратегией a{oSIndex}.\n" +
                          $"Соответствующее математическое ожидание выигрыша при этом " +
                          $"максимально и равно {cS.Max() / matrix.GetLength(1)}.\n");
        return $"a{oSIndex}";
    }

    static string ValdCriteria(double[,] matrix)
    {
        double[] cM = new double[matrix.GetLength(0)];
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            cM[i] = double.MaxValue;
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                cM[i] = Math.Min(cM[i], matrix[i, j]);
            }
        }

        OutMatrix(matrix, cM, "αᵢ");

        int oSIndex = Array.IndexOf(cM, cM.Max()) + 1;
        Console.WriteLine($"2) Критерий пессимизма Вальда.\n" +
                          $"Пессимистическая стратегия определяет выбор a{oSIndex} " +
                          $"(нижняя цена игры равна {cM.Max()}).\n");
        return $"a{oSIndex}";
    }

    static string GurvitzCriteria(double[,] matrix)
    {
        double[] p = new double[matrix.GetLength(0)];
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            double min = double.MaxValue;
            double max = double.MinValue;
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                min = Math.Min(min, matrix[i, j]);
                max = Math.Max(max, matrix[i, j]);
            }
            p[i] = alpha * min + (1 - alpha) * max;
        }

        OutMatrix(matrix, p, "Ψ");
        int oSIndex = Array.IndexOf(p, p.Max()) + 1;
        Console.WriteLine($"4) Критерий Гурвица.\n" +
                          $"α = {alpha}\nНаилучшая стратегия a{oSIndex}\n" +
                          $"Ожидаемый выигрыш: {p.Max()}");
        return $"a{oSIndex}";
    }
    static string SevigeCriteria(double[,] m)
    {
        double[,] r = new double[m.GetLength(0), m.GetLength(1)];
        for (int i = 0; i < m.GetLength(0); i++)
        {
            for (int j = 0; j < m.GetLength(1); j++)
            {
                r[i, j] = GetMaxInColumns(m)[j] - m[i, j];
            }
        }
        double[] mC = new double[m.GetLength(0)];
        for (int i = 0; i < m.GetLength(0); i++)
        {
            mC[i] = GetMaxInColumns(r).Max();
        }

        OutMatrix(r, mC, "αᵢ");

        int oSIndex = Array.IndexOf(mC, mC.Min()) + 1;
        Console.WriteLine($"5) Критерий Севиджа.\n" +
                          $"Таким образом, оптимальная рисковая стратегия - a{oSIndex}.");
        return $"a{oSIndex}";
    }

    static double[] GetMaxInColumns(double[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        double[] maxInCols = new double[cols];

        for (int j = 0; j < cols; j++)
        {
            double max = double.MinValue;
            for (int i = 0; i < rows; i++)
            {
                max = Math.Max(max, matrix[i, j]);
            }
            maxInCols[j] = max;
        }

        return maxInCols;
    }
    static string OptimismCriteria(double[,] matrix)
    {
        ;
        double[] cM = new double[matrix.GetLength(0)];
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            cM[i] = double.MaxValue;
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                cM[i] = Math.Min(cM[i], matrix[i, j]);
            }
        }

        OutMatrix(matrix, cM, "αᵢ");

        int oSIndex = Array.IndexOf(GetMaxInRows(matrix), GetMaxInRows(matrix).Max()) + 1;
        Console.WriteLine($"3) Критерий авантюры.\n " +
                          $"Оптимистическая стратегия соответствует выбору a{oSIndex} " +
                          $"c максимальным выигрышем в матрице - {matrix.Cast<double>().Max()}.");
        return $"a{oSIndex}";
    }

    static void ChooseBestStrategy(double[,] m)
    {
        string[] s = Enumerable.Range(1, m.GetLength(0)).Select(i => $"a{i}").ToArray();
        int[] r = new int[s.Length];

        r[Array.IndexOf(s, BernulliCriteria(m))]++;
        r[Array.IndexOf(s, ValdCriteria(m))]++;
        r[Array.IndexOf(s, OptimismCriteria(m))]++;
        r[Array.IndexOf(s, GurvitzCriteria(m))]++;
        r[Array.IndexOf(s, SevigeCriteria(m))]++;

        Console.WriteLine($"\nВыберем стратегию, которая оказалась оптимальной в большем числе критериев:\n" +
                          $"{string.Join("\t", s)}\n{string.Join("\t", r)}\n" +
                          $"По принципу большинства рекомендуем стратегию {s[r.ToList().IndexOf(r.Max())]}");
    }

    static double[] GetMaxInRows(double[,] m)
    {
        int r = m.GetLength(0);
        int c = m.GetLength(1);
        double[] gIR = new double[r];

        for (int i = 0; i < r; i++)
        {
            double max = double.MinValue;
            for (int j = 0; j < c; j++)
            {
                max = Math.Max(max, m[i, j]);
            }
            gIR[i] = max;
        }
        return gIR;
    }
}