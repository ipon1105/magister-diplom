using newAlgorithm.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;

namespace newAlgorithm.Service
{
    public class CalculationService
    {

        /// <summary>
        /// Матрица P представляет из себя матрицу последовательностей состоящая из 0 и 1. Структура матрица следующая [dataTypesCount x maxBatchesPositions]
        /// </summary>
        /// <param name="pMatrix">Двойной список P</param>
        /// <param name="batchIndex">Индекс партии в последовательности</param>
        /// <returns>Тип данных</returns>
        public static int CalculationDataTypeFromBatch(List<List<int>> pMatrix, int batchIndex)
        {

            // Инициализируем тип данных
            int dataType = 0;

            // До тех пор, пока не нашли
            while (true)
                if (pMatrix[dataType][batchIndex] == 1)
                    return dataType;
                else
                    dataType++;
        }

        /// <summary>
        /// Матрица P представляет из себя матрицу последовательностей состоящая из 0 и 1. Структура матрица следующая [dataTypesCount x maxBatchesPositions]
        /// </summary>
        /// <param name="pMatrix">Матрица P</param>
        /// <param name="batchIndex">Индекс партии в последовательности</param>
        /// <returns>Тип данных</returns>
        public static int CalculationDataTypeFromBatch(Matrix pMatrix, int batchIndex)
        {

            // Инициализируем тип данных
            int dataType = 0;

            // До тех пор, пока не нашли
            while (true)
                if (pMatrix[dataType, batchIndex] == 1)
                    return dataType;
                else 
                    dataType++;
        }

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
            List<List<int>> rMatrix,
            List<List<int>> pMatrix,
            List<List<int>> timeProcessing,
            TreeDimMatrix timeChangeover,
            int bufferSize
            )
        {

            TreeDimMatrix tnMatrix = new TreeDimMatrix(timeChangeover.deviceCount);

            // Количество пакетов для всех типов данных, так же известное как n_p
            int maxBatchesPositions = pMatrix[0].Count;

            // Количество приборов в конвейерной системе
            int deviceCount = timeChangeover.deviceCount;

            // Предыдущий тип
            int previousType = 0;
            int previousJob = 0;

            // Для всех пакетов выполняем обработку. batchIndex так же известен, как h
            for (int batchIndex = 0; batchIndex < maxBatchesPositions; batchIndex++)
            {

                // Получаем узел матрицы R в позиции batchIndex
                int currentJobCount = 0;
                int currentDataType = 0;

                // Из узла матрицы R вытаскиваем тип данных и количество заданий данного типа
                for (int dataType = 0; dataType < rMatrix.Count; dataType++)
                    
                    // Если для текущей позиции найден ПЗ
                    if (rMatrix[dataType][batchIndex] != 0) { 
                        currentJobCount = rMatrix[dataType][batchIndex];
                        currentDataType = dataType + 1;
                        break;
                    }
                

                // Для всех заданий выполняем обработку. job так же известен, как q
                for (int job = 0; job < currentJobCount; job++)
                {

                    // Данная функция высчитываем время
                    int getProccessingTimeOnDeviceInBatch(int _device, int _batchIndex)
                    {

                        // Время выполнения
                        int _procTime = 0;
                        int _dataType = 0;

                        // До тех пор, пока не нашли нужное время выполнения
                        while (_procTime == 0)
                        {
                            // Подсчёт времени выполнения типа данных dataType на устройстве _device
                            int _position = pMatrix[_dataType][_batchIndex];
                            
                            // Получаем время выполнения типа _dataType на приборе _device
                            int _proccessingTime = timeProcessing[_device][_dataType++];

                            // Выполяем подсчёт времени выполнения на позицию
                            _procTime = _proccessingTime * _position;
                        }

                        return _procTime;
                    }

                    // Для всех приборов выполняем обработку
                    for (int device = 0; device < deviceCount; device++)
                    {

                        // Для первого прибора выполняем обработку
                        if (device + 1 == 1)
                        {

                            // Для первого пакета в последовательности выполняем обработку
                            // 4.1 (4)
                            if (batchIndex + 1 == 1)
                            {

                                // Если данное задание является первым в пакете, добавляем его в матрицу
                                if (job + 1 == 1)

                                    // TODO: Релазиовать наладку приборов
                                    tnMatrix.AddNode(device + 1, batchIndex + 1, job + 1, 0);

                                // Если данное задание не первое и не превышает размер буфера, выполняем обработку
                                if (1 < job + 1 && job + 1 <= bufferSize + 1)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = tnMatrix[device + 1, batchIndex + 1, job + 1 - 1];
                                    int procTime = getProccessingTimeOnDeviceInBatch(device + 1 - 1, batchIndex + 1 - 1);

                                    // Высчитываем время конца выполнения задания
                                    int stopTime = startTime + procTime;

                                    // Добавляем время конца выполнения задания матрицу
                                    tnMatrix.AddNode(device + 1, batchIndex + 1, job + 1, stopTime);

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }

                                // Если данное задание превышает размер буфера, выполняем обработку
                                // 45
                                // TODO: нет необходимости обрабатывать случай (job + 1 <= currentJobCount), если значение job + 1 не имзеняется динамично, так как условие прописано в 
                                if (bufferSize + 1 < job + 1 && job + 1 <= currentJobCount)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = tnMatrix[device + 1, batchIndex + 1, job + 1 - 1];
                                    int procTime = getProccessingTimeOnDeviceInBatch(device + 1 - 1, batchIndex + 1 - 1);

                                    // Высчитываем время конца выполнения задания
                                    int stopTime = startTime + procTime;

                                    // Время начала задания на следующем приборе 
                                    int startBufferTime = tnMatrix[device + 1 + 1, batchIndex + 1, job + 1 - bufferSize];

                                    // Выбираем между время между концом выполнения текущего задания и началом выполнения задания в буфере на следующем приборе
                                    int resultTime = Math.Max(stopTime, startBufferTime);

                                    // Добавляем результат в матрицу
                                    tnMatrix.AddNode(device + 1, batchIndex + 1, job + 1, resultTime);
                                }

                                // Продолжаем вычисления для следующего прибора
                                continue;
                            }

                            // Для любого не первого пакета в последовательности выполняем обработку
                            // TODO: нет необходимости обрабатывать случай (batchIndex + 1 <= batchesForAllDataTypes), если значение batchIndex + 1 не имзеняется динамично, так как условие прописано в цикле
                            if (2 <= batchIndex + 1 && batchIndex + 1 <= maxBatchesPositions)
                            {

                                // Для первого задания в пакете выполняем обработку
                                if (job + 1 == 1)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = tnMatrix[device + 1, batchIndex + 1 - 1, previousJob];
                                    int procTime = getProccessingTimeOnDeviceInBatch(device + 1 - 1, batchIndex + 1 - 1 - 1);

                                    // Высчитываем время переналадки с предыдущего типа на текущий
                                    int changeTime = timeChangeover[device + 1, previousType, currentDataType];

                                    // Высчитываем время конца выполнения задания
                                    int stopTime = changeTime + startTime + procTime;

                                    // Время начала задания на следующем приборе предыдущего пакета предыдущего задания
                                    int startBufferTime = tnMatrix[device + 1 + 1, batchIndex + 1 - 1, previousJob - bufferSize + 1];

                                    // Выбираем между время между концом выполнения текущего задания и началом выполнения задания в буфере на следующем приборе
                                    int result = Math.Max(stopTime, startBufferTime);

                                    // Добавляем результат в матрицу
                                    tnMatrix.AddNode(device + 1, batchIndex + 1, job + 1, result);

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }

                                // Если данное задание не первое и не превышает размер буфера, выполняем обработку
                                if (1 < job + 1 && job + 1 <= bufferSize)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = tnMatrix[device + 1, batchIndex + 1, job + 1 - 1];
                                    int procTime = getProccessingTimeOnDeviceInBatch(device + 1 - 1, batchIndex + 1 - 1);

                                    // Высчитываем время конца выполнения задания
                                    int stopTime = startTime + procTime;

                                    // Время начала задания на следующем приборе предыдущего пакета предыдущего задания
                                    int startBufferTime = tnMatrix[device + 1 + 1, batchIndex + 1 - 1, previousJob - bufferSize + job + 1];

                                    // Выбираем между время между концом выполнения текущего задания и началом выполнения задания в буфере на следующем приборе
                                    stopTime = Math.Max(stopTime, startBufferTime);

                                    // Добавляем результат в матрицу
                                    tnMatrix.AddNode(device + 1, batchIndex + 1, job + 1, stopTime);

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }

                                // Если данное задание превышает размер буфера, выполняем обработку
                                // 45
                                // TODO: нет необходимости обрабатывать случай (job + 1 <= currentJobCount), если значение job + 1 не имзеняется динамично, так как условие прописано в 
                                if (bufferSize + 1 <= job + 1 && job + 1 <= currentJobCount)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = tnMatrix[device + 1, batchIndex + 1, job + 1 - 1];
                                    int procTime = getProccessingTimeOnDeviceInBatch(device + 1 - 1, batchIndex + 1 - 1);

                                    // Высчитываем время конца выполнения задания
                                    int stopTime = startTime + procTime;

                                    // Время начала задания на следующем приборе предыдущего пакета предыдущего задания
                                    int startBufferTime = tnMatrix[device + 1 + 1, batchIndex + 1, job + 1 - bufferSize];

                                    // Выбираем между концом выполнения текущего задания и началом выполнения задания в буфере на следующем приборе
                                    int result = Math.Max(stopTime, startBufferTime);

                                    // Добавляем результат в матрицу
                                    tnMatrix.AddNode(device + 1, batchIndex + 1, job + 1, result);
                                }

                                // Продолжаем вычисления для следующего прибора
                                continue;
                            }
                        }

                        // Для любого не первого и не последнего прибора выполняем обработку
                        //4.3 (6)
                        if (2 <= device + 1 && device + 1 <= deviceCount - 1)
                        {

                            // Для первого пакета в последовательности выполняем обработку
                            if(batchIndex + 1 == 1)
                            {

                                // Для первого задания в пакете выполняем обработку
                                // 49
                                if (job + 1 == 1)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = tnMatrix[device + 1 - 1, batchIndex + 1, batchIndex + 1];
                                    int procTime = getProccessingTimeOnDeviceInBatch(device + 1 - 1 - 1, batchIndex + 1 - 1);

                                    // Высчитываем время конца выполнения задания
                                    int stopTime = startTime + procTime;

                                    // Добавляем результат в матрицу
                                    tnMatrix.AddNode(device + 1, batchIndex + 1, job + 1, stopTime);

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }

                                // Если данное задание не первое и не превышает размер буфера, выполняем обработку
                                //50
                                if (1 < job + 1 && job + 1 <= bufferSize + 1)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = tnMatrix[device + 1 - 1, batchIndex + 1, job + 1];
                                    int procTime = getProccessingTimeOnDeviceInBatch(device + 1 - 1 - 1, 1 - 1);

                                    // Высчитываем время конца выполнения задания
                                    int stopTimeCurrentJob = startTime + procTime;

                                    // Высчитываем время начала и выполнения предыдущего задания
                                    startTime = tnMatrix[device + 1, batchIndex + 1, job + 1 - 1];
                                    procTime = getProccessingTimeOnDeviceInBatch(device + 1 - 1, batchIndex + 1 - 1);

                                    // Высчитываем время конца выполнения предыдущего задания
                                    int stopTimePreviousJob = startTime + procTime;

                                    // Выбираем между концом выполнения текущего задания и концом выполнения предыдущего задания
                                    int result = Math.Max(stopTimeCurrentJob, stopTimePreviousJob);

                                    // Добавляем результат в матрицу
                                    tnMatrix.AddNode(device + 1, batchIndex + 1, job + 1, result);

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }

                                // Если данное задание превышает размер буфера, выполняем обработку
                                // 45
                                // TODO: нет необходимости обрабатывать случай (job + 1 <= currentJobCount), если значение job + 1 не имзеняется динамично, так как условие прописано в 
                                if (bufferSize + 1 < job + 1 && job + 1 <= currentJobCount)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = tnMatrix[device + 1 - 1, batchIndex + 1, job + 1];
                                    int procTime = getProccessingTimeOnDeviceInBatch(device + 1 - 1 - 1, 1 - 1);

                                    // Высчитываем время конца выполнения задания
                                    int stopTimeCurrentJob = startTime + procTime;

                                    // Высчитываем время начала и выполнения предыдущего задания
                                    startTime = tnMatrix[device + 1, batchIndex + 1, job + 1 - 1];
                                    procTime = getProccessingTimeOnDeviceInBatch(device + 1 - 1, batchIndex + 1 - 1);

                                    // Высчитываем время конца выполнения предыдущего задания
                                    int stopTimePreviousJob = startTime + procTime;

                                    // Выбираем между концом выполнения текущего задания и концом выполнения предыдущего задания
                                    int stopTime = Math.Max(stopTimeCurrentJob, stopTimePreviousJob);

                                    // Время начала задания на следующем приборе предыдущего задания через буфер
                                    int startBufferTime = tnMatrix[device + 1 + 1, batchIndex + 1, job + 1 - bufferSize];

                                    // Выбираем между концом выполнения задания и концом выполнения задания через буфер
                                    int result = Math.Max(stopTime, startBufferTime);

                                    // Добавляем результат в матрицу
                                    tnMatrix.AddNode(device + 1, batchIndex + 1, job + 1, result);
                                }

                                // Продолжаем вычисления для следующего прибора
                                continue;
                            }

                            // Для любого не первого пакета в последовательности выполняем обработку
                            //4.4 (7)
                            // TODO: нет необходимости обрабатывать случай (batchIndex + 1 <= batchesForAllDataTypes), если значение batchIndex + 1 не имзеняется динамично, так как условие прописано в цикле
                            if (2 <= batchIndex + 1 && batchIndex + 1 <= maxBatchesPositions)
                            {
                                
                                // Для первого задания в пакете выполняем обработку
                                if (job + 1 == 1)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = tnMatrix[device + 1 - 1, batchIndex + 1, 1];
                                    int procTime = getProccessingTimeOnDeviceInBatch(device + 1 - 1 - 1, batchIndex + 1 - 1);

                                    // Высчитываем время конца выполнения задания
                                    int stopTimeCurrentJob = startTime + procTime;

                                    // Высчитываем время начала и выполнения предыдущего задания
                                    startTime = tnMatrix[device + 1, batchIndex + 1 - 1, previousJob];
                                    procTime = getProccessingTimeOnDeviceInBatch(device + 1 - 1, batchIndex + 1 - 1 - 1);

                                    // Время переналадки прибора с предыдущего типа на текущий
                                    int changeTime = timeChangeover[device + 1, previousType, currentDataType];

                                    // Высчитываем время конца выполнения предыдущего задания
                                    int stopTimePreviousJob = startTime + procTime + changeTime;

                                    // Выбираем между концом выполнения текущего задания и концом выполнения предыдущего задания
                                    int stopTime = Math.Max(stopTimeCurrentJob, stopTimePreviousJob);

                                    // Время начала задания на следующем приборе предыдущего задания через буфер
                                    int startBufferTime = tnMatrix[device + 1 + 1, batchIndex + 1 - 1, previousJob - bufferSize + 1];

                                    // Выбираем между концом выполнения задания и концом выполнения задания через буфер
                                    int result = Math.Max(stopTime, startBufferTime);

                                    // Добавляем результат в матрицу
                                    tnMatrix.AddNode(device + 1, batchIndex + 1, job + 1, result);

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }

                                // Если данное задание не первое и не превышает размер буфера, выполняем обработку
                                //53
                                if (1 < job + 1 && job + 1 <= bufferSize)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = tnMatrix[device + 1 - 1, batchIndex + 1, job + 1];
                                    int procTime = getProccessingTimeOnDeviceInBatch(device + 1 - 1 - 1, batchIndex + 1 - 1);

                                    // Высчитываем время конца выполнения задания
                                    int stopTimeCurrentJob = startTime + procTime;

                                    // Высчитываем время начала и выполнения предыдущего задания
                                    startTime = tnMatrix[device + 1, batchIndex + 1, job + 1 - 1];
                                    procTime = getProccessingTimeOnDeviceInBatch(device + 1 - 1, batchIndex + 1 - 1);

                                    // Высчитываем время конца выполнения предыдущего задания
                                    int stopTimePreviousJob = startTime + procTime;

                                    // Выбираем между концом выполнения текущего задания и концом выполнения предыдущего задания
                                    int stopTime = Math.Max(stopTimeCurrentJob, stopTimePreviousJob);

                                    // Время начала задания на следующем приборе предыдущего задания через буфер
                                    int startBufferTime = tnMatrix[device + 1 + 1, batchIndex + 1 - 1, previousJob - bufferSize + job + 1];

                                    // Выбираем между концом выполнения задания и концом выполнения задания через буфер
                                    int result = Math.Max(stopTime, startBufferTime);

                                    // Добавляем результат в матрицу
                                    tnMatrix.AddNode(device + 1, batchIndex + 1, job + 1, result);

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }

                                // Если данное задание превышает размер буфера, выполняем обработку
                                // 54
                                // TODO: нет необходимости обрабатывать случай (job + 1 <= currentJobCount), если значение job + 1 не имзеняется динамично, так как условие прописано в 
                                if (bufferSize + 1 <= job + 1 && job + 1 <= currentJobCount)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = tnMatrix[device + 1 - 1, batchIndex + 1, job + 1];
                                    int procTime = getProccessingTimeOnDeviceInBatch(device + 1 - 1 - 1, batchIndex + 1 - 1);

                                    // Высчитываем время конца выполнения задания
                                    int stopTimeCurrentJob = startTime + procTime;

                                    // Высчитываем время начала и выполнения предыдущего задания
                                    startTime = tnMatrix[device + 1, batchIndex + 1, job + 1 - 1];
                                    procTime = getProccessingTimeOnDeviceInBatch(device + 1 - 1, batchIndex + 1 - 1);

                                    // Высчитываем время конца выполнения предыдущего задания
                                    int stopTimePreviousJob = startTime + procTime;

                                    // Выбираем между концом выполнения текущего задания и концом выполнения предыдущего задания
                                    int stopTime = Math.Max(stopTimeCurrentJob, stopTimePreviousJob);

                                    // Время начала задания на следующем приборе задания через буфер
                                    int startBufferTime = tnMatrix[device + 1 + 1, batchIndex + 1, job + 1 - bufferSize];

                                    // Выбираем между концом выполнения задания и концом выполнения задания через буфер
                                    int result = Math.Max(stopTime, startBufferTime);

                                    // Добавляем результат в матрицу
                                    tnMatrix.AddNode(device + 1, batchIndex + 1, job + 1, result);
                                }

                                // Продолжаем вычисления для следующего прибора
                                continue;
                            }
                        }

                        // Для последнего прибора выполняем обработку
                        if (device + 1 == deviceCount)
                        {

                            // Для первого пакета в последовательности выполняем обработку
                            if (batchIndex + 1 == 1)
                            {

                                // Для первого задания в пакете выполняем обработку
                                if (job + 1 == 1)
                                {

                                    // Подсчитываем время выполнения для всех пакетов
                                    int result = 0;
                                    for (int li = 1; li <= deviceCount - 1; li++)
                                        result += getProccessingTimeOnDeviceInBatch(li - 1, 0);

                                    // Добавляем результат в матрицу
                                    tnMatrix.AddNode(deviceCount, batchIndex + 1, job + 1, result);

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }

                                // Если данное задание превышает размер буфера, выполняем обработку
                                // 45
                                // TODO: нет необходимости обрабатывать случай (job + 1 <= currentJobCount), если значение job + 1 не имзеняется динамично, так как условие прописано в 
                                if (1 < job + 1 && job + 1 <= currentJobCount)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = tnMatrix[device + 1 - 1, batchIndex + 1, job + 1];
                                    int procTime = getProccessingTimeOnDeviceInBatch(device + 1 - 1 - 1, batchIndex + 1 - 1);

                                    // Высчитываем время конца выполнения задания
                                    int stopTimeCurrentJob = startTime + procTime;

                                    // Высчитываем время начала и выполнения предыдущего задания
                                    startTime = tnMatrix[device + 1, batchIndex + 1, job + 1 - 1];
                                    procTime = getProccessingTimeOnDeviceInBatch(deviceCount - 1, batchIndex + 1 - 1);

                                    // Высчитываем время конца выполнения предыдущего задания
                                    int stopTimePreviousJob = startTime + procTime;

                                    // Выбираем между концом выполнения текущего и предыдущего задания
                                    int result = Math.Max(stopTimeCurrentJob, stopTimePreviousJob);

                                    // Добавляем результат в матрицу
                                    tnMatrix.AddNode(deviceCount, batchIndex + 1, job + 1, result);
                                }

                                // Продолжаем вычисления для следующего прибора
                                continue;
                            }

                            // Дл
                            // 4.5 (9)
                            // TODO: нет необходимости обрабатывать случай (batchIndex + 1 <= batchesForAllDataTypes), если значение batchIndex + 1 не имзеняется динамично, так как условие прописано в цикле
                            if (2 <= batchIndex + 1 && batchIndex + 1 <= maxBatchesPositions)
                            {

                                // Для первого задания в пакете выполняем 
                                if (job + 1 == 1)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = tnMatrix[device + 1 - 1, batchIndex + 1, job + 1];
                                    int procTime = getProccessingTimeOnDeviceInBatch(deviceCount - 1 - 1, batchIndex + 1 - 1);

                                    // Высчитываем время конца выполнения задания
                                    int stopTimeCurrentJob = startTime + procTime;

                                    // Высчитываем время начала и выполнения предыдущего задания
                                    startTime = tnMatrix[deviceCount, batchIndex + 1 - 1, previousJob];
                                    procTime = getProccessingTimeOnDeviceInBatch(deviceCount - 1, batchIndex + 1 - 1 - 1);

                                    // Время переналадки с предыдущего типа на текущей
                                    int changeTime = timeChangeover[deviceCount, previousType, currentDataType];

                                    // Высчитываем время конца выполнения предыдущего задания
                                    int stopTimePreviousJob = changeTime + startTime + procTime;

                                    // Выбираем между концом выполнения текущего и предыдущего задания
                                    int result = Math.Max(stopTimeCurrentJob, stopTimePreviousJob);

                                    // Добавляем результат в матрицу
                                    tnMatrix.AddNode(deviceCount, batchIndex + 1, job + 1, result);

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }


                                // Если данное задание превышает размер буфера, выполняем обработку
                                // 48
                                // TODO: нет необходимости обрабатывать случай (job + 1 <= currentJobCount), если значение job + 1 не имзеняется динамично, так как условие прописано в 
                                if (1 < job + 1 && job + 1 <= currentJobCount)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = tnMatrix[deviceCount - 1, batchIndex + 1, job + 1];
                                    int procTime = getProccessingTimeOnDeviceInBatch(deviceCount - 1 - 1, batchIndex + 1 - 1);

                                    // Высчитываем время конца выполнения задания
                                    int stopTimeCurrentJob = startTime + procTime;

                                    // Высчитываем время начала и выполнения предыдущего задания
                                    startTime = tnMatrix[deviceCount, batchIndex + 1, job + 1 - 1];
                                    procTime = getProccessingTimeOnDeviceInBatch(deviceCount - 1, batchIndex + 1 - 1);

                                    // Высчитываем время конца выполнения предыдущего задания
                                    int stopTimePreviousJob = startTime + procTime;

                                    // Выбираем между концом выполнения текущего и предыдущего задания
                                    int result = Math.Max(stopTimeCurrentJob, stopTimePreviousJob);

                                    // Добавляем результат в матрицу
                                    tnMatrix.AddNode(deviceCount, batchIndex + 1, job + 1, result);

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }

                            }
                        }
                    }
                }

                // Переопределяем предыдущий тип и задание
                previousType = currentDataType;
                previousJob = currentJobCount;
            }

            return tnMatrix;
        }
    }
}
