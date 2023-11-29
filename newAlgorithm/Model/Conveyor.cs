using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magisterDiplom.Model
{

    /// <summary>
    /// Данный класс описывает конвейер для симуляции
    /// </summary>
    public class Conveyor
    {

        /// <summary>
        /// Описывает количество приборов в конвейерной системеыы
        /// </summary>
        private int deviceCount;

        /// <summary>
        /// Описывает размер буфера перед приборами
        /// </summary>
        private int buffer;

        /// <summary>
        /// Конструктор возвращает экземпляр данного класса
        /// </summary>
        /// <param name="deviceCount">Количество приборов</param>
        /// <param name="buffer">Буфер перед приборам</param>
        public Conveyor(int deviceCount, int buffer)
        {

            // Инициализация
            this.deviceCount = deviceCount;
            this.buffer = buffer;
        }

        /// <summary>
        /// Преобразует входную последовательность 
        /// </summary>
        /// <returns>Строка с выходными данными</returns>
        public override string ToString()
        {
            string res = "";
            string bufferElement = string.Concat("|", Environment.NewLine);
            string deviceElement =
                string.Concat("+------+", Environment.NewLine) +
                string.Concat("|device|", Environment.NewLine) +
                string.Concat("+------+", Environment.NewLine);

            for (int i = 0; i < deviceCount; i++)
                res += string.Concat(Enumerable.Repeat(bufferElement, buffer)) + deviceElement;

            return res;
        }
    }
}
