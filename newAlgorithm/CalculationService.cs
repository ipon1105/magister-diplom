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
        /// <param name="pMatrix">Матрица P - матрица порядка обработка партий [dataTypesCount x n_p]</param>
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

            // Количество пакетов для всех типов данных, так же известное как n_p
            int batchesForAllDataTypes = pMatrix.columnCount;

            // Количество приборов в конвейерной системе
            int deviceCount = timeChangeover.deviceCount;

            // Предыдущий тип
            int previousType = 0;
            int nJPrevious = 0;

            // Для всех пакето выполняем обработку. batchIndex так же известен, как h
            for (int batchIndex = 1; batchIndex <= batchesForAllDataTypes; batchIndex++)
            {

                // Получаем узел матрицы R
                RMatrixNode currentNode = rMatrix[batchIndex];

                // Из узла матрицы R вытаскиваем тип данных и количество пакетов 
                int currentbatchCount = currentNode.batchCount;
                int currentDataType = currentNode.dataType;

                // Для всех пакетов выполняем обработку
                for (int q = 1; q <= currentbatchCount; q++)
                {

                    // Данная функция высчитываем время
                    int timeCalc(int _device, int _batchIndex)
                    {
                        int _time = 0;

                        // Для всех типов данных матрицы R выполняем обработку
                        for (int dataType = 0; dataType < rMatrix.dataTypesCount; dataType++)
                        {

                            // Подсчёт времени выполнения типа данных dataType на устройстве _device
                            int _proccessingTime = timeProcessing[_device, dataType];

                            // Получаем позицию типа данных dataType в партии _batchIndex
                            int _position = pMatrix[dataType, _batchIndex];

                            // Выполяем подсчёт времени выполнения на позицию
                            _time += _proccessingTime * _position;
                        }

                        return _time;
                    }


                    for (int device = 1; device <= deviceCount; device++)
                    {
                        //4.1 (4)
                        if (device == 1 && batchIndex == 1)
                        {
                            //43
                            if (q == 1)
                                tnMatrix.AddNode(device, batchIndex, q, 0);


                            //44
                            if (1 < q && q <= b + 1)
                            {
                                int t1 = tnMatrix[device, batchIndex, q - 1];
                                int t2 = 0;// timeCalc(device - 1, batchIndex - 1);
                                for (int _dataType = 1; _dataType <= rMatrix.dataTypesCount; _dataType++)
                                {
                                    int timeProces = timeProcessing[device - 1, _dataType - 1];
                                    int ps1 = pMatrix[_dataType - 1, batchIndex - 1];

                                    t2 += timeProces * ps1;
                                }
                                int value = t1 + t2;
                                tnMatrix.AddNode(device, batchIndex, q, value);
                                continue;
                            }

                            //45
                            if (b + 1 < q && q <= currentbatchCount)
                            {
                                int t1 = tnMatrix[device, batchIndex, q - 1];
                                int t2 = 0; //timeCalc(device - 1, batchIndex - 1)
                                for (int _dataType = 1; _dataType <= rMatrix.dataTypesCount; _dataType++)
                                {
                                    int timeProces = timeProcessing[device - 1, _dataType - 1];
                                    int ps1 = pMatrix[_dataType - 1, batchIndex - 1];

                                    t2 += timeProces * ps1;
                                }
                                int value1 = t1 + t2;
                                int value2 = tnMatrix[device + 1, batchIndex, q - b];

                                int value = Math.Max(value1, value2);
                                tnMatrix.AddNode(device, batchIndex, q, value);
                            }
                            continue;
                        }


                        //4.2 (5)
                        if (device == 1 && 2 <= batchIndex && batchIndex <= batchesForAllDataTypes)
                        {
                            //46
                            if (q == 1)
                            {
                                int t1 = tnMatrix[device, batchIndex - 1, nJPrevious];
                                int t2 = 0; // timeCalc(device - 1, batchIndex - 1 - 1);
                                for (int _dataType = 1; _dataType <= rMatrix.dataTypesCount; _dataType++)
                                {
                                    int timeProces = timeProcessing[device - 1, _dataType - 1];
                                    int psj = pMatrix[_dataType - 1, batchIndex - 1 - 1];

                                    t2 += timeProces * psj;
                                }
                                int t3 = timeChangeover[device, previousType, currentDataType];

                                int value1 = t1 + t2 + t3;
                                int value2 = tnMatrix[device + 1, batchIndex - 1, nJPrevious - b + 1];

                                int value = Math.Max(value1, value2);
                                tnMatrix.AddNode(device, batchIndex, q, value);
                                continue;
                            }

                            //47
                            if (1 < q && q <= b)
                            {
                                int t1 = tnMatrix[device, batchIndex, q - 1];
                                int t2 = 0;// timeCalc(device - 1, batchIndex - 1);
                                for (int _dataType = 1; _dataType <= rMatrix.dataTypesCount; _dataType++)
                                {
                                    int timeProces = timeProcessing[device - 1, _dataType - 1];
                                    int psj = pMatrix[_dataType - 1, batchIndex - 1];

                                    t2 += timeProces * psj;
                                }
                                int value1 = t1 + t2;
                                int value2 = tnMatrix[device + 1, batchIndex - 1, nJPrevious - b + q];

                                int value = Math.Max(value1, value2);

                                tnMatrix.AddNode(device, batchIndex, q, value);
                                continue;
                            }

                            //48
                            if (b + 1 <= q && q <= currentbatchCount)
                            {
                                int t1 = tnMatrix[device, batchIndex, q - 1];
                                int t2 = 0;// timeCalc(device - 1, batchIndex - 1);

                                for (int _dataType = 1; _dataType <= rMatrix.dataTypesCount; _dataType++)
                                {
                                    int timeProces = timeProcessing[device - 1, _dataType - 1];
                                    int psj = pMatrix[_dataType - 1, batchIndex - 1];

                                    t2 += timeProces * psj;
                                }

                                int value1 = t1 + t2;
                                int value2 = tnMatrix[device + 1, batchIndex, q - b];

                                int value = Math.Max(value1, value2);
                                tnMatrix.AddNode(device, batchIndex, q, value);
                            }
                            continue;
                        }


                        //4.3 (6)
                        if (2 <= device && device <= deviceCount - 1 && batchIndex == 1)
                        {
                            //49
                            if (q == 1)
                            {
                                int t1 = tnMatrix[device - 1, batchIndex, batchIndex];
                                int t2 = 0; // timeCalc(device - 1 - 1, batchIndex - 1);
                                for (int _dataType = 1; _dataType <= rMatrix.dataTypesCount; _dataType++)
                                {
                                    int timeProces = timeProcessing[device - 1 - 1, _dataType - 1];
                                    int ps1 = pMatrix[_dataType - 1, batchIndex - 1];

                                    t2 += timeProces * ps1;
                                }
                                int value = t1 + t2;
                                tnMatrix.AddNode(device, batchIndex, q, value);
                                continue;
                            }

                            //50
                            if (1 < q && q <= b + 1)
                            {
                                int t1 = tnMatrix[device - 1, batchIndex, q];
                                int t2 = 0; // timeCalc(device - 1 - 1, 1 - 1);

                                for (int _dataType = 1; _dataType <= rMatrix.dataTypesCount; _dataType++)
                                {
                                    int timeProces = timeProcessing[device - 1 - 1, _dataType - 1];
                                    int ps1 = pMatrix[_dataType - 1, 1 - 1];

                                    t2 += timeProces * ps1;
                                }
                                int value1 = t1 + t2;

                                t1 = tnMatrix[device, batchIndex, q - 1];
                                t2 = 0; // timeCalc(device - 1, batchIndex - 1);
                                for (int _dataType = 1; _dataType <= rMatrix.dataTypesCount; _dataType++)
                                {
                                    int timeProces = timeProcessing[device - 1, _dataType - 1];
                                    int ps1 = pMatrix[_dataType - 1, batchIndex - 1];

                                    t2 += timeProces * ps1;
                                }
                                int value2 = t1 + t2;

                                int value = Math.Max(value1, value2);

                                tnMatrix.AddNode(device, batchIndex, q, value);
                                continue;
                            }

                            //45
                            if (b + 1 < q && q <= currentbatchCount)
                            {
                                int t1 = tnMatrix[device - 1, batchIndex, q];
                                int t2 = 0; // timeCalc(device - 1 - 1, 1 - 1);

                                for (int _dataType = 1; _dataType <= rMatrix.dataTypesCount; _dataType++)
                                {
                                    int timeProces = timeProcessing[device - 1 - 1, _dataType - 1];
                                    int ps1 = pMatrix[_dataType - 1, 1 - 1];

                                    t2 += timeProces * ps1;
                                }
                                int value1 = t1 + t2;

                                t1 = tnMatrix[device, batchIndex, q - 1];
                                t2 = 0; // timeCalc(device - 1, batchIndex - 1);
                                for (int _dataType = 1; _dataType <= rMatrix.dataTypesCount; _dataType++)
                                {
                                    int timeProces = timeProcessing[device - 1, _dataType - 1];
                                    int ps1 = pMatrix[_dataType - 1, batchIndex - 1];

                                    t2 += timeProces * ps1;
                                }
                                int value2 = t1 + t2;

                                int value12max = Math.Max(value1, value2);

                                int value3 = tnMatrix[device + 1, batchIndex, q - b];

                                int value = Math.Max(value12max, value3);

                                tnMatrix.AddNode(device, batchIndex, q, value);
                            }
                            continue;
                        }



                        //4.4 (7)
                        if (2 <= device && device <= deviceCount - 1 && 2 <= batchIndex && batchIndex <= batchesForAllDataTypes)
                        {
                            //52
                            if (q == 1)
                            {
                                int t1 = tnMatrix[device - 1, batchIndex, 1];

                                int t2 = 0; // timeCalc(device - 1 - 2, batchIndex - 1);

                                for (int _dataType = 1; _dataType <= rMatrix.dataTypesCount; _dataType++)
                                {
                                    int timeProces = timeProcessing[device - 1 - 1, _dataType - 1];
                                    int psj = pMatrix[_dataType - 1, batchIndex - 1];

                                    t2 += timeProces * psj;
                                }
                                int value1 = t1 + t2;

                                t1 = tnMatrix[device, batchIndex - 1, nJPrevious];
                                t2 = 0;
                                int t3 = timeChangeover[device, previousType, currentDataType];

                                t2 = 0; // timeCalc(device - 1, batchIndex - 1 - 1);

                                for (int _dataType = 1; _dataType <= rMatrix.dataTypesCount; _dataType++)
                                {
                                    int timeProces = timeProcessing[device - 1, _dataType - 1];
                                    int psj = pMatrix[_dataType - 1, batchIndex - 1 - 1];

                                    t2 += timeProces * psj;
                                }
                                int value2 = t1 + t2 + t3;

                                int value12max = Math.Max(value1, value2);

                                int value3 = tnMatrix[device + 1, batchIndex - 1, nJPrevious - b + 1];

                                int value = Math.Max(value12max, value3);

                                tnMatrix.AddNode(device, batchIndex, q, value);
                                continue;
                            }

                            //53
                            if (1 < q && q <= b)
                            {
                                int t1 = tnMatrix[device - 1, batchIndex, q];
                                int t2 = 0; // timeCalc(device - 1 - 1, batchIndex - 1);


                                for (int _dataType = 1; _dataType <= rMatrix.dataTypesCount; _dataType++)
                                {
                                    int timeProces = timeProcessing[device - 1 - 1, _dataType - 1];
                                    int psj = pMatrix[_dataType - 1, batchIndex - 1];

                                    t2 += timeProces * psj;
                                }
                                int value1 = t1 + t2;

                                t1 = tnMatrix[device, batchIndex, q - 1];
                                t2 = 0;

                                t2 = 0; // timeCalc(device - 1, batchIndex - 1);

                                for (int _dataType = 1; _dataType <= rMatrix.dataTypesCount; _dataType++)
                                {
                                    int timeProces = timeProcessing[device - 1, _dataType - 1];
                                    int psj = pMatrix[_dataType - 1, batchIndex - 1];

                                    t2 += timeProces * psj;
                                }

                                int value2 = t1 + t2;

                                int value12max = Math.Max(value1, value2);

                                int value3 = tnMatrix[device + 1, batchIndex - 1, nJPrevious - b + q];

                                int value = Math.Max(value12max, value3);

                                tnMatrix.AddNode(device, batchIndex, q, value);
                                continue;
                            }

                            //54
                            if (b + 1 <= q && q <= currentbatchCount)
                            {
                                int t1 = tnMatrix[device - 1, batchIndex, q];
                                int t2 = 0; // timeCalc(device - 1 - 1, batchIndex - 1);

                                for (int _dataType = 1; _dataType <= rMatrix.dataTypesCount; _dataType++)
                                {
                                    int timeProces = timeProcessing[device - 1 - 1, _dataType - 1];
                                    int psj = pMatrix[_dataType - 1, batchIndex - 1];

                                    t2 += timeProces * psj;
                                }

                                int value1 = t1 + t2;

                                t1 = tnMatrix[device, batchIndex, q - 1];
                                t2 = 0;

                                t2 = 0;// timeCalc(device - 1, batchIndex - 1 - 1); // TODO: или  batchIndex - 1 - 1

                                for (int _dataType = 1; _dataType <= rMatrix.dataTypesCount; _dataType++)
                                {
                                    int timeProces = timeProcessing[device - 1, _dataType - 1];
                                    int psj = pMatrix[_dataType - 1, batchIndex - 1];

                                    t2 += timeProces * psj;
                                }


                                int value2 = t1 + t2;

                                int value12max = Math.Max(value1, value2);

                                int value3 = tnMatrix[device + 1, batchIndex, q - b];

                                int value = Math.Max(value12max, value3);

                                tnMatrix.AddNode(device, batchIndex, q, value);
                            }
                            continue;
                        }



                        if (device == deviceCount)
                        {

                            //4.5 (8)
                            if (batchIndex == 1)
                            {

                                //55
                                if (q == 1)
                                {
                                    int value = 0;
                                    for (int li = 1; li <= deviceCount - 1; li++)
                                    {

                                        // value += timeCalc(li - 1, 1 - 1);
                                        for (int si = 1; si <= rMatrix.dataTypesCount; si++)
                                        {
                                            int timeProces = timeProcessing[li - 1, si - 1];
                                            int psj = pMatrix[si - 1, 1 - 1];

                                            value += timeProces * psj;
                                        }
                                    }

                                    tnMatrix.AddNode(deviceCount, batchIndex, q, value);
                                    continue;
                                }

                                //56
                                if (1 < q && q <= currentbatchCount)
                                {
                                    int t1 = tnMatrix[device - 1, batchIndex, q];

                                    int t2 = 0; // timeCalc(device - 1 - 1, batchIndex - 1);

                                    for (int _dataType = 1; _dataType <= rMatrix.dataTypesCount; _dataType++)
                                    {
                                        int timeProces = timeProcessing[device - 1 - 1, _dataType - 1];
                                        int psj = pMatrix[_dataType - 1, batchIndex - 1];

                                        t2 += timeProces * psj;
                                    }

                                    int value1 = t1 + t2;

                                    t1 = tnMatrix[device, batchIndex, q - 1];
                                    t2 = 0;

                                    t2 = 0; // timeCalc(deviceCount - 1, batchIndex - 1);

                                    for (int _dataType = 1; _dataType <= rMatrix.dataTypesCount; _dataType++)
                                    {
                                        int timeProces = timeProcessing[deviceCount - 1, _dataType - 1];
                                        int psj = pMatrix[_dataType - 1, batchIndex - 1];

                                        t2 += timeProces * psj;
                                    }

                                    int value2 = t1 + t2;

                                    int value = Math.Max(value1, value2);

                                    tnMatrix.AddNode(deviceCount, batchIndex, q, value);
                                }
                                continue;
                            }

                            //4.5 (9)
                            if (2 <= batchIndex && batchIndex <= batchesForAllDataTypes)
                            {
                                //57
                                if (q == 1)
                                {
                                    int t1 = tnMatrix[device - 1, batchIndex, q];
                                    int t2 = 0;
                                    t2 = 0; // timeCalc(deviceCount - 1 - 1, batchIndex - 1);

                                    for (int _dataType = 1; _dataType <= rMatrix.dataTypesCount; _dataType++)
                                    {
                                        int timeProces = timeProcessing[deviceCount - 1 - 1, _dataType - 1];
                                        int psj = pMatrix[_dataType - 1, batchIndex - 1];

                                        t2 += timeProces * psj;
                                    }

                                    int value1 = t1 + t2;

                                    t1 = tnMatrix[deviceCount, batchIndex - 1, nJPrevious];
                                    t2 = 0;
                                    int t3 = timeChangeover[deviceCount, previousType, currentDataType];

                                    t2 = 0; // timeCalc(deviceCount - 1, batchIndex - 1 - 1);

                                    for (int _dataType = 1; _dataType <= rMatrix.dataTypesCount; _dataType++)
                                    {
                                        int timeProces = timeProcessing[deviceCount - 1, _dataType - 1];
                                        int psj = pMatrix[_dataType - 1, batchIndex - 1 - 1];

                                        t2 += timeProces * psj;
                                    }
                                    int value2 = t1 + t2 + t3;

                                    int value = Math.Max(value1, value2);

                                    tnMatrix.AddNode(deviceCount, batchIndex, q, value);
                                    continue;
                                }

                                //58
                                if (1 < q && q <= currentbatchCount)
                                {
                                    int t1 = tnMatrix[deviceCount - 1, batchIndex, q];
                                    int t2 = 0;
                                    t2 = 0; // timeCalc(deviceCount - 1 - 1, batchIndex - 1);


                                    for (int _dataType = 1; _dataType <= rMatrix.dataTypesCount; _dataType++)
                                    {
                                        int timeProces = timeProcessing[deviceCount - 1 - 1, _dataType - 1];
                                        int psj = pMatrix[_dataType - 1, batchIndex - 1];

                                        t2 += timeProces * psj;
                                    }
                                    int value1 = t1 + t2;

                                    t1 = tnMatrix[deviceCount, batchIndex, q - 1];
                                    t2 = 0;
                                    t2 = 0; // timeCalc(deviceCount - 1, batchIndex - 1);

                                    for (int _dataType = 1; _dataType <= rMatrix.dataTypesCount; _dataType++)
                                    {
                                        int timeProces = timeProcessing[deviceCount - 1, _dataType - 1];
                                        int psj = pMatrix[_dataType - 1, batchIndex - 1];

                                        t2 += timeProces * psj;
                                    }
                                    int value2 = t1 + t2;

                                    int value = Math.Max(value1, value2);

                                    tnMatrix.AddNode(deviceCount, batchIndex, q, value);
                                    continue;
                                }
                            }

                        }
                    }
                }

                previousType = currentNode.dataType;
                nJPrevious = currentbatchCount;
            }

            return tnMatrix;
        }
    }
}
