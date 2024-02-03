using magisterDiplom.Model;
using newAlgorithm.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalTest.Model
{

    [TestClass]
    public class ConfigTest
    {

        /// <summary>
        /// Данная функция обкладывает функцию ToString() структуры Config тестом
        /// </summary>
        [TestMethod]
        public void ConfigStringTest()
        {

            // Формируем входные данные
            #region Input

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

            #endregion

            // Формируем выходные данные
            string output = ""+
                "\tisFixedBatches: False" +         Environment.NewLine +
                "\tdataTypesCount: 2"+              Environment.NewLine +
                "\tdeviceCount:    2"+              Environment.NewLine +
                "\tbuffer:         999" +           Environment.NewLine +
                "\tpreMaintenanceTimes: 3 , 1;" +   Environment.NewLine +
                "\tproccessingTime:" +              Environment.NewLine +
                "\t\tDevice 0: \t1 , 2;" +          Environment.NewLine +
                "\t\tDevice 1: \t3 , 4;" +          Environment.NewLine +
                "\tchangeoverTime:" +               Environment.NewLine +
                "\t\tDevice 0: " +                  Environment.NewLine +
                "\t\t\tType 0: \t1 , 2;" +          Environment.NewLine +
                "\t\t\tType 1: \t3 , 4;" +          Environment.NewLine +
                "\t\tDevice 1: " +                  Environment.NewLine +
                "\t\t\tType 0: \t4 , 2;" +          Environment.NewLine +
                "\t\t\tType 1: \t3 , 1;" +          Environment.NewLine +
                "";

            // Выполняем сравнение
            Assert.AreEqual( output, config.ToString("\t") );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InitConfig_PreMaintenanceTimes_ArgumentNullException()
        {

            // Формируем входные данные
            #region Input

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

            #endregion

            // Формируем конфигурационный файл
            Config config = new Config(
                2, // int dataTypesCount,
                2, // int deviceCount,
                999, // int buffer,
                proccessingTime, // Matrix proccessingTime,
                changeoverTime,// Dictionary<int, Matrix> changeoverTime,
                null,
                false// bool isFixedBatches
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InitConfig_PreMaintenanceTimes_ArgumentException()
        {

            // Формируем входные данные
            #region Input

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

            #endregion

            // Формируем конфигурационный файл
            Config config = new Config(
                3, // int dataTypesCount,
                2, // int deviceCount,
                999, // int buffer,
                proccessingTime, // Matrix proccessingTime,
                changeoverTime,// Dictionary<int, Matrix> changeoverTime,
                new Vector(new List<int> { 1, 2, 3 }),
                false// bool isFixedBatches
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InitConfig_ProccessingTime_ArgumentNullException()
        {

            // Формируем входные данные
            #region Input

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

            #endregion

            // Формируем конфигурационный файл
            Config config = new Config(
                3, // int dataTypesCount,
                2, // int deviceCount,
                999, // int buffer,
                null, // Matrix proccessingTime,
                changeoverTime,// Dictionary<int, Matrix> changeoverTime,
                new Vector(new List<int> { 1, 2 }),
                false// bool isFixedBatches
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InitConfig_ProccessingTime_ArgumentException()
        {

            // Формируем входные данные
            #region Input

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

            #endregion

            // Формируем конфигурационный файл
            Config config = new Config(
                2, // int dataTypesCount,
                3, // int deviceCount,
                999, // int buffer,
                proccessingTime, // Matrix proccessingTime,
                changeoverTime,// Dictionary<int, Matrix> changeoverTime,
                new Vector(new List<int> { 1, 2, 4 }),
                false// bool isFixedBatches
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InitConfig_ProccessingTime_ArgumentException_2()
        {

            // Формируем входные данные
            #region Input

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
                    new List<int> { 1, 2, 3 },
                    new List<int> { 3, 4 },
                });

            #endregion

            // Формируем конфигурационный файл
            Config config = new Config(
                2, // int dataTypesCount,
                2, // int deviceCount,
                999, // int buffer,
                proccessingTime, // Matrix proccessingTime,
                changeoverTime,// Dictionary<int, Matrix> changeoverTime,
                new Vector(new List<int> { 1, 2 }),
                false// bool isFixedBatches
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InitConfig_ProccessingTime_ArgumentException_3()
        {

            // Формируем входные данные
            #region Input

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
                    new List<int> { 3, 4 },
                    new List<int> { 1, 2, 3 },
                });

            #endregion

            // Формируем конфигурационный файл
            Config config = new Config(
                2, // int dataTypesCount,
                2, // int deviceCount,
                999, // int buffer,
                proccessingTime, // Matrix proccessingTime,
                changeoverTime,// Dictionary<int, Matrix> changeoverTime,
                new Vector(new List<int> { 1, 2 }),
                false// bool isFixedBatches
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InitConfig_ChangeoverTime_ArgumentNullException()
        {

            // Формируем входные данные
            #region Input

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
                    new List<int> { 3, 4 },
                    new List<int> { 1, 2 },
                });

            #endregion

            // Формируем конфигурационный файл
            Config config = new Config(
                2, // int dataTypesCount,
                2, // int deviceCount,
                999, // int buffer,
                proccessingTime, // Matrix proccessingTime,
                null,// Dictionary<int, Matrix> changeoverTime,
                new Vector(new List<int> { 1, 2 }),
                false// bool isFixedBatches
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InitConfig_ChangeoverTime_ArgumentException()
        {

            // Формируем входные данные
            #region Input

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
            changeoverTime.Add(2, null);

            // Создаём матрицу времени выполнения
            Matrix proccessingTime = new Matrix(new List<List<int>>
                {
                    new List<int> { 3, 4 },
                    new List<int> { 1, 2 },
                });

            #endregion

            // Формируем конфигурационный файл
            Config config = new Config(
                2, // int dataTypesCount,
                2, // int deviceCount,
                999, // int buffer,
                proccessingTime, // Matrix proccessingTime,
                changeoverTime,// Dictionary<int, Matrix> changeoverTime,
                new Vector(new List<int> { 1, 2 }),
                false// bool isFixedBatches
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InitConfig_ChangeoverTime_ArgumentException_2()
        {

            // Формируем входные данные
            #region Input

            // Объявляем матрицу переналадки
            Dictionary<int, Matrix> changeoverTime = new Dictionary<int, Matrix>();

            // Создаём матрицу переналадки для 1 прибора
            Matrix changeoverTime_1 = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4 },
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
                    new List<int> { 3, 4 },
                    new List<int> { 1, 2 },
                });

            #endregion

            // Формируем конфигурационный файл
            Config config = new Config(
                2, // int dataTypesCount,
                2, // int deviceCount,
                999, // int buffer,
                proccessingTime, // Matrix proccessingTime,
                changeoverTime,// Dictionary<int, Matrix> changeoverTime,
                new Vector(new List<int> { 1, 2 }),
                false// bool isFixedBatches
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InitConfig_ChangeoverTime_ArgumentException_3()
        {

            // Формируем входные данные
            #region Input

            // Объявляем матрицу переналадки
            Dictionary<int, Matrix> changeoverTime = new Dictionary<int, Matrix>();

            // Создаём матрицу переналадки для 1 прибора
            Matrix changeoverTime_1 = new Matrix(new List<List<int>>
                {
                    new List<int> { 1, 2 },
                    new List<int> { 3, 4, 5 },
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
                    new List<int> { 3, 4 },
                    new List<int> { 1, 2 },
                });

            #endregion

            // Формируем конфигурационный файл
            Config config = new Config(
                2, // int dataTypesCount,
                2, // int deviceCount,
                999, // int buffer,
                proccessingTime, // Matrix proccessingTime,
                changeoverTime,// Dictionary<int, Matrix> changeoverTime,
                new Vector(new List<int> { 1, 2 }),
                false// bool isFixedBatches
            );
        }
    }
}
