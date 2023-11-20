using newAlgorithm.Model;
using System;

namespace newAlgorithm.Service
{
    public class CalculationService
    {

        /// <summary>
        /// Ультралютая функция на 500 строк непотно чего
        /// </summary>
        /// <param name="rMatrix"></param>
        /// <param name="pMatrix"></param>
        /// <param name="timeProcessing"></param>
        /// <param name="timeChangeover"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static TreeDimMatrix CalculateTnMatrix(
            RMatrix rMatrix,
            Matrix pMatrix,
            Matrix timeProcessing,
            TreeDimMatrix timeChangeover,
            int b
            )
        {
            TreeDimMatrix tnMatrix = new TreeDimMatrix(timeChangeover.deviceCount);

            int nP = pMatrix.columnCount;

            int deviceCount = timeChangeover.deviceCount;

            int previousType = 0;
            int nJPrevious = 0;
            for (int j = 1; j <= nP; j++)
            {
                RMatrixNode currentNode = rMatrix[j];
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
                                int t1 = tnMatrix[l, j, q - 1];
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.dataTypesCount; s++)
                                {
                                    int timeProces = timeProcessing[l-1, s-1];
                                    int ps1 = pMatrix[s-1, j-1];

                                    t2 += timeProces * ps1;
                                }

                                int value = t1 + t2;

                                tnMatrix.AddNode(l, j, q, value);
                                continue;
                            }

                            //45
                            if (b + 1 < q && q <= nJ)
                            {
                                int t1 = tnMatrix[l, j, q - 1];
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.dataTypesCount; s++)
                                {
                                    int timeProces = timeProcessing[l - 1, s - 1];
                                    int ps1 = pMatrix[s - 1, j - 1];

                                    t2 += timeProces * ps1;
                                }

                                int value1 = t1 + t2;
                                int value2 = tnMatrix[l + 1, j, q - b];

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
                                int t1 = tnMatrix[l, j - 1, nJPrevious];
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.dataTypesCount; s++)
                                {
                                    int timeProces = timeProcessing[l - 1, s - 1];
                                    int psj = pMatrix[s - 1, j - 1 - 1];

                                    t2 += timeProces * psj;
                                }

                                int t3 = timeChangeover[l, previousType, currentType];

                                int value1 = t1 + t2 + t3;
                                int value2 = tnMatrix[l + 1, j - 1, nJPrevious - b + 1];

                                int value = Math.Max(value1, value2);
                                tnMatrix.AddNode(l, j, q, value);
                                continue;
                            }

                            //47
                            if (1 < q && q <= b)
                            {
                                int t1 = tnMatrix[l, j, q - 1];
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.dataTypesCount; s++)
                                {
                                    int timeProces = timeProcessing[l - 1, s - 1];
                                    int psj = pMatrix[s - 1, j - 1];

                                    t2 += timeProces * psj;
                                }

                                int value1 = t1 + t2;
                                int value2 = tnMatrix[l + 1, j - 1, nJPrevious - b + q];

                                int value = Math.Max(value1, value2);

                                tnMatrix.AddNode(l, j, q, value);
                                continue;
                            }

                            //48
                            if (b + 1 <= q && q <= nJ)
                            {
                                int t1 = tnMatrix[l, j, q - 1];
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.dataTypesCount; s++)
                                {
                                    int timeProces = timeProcessing[l - 1, s - 1];
                                    int psj = pMatrix[s - 1, j - 1];

                                    t2 += timeProces * psj;
                                }

                                int value1 = t1 + t2;
                                int value2 = tnMatrix[l + 1, j, q - b];

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
                                int t1 = tnMatrix[l - 1, j, j];
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.dataTypesCount; s++)
                                {
                                    int timeProces = timeProcessing[l - 1 - 1, s - 1];
                                    int ps1 = pMatrix[s - 1, j - 1];

                                    t2 += timeProces * ps1;
                                }

                                int value = t1 + t2;
                                tnMatrix.AddNode(l, j, q, value);
                                continue;
                            }

                            //50
                            if (1 < q && q <= b + 1)
                            {
                                int t1 = tnMatrix[l - 1, j, q];
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.dataTypesCount; s++)
                                {
                                    int timeProces = timeProcessing[l - 1 - 1, s - 1];
                                    int ps1 = pMatrix[s - 1, 1 - 1];

                                    t2 += timeProces * ps1;
                                }

                                int value1 = t1 + t2;

                                t1 = tnMatrix[l, j, q - 1];
                                t2 = 0;
                                for (int s = 1; s <= rMatrix.dataTypesCount; s++)
                                {
                                    int timeProces = timeProcessing[l - 1, s - 1];
                                    int ps1 = pMatrix[s - 1, j - 1];

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
                                int t1 = tnMatrix[l - 1, j, q];
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.dataTypesCount; s++)
                                {
                                    int timeProces = timeProcessing[l - 1 - 1, s - 1];
                                    int ps1 = pMatrix[s - 1, 1 - 1];

                                    t2 += timeProces * ps1;
                                }

                                int value1 = t1 + t2;

                                t1 = tnMatrix[l, j, q - 1];
                                t2 = 0;
                                for (int s = 1; s <= rMatrix.dataTypesCount; s++)
                                {
                                    int timeProces = timeProcessing[l - 1, s - 1];
                                    int ps1 = pMatrix[s - 1, j - 1];

                                    t2 += timeProces * ps1;
                                }

                                int value2 = t1 + t2;

                                int value12max = Math.Max(value1, value2);

                                int value3 = tnMatrix[l + 1, j, q - b];

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
                                int t1 = tnMatrix[l - 1, j, 1];
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.dataTypesCount; s++)
                                {
                                    int timeProces = timeProcessing[l - 1 - 1, s - 1];
                                    int psj = pMatrix[s - 1, j - 1];

                                    t2 += timeProces * psj;
                                }

                                int value1 = t1 + t2;

                                t1 = tnMatrix[l, j - 1, nJPrevious];
                                t2 = 0;
                                int t3 = timeChangeover[l, previousType, currentType];

                                for (int s = 1; s <= rMatrix.dataTypesCount; s++)
                                {
                                    int timeProces = timeProcessing[l - 1, s - 1];
                                    int psj = pMatrix[s - 1, j - 1 - 1];

                                    t2 += timeProces * psj;
                                }

                                int value2 = t1 + t2 + t3;

                                int value12max = Math.Max(value1, value2);

                                int value3 = tnMatrix[l + 1, j - 1, nJPrevious - b + 1];

                                int value = Math.Max(value12max, value3);

                                tnMatrix.AddNode(l, j, q, value);
                                continue;
                            }

                            //53
                            if (1 < q && q <= b)
                            {
                                int t1 = tnMatrix[l - 1, j, q];
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.dataTypesCount; s++)
                                {
                                    int timeProces = timeProcessing[l - 1 - 1, s - 1];
                                    int psj = pMatrix[s - 1, j - 1];

                                    t2 += timeProces * psj;
                                }

                                int value1 = t1 + t2;

                                t1 = tnMatrix[l, j, q - 1];
                                t2 = 0;

                                for (int s = 1; s <= rMatrix.dataTypesCount; s++)
                                {
                                    int timeProces = timeProcessing[l - 1, s - 1];
                                    int psj = pMatrix[s - 1, j - 1];

                                    t2 += timeProces * psj;
                                }

                                int value2 = t1 + t2;

                                int value12max = Math.Max(value1, value2);

                                int value3 = tnMatrix[l + 1, j - 1, nJPrevious - b + q];

                                int value = Math.Max(value12max, value3);

                                tnMatrix.AddNode(l, j, q, value);
                                continue;
                            }

                            //54
                            if (b + 1 <= q && q <= nJ)
                            {
                                int t1 = tnMatrix[l - 1, j, q];
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.dataTypesCount; s++)
                                {
                                    int timeProces = timeProcessing[l - 1 - 1, s - 1];
                                    int psj = pMatrix[s - 1, j - 1];

                                    t2 += timeProces * psj;
                                }

                                int value1 = t1 + t2;

                                t1 = tnMatrix[l, j, q - 1];
                                t2 = 0;

                                for (int s = 1; s <= rMatrix.dataTypesCount; s++)
                                {
                                    int timeProces = timeProcessing[l - 1, s - 1];
                                    int psj = pMatrix[s - 1, j - 1];

                                    t2 += timeProces * psj;
                                }

                                int value2 = t1 + t2;

                                int value12max = Math.Max(value1, value2);

                                int value3 = tnMatrix[l + 1, j, q - b];

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
                                    for (int si = 1; si <= rMatrix.dataTypesCount; si++)
                                    {
                                        int timeProces = timeProcessing[li - 1, si - 1];
                                        int psj = pMatrix[si - 1, 1 - 1];

                                        value += timeProces * psj;
                                    }
                                }

                                tnMatrix.AddNode(deviceCount, j, q, value);
                                continue;
                            }

                            //56
                            if (1 < q && q <= nJ)
                            {
                                int t1 = tnMatrix[l - 1, j, q];
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.dataTypesCount; s++)
                                {
                                    int timeProces = timeProcessing[l - 1 - 1, s - 1];
                                    int psj = pMatrix[s - 1, j - 1];

                                    t2 += timeProces * psj;
                                }

                                int value1 = t1 + t2;

                                t1 = tnMatrix[l, j, q - 1];
                                t2 = 0;

                                for (int s = 1; s <= rMatrix.dataTypesCount; s++)
                                {
                                    int timeProces = timeProcessing[deviceCount - 1, s - 1];
                                    int psj = pMatrix[s - 1, j - 1];

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
                                int t1 = tnMatrix[l - 1, j, q];
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.dataTypesCount; s++)
                                {
                                    int timeProces = timeProcessing[deviceCount - 1 - 1, s - 1];
                                    int psj = pMatrix[s - 1, j - 1];

                                    t2 += timeProces * psj;
                                }

                                int value1 = t1 + t2;

                                t1 = tnMatrix[deviceCount, j - 1, nJPrevious];
                                t2 = 0;
                                int t3 = timeChangeover[deviceCount, previousType, currentType];

                                for (int s = 1; s <= rMatrix.dataTypesCount; s++)
                                {
                                    int timeProces = timeProcessing[deviceCount - 1, s - 1];
                                    int psj = pMatrix[s - 1, j - 1 - 1];

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
                                int t1 = tnMatrix[deviceCount - 1, j, q];
                                int t2 = 0;

                                for (int s = 1; s <= rMatrix.dataTypesCount; s++)
                                {
                                    int timeProces = timeProcessing[deviceCount - 1 - 1, s - 1];
                                    int psj = pMatrix[s - 1, j - 1];

                                    t2 += timeProces * psj;
                                }

                                int value1 = t1 + t2;

                                t1 = tnMatrix[deviceCount, j, q - 1];
                                t2 = 0;

                                for (int s = 1; s <= rMatrix.dataTypesCount; s++)
                                {
                                    int timeProces = timeProcessing[deviceCount - 1, s - 1];
                                    int psj = pMatrix[s - 1, j - 1];

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
