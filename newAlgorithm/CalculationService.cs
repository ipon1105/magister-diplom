using newAlgorithm.Model;
using System;

namespace newAlgorithm.Service
{
    public class CalculationService
    {
        public static TreeDimMatrix CalculateTnMatrix(
            RMatrix rMatrix,
            Matrix pMatrix,
            Matrix timeProcessing,
            TreeDimMatrix timeChangeover,
            int b
            )
        {
            TreeDimMatrix tnMatrix = new TreeDimMatrix(timeChangeover.deviceCount);

            int nP = pMatrix.matrix[0].Count;

            int deviceCount = timeChangeover.deviceCount;

            int previousType = 0;
            int nJPrevious = 0;
            for (int j = 1; j <= nP; j++)
            {
                RMatrixNode currentNode = rMatrix.Find(j);
                int nJ = currentNode.Count;
                int currentType = currentNode.Type;
                for (int q = 1; q <= nJ; q++)
                {
                    for (int l = 1; l <= deviceCount; l++)
                    {
                        //4.1 (4)
                        if (l == 1 && j == 1)
                        {
                            //43
                            if (q == 1)
                            {
                                tnMatrix.AddNode(l, j, q, 0);
                            }

                            //44
                            if (1 < q && q <= b + 1)
                            {
                                int t1 = tnMatrix.GetValue(l, j, q - 1);
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.countType; s++)
                                {
                                    int timeProces = timeProcessing.GetItem(l, s);
                                    int ps1 = pMatrix.GetItem(s, j);

                                    t2 += timeProces * ps1;
                                }

                                int value = t1 + t2;

                                tnMatrix.AddNode(l, j, q, value);
                                continue;
                            }

                            //45
                            if (b + 1 < q && q <= nJ)
                            {
                                int t1 = tnMatrix.GetValue(l, j, q - 1);
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.countType; s++)
                                {
                                    int timeProces = timeProcessing.GetItem(l, s);
                                    int ps1 = pMatrix.GetItem(s, j);

                                    t2 += timeProces * ps1;
                                }

                                int value1 = t1 + t2;
                                int value2 = tnMatrix.GetValue(l + 1, j, q - b);

                                int value = Math.Max(value1, value2);
                                tnMatrix.AddNode(l, j, q, value);
                            }
                            continue;
                        }


                        //4.2 (5)
                        if (l == 1 && 2 <= j && j <= nP)
                        {
                            //46
                            if (q == 1)
                            {
                                int t1 = tnMatrix.GetValue(l, j - 1, nJPrevious);
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.countType; s++)
                                {
                                    int timeProces = timeProcessing.GetItem(l, s);
                                    int psj = pMatrix.GetItem(s, j - 1);

                                    t2 += timeProces * psj;
                                }

                                int t3 = timeChangeover.GetValue(l, previousType, currentType);

                                int value1 = t1 + t2 + t3;
                                int value2 = tnMatrix.GetValue(l + 1, j - 1, nJPrevious - b + 1);

                                int value = Math.Max(value1, value2);
                                tnMatrix.AddNode(l, j, q, value);
                                continue;
                            }

                            //47
                            if (1 < q && q <= b)
                            {
                                int t1 = tnMatrix.GetValue(l, j, q - 1);
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.countType; s++)
                                {
                                    int timeProces = timeProcessing.GetItem(l, s);
                                    int psj = pMatrix.GetItem(s, j);

                                    t2 += timeProces * psj;
                                }

                                int value1 = t1 + t2;
                                int value2 = tnMatrix.GetValue(l + 1, j - 1, nJPrevious - b + q);

                                int value = Math.Max(value1, value2);

                                tnMatrix.AddNode(l, j, q, value);
                                continue;
                            }

                            //48
                            if (b + 1 <= q && q <= nJ)
                            {
                                int t1 = tnMatrix.GetValue(l, j, q - 1);
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.countType; s++)
                                {
                                    int timeProces = timeProcessing.GetItem(l, s);
                                    int psj = pMatrix.GetItem(s, j);

                                    t2 += timeProces * psj;
                                }

                                int value1 = t1 + t2;
                                int value2 = tnMatrix.GetValue(l + 1, j, q - b);

                                int value = Math.Max(value1, value2);
                                tnMatrix.AddNode(l, j, q, value);
                            }
                            continue;
                        }


                        //4.3 (6)
                        if (2 <= l && l <= deviceCount - 1 && j == 1)
                        {
                            //49
                            if (q == 1)
                            {
                                int t1 = tnMatrix.GetValue(l - 1, j, j);
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.countType; s++)
                                {
                                    int timeProces = timeProcessing.GetItem(l - 1, s);
                                    int ps1 = pMatrix.GetItem(s, j);

                                    t2 += timeProces * ps1;
                                }

                                int value = t1 + t2;
                                tnMatrix.AddNode(l, j, q, value);
                                continue;
                            }

                            //50
                            if (1 < q && q <= b + 1)
                            {
                                int t1 = tnMatrix.GetValue(l - 1, j, q);
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.countType; s++)
                                {
                                    int timeProces = timeProcessing.GetItem(l - 1, s);
                                    int ps1 = pMatrix.GetItem(s, 1);

                                    t2 += timeProces * ps1;
                                }

                                int value1 = t1 + t2;

                                t1 = tnMatrix.GetValue(l, j, q - 1);
                                t2 = 0;
                                for (int s = 1; s <= rMatrix.countType; s++)
                                {
                                    int timeProces = timeProcessing.GetItem(l, s);
                                    int ps1 = pMatrix.GetItem(s, j);

                                    t2 += timeProces * ps1;
                                }

                                int value2 = t1 + t2;

                                int value = Math.Max(value1, value2);

                                tnMatrix.AddNode(l, j, q, value);
                                continue;
                            }

                            //45
                            if (b + 1 < q && q <= nJ)
                            {
                                int t1 = tnMatrix.GetValue(l - 1, j, q);
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.countType; s++)
                                {
                                    int timeProces = timeProcessing.GetItem(l - 1, s);
                                    int ps1 = pMatrix.GetItem(s, 1);

                                    t2 += timeProces * ps1;
                                }

                                int value1 = t1 + t2;

                                t1 = tnMatrix.GetValue(l, j, q - 1);
                                t2 = 0;
                                for (int s = 1; s <= rMatrix.countType; s++)
                                {
                                    int timeProces = timeProcessing.GetItem(l, s);
                                    int ps1 = pMatrix.GetItem(s, j);

                                    t2 += timeProces * ps1;
                                }

                                int value2 = t1 + t2;

                                int value12max = Math.Max(value1, value2);

                                int value3 = tnMatrix.GetValue(l + 1, j, q - b);

                                int value = Math.Max(value12max, value3);

                                tnMatrix.AddNode(l, j, q, value);
                            }
                            continue;
                        }



                        //4.4 (7)
                        if (2 <= l && l <= deviceCount - 1 && 2 <= j && j <= nP)
                        {
                            //52
                            if (q == 1)
                            {
                                int t1 = tnMatrix.GetValue(l - 1, j, 1);
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.countType; s++)
                                {
                                    int timeProces = timeProcessing.GetItem(l - 1, s);
                                    int psj = pMatrix.GetItem(s, j);

                                    t2 += timeProces * psj;
                                }

                                int value1 = t1 + t2;

                                t1 = tnMatrix.GetValue(l, j - 1, nJPrevious);
                                t2 = 0;
                                int t3 = timeChangeover.GetValue(l, previousType, currentType);

                                for (int s = 1; s <= rMatrix.countType; s++)
                                {
                                    int timeProces = timeProcessing.GetItem(l, s);
                                    int psj = pMatrix.GetItem(s, j - 1);

                                    t2 += timeProces * psj;
                                }

                                int value2 = t1 + t2 + t3;

                                int value12max = Math.Max(value1, value2);

                                int value3 = tnMatrix.GetValue(l + 1, j - 1, nJPrevious - b + 1);

                                int value = Math.Max(value12max, value3);

                                tnMatrix.AddNode(l, j, q, value);
                                continue;
                            }

                            //53
                            if (1 < q && q <= b)
                            {
                                int t1 = tnMatrix.GetValue(l - 1, j, q);
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.countType; s++)
                                {
                                    int timeProces = timeProcessing.GetItem(l - 1, s);
                                    int psj = pMatrix.GetItem(s, j);

                                    t2 += timeProces * psj;
                                }

                                int value1 = t1 + t2;

                                t1 = tnMatrix.GetValue(l, j, q - 1);
                                t2 = 0;

                                for (int s = 1; s <= rMatrix.countType; s++)
                                {
                                    int timeProces = timeProcessing.GetItem(l, s);
                                    int psj = pMatrix.GetItem(s, j);

                                    t2 += timeProces * psj;
                                }

                                int value2 = t1 + t2;

                                int value12max = Math.Max(value1, value2);

                                int value3 = tnMatrix.GetValue(l + 1, j - 1, nJPrevious - b + q);

                                int value = Math.Max(value12max, value3);

                                tnMatrix.AddNode(l, j, q, value);
                                continue;
                            }

                            //54
                            if (b + 1 <= q && q <= nJ)
                            {
                                int t1 = tnMatrix.GetValue(l - 1, j, q);
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.countType; s++)
                                {
                                    int timeProces = timeProcessing.GetItem(l - 1, s);
                                    int psj = pMatrix.GetItem(s, j);

                                    t2 += timeProces * psj;
                                }

                                int value1 = t1 + t2;

                                t1 = tnMatrix.GetValue(l, j, q - 1);
                                t2 = 0;

                                for (int s = 1; s <= rMatrix.countType; s++)
                                {
                                    int timeProces = timeProcessing.GetItem(l, s);
                                    int psj = pMatrix.GetItem(s, j);

                                    t2 += timeProces * psj;
                                }

                                int value2 = t1 + t2;

                                int value12max = Math.Max(value1, value2);

                                int value3 = tnMatrix.GetValue(l + 1, j, q - b);

                                int value = Math.Max(value12max, value3);

                                tnMatrix.AddNode(l, j, q, value);
                            }
                            continue;
                        }




                        //4.5 (8)
                        if (l == deviceCount && j == 1)
                        {
                            //55
                            if (q == 1)
                            {
                                int value = 0;
                                for (int li = 1; li <= deviceCount - 1; li++)
                                {
                                    for (int si = 1; si <= rMatrix.countType; si++)
                                    {
                                        int timeProces = timeProcessing.GetItem(li, si);
                                        int psj = pMatrix.GetItem(si, 1);

                                        value += timeProces * psj;
                                    }
                                }

                                tnMatrix.AddNode(deviceCount, j, q, value);
                                continue;
                            }

                            //56
                            if (1 < q && q <= nJ)
                            {
                                int t1 = tnMatrix.GetValue(l - 1, j, q);
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.countType; s++)
                                {
                                    int timeProces = timeProcessing.GetItem(l - 1, s);
                                    int psj = pMatrix.GetItem(s, j);

                                    t2 += timeProces * psj;
                                }

                                int value1 = t1 + t2;

                                t1 = tnMatrix.GetValue(l, j, q - 1);
                                t2 = 0;

                                for (int s = 1; s <= rMatrix.countType; s++)
                                {
                                    int timeProces = timeProcessing.GetItem(deviceCount, s);
                                    int psj = pMatrix.GetItem(s, j);

                                    t2 += timeProces * psj;
                                }

                                int value2 = t1 + t2;

                                int value = Math.Max(value1, value2);

                                tnMatrix.AddNode(deviceCount, j, q, value);
                            }
                            continue;
                        }




                        //4.5 (9)
                        if (l == deviceCount && 2 <= j && j <= nP)
                        {
                            //57
                            if (q == 1)
                            {
                                int t1 = tnMatrix.GetValue(l - 1, j, q);
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.countType; s++)
                                {
                                    int timeProces = timeProcessing.GetItem(deviceCount - 1, s);
                                    int psj = pMatrix.GetItem(s, j);

                                    t2 += timeProces * psj;
                                }

                                int value1 = t1 + t2;

                                t1 = tnMatrix.GetValue(deviceCount, j - 1, nJPrevious);
                                t2 = 0;
                                int t3 = timeChangeover.GetValue(deviceCount, previousType, currentType);

                                for (int s = 1; s <= rMatrix.countType; s++)
                                {
                                    int timeProces = timeProcessing.GetItem(deviceCount, s);
                                    int psj = pMatrix.GetItem(s, j - 1);

                                    t2 += timeProces * psj;
                                }

                                int value2 = t1 + t2 + t3;

                                int value = Math.Max(value1, value2);

                                tnMatrix.AddNode(deviceCount, j, q, value);
                                continue;
                            }

                            //58
                            if (1 < q && q <= nJ)
                            {
                                int t1 = tnMatrix.GetValue(deviceCount - 1, j, q);
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.countType; s++)
                                {
                                    int timeProces = timeProcessing.GetItem(deviceCount - 1, s);
                                    int psj = pMatrix.GetItem(s, j);

                                    t2 += timeProces * psj;
                                }

                                int value1 = t1 + t2;

                                t1 = tnMatrix.GetValue(deviceCount, j, q - 1);
                                t2 = 0;

                                for (int s = 1; s <= rMatrix.countType; s++)
                                {
                                    int timeProces = timeProcessing.GetItem(deviceCount, s);
                                    int psj = pMatrix.GetItem(s, j);

                                    t2 += timeProces * psj;
                                }

                                int value2 = t1 + t2;

                                int value = Math.Max(value1, value2);

                                tnMatrix.AddNode(deviceCount, j, q, value);
                                continue;
                            }
                        }
                    }
                }

                previousType = currentNode.Type;
                nJPrevious = nJ;
            }

            return tnMatrix;
        }
    }
}
