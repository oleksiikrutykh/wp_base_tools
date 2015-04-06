namespace BaseTools.Core.Common
{
    using BaseTools.Core.Ioc;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RandomProvider
    {
        private Random random;

        private static RandomProvider instance = Factory.Common.GetInstance<RandomProvider>();

        public static RandomProvider Instance
        {
            get
            {
                return instance;
            }
        }

        public virtual int Next()
        {
            return this.Next(0, Int32.MaxValue);
        }

        public virtual int Next(int maxValue)
        {
            return this.Next(0, maxValue);
        }

        public virtual int Next(int minValue, int maxValue)
        {
            if (random == null)
            {
                random = new Random();
            }

            return random.Next(minValue, maxValue);
        }
    }
}
