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
        /// <param name="pMatrix">Матрица P - матрица порядка обработка партий [dataTypesCount x maxBatchesPositions] = [n x n_p]</param>
        /// <param name="timeProcessing">Матрица времени выполнения</param>
        /// <param name="timeChangeover">Трёхмерная матрица времени переналадки</param>
        /// <param name="bufferSize">Целочисленный размер буфера</param>
        /// <returns></returns>
        public static TreeDimMatrix CalculateTnMatrix(
            RMatrix rMatrix,
            Matrix pMatrix,
            Matrix timeProcessing,
            TreeDimMatrix timeChangeover,
            int bufferSize
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

            // Для всех пакетов выполняем обработку. batchIndex так же известен, как h
            for (int batchIndex = 1; batchIndex <= batchesForAllDataTypes; batchIndex++)
            {

                // Получаем узел матрицы R в позиции batchIndex
                RMatrixNode currentNode = rMatrix[batchIndex];

                // Из узла матрицы R вытаскиваем тип данных и количество заданий данного типа
                int currentJobCount = currentNode.batchCount;
                int currentDataType = currentNode.dataType;

                // Для всех заданий выполняем обработку. job так же известен, как q
                for (int job = 1; job <= currentJobCount; job++)
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


                    // Для всех приборов выполняем обработку
                    for (int device = 1; device <= deviceCount; device++)
                    {

                        // Для первого прибора выполняем обработку
                        if (device == 1)
                        {

                            // Для первого пакета в последовательности выполняем обработку
                            // 4.1 (4)
                            if (batchIndex == 1)
                            {

                                // Если данное задание является первым в пакете, добавляем его в матрицу
                                if (job == 1)
                                    tnMatrix.AddNode(device, batchIndex, job, 0);

                                // Если данное задание не первое и не превышает размер буфера, выполняем обработку
                                if (1 < job && job <= bufferSize + 1)
                                {
                                    int t1 = tnMatrix[device, batchIndex, job - 1];
                                    int t2 = timeCalc(device - 1, batchIndex - 1);

                                    int value = t1 + t2;
                                    tnMatrix.AddNode(device, batchIndex, job, value);
                                    continue;
                                }

                                // Если данное задание превышает размер буфера, выполняем обработку
                                // 45
                                // TODO: нет необходимости обрабатывать случай (job <= currentJobCount), если значение job не имзеняется динамично, так как условие прописано в 
                                if (bufferSize + 1 < job && job <= currentJobCount)
                                {
                                    int t1 = tnMatrix[device, batchIndex, job - 1];
                                    int t2 = timeCalc(device - 1, batchIndex - 1);

                                    int value1 = t1 + t2;
                                    int value2 = tnMatrix[device + 1, batchIndex, job - bufferSize];

                                    int value = Math.Max(value1, value2);
                                    tnMatrix.AddNode(device, batchIndex, job, value);
                                }

                                // Продолжаем вычисления для следующего прибора
                                continue;

                            }

                            // Для любого не первого пакета в последовательности выполняем обработку
                            // TODO: нет необходимости обрабатывать случай (batchIndex <= batchesForAllDataTypes), если значение batchIndex не имзеняется динамично, так как условие прописано в цикле
                            if (2 <= batchIndex && batchIndex <= batchesForAllDataTypes)
                            {

                                // Для первого задания в пакете выполняем обработку
                                if (job == 1)
                                {
                                    int t1 = tnMatrix[device, batchIndex - 1, nJPrevious];
                                    int t2 = timeCalc(device - 1, batchIndex - 1 - 1);

                                    int t3 = timeChangeover[device, previousType, currentDataType];

                                    int value1 = t1 + t2 + t3;
                                    int value2 = tnMatrix[device + 1, batchIndex - 1, nJPrevious - bufferSize + 1];

                                    int value = Math.Max(value1, value2);
                                    tnMatrix.AddNode(device, batchIndex, job, value);
                                    continue;
                                }

                                // Если данное задание не первое и не превышает размер буфера, выполняем обработку
                                if (1 < job && job <= bufferSize)
                                {
                                    int t1 = tnMatrix[device, batchIndex, job - 1];
                                    int t2 = timeCalc(device - 1, batchIndex - 1);

                                    int value1 = t1 + t2;
                                    int value2 = tnMatrix[device + 1, batchIndex - 1, nJPrevious - bufferSize + job];

                                    int value = Math.Max(value1, value2);

                                    tnMatrix.AddNode(device, batchIndex, job, value);
                                    continue;
                                }

                                // Если данное задание превышает размер буфера, выполняем обработку
                                // 45
                                // TODO: нет необходимости обрабатывать случай (job <= currentJobCount), если значение job не имзеняется динамично, так как условие прописано в 
                                if (bufferSize + 1 <= job && job <= currentJobCount)
                                {
                                    int t1 = tnMatrix[device, batchIndex, job - 1];
                                    int t2 = timeCalc(device - 1, batchIndex - 1);

                                    int value1 = t1 + t2;
                                    int value2 = tnMatrix[device + 1, batchIndex, job - bufferSize];

                                    int value = Math.Max(value1, value2);
                                    tnMatrix.AddNode(device, batchIndex, job, value);
                                }

                                // Продолжаем подсчёт
                                continue;
                            }

                        }

                        // Для любого не первого и не последнего прибора выполняем обработку
                        //4.3 (6)
                        if (2 <= device && device <= deviceCount - 1)
                        {

                            // Для первого пакета в последовательности выполняем обработку
                            if(batchIndex == 1)
                            {

                                // Для первого задания в пакете выполняем обработку
                                // 49
                                if (job == 1)
                                {
                                    int t1 = tnMatrix[device - 1, batchIndex, batchIndex];
                                    int t2 = timeCalc(device - 1 - 1, batchIndex - 1);

                                    int value = t1 + t2;
                                    tnMatrix.AddNode(device, batchIndex, job, value);

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }

                                // Если данное задание не первое и не превышает размер буфера, выполняем обработку
                                //50
                                if (1 < job && job <= bufferSize + 1)
                                {
                                    int t1 = tnMatrix[device - 1, batchIndex, job];
                                    int t2 = timeCalc(device - 1 - 1, 1 - 1);

                                    int value1 = t1 + t2;

                                    t1 = tnMatrix[device, batchIndex, job - 1];
                                    t2 = timeCalc(device - 1, batchIndex - 1);

                                    int value2 = t1 + t2;

                                    int value = Math.Max(value1, value2);

                                    tnMatrix.AddNode(device, batchIndex, job, value);

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }

                                // Если данное задание превышает размер буфера, выполняем обработку
                                // 45
                                // TODO: нет необходимости обрабатывать случай (job <= currentJobCount), если значение job не имзеняется динамично, так как условие прописано в 
                                if (bufferSize + 1 < job && job <= currentJobCount)
                                {
                                    int t1 = tnMatrix[device - 1, batchIndex, job];
                                    int t2 = timeCalc(device - 1 - 1, 1 - 1);

                                    int value1 = t1 + t2;

                                    t1 = tnMatrix[device, batchIndex, job - 1];
                                    t2 = timeCalc(device - 1, batchIndex - 1);

                                    int value2 = t1 + t2;

                                    int value12max = Math.Max(value1, value2);

                                    int value3 = tnMatrix[device + 1, batchIndex, job - bufferSize];

                                    int value = Math.Max(value12max, value3);

                                    tnMatrix.AddNode(device, batchIndex, job, value);
                                }

                                // Продолжаем вычисления для следующего прибора
                                continue;
                            }

                            // Для любого не первого пакета в последовательности выполняем обработку
                            //4.4 (7)
                            // TODO: нет необходимости обрабатывать случай (batchIndex <= batchesForAllDataTypes), если значение batchIndex не имзеняется динамично, так как условие прописано в цикле
                            if (2 <= batchIndex && batchIndex <= batchesForAllDataTypes)
                            {
                                
                                // Для первого задания в пакете выполняем обработку
                                if (job == 1)
                                {
                                    int t1 = tnMatrix[device - 1, batchIndex, 1];
                                    int t2 = timeCalc(device - 1 - 1, batchIndex - 1);

                                    int value1 = t1 + t2;

                                    t1 = tnMatrix[device, batchIndex - 1, nJPrevious];
                                    t2 = timeCalc(device - 1, batchIndex - 1 - 1);
                                    int t3 = timeChangeover[device, previousType, currentDataType];

                                    int value2 = t1 + t2 + t3;

                                    int value12max = Math.Max(value1, value2);

                                    int value3 = tnMatrix[device + 1, batchIndex - 1, nJPrevious - bufferSize + 1];

                                    int value = Math.Max(value12max, value3);

                                    tnMatrix.AddNode(device, batchIndex, job, value);

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }

                                // Если данное задание не первое и не превышает размер буфера, выполняем обработку
                                //53
                                if (1 < job && job <= bufferSize)
                                {
                                    int t1 = tnMatrix[device - 1, batchIndex, job];
                                    int t2 = timeCalc(device - 1 - 1, batchIndex - 1);

                                    int value1 = t1 + t2;

                                    t1 = tnMatrix[device, batchIndex, job - 1];
                                    t2 = timeCalc(device - 1, batchIndex - 1);

                                    int value2 = t1 + t2;

                                    int value12max = Math.Max(value1, value2);

                                    int value3 = tnMatrix[device + 1, batchIndex - 1, nJPrevious - bufferSize + job];

                                    int value = Math.Max(value12max, value3);

                                    tnMatrix.AddNode(device, batchIndex, job, value);

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }

                                // Если данное задание превышает размер буфера, выполняем обработку
                                // 54
                                // TODO: нет необходимости обрабатывать случай (job <= currentJobCount), если значение job не имзеняется динамично, так как условие прописано в 
                                if (bufferSize + 1 <= job && job <= currentJobCount)
                                {
                                    int t1 = tnMatrix[device - 1, batchIndex, job];
                                    int t2 = timeCalc(device - 1 - 1, batchIndex - 1);

                                    int value1 = t1 + t2;

                                    t1 = tnMatrix[device, batchIndex, job - 1];
                                    t2 = timeCalc(device - 1, batchIndex - 1);

                                    int value2 = t1 + t2;

                                    int value12max = Math.Max(value1, value2);

                                    int value3 = tnMatrix[device + 1, batchIndex, job - bufferSize];

                                    int value = Math.Max(value12max, value3);

                                    tnMatrix.AddNode(device, batchIndex, job, value);
                                }

                                // Продолжаем вычисления для следующего прибора
                                continue;
                            }

                        }

                        // Для последнего прибора выполняем обработку
                        if (device == deviceCount)
                        {

                            // Для первого пакета в последовательности выполняем обработку
                            if (batchIndex == 1)
                            {

                                // Для первого задания в пакете выполняем обработку
                                if (job == 1)
                                {
                                    int value = 0;
                                    for (int li = 1; li <= deviceCount - 1; li++)
                                    {
                                        value += timeCalc(li - 1, 1 - 1);
                                    }

                                    tnMatrix.AddNode(deviceCount, batchIndex, job, value);

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }

                                // Если данное задание превышает размер буфера, выполняем обработку
                                // 45
                                // TODO: нет необходимости обрабатывать случай (job <= currentJobCount), если значение job не имзеняется динамично, так как условие прописано в 
                                if (1 < job && job <= currentJobCount)
                                {
                                    int t1 = tnMatrix[device - 1, batchIndex, job];
                                    int t2 = timeCalc(device - 1 - 1, batchIndex - 1);

                                    int value1 = t1 + t2;

                                    t1 = tnMatrix[device, batchIndex, job - 1];
                                    t2 = timeCalc(deviceCount - 1, batchIndex - 1);

                                    int value2 = t1 + t2;

                                    int value = Math.Max(value1, value2);

                                    tnMatrix.AddNode(deviceCount, batchIndex, job, value);
                                }

                                // Продолжаем вычисления для следующего прибора
                                continue;
                            }

                            // Дл
                            // 4.5 (9)
                            // TODO: нет необходимости обрабатывать случай (batchIndex <= batchesForAllDataTypes), если значение batchIndex не имзеняется динамично, так как условие прописано в цикле
                            if (2 <= batchIndex && batchIndex <= batchesForAllDataTypes)
                            {

                                // Для первого задания в пакете выполняем 
                                if (job == 1)
                                {
                                    int t1 = tnMatrix[device - 1, batchIndex, job];
                                    int t2 = timeCalc(deviceCount - 1 - 1, batchIndex - 1);

                                    int value1 = t1 + t2;

                                    t1 = tnMatrix[deviceCount, batchIndex - 1, nJPrevious];
                                    t2 = timeCalc(deviceCount - 1, batchIndex - 1 - 1);
                                    int t3 = timeChangeover[deviceCount, previousType, currentDataType];

                                    int value2 = t1 + t2 + t3;

                                    int value = Math.Max(value1, value2);

                                    tnMatrix.AddNode(deviceCount, batchIndex, job, value);

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }


                                // Если данное задание превышает размер буфера, выполняем обработку
                                // 48
                                // TODO: нет необходимости обрабатывать случай (job <= currentJobCount), если значение job не имзеняется динамично, так как условие прописано в 
                                if (1 < job && job <= currentJobCount)
                                {
                                    int t1 = tnMatrix[deviceCount - 1, batchIndex, job];
                                    int t2 = timeCalc(deviceCount - 1 - 1, batchIndex - 1);

                                    int value1 = t1 + t2;

                                    t1 = tnMatrix[deviceCount, batchIndex, job - 1];
                                    t2 = timeCalc(deviceCount - 1, batchIndex - 1);
                                    int value2 = t1 + t2;

                                    int value = Math.Max(value1, value2);

                                    tnMatrix.AddNode(deviceCount, batchIndex, job, value);

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }

                            }
                        }
                    }
                }

                previousType = currentNode.dataType;
                nJPrevious = currentJobCount;
            }

            return tnMatrix;
        }
    }
}
