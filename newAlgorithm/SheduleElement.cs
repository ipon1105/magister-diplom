using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newAlgorithm
{
    class SheduleElement
    {
        private int _value;
        private int _type = 0;
        private List<int> _time;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="time"></param>
        public SheduleElement(int value, int type, List<int> time)
        {
            _value = value;
            _type = type;
            _time = new List<int>(time);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int getValue()
        {
            return _value;
        }

        /// <summary>
        /// Данная функция вовзращает тип
        /// </summary>
        /// <returns>Тип</returns>
        public int getType()
        {
            return _type;
        }

        /// <summary>
        /// Данная функция вовзращает список временной список
        /// </summary>
        /// <returns>Список из элементов времени</returns>
        public List<int> getTime()
        {
            return _time;
        }
    }
}
