using newAlgorithm.Model;
using System;

namespace newAlgorithm.Service
{
    public class CalculationService
    {

        /// <summary>
        /// Ультралютая функция на 500 строк непотно чего. Данная фукнция помогает высчитать время конца текущего состава пакетов
        /// </summary>
        /// <param name="rMatrix">Матрица R</param>
        /// <param name="pMatrix">Матрица P</param>
        /// <param name="timeProcessing">Матрица времени выполнения</param>
        /// <param name="timeChangeover">Трёхмерная матрица времени переналадки</param>
        /// <param name="b">b?</param>
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
                int nJ = currentNode.batchCount;
                int currentType = currentNode.dataType;

                for (int q = 1; q <= nJ; q++)
                {

                    // Данная функция высчитываем время
                    int timeCalc(int timeProcessingLeft, int pMatrixRight)
                    {
                        int t2 = 0;

                        for (int dataType = 1; dataType <= rMatrix.dataTypesCount; dataType++)
                        {
                            int timeProces = timeProcessing[timeProcessingLeft, dataType - 1];
                            int ps1 = pMatrix[dataType - 1, pMatrixRight];

                            t2 += timeProces * ps1;
                        }

                        return t2;
                    }


                    for (int device = 1; device <= deviceCount; device++)
                    {
                        //4.1 (4)
                        if (device == 1 && j == 1)
                        {
                            //43
                            if (q == 1)
                                tnMatrix.AddNode(device, j, q, 0);
                            

                            //44
                            if (1 < q && q <= b + 1)
                            {
                                int t1 = tnMatrix[device, j, q - 1];
                                int t2 = timeCalc(device - 1, j - 1);

                                int value = t1 + t2;
                                tnMatrix.AddNode(device, j, q, value);
                                continue;
                            }

                            //45
                            if (b + 1 < q && q <= nJ)
                            {
                                int t1 = tnMatrix[device, j, q - 1];
                                int t2 = timeCalc(device - 1, j - 1);

                                int value1 = t1 + t2;
                                int value2 = tnMatrix[device + 1, j, q - b];

                                int value = Math.Max(value1, value2);
                                tnMatrix.AddNode(device, j, q, value);
                            }
                            continue;
                        }


                        //4.2 (5)
                        if (device == 1 && 2 <= j && j <= nP)
                        {
                            //46
                            if (q == 1)
                            {
                                int t1 = tnMatrix[device, j - 1, nJPrevious];
                                int t2 = timeCalc(device - 1, j - 1 - 1);
                                int t3 = timeChangeover[device, previousType, currentType];

                                int value1 = t1 + t2 + t3;
                                int value2 = tnMatrix[device + 1, j - 1, nJPrevious - b + 1];

                                int value = Math.Max(value1, value2);
                                tnMatrix.AddNode(device, j, q, value);
                                continue;
                            }

                            //47
                            if (1 < q && q <= b)
                            {
                                int t1 = tnMatrix[device, j, q - 1];
                                int t2 = timeCalc(device - 1, j - 1);

                                int value1 = t1 + t2;
                                int value2 = tnMatrix[device + 1, j - 1, nJPrevious - b + q];

                                int value = Math.Max(value1, value2);

                                tnMatrix.AddNode(device, j, q, value);
                                continue;
                            }

                            //48
                            if (b + 1 <= q && q <= nJ)
                            {
                                int t1 = tnMatrix[device, j, q - 1];
                                int t2 = timeCalc(device - 1, j - 1);

                                int value1 = t1 + t2;
                                int value2 = tnMatrix[device + 1, j, q - b];

                                int value = Math.Max(value1, value2);
                                tnMatrix.AddNode(device, j, q, value);
                            }
                            continue;
                        }


                        //4.3 (6)
                        if (2 <= device && device <= deviceCount - 1 && j == 1)
                        {
                            //49
                            if (q == 1)
                            {
                                int t1 = tnMatrix[device - 1, j, j];
                                int t2 = timeCalc(device - 1 - 1, j - 1);

                                int value = t1 + t2;
                                tnMatrix.AddNode(device, j, q, value);
                                continue;
                            }

                            //50
                            if (1 < q && q <= b + 1)
                            {
                                int t1 = tnMatrix[device - 1, j, q];
                                int t2 = timeCalc(device - 1 - 1, 1 - 1);

                                int value1 = t1 + t2;

                                t1 = tnMatrix[device, j, q - 1];
                                t2 = timeCalc(device - 1, j - 1);

                                int value2 = t1 + t2;

                                int value = Math.Max(value1, value2);

                                tnMatrix.AddNode(device, j, q, value);
                                continue;
                            }

                            //45
                            if (b + 1 < q && q <= nJ)
                            {
                                int t1 = tnMatrix[device - 1, j, q];
                                int t2 = timeCalc(device - 1 - 1, 1 - 1);

                                int value1 = t1 + t2;

                                t1 = tnMatrix[device, j, q - 1];
                                t2 = timeCalc(device - 1, j - 1);

                                int value2 = t1 + t2;

                                int value12max = Math.Max(value1, value2);

                                int value3 = tnMatrix[device + 1, j, q - b];

                                int value = Math.Max(value12max, value3);

                                tnMatrix.AddNode(device, j, q, value);
                            }
                            continue;
                        }



                        //4.4 (7)
                        if (2 <= device && device <= deviceCount - 1 && 2 <= j && j <= nP)
                        {
                            //52
                            if (q == 1)
                            {
                                int t1 = tnMatrix[device - 1, j, 1];

                                int t2 = timeCalc(device - 1, j - 1);

                                int value1 = t1 + t2;

                                t1 = tnMatrix[device, j - 1, nJPrevious];
                                t2 = 0;
                                int t3 = timeChangeover[device, previousType, currentType];

                                t2 = timeCalc(device - 1, j - 1 - 1);

                                int value2 = t1 + t2 + t3;

                                int value12max = Math.Max(value1, value2);

                                int value3 = tnMatrix[device + 1, j - 1, nJPrevious - b + 1];

                                int value = Math.Max(value12max, value3);

                                tnMatrix.AddNode(device, j, q, value);
                                continue;
                            }

                            //53
                            if (1 < q && q <= b)
                            {
                                int t1 = tnMatrix[device - 1, j, q];
                                int t2 = timeCalc(device - 1 - 1, j - 1);

                                int value1 = t1 + t2;

                                t1 = tnMatrix[device, j, q - 1];
                                t2 = 0;

                                t2 = timeCalc(device - 1, j - 1);

                                int value2 = t1 + t2;

                                int value12max = Math.Max(value1, value2);

                                int value3 = tnMatrix[device + 1, j - 1, nJPrevious - b + q];

                                int value = Math.Max(value12max, value3);

                                tnMatrix.AddNode(device, j, q, value);
                                continue;
                            }

                            //54
                            if (b + 1 <= q && q <= nJ)
                            {
                                int t1 = tnMatrix[device - 1, j, q];
                                int t2 = timeCalc(device - 1 - 1, j - 1);

                                int value1 = t1 + t2;

                                t1 = tnMatrix[device, j, q - 1];
                                t2 = 0;

                                t2 = timeCalc(device - 1, j - 1 - 1);

                                int value2 = t1 + t2;

                                int value12max = Math.Max(value1, value2);

                                int value3 = tnMatrix[device + 1, j, q - b];

                                int value = Math.Max(value12max, value3);

                                tnMatrix.AddNode(device, j, q, value);
                            }
                            continue;
                        }




                        //4.5 (8)
                        if (device == deviceCount && j == 1)
                        {
                            //55
                            if (q == 1)
                            {
                                int value = 0;
                                for (int li = 1; li <= deviceCount - 1; li++)
                                {

                                    value += timeCalc(li - 1, 1 - 1);
                                    
                                }

                                tnMatrix.AddNode(deviceCount, j, q, value);
                                continue;
                            }

                            //56
                            if (1 < q && q <= nJ)
                            {
                                int t1 = tnMatrix[device - 1, j, q];

                                int t2 = timeCalc(device - 1 - 1, j - 1);

                                int value1 = t1 + t2;

                                t1 = tnMatrix[device, j, q - 1];
                                t2 = 0;

                                t2 = timeCalc(deviceCount - 1, j - 1);

                                int value2 = t1 + t2;

                                int value = Math.Max(value1, value2);

                                tnMatrix.AddNode(deviceCount, j, q, value);
                            }
                            continue;
                        }




                        //4.5 (9)
                        if (device == deviceCount && 2 <= j && j <= nP)
                        {
                            //57
                            if (q == 1)
                            {
                                int t1 = tnMatrix[device - 1, j, q];
                                int t2 = 0;
                                t2 = timeCalc(deviceCount - 1 - 1, j - 1 - 1);

                                int value1 = t1 + t2;

                                t1 = tnMatrix[deviceCount, j - 1, nJPrevious];
                                t2 = 0;
                                int t3 = timeChangeover[deviceCount, previousType, currentType];

                                t2 = timeCalc(deviceCount - 1, j - 1 - 1);

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
                                t2 = timeCalc(deviceCount - 1 - 1, j - 1);

                                int value1 = t1 + t2;

                                t1 = tnMatrix[deviceCount, j, q - 1];
                                t2 = 0;
                                t2 = timeCalc(deviceCount - 1, j - 1);

                                int value2 = t1 + t2;

                                int value = Math.Max(value1, value2);

                                tnMatrix.AddNode(deviceCount, j, q, value);
                                continue;
                            }
                        }
                    }
                }

                previousType = currentNode.dataType;
                nJPrevious = nJ;
            }

            return tnMatrix;
        }
    }
}
