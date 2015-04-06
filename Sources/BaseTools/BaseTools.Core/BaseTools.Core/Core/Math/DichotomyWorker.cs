namespace BaseTools.Core.Math
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class DichotomyWorker
    {
        public double MinLimit { get; set; }

        public double MaxLimit { get; set; }

        public double Quality { get; set; }

        public Func<double, bool> CheckOperation { get; set; }

        public DichotomyResult FindIntersection()
        {
            var result = new DichotomyResult();
            var minPoint = this.MinLimit;
            var maxPoint = this.MaxLimit;
            var maxResult = this.CheckOperation.Invoke(maxPoint);
            var minResult = this.CheckOperation.Invoke(minPoint);
            if (maxResult != minResult)
            {
                while (Math.Abs(maxPoint - minPoint) > this.Quality)
                {
                    var middlePoint = CalculateMiddlePoint(minPoint, maxPoint);
                    var middleResult = this.CheckOperation.Invoke(middlePoint);
                    if (maxResult != middleResult)
                    {
                        minResult = middleResult;
                        minPoint = middlePoint;
                    }
                    else
                    {
                        maxPoint = middlePoint;
                        maxResult = middleResult;
                    }
                }

                result.Type = DichotomyResultType.Intersection;
                if (maxResult == true)
                {
                    result.IntersectionPointGreater = maxPoint;
                    result.IntersectionPointLower = minPoint;
                }
                else
                {
                    result.IntersectionPointGreater = minPoint;
                    result.IntersectionPointLower = maxPoint;
                }
            }
            else
            {
                if (maxResult == true)
                {
                    result.Type = DichotomyResultType.Greater;
                }
                else
                {
                    result.Type = DichotomyResultType.Lower;
                }
            }

            return result;
        }

        private double CalculateMiddlePoint(double minValue, double maxValue)
        {
            var middlePoint = minValue + (maxValue - minValue) / 2;
            return middlePoint;
        }
    }
}
