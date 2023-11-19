using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newAlgorithm
{
    class Sets
    {
        /// <summary>
        /// Данная переменная определяем типы данных
        /// </summary>
        private readonly int _types;
        private readonly List<List<int>> _composition; // состав
        private readonly List<List<int>> _time;
        private List<List<Kit>> _readySets;

        /// <summary>
        /// Данный конструктур создаёт и возвращает экземпляр класса 
        /// </summary>
        /// <param name="composition"></param>
        /// <param name="time"></param>
        public Sets(List<List<int>> composition, List<List<int>> time)
        {
            _types = composition.Count;
            _composition = composition;
            _time = time;
            _readySets = new List<List<Kit>>();

            // Для каджого из типов данных
            for (int i = 0; i < _types; i++)
            {

                // Создаём набор и добавляем их в переменную _readySets
                _readySets.Add(new List<Kit>());

                // Для каждого из списка 
                for (int j = 0; j < time[i].Count; j++)
                {

                    _readySets[i].Add(new Kit(composition[i], time[i][j]));
                }
            }
        }

        /// <summary>
        /// Данная функция формирует новый критерий на основе переданного в аргументе значения диррективных строков
        /// </summary>
        /// <returns></returns>
        public int GetNewCriterion(bool deadline)
        {
            
            // Инициализируем результирующую переменную
            int res = 0;

            // Если диррективные сроки были установлены
            if (deadline)
            {
                foreach (var row in _readySets)
                {
                    foreach (var elem in row)
                    {
                        if (elem.GetTime() > elem.GetCompositionTime())
                        {
                            res += elem.GetTime() - elem.GetCompositionTime();
                        }
                    }
                }
            }
            else
            {
                var count = 0;

                foreach (var row in _readySets)
                {
                    count += row.Count;
                    foreach (var elem in row)
                    {
                        res += elem.GetTime();
                        if (res < elem.GetTime())
                        {
                            res = elem.GetTime();
                        }
                    }
                }

                res /= count;
            }

            // Возвращаем результат
            return res;
        }

        /// <summary>
        /// Старый критерий
        /// </summary>
        /// <returns></returns>
        public int GetCriterion()
        {
            int res = 0;
            foreach (var row in _readySets)
            {
                foreach (var elem in row)
                {
                    if (res < elem.GetTime())
                    {
                        res = elem.GetTime();
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="type"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        protected void AddBatches(SheduleElement sheduleElement)
        {
            var sets = new List<Kit>();
            foreach (var row in _readySets)
            {
                foreach (var elem in row)
                {
                    sets.Add(elem);
                }
            }

            sets.Sort(
                (Kit kit1, Kit kit2) => kit1.CompareTo(kit2)
                
            );

            foreach (var elem in sets)
            {
                if (!elem.IsSetAllComposition())
                {
                    sheduleElement = elem.AddBatch(sheduleElement.getValue(), sheduleElement.getType(), sheduleElement.getTime());
                }
                if (sheduleElement.getValue() <= 0)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shedule"></param>
        public void GetSolution(List<SheduleElement> shedule)
        {
            foreach (var element in shedule)
            {
                AddBatches(element);
            }
        }
    }
}
