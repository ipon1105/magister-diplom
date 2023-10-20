namespace newAlgorithm.Model
{
    public class Job
    {
        public int Type { get; private set; } = 0;
        public int Position { get; private set; } = 0;

        /// <summary>
        ///  -1 - not playced, 0 - into buffer, 1-n - in devices
        /// </summary>
        private int State { get; set; } = -2;

        public Job(int type, int position, int state)
        {
            Type = type;
            Position = position;
            State = state;
        }
    }
}
