using System.Collections.Generic;
using System.Linq;

namespace newAlgorithm.Model
{
    public class JobContainer
    {
        public List<Job> JobList { get; private set; }

        public int LastPosition { get; private set; } = 0;

        public JobContainer()
        {
            JobList = new List<Job>();
        }

        public void add(Job job)
        {
            JobList.Add(job);
            LastPosition++;
        }

        public Job find(int position)
        {
            foreach (Job job in JobList)
            {
                if (job.Position == position)
                    return job;
            }

            return null;
        }

        public Job getFirstJob()
        {
            Job first = JobList.First();
            JobList.RemoveAt(0);

            return first;
        }
    }
}
