using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newAlgorithm
{
    class SheduleElement
    {
        private int jobCount;
        private int dataType = 0;
        private List<int> _time;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobCount"></param>
        /// <param name="dataType"></param>
        /// <param name="time"></param>
        public SheduleElement(int jobCount, int dataType, List<int> time)
        {
            this.jobCount = jobCount;
            this.dataType = dataType;
            _time = new List<int>(time);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int getValue()
        {
            return jobCount;
        }

        /// <summary>
        /// Данная функция вовзращает тип
        /// </summary>
        /// <returns>Тип</returns>
        public int getType()
        {
            return dataType;
        }

        /// <summary>
        /// Данная функция вовзращает временной список
        /// </summary>
        /// <returns>Список из элементов времени</returns>
        public List<int> getTime()
        {
            return _time;
        }
    }
}
