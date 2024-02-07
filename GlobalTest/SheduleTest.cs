using magisterDiplom.Model;
using newAlgorithm;
using newAlgorithm.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalTest
{

    /// <summary>
    /// Данный класс описывает тесты для класса PreM
    /// </summary>
    [TestClass]
    public class SheduleTest
    {

        [TestMethod]
        public void ScheduleBuild_without_preM_1()
        {

            // Формируем входные данные
            #region Init input

            /*
            // dataTypesCount:
            // 2
            // 
            // deviceCount:
            // 2
            // 
            // buffer:
            // 999
            // 
            // proccessingTime:
            // +---+---+
            // | 1 | 2 |
            // +---+---+
            // | 3 | 4 |
            // +---+---+
            //
            // changeoverTime:
            // +---+---+---+
            // |   | 1 | 2 |
            // + 1 +---+---+
            // |   | 3 | 4 |
            // +---+---+---+
            // |   | 4 | 2 |
            // + 2 +---+---+
            // |   | 3 | 1 |
            // +---+---+---+
            //
            // preMaintenanceTimes
            // +---+---+
            // | 3 | 1 |
            // +---+---+
            //
            // isFixedBatches:
            // false
            */

            // Объявляем матрицу переналадки
            Dictionary<int, Matrix> changeoverTime = new Dictionary<int, Matrix>();

            // Создаём матрицу переналадки для 1 прибора
            Matrix changeoverTime_1 = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Создаём матрицу переналадки для 2 прибора
            Matrix changeoverTime_2 = new Matrix(new List<List<int>>
                {
                    new List<int> { 4, 2 },
                    new List<int> { 3, 1 },
                });

            // Добавляем матрицы переналадки в changeoverTime
            changeoverTime.Add(0, changeoverTime_1);
            changeoverTime.Add(1, changeoverTime_2);

            // Создаём матрицу времени выполнения
            Matrix proccessingTime = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Формируем конфигурационный файл
            Config config = new Config(
                2, // int dataTypesCount,
                2, // int deviceCount,
                999, // int buffer,
                proccessingTime, // Matrix proccessingTime,
                changeoverTime,// Dictionary<int, Matrix> changeoverTime,
                new Vector(new List<int> { 3, 1 }),
                false// bool isFixedBatches
            );

            // Объявляем и инициализируем последовательность ПЗ
            List<magisterDiplom.Model.Batch> schedule = new List<magisterDiplom.Model.Batch>
            {
                new magisterDiplom.Model.Batch(0, 3),
                new magisterDiplom.Model.Batch(1, 2),
            };

            // Создаём позиционную матрицу Y
            List<List<int>> Y = new List<List<int>>
            {
                new List<int> { 0, 0 },
                new List<int> { 0, 0 }
            };
            #endregion

            // Формируем выходные данные
            #region Init output

            /*
            //   0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 24
            //   |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // d1|  |A |A |A |  |  |  B  |  B  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // d2|  |  |  |  |   A    |   A    |   A    |  |  |     B     |     B     |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            */

            // Объявляем выходную матрицу T
            Dictionary<int, List<List<int>>> _matrixT = new Dictionary<int, List<List<int>>>();

            // Инициализируем её выходными данными для 1 прибора
            _matrixT.Add(0, new List<List<int>> {
                new List<int> { 1,2,3 },
                new List<int> { 6,8 },
            });

            // Инициализируем её выходными данными для 2 прибора
            _matrixT.Add(1, new List<List<int>> {
                new List<int> { 4,7,10 },
                new List<int> { 15,19 },
            });

            #endregion

            // Выполняем вызов функции построения расписания
            Dictionary<int, List<List<int>>> matrixT = PreM.Build(
                config,
                schedule,
                Y
            );

            // Выполняем проверку на соответствие количества приборов в входной и выходной матрице
            Assert.IsTrue(matrixT.Count == _matrixT.Count);

            // Для каждого прибора выполняем проверку
            for (int device = 0; device < config.deviceCount; device++)
            {

                // Выполняем проверку на соответствие количества ПЗ в входной и выходной матрице
                Assert.IsTrue(_matrixT[device].Count == matrixT[device].Count);

                // Для каждого ПЗ выполняем проверку
                for (int batchIndex = 0; batchIndex < matrixT[device].Count; batchIndex++)
                {

                    // Выполняем проверку на соответствие количества заданий в входной и выходной матрице
                    Assert.IsTrue(_matrixT[device][batchIndex].Count == matrixT[device][batchIndex].Count);

                    // Для каждого задания выполняем проверку
                    for (int job = 0; job < matrixT[device][batchIndex].Count; job++)

                        // Выполняем проверку на равнозначность моментов начала времени
                        // выполнения заданийв входной и выходной матрице
                        Assert.AreEqual(_matrixT[device][batchIndex][job], matrixT[device][batchIndex][job]);
                }
            }
        }

        [TestMethod]
        public void ScheduleBuild_without_preM_2()
        {

            // Формируем входные данные
            #region Init input

            /*
            // dataTypesCount:
            // 2
            // 
            // deviceCount:
            // 2
            // 
            // buffer:
            // 999
            // 
            // proccessingTime:
            // +---+---+
            // | 1 | 2 |
            // +---+---+
            // | 3 | 4 |
            // +---+---+
            //
            // changeoverTime:
            // +---+---+---+
            // |   | 1 | 2 |
            // + 1 +---+---+
            // |   | 3 | 4 |
            // +---+---+---+
            // |   | 4 | 2 |
            // + 2 +---+---+
            // |   | 3 | 1 |
            // +---+---+---+
            //
            // preMaintenanceTimes
            // +---+---+
            // | 3 | 1 |
            // +---+---+
            //
            // isFixedBatches:
            // false
            */

            // Объявляем матрицу переналадки
            Dictionary<int, Matrix> changeoverTime = new Dictionary<int, Matrix>();

            // Создаём матрицу переналадки для 1 прибора
            Matrix changeoverTime_1 = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Создаём матрицу переналадки для 2 прибора
            Matrix changeoverTime_2 = new Matrix(new List<List<int>>
                {
                    new List<int> { 4, 2 },
                    new List<int> { 3, 1 },
                });

            // Добавляем матрицы переналадки в changeoverTime
            changeoverTime.Add(0, changeoverTime_1);
            changeoverTime.Add(1, changeoverTime_2);

            // Создаём матрицу времени выполнения
            Matrix proccessingTime = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Формируем конфигурационный файл
            Config config = new Config(
                2, // int dataTypesCount,
                2, // int deviceCount,
                999, // int buffer,
                proccessingTime, // Matrix proccessingTime,
                changeoverTime,// Dictionary<int, Matrix> changeoverTime,
                new Vector(new List<int> { 3, 1 }),
                false// bool isFixedBatches
            );

            // Объявляем и инициализируем последовательность ПЗ
            List<magisterDiplom.Model.Batch> schedule = new List<magisterDiplom.Model.Batch>
            {
                new magisterDiplom.Model.Batch(1, 2),
                new magisterDiplom.Model.Batch(0, 3),
            };

            // Создаём позиционную матрицу Y
            List<List<int>> Y = new List<List<int>>
            {
                new List<int> { 0, 0 },
                new List<int> { 0, 0 }
            };
            #endregion

            // Формируем выходные данные
            #region Init output

            /*
            //   0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28
            //   |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // d1|  |  |  |  |  B  |  B  |  |  |  |A |A |A |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // d2|  |  |  |  |  |  |    B      |     B     |  |  |  |   A    |   A    |   A    |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            */

            // Объявляем выходную матрицу T
            Dictionary<int, List<List<int>>> _matrixT = new Dictionary<int, List<List<int>>>();

            // Инициализируем её выходными данными для 1 прибора
            _matrixT.Add(0, new List<List<int>> {
                new List<int> { 4,6 },
                new List<int> { 11,12,13 },
            });

            // Инициализируем её выходными данными для 2 прибора
            _matrixT.Add(1, new List<List<int>> {
                new List<int> { 6,10 },
                new List<int> { 17,20,23 },
            });

            #endregion

            // Выполняем вызов функции построения расписания
            Dictionary<int, List<List<int>>> matrixT = PreM.Build(
                config,
                schedule,
                Y
            );

            // Выполняем проверку на соответствие количества приборов в входной и выходной матрице
            Assert.IsTrue(matrixT.Count == _matrixT.Count);

            // Для каждого прибора выполняем проверку
            for (int device = 0; device < config.deviceCount; device++)
            {

                // Выполняем проверку на соответствие количества ПЗ в входной и выходной матрице
                Assert.IsTrue(_matrixT[device].Count == matrixT[device].Count);

                // Для каждого ПЗ выполняем проверку
                for (int batchIndex = 0; batchIndex < matrixT[device].Count; batchIndex++)
                {

                    // Выполняем проверку на соответствие количества заданий в входной и выходной матрице
                    Assert.IsTrue(_matrixT[device][batchIndex].Count == matrixT[device][batchIndex].Count);

                    // Для каждого задания выполняем проверку
                    for (int job = 0; job < matrixT[device][batchIndex].Count; job++)

                        // Выполняем проверку на равнозначность моментов начала времени
                        // выполнения заданийв входной и выходной матрице
                        Assert.AreEqual(_matrixT[device][batchIndex][job], matrixT[device][batchIndex][job]);
                }
            }
        }

        [TestMethod]
        public void ScheduleBuild_preM_1()
        {

            // Формируем входные данные
            #region Init input

            /*
            // dataTypesCount:
            // 2
            // 
            // deviceCount:
            // 2
            // 
            // buffer:
            // 999
            // 
            // proccessingTime:
            // +---+---+
            // | 1 | 2 |
            // +---+---+
            // | 3 | 4 |
            // +---+---+
            //
            // changeoverTime:
            // +---+---+---+
            // |   | 1 | 2 |
            // + 1 +---+---+
            // |   | 3 | 4 |
            // +---+---+---+
            // |   | 4 | 2 |
            // + 2 +---+---+
            // |   | 3 | 1 |
            // +---+---+---+
            //
            // preMaintenanceTimes
            // +---+---+
            // | 3 | 1 |
            // +---+---+
            //
            // isFixedBatches:
            // false
            */

            // Объявляем матрицу переналадки
            Dictionary<int, Matrix> changeoverTime = new Dictionary<int, Matrix>();

            // Создаём матрицу переналадки для 1 прибора
            Matrix changeoverTime_1 = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Создаём матрицу переналадки для 2 прибора
            Matrix changeoverTime_2 = new Matrix(new List<List<int>>
                {
                    new List<int> { 4, 2 },
                    new List<int> { 3, 1 },
                });

            // Добавляем матрицы переналадки в changeoverTime
            changeoverTime.Add(0, changeoverTime_1);
            changeoverTime.Add(1, changeoverTime_2);

            // Создаём матрицу времени выполнения
            Matrix proccessingTime = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Формируем конфигурационный файл
            Config config = new Config(
                2, // int dataTypesCount,
                2, // int deviceCount,
                999, // int buffer,
                proccessingTime, // Matrix proccessingTime,
                changeoverTime,// Dictionary<int, Matrix> changeoverTime,
                new Vector(new List<int> { 3, 1 }),
                false// bool isFixedBatches
            );

            // Объявляем и инициализируем последовательность ПЗ
            List<magisterDiplom.Model.Batch> schedule = new List<magisterDiplom.Model.Batch>
            {
                new magisterDiplom.Model.Batch(0, 3),
                new magisterDiplom.Model.Batch(1, 2),
            };

            // Создаём позиционную матрицу Y
            List<List<int>> Y = new List<List<int>>
            {
                new List<int> { 1, 0 },
                new List<int> { 0, 0 }
            };
            #endregion

            // Формируем выходные данные
            #region Init output

            /*
            //   0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 24
            //   |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // d1|  |A |A |A |  |  |  |  |  |  B  |  B  |  |  |  |  |  |  |  |  |  |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // d2|  |  |  |  |   A    |   A    |   A    |  |  |     B     |     B     |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            */

            // Объявляем выходную матрицу T
            Dictionary<int, List<List<int>>> _matrixT = new Dictionary<int, List<List<int>>>();

            // Инициализируем её выходными данными для 1 прибора
            _matrixT.Add(0, new List<List<int>> {
                new List<int> { 1,2,3 },
                new List<int> { 9,11 },
            });

            // Инициализируем её выходными данными для 2 прибора
            _matrixT.Add(1, new List<List<int>> {
                new List<int> { 4,7,10 },
                new List<int> { 15,19 },
            });

            #endregion

            // Выполняем вызов функции построения расписания
            Dictionary<int, List<List<int>>> matrixT = PreM.Build(
                config,
                schedule,
                Y
            );

            // Выполняем проверку на соответствие количества приборов в входной и выходной матрице
            Assert.AreEqual(matrixT.Count, _matrixT.Count);

            // Для каждого прибора выполняем проверку
            for (int device = 0; device < config.deviceCount; device++)
            {

                // Выполняем проверку на соответствие количества ПЗ в входной и выходной матрице
                Assert.AreEqual(_matrixT[device].Count,matrixT[device].Count);

                // Для каждого ПЗ выполняем проверку
                for (int batchIndex = 0; batchIndex < matrixT[device].Count; batchIndex++)
                {

                    // Выполняем проверку на соответствие количества заданий в входной и выходной матрице
                    Assert.AreEqual(_matrixT[device][batchIndex].Count, matrixT[device][batchIndex].Count);

                    // Для каждого задания выполняем проверку
                    for (int job = 0; job < matrixT[device][batchIndex].Count; job++)

                        // Выполняем проверку на равнозначность моментов начала времени
                        // выполнения заданийв входной и выходной матрице
                        Assert.AreEqual(_matrixT[device][batchIndex][job], matrixT[device][batchIndex][job]);
                }
            }
        }

        [TestMethod]
        public void ScheduleBuild_preM_2()
        {

            // Формируем входные данные
            #region Init input

            /*
            // dataTypesCount:
            // 2
            // 
            // deviceCount:
            // 2
            // 
            // buffer:
            // 999
            // 
            // proccessingTime:
            // +---+---+
            // | 1 | 2 |
            // +---+---+
            // | 3 | 4 |
            // +---+---+
            //
            // changeoverTime:
            // +---+---+---+
            // |   | 1 | 2 |
            // + 1 +---+---+
            // |   | 3 | 4 |
            // +---+---+---+
            // |   | 4 | 2 |
            // + 2 +---+---+
            // |   | 3 | 1 |
            // +---+---+---+
            //
            // preMaintenanceTimes
            // +---+---+
            // | 3 | 1 |
            // +---+---+
            //
            // isFixedBatches:
            // false
            */

            // Объявляем матрицу переналадки
            Dictionary<int, Matrix> changeoverTime = new Dictionary<int, Matrix>();

            // Создаём матрицу переналадки для 1 прибора
            Matrix changeoverTime_1 = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Создаём матрицу переналадки для 2 прибора
            Matrix changeoverTime_2 = new Matrix(new List<List<int>>
                {
                    new List<int> { 4, 2 },
                    new List<int> { 3, 1 },
                });

            // Добавляем матрицы переналадки в changeoverTime
            changeoverTime.Add(0, changeoverTime_1);
            changeoverTime.Add(1, changeoverTime_2);

            // Создаём матрицу времени выполнения
            Matrix proccessingTime = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Формируем конфигурационный файл
            Config config = new Config(
                2, // int dataTypesCount,
                2, // int deviceCount,
                999, // int buffer,
                proccessingTime, // Matrix proccessingTime,
                changeoverTime,// Dictionary<int, Matrix> changeoverTime,
                new Vector(new List<int> { 3, 1 }),
                false// bool isFixedBatches
            );

            // Объявляем и инициализируем последовательность ПЗ
            List<magisterDiplom.Model.Batch> schedule = new List<magisterDiplom.Model.Batch>
            {
                new magisterDiplom.Model.Batch(1, 2),
                new magisterDiplom.Model.Batch(0, 3),
            };

            // Создаём позиционную матрицу Y
            List<List<int>> Y = new List<List<int>>
            {
                new List<int> { 0, 0 },
                new List<int> { 1, 0 }
            };
            #endregion

            // Формируем выходные данные
            #region Init output

            /*
            //   0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28
            //   |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // d1|  |  |  |  |  B  |  B  |  |  |  |A |A |A |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // d2|  |  |  |  |  |  |    B      |     B     |  |  |  |  |   A    |   A    |   A    |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            */

            // Объявляем выходную матрицу T
            Dictionary<int, List<List<int>>> _matrixT = new Dictionary<int, List<List<int>>>();

            // Инициализируем её выходными данными для 1 прибора
            _matrixT.Add(0, new List<List<int>> {
                new List<int> { 4,6 },
                new List<int> { 11,12,13 },
            });

            // Инициализируем её выходными данными для 2 прибора
            _matrixT.Add(1, new List<List<int>> {
                new List<int> { 6,10 },
                new List<int> { 18,21,24 },
            });

            #endregion

            // Выполняем вызов функции построения расписания
            Dictionary<int, List<List<int>>> matrixT = PreM.Build(
                config,
                schedule,
                Y
            );

            // Выполняем проверку на соответствие количества приборов в входной и выходной матрице
            Assert.IsTrue(matrixT.Count == _matrixT.Count);

            // Для каждого прибора выполняем проверку
            for (int device = 0; device < config.deviceCount; device++)
            {

                // Выполняем проверку на соответствие количества ПЗ в входной и выходной матрице
                Assert.IsTrue(_matrixT[device].Count == matrixT[device].Count);

                // Для каждого ПЗ выполняем проверку
                for (int batchIndex = 0; batchIndex < matrixT[device].Count; batchIndex++)
                {

                    // Выполняем проверку на соответствие количества заданий в входной и выходной матрице
                    Assert.IsTrue(_matrixT[device][batchIndex].Count == matrixT[device][batchIndex].Count);

                    // Для каждого задания выполняем проверку
                    for (int job = 0; job < matrixT[device][batchIndex].Count; job++)

                        // Выполняем проверку на равнозначность моментов начала времени
                        // выполнения заданийв входной и выходной матрице
                        Assert.AreEqual(_matrixT[device][batchIndex][job], matrixT[device][batchIndex][job]);
                }
            }
        }

        [TestMethod]
        public void CorrectScheduleBuild()
        {

            // Формируем входные данные
            #region Init input

            /*
            // dataTypesCount:
            // 2
            // 
            // deviceCount:
            // 2
            // 
            // buffer:
            // 999
            // 
            // proccessingTime:
            // +---+---+
            // | 1 | 2 |
            // +---+---+
            // | 3 | 4 |
            // +---+---+
            //
            // changeoverTime:
            // +---+---+---+
            // |   | 1 | 2 |
            // + 1 +---+---+
            // |   | 3 | 4 |
            // +---+---+---+
            // |   | 4 | 2 |
            // + 2 +---+---+
            // |   | 3 | 1 |
            // +---+---+---+
            //
            // preMaintenanceTimes
            // +---+---+
            // | 3 | 1 |
            // +---+---+
            //
            // isFixedBatches:
            // false
            */

            // Объявляем матрицу переналадки
            Dictionary<int, Matrix> changeoverTime = new Dictionary<int, Matrix>();

            // Создаём матрицу переналадки для 1 прибора
            Matrix changeoverTime_1 = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Создаём матрицу переналадки для 2 прибора
            Matrix changeoverTime_2 = new Matrix(new List<List<int>>
                {
                    new List<int> { 4, 2 },
                    new List<int> { 3, 1 },
                });

            // Добавляем матрицы переналадки в changeoverTime
            changeoverTime.Add(0, changeoverTime_1);
            changeoverTime.Add(1, changeoverTime_2);

            // Создаём матрицу времени выполнения
            Matrix proccessingTime = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Формируем конфигурационный файл
            Config config = new Config(
                2, // int dataTypesCount,
                2, // int deviceCount,
                999, // int buffer,
                proccessingTime, // Matrix proccessingTime,
                changeoverTime,// Dictionary<int, Matrix> changeoverTime,
                new Vector(new List<int> { 3, 1 }),
                false// bool isFixedBatches
            );

            // Объявляем и инициализируем последовательность ПЗ
            List<List<int>> matrixA = new List<List<int>>
            {
                new List<int>{3},
                new List<int>{2},
            };

            #endregion

            // Выполняем вызов построения корректного расписания
            List<Batch> schedule = CorrectSchedule.Build(config, matrixA);
            Dictionary<int, List<List<int>>> matrixT = PreM.Build(config, schedule, null);
            int time = matrixT[config.deviceCount - 1].Last().Last() + config.proccessingTime[config.deviceCount - 1, schedule.Last().Type];
            Assert.AreEqual(23, time);
        }

        [TestMethod]
        public void GetDowntimeFrom_withot_preM_1()
        {

            // Формируем входные данные
            #region Init input

            /*
            // dataTypesCount:
            // 2
            // 
            // deviceCount:
            // 2
            // 
            // buffer:
            // 999
            // 
            // proccessingTime:
            // +---+---+
            // | 1 | 2 |
            // +---+---+
            // | 3 | 4 |
            // +---+---+
            //
            // changeoverTime:
            // +---+---+---+
            // |   | 1 | 2 |
            // + 1 +---+---+
            // |   | 3 | 4 |
            // +---+---+---+
            // |   | 4 | 2 |
            // + 2 +---+---+
            // |   | 3 | 1 |
            // +---+---+---+
            //
            // preMaintenanceTimes
            // +---+---+
            // | 3 | 1 |
            // +---+---+
            //
            // isFixedBatches:
            // false
            */

            // Объявляем матрицу переналадки
            Dictionary<int, Matrix> changeoverTime = new Dictionary<int, Matrix>();

            // Создаём матрицу переналадки для 1 прибора
            Matrix changeoverTime_1 = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Создаём матрицу переналадки для 2 прибора
            Matrix changeoverTime_2 = new Matrix(new List<List<int>>
                {
                    new List<int> { 4, 2 },
                    new List<int> { 3, 1 },
                });

            // Добавляем матрицы переналадки в changeoverTime
            changeoverTime.Add(0, changeoverTime_1);
            changeoverTime.Add(1, changeoverTime_2);

            // Создаём матрицу времени выполнения
            Matrix proccessingTime = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Формируем конфигурационный файл
            Config config = new Config(
                2, // int dataTypesCount,
                2, // int deviceCount,
                999, // int buffer,
                proccessingTime, // Matrix proccessingTime,
                changeoverTime,// Dictionary<int, Matrix> changeoverTime,
                new Vector(new List<int> { 3, 1 }),
                false// bool isFixedBatches
            );

            // Объявляем и инициализируем последовательность ПЗ
            List<magisterDiplom.Model.Batch> schedule = new List<magisterDiplom.Model.Batch>
            {
                new magisterDiplom.Model.Batch(0, 3),
                new magisterDiplom.Model.Batch(1, 2),
            };

            // Создаём позиционную матрицу Y
            List<List<int>> Y = new List<List<int>>
            {
                new List<int> { 0, 0 },
                new List<int> { 0, 0 }
            };

            /*
            //   0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 24
            //   |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // d1|  |A |A |A |  |  |  B  |  B  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // d2|  |  |  |  |   A    |   A    |   A    |  |  |     B     |     B     |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            */

            // Объявляем выходную матрицу T
            Dictionary<int, List<List<int>>> _matrixT = new Dictionary<int, List<List<int>>>();

            // Инициализируем её выходными данными для 1 прибора
            _matrixT.Add(0, new List<List<int>> {
                new List<int> { 1,2,3 },
                new List<int> { 6,8 },
            });

            // Инициализируем её выходными данными для 2 прибора
            _matrixT.Add(1, new List<List<int>> {
                new List<int> { 4,7,10 },
                new List<int> { 15,19 },
            });

            #endregion

            // Подсчитываем простои
            int downTime = CorrectSchedule.GetDowntimeFrom(config, _matrixT, schedule);

            // Инициализируем правильный ответ
            int result = 9;

            // Выполняем проверку
            Assert.AreEqual(downTime, result);
        }

        [TestMethod]
        public void GetDowntimeFrom_withot_preM_2()
        {

            // Формируем входные данные
            #region Init input

            /*
            // dataTypesCount:
            // 2
            // 
            // deviceCount:
            // 2
            // 
            // buffer:
            // 999
            // 
            // proccessingTime:
            // +---+---+
            // | 1 | 2 |
            // +---+---+
            // | 3 | 4 |
            // +---+---+
            //
            // changeoverTime:
            // +---+---+---+
            // |   | 1 | 2 |
            // + 1 +---+---+
            // |   | 3 | 4 |
            // +---+---+---+
            // |   | 4 | 2 |
            // + 2 +---+---+
            // |   | 3 | 1 |
            // +---+---+---+
            //
            // preMaintenanceTimes
            // +---+---+
            // | 3 | 1 |
            // +---+---+
            //
            // isFixedBatches:
            // false
            */

            // Объявляем матрицу переналадки
            Dictionary<int, Matrix> changeoverTime = new Dictionary<int, Matrix>();

            // Создаём матрицу переналадки для 1 прибора
            Matrix changeoverTime_1 = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Создаём матрицу переналадки для 2 прибора
            Matrix changeoverTime_2 = new Matrix(new List<List<int>>
                {
                    new List<int> { 4, 2 },
                    new List<int> { 3, 1 },
                });

            // Добавляем матрицы переналадки в changeoverTime
            changeoverTime.Add(0, changeoverTime_1);
            changeoverTime.Add(1, changeoverTime_2);

            // Создаём матрицу времени выполнения
            Matrix proccessingTime = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Формируем конфигурационный файл
            Config config = new Config(
                2, // int dataTypesCount,
                2, // int deviceCount,
                999, // int buffer,
                proccessingTime, // Matrix proccessingTime,
                changeoverTime,// Dictionary<int, Matrix> changeoverTime,
                new Vector(new List<int> { 3, 1 }),
                false// bool isFixedBatches
            );

            // Объявляем и инициализируем последовательность ПЗ
            List<magisterDiplom.Model.Batch> schedule = new List<magisterDiplom.Model.Batch>
            {
                new magisterDiplom.Model.Batch(1, 2),
                new magisterDiplom.Model.Batch(0, 3),
            };

            // Создаём позиционную матрицу Y
            List<List<int>> Y = new List<List<int>>
            {
                new List<int> { 0, 0 },
                new List<int> { 0, 0 }
            };

            /*
            //   0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28
            //   |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // d1|  |  |  |  |  B  |  B  |  |  |  |A |A |A |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // d2|  |  |  |  |  |  |    B      |     B     |  |  |  |   A    |   A    |   A    |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            */

            // Объявляем выходную матрицу T
            Dictionary<int, List<List<int>>> _matrixT = new Dictionary<int, List<List<int>>>();

            // Инициализируем её выходными данными для 1 прибора
            _matrixT.Add(0, new List<List<int>> {
                new List<int> { 4,6 },
                new List<int> { 11,12,13 },
            });

            // Инициализируем её выходными данными для 2 прибора
            _matrixT.Add(1, new List<List<int>> {
                new List<int> { 6,10 },
                new List<int> { 17,20,23 },
            });

            #endregion

            // Подсчитываем простои
            int downTime = CorrectSchedule.GetDowntimeFrom(config, _matrixT, schedule);

            // Инициализируем правильный ответ
            int result = 16;

            // Выполняем проверку
            Assert.AreEqual(downTime, result);
        }

        [TestMethod]
        public void GetDowntimeFrom_preM_1()
        {

            // Формируем входные данные
            #region Init input

            /*
            // dataTypesCount:
            // 2
            // 
            // deviceCount:
            // 2
            // 
            // buffer:
            // 999
            // 
            // proccessingTime:
            // +---+---+
            // | 1 | 2 |
            // +---+---+
            // | 3 | 4 |
            // +---+---+
            //
            // changeoverTime:
            // +---+---+---+
            // |   | 1 | 2 |
            // + 1 +---+---+
            // |   | 3 | 4 |
            // +---+---+---+
            // |   | 4 | 2 |
            // + 2 +---+---+
            // |   | 3 | 1 |
            // +---+---+---+
            //
            // preMaintenanceTimes
            // +---+---+
            // | 3 | 1 |
            // +---+---+
            //
            // isFixedBatches:
            // false
            */

            // Объявляем матрицу переналадки
            Dictionary<int, Matrix> changeoverTime = new Dictionary<int, Matrix>();

            // Создаём матрицу переналадки для 1 прибора
            Matrix changeoverTime_1 = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Создаём матрицу переналадки для 2 прибора
            Matrix changeoverTime_2 = new Matrix(new List<List<int>>
                {
                    new List<int> { 4, 2 },
                    new List<int> { 3, 1 },
                });

            // Добавляем матрицы переналадки в changeoverTime
            changeoverTime.Add(0, changeoverTime_1);
            changeoverTime.Add(1, changeoverTime_2);

            // Создаём матрицу времени выполнения
            Matrix proccessingTime = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Формируем конфигурационный файл
            Config config = new Config(
                2, // int dataTypesCount,
                2, // int deviceCount,
                999, // int buffer,
                proccessingTime, // Matrix proccessingTime,
                changeoverTime,// Dictionary<int, Matrix> changeoverTime,
                new Vector(new List<int> { 3, 1 }),
                false// bool isFixedBatches
            );

            // Объявляем и инициализируем последовательность ПЗ
            List<magisterDiplom.Model.Batch> schedule = new List<magisterDiplom.Model.Batch>
            {
                new magisterDiplom.Model.Batch(0, 3),
                new magisterDiplom.Model.Batch(1, 2),
            };

            // Создаём позиционную матрицу Y
            List<List<int>> Y = new List<List<int>>
            {
                new List<int> { 1, 0 },
                new List<int> { 0, 0 }
            };

            /*
            //   0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 24
            //   |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // d1|  |A |A |A |  |  |  |  |  |  B  |  B  |  |  |  |  |  |  |  |  |  |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // d2|  |  |  |  |   A    |   A    |   A    |  |  |     B     |     B     |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            */

            // Объявляем выходную матрицу T
            Dictionary<int, List<List<int>>> _matrixT = new Dictionary<int, List<List<int>>>();

            // Инициализируем её выходными данными для 1 прибора
            _matrixT.Add(0, new List<List<int>> {
                new List<int> { 1,2,3 },
                new List<int> { 9,11 },
            });

            // Инициализируем её выходными данными для 2 прибора
            _matrixT.Add(1, new List<List<int>> {
                new List<int> { 4,7,10 },
                new List<int> { 15,19 },
            });

            #endregion

            // Подсчитываем простои
            int downTime = CorrectSchedule.GetDowntimeFrom(config, _matrixT, schedule);

            // Инициализируем правильный ответ
            int result = 12;

            // Выполняем проверку
            Assert.AreEqual(downTime, result);
        }

        [TestMethod]
        public void GetDowntimeFrom_preM_2()
        {


            // Формируем входные данные
            #region Init input

            /*
            // dataTypesCount:
            // 2
            // 
            // deviceCount:
            // 2
            // 
            // buffer:
            // 999
            // 
            // proccessingTime:
            // +---+---+
            // | 1 | 2 |
            // +---+---+
            // | 3 | 4 |
            // +---+---+
            //
            // changeoverTime:
            // +---+---+---+
            // |   | 1 | 2 |
            // + 1 +---+---+
            // |   | 3 | 4 |
            // +---+---+---+
            // |   | 4 | 2 |
            // + 2 +---+---+
            // |   | 3 | 1 |
            // +---+---+---+
            //
            // preMaintenanceTimes
            // +---+---+
            // | 3 | 1 |
            // +---+---+
            //
            // isFixedBatches:
            // false
            */

            // Объявляем матрицу переналадки
            Dictionary<int, Matrix> changeoverTime = new Dictionary<int, Matrix>();

            // Создаём матрицу переналадки для 1 прибора
            Matrix changeoverTime_1 = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Создаём матрицу переналадки для 2 прибора
            Matrix changeoverTime_2 = new Matrix(new List<List<int>>
                {
                    new List<int> { 4, 2 },
                    new List<int> { 3, 1 },
                });

            // Добавляем матрицы переналадки в changeoverTime
            changeoverTime.Add(0, changeoverTime_1);
            changeoverTime.Add(1, changeoverTime_2);

            // Создаём матрицу времени выполнения
            Matrix proccessingTime = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Формируем конфигурационный файл
            Config config = new Config(
                2, // int dataTypesCount,
                2, // int deviceCount,
                999, // int buffer,
                proccessingTime, // Matrix proccessingTime,
                changeoverTime,// Dictionary<int, Matrix> changeoverTime,
                new Vector(new List<int> { 3, 1 }),
                false// bool isFixedBatches
            );

            // Объявляем и инициализируем последовательность ПЗ
            List<magisterDiplom.Model.Batch> schedule = new List<magisterDiplom.Model.Batch>
            {
                new magisterDiplom.Model.Batch(1, 2),
                new magisterDiplom.Model.Batch(0, 3),
            };

            // Создаём позиционную матрицу Y
            List<List<int>> Y = new List<List<int>>
            {
                new List<int> { 0, 0 },
                new List<int> { 1, 0 }
            };

            /*
            //   0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28
            //   |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // d1|  |  |  |  |  B  |  B  |  |  |  |A |A |A |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // d2|  |  |  |  |  |  |    B      |     B     |  |  |  |  |   A    |   A    |   A    |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            */

            // Объявляем выходную матрицу T
            Dictionary<int, List<List<int>>> _matrixT = new Dictionary<int, List<List<int>>>();

            // Инициализируем её выходными данными для 1 прибора
            _matrixT.Add(0, new List<List<int>> {
                new List<int> { 4,6 },
                new List<int> { 11,12,13 },
            });

            // Инициализируем её выходными данными для 2 прибора
            _matrixT.Add(1, new List<List<int>> {
                new List<int> { 6,10 },
                new List<int> { 18,21,24 },
            });

            #endregion

            // Подсчитываем простои
            int downTime = CorrectSchedule.GetDowntimeFrom(config, _matrixT, schedule);

            // Инициализируем правильный ответ
            int result = 17;

            // Выполняем проверку
            Assert.AreEqual(downTime, result);
        }



        [TestMethod]
        public void GetUtilityFrom_withot_preM_1()
        {

            // Формируем входные данные
            #region Init input

            /*
            // dataTypesCount:
            // 2
            // 
            // deviceCount:
            // 2
            // 
            // buffer:
            // 999
            // 
            // proccessingTime:
            // +---+---+
            // | 1 | 2 |
            // +---+---+
            // | 3 | 4 |
            // +---+---+
            //
            // changeoverTime:
            // +---+---+---+
            // |   | 1 | 2 |
            // + 1 +---+---+
            // |   | 3 | 4 |
            // +---+---+---+
            // |   | 4 | 2 |
            // + 2 +---+---+
            // |   | 3 | 1 |
            // +---+---+---+
            //
            // preMaintenanceTimes
            // +---+---+
            // | 3 | 1 |
            // +---+---+
            //
            // isFixedBatches:
            // false
            */

            // Объявляем матрицу переналадки
            Dictionary<int, Matrix> changeoverTime = new Dictionary<int, Matrix>();

            // Создаём матрицу переналадки для 1 прибора
            Matrix changeoverTime_1 = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Создаём матрицу переналадки для 2 прибора
            Matrix changeoverTime_2 = new Matrix(new List<List<int>>
                {
                    new List<int> { 4, 2 },
                    new List<int> { 3, 1 },
                });

            // Добавляем матрицы переналадки в changeoverTime
            changeoverTime.Add(0, changeoverTime_1);
            changeoverTime.Add(1, changeoverTime_2);

            // Создаём матрицу времени выполнения
            Matrix proccessingTime = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Формируем конфигурационный файл
            Config config = new Config(
                2, // int dataTypesCount,
                2, // int deviceCount,
                999, // int buffer,
                proccessingTime, // Matrix proccessingTime,
                changeoverTime,// Dictionary<int, Matrix> changeoverTime,
                new Vector(new List<int> { 3, 1 }),
                false// bool isFixedBatches
            );

            // Объявляем и инициализируем последовательность ПЗ
            List<magisterDiplom.Model.Batch> schedule = new List<magisterDiplom.Model.Batch>
            {
                new magisterDiplom.Model.Batch(0, 3),
                new magisterDiplom.Model.Batch(1, 2),
            };

            // Создаём позиционную матрицу Y
            List<List<int>> Y = new List<List<int>>
            {
                new List<int> { 0, 0 },
                new List<int> { 0, 0 }
            };

            /*
            //   0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 24
            //   |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // d1|  |A |A |A |  |  |  B  |  B  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // d2|  |  |  |  |   A    |   A    |   A    |  |  |     B     |     B     |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            */

            // Объявляем выходную матрицу T
            Dictionary<int, List<List<int>>> _matrixT = new Dictionary<int, List<List<int>>>();

            // Инициализируем её выходными данными для 1 прибора
            _matrixT.Add(0, new List<List<int>> {
                new List<int> { 1,2,3 },
                new List<int> { 6,8 },
            });

            // Инициализируем её выходными данными для 2 прибора
            _matrixT.Add(1, new List<List<int>> {
                new List<int> { 4,7,10 },
                new List<int> { 15,19 },
            });

            #endregion

            // Подсчитываем простои
            int utility = CorrectSchedule.GetUtilityFrom(config, _matrixT, schedule);

            // Инициализируем правильный ответ
            int result = 23 - 9;

            // Выполняем проверку
            Assert.AreEqual(utility, result);
        }

        [TestMethod]
        public void GetUtilityFrom_withot_preM_2()
        {

            // Формируем входные данные
            #region Init input

            /*
            // dataTypesCount:
            // 2
            // 
            // deviceCount:
            // 2
            // 
            // buffer:
            // 999
            // 
            // proccessingTime:
            // +---+---+
            // | 1 | 2 |
            // +---+---+
            // | 3 | 4 |
            // +---+---+
            //
            // changeoverTime:
            // +---+---+---+
            // |   | 1 | 2 |
            // + 1 +---+---+
            // |   | 3 | 4 |
            // +---+---+---+
            // |   | 4 | 2 |
            // + 2 +---+---+
            // |   | 3 | 1 |
            // +---+---+---+
            //
            // preMaintenanceTimes
            // +---+---+
            // | 3 | 1 |
            // +---+---+
            //
            // isFixedBatches:
            // false
            */

            // Объявляем матрицу переналадки
            Dictionary<int, Matrix> changeoverTime = new Dictionary<int, Matrix>();

            // Создаём матрицу переналадки для 1 прибора
            Matrix changeoverTime_1 = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Создаём матрицу переналадки для 2 прибора
            Matrix changeoverTime_2 = new Matrix(new List<List<int>>
                {
                    new List<int> { 4, 2 },
                    new List<int> { 3, 1 },
                });

            // Добавляем матрицы переналадки в changeoverTime
            changeoverTime.Add(0, changeoverTime_1);
            changeoverTime.Add(1, changeoverTime_2);

            // Создаём матрицу времени выполнения
            Matrix proccessingTime = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Формируем конфигурационный файл
            Config config = new Config(
                2, // int dataTypesCount,
                2, // int deviceCount,
                999, // int buffer,
                proccessingTime, // Matrix proccessingTime,
                changeoverTime,// Dictionary<int, Matrix> changeoverTime,
                new Vector(new List<int> { 3, 1 }),
                false// bool isFixedBatches
            );

            // Объявляем и инициализируем последовательность ПЗ
            List<magisterDiplom.Model.Batch> schedule = new List<magisterDiplom.Model.Batch>
            {
                new magisterDiplom.Model.Batch(1, 2),
                new magisterDiplom.Model.Batch(0, 3),
            };

            // Создаём позиционную матрицу Y
            List<List<int>> Y = new List<List<int>>
            {
                new List<int> { 0, 0 },
                new List<int> { 0, 0 }
            };

            /*
            //   0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28
            //   |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // d1|  |  |  |  |  B  |  B  |  |  |  |A |A |A |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // d2|  |  |  |  |  |  |    B      |     B     |  |  |  |   A    |   A    |   A    |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            */

            // Объявляем выходную матрицу T
            Dictionary<int, List<List<int>>> _matrixT = new Dictionary<int, List<List<int>>>();

            // Инициализируем её выходными данными для 1 прибора
            _matrixT.Add(0, new List<List<int>> {
                new List<int> { 4,6 },
                new List<int> { 11,12,13 },
            });

            // Инициализируем её выходными данными для 2 прибора
            _matrixT.Add(1, new List<List<int>> {
                new List<int> { 6,10 },
                new List<int> { 17,20,23 },
            });

            #endregion

            // Подсчитываем простои
            int utility = CorrectSchedule.GetUtilityFrom(config, _matrixT, schedule);

            // Инициализируем правильный ответ
            int result = 26 - 16;

            // Выполняем проверку
            Assert.AreEqual(utility, result);
        }

        [TestMethod]
        public void GetUtilityFrom_preM_1()
        {

            // Формируем входные данные
            #region Init input

            /*
            // dataTypesCount:
            // 2
            // 
            // deviceCount:
            // 2
            // 
            // buffer:
            // 999
            // 
            // proccessingTime:
            // +---+---+
            // | 1 | 2 |
            // +---+---+
            // | 3 | 4 |
            // +---+---+
            //
            // changeoverTime:
            // +---+---+---+
            // |   | 1 | 2 |
            // + 1 +---+---+
            // |   | 3 | 4 |
            // +---+---+---+
            // |   | 4 | 2 |
            // + 2 +---+---+
            // |   | 3 | 1 |
            // +---+---+---+
            //
            // preMaintenanceTimes
            // +---+---+
            // | 3 | 1 |
            // +---+---+
            //
            // isFixedBatches:
            // false
            */

            // Объявляем матрицу переналадки
            Dictionary<int, Matrix> changeoverTime = new Dictionary<int, Matrix>();

            // Создаём матрицу переналадки для 1 прибора
            Matrix changeoverTime_1 = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Создаём матрицу переналадки для 2 прибора
            Matrix changeoverTime_2 = new Matrix(new List<List<int>>
                {
                    new List<int> { 4, 2 },
                    new List<int> { 3, 1 },
                });

            // Добавляем матрицы переналадки в changeoverTime
            changeoverTime.Add(0, changeoverTime_1);
            changeoverTime.Add(1, changeoverTime_2);

            // Создаём матрицу времени выполнения
            Matrix proccessingTime = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Формируем конфигурационный файл
            Config config = new Config(
                2, // int dataTypesCount,
                2, // int deviceCount,
                999, // int buffer,
                proccessingTime, // Matrix proccessingTime,
                changeoverTime,// Dictionary<int, Matrix> changeoverTime,
                new Vector(new List<int> { 3, 1 }),
                false// bool isFixedBatches
            );

            // Объявляем и инициализируем последовательность ПЗ
            List<magisterDiplom.Model.Batch> schedule = new List<magisterDiplom.Model.Batch>
            {
                new magisterDiplom.Model.Batch(0, 3),
                new magisterDiplom.Model.Batch(1, 2),
            };

            // Создаём позиционную матрицу Y
            List<List<int>> Y = new List<List<int>>
            {
                new List<int> { 1, 0 },
                new List<int> { 0, 0 }
            };

            /*
            //   0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 24
            //   |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // d1|  |A |A |A |  |  |  |  |  |  B  |  B  |  |  |  |  |  |  |  |  |  |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // d2|  |  |  |  |   A    |   A    |   A    |  |  |     B     |     B     |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            */

            // Объявляем выходную матрицу T
            Dictionary<int, List<List<int>>> _matrixT = new Dictionary<int, List<List<int>>>();

            // Инициализируем её выходными данными для 1 прибора
            _matrixT.Add(0, new List<List<int>> {
                new List<int> { 1,2,3 },
                new List<int> { 9,11 },
            });

            // Инициализируем её выходными данными для 2 прибора
            _matrixT.Add(1, new List<List<int>> {
                new List<int> { 4,7,10 },
                new List<int> { 15,19 },
            });

            #endregion

            // Подсчитываем простои
            int utility = CorrectSchedule.GetUtilityFrom(config, _matrixT, schedule);

            // Инициализируем правильный ответ
            int result = 23 - 12;

            // Выполняем проверку
            Assert.AreEqual(utility, result);
        }

        [TestMethod]
        public void GetUtilityFrom_preM_2()
        {


            // Формируем входные данные
            #region Init input

            /*
            // dataTypesCount:
            // 2
            // 
            // deviceCount:
            // 2
            // 
            // buffer:
            // 999
            // 
            // proccessingTime:
            // +---+---+
            // | 1 | 2 |
            // +---+---+
            // | 3 | 4 |
            // +---+---+
            //
            // changeoverTime:
            // +---+---+---+
            // |   | 1 | 2 |
            // + 1 +---+---+
            // |   | 3 | 4 |
            // +---+---+---+
            // |   | 4 | 2 |
            // + 2 +---+---+
            // |   | 3 | 1 |
            // +---+---+---+
            //
            // preMaintenanceTimes
            // +---+---+
            // | 3 | 1 |
            // +---+---+
            //
            // isFixedBatches:
            // false
            */

            // Объявляем матрицу переналадки
            Dictionary<int, Matrix> changeoverTime = new Dictionary<int, Matrix>();

            // Создаём матрицу переналадки для 1 прибора
            Matrix changeoverTime_1 = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Создаём матрицу переналадки для 2 прибора
            Matrix changeoverTime_2 = new Matrix(new List<List<int>>
                {
                    new List<int> { 4, 2 },
                    new List<int> { 3, 1 },
                });

            // Добавляем матрицы переналадки в changeoverTime
            changeoverTime.Add(0, changeoverTime_1);
            changeoverTime.Add(1, changeoverTime_2);

            // Создаём матрицу времени выполнения
            Matrix proccessingTime = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
                });

            // Формируем конфигурационный файл
            Config config = new Config(
                2, // int dataTypesCount,
                2, // int deviceCount,
                999, // int buffer,
                proccessingTime, // Matrix proccessingTime,
                changeoverTime,// Dictionary<int, Matrix> changeoverTime,
                new Vector(new List<int> { 3, 1 }),
                false// bool isFixedBatches
            );

            // Объявляем и инициализируем последовательность ПЗ
            List<magisterDiplom.Model.Batch> schedule = new List<magisterDiplom.Model.Batch>
            {
                new magisterDiplom.Model.Batch(1, 2),
                new magisterDiplom.Model.Batch(0, 3),
            };

            // Создаём позиционную матрицу Y
            List<List<int>> Y = new List<List<int>>
            {
                new List<int> { 0, 0 },
                new List<int> { 1, 0 }
            };

            /*
            //   0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28
            //   |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // d1|  |  |  |  |  B  |  B  |  |  |  |A |A |A |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // d2|  |  |  |  |  |  |    B      |     B     |  |  |  |  |   A    |   A    |   A    |  |
            //   +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            */

            // Объявляем выходную матрицу T
            Dictionary<int, List<List<int>>> _matrixT = new Dictionary<int, List<List<int>>>();

            // Инициализируем её выходными данными для 1 прибора
            _matrixT.Add(0, new List<List<int>> {
                new List<int> { 4,6 },
                new List<int> { 11,12,13 },
            });

            // Инициализируем её выходными данными для 2 прибора
            _matrixT.Add(1, new List<List<int>> {
                new List<int> { 6,10 },
                new List<int> { 18,21,24 },
            });

            #endregion

            // Подсчитываем простои
            int utility = CorrectSchedule.GetUtilityFrom(config, _matrixT, schedule);

            // Инициализируем правильный ответ
            int result = 27 - 17;

            // Выполняем проверку
            Assert.AreEqual(utility, result);
        }
    }
}
