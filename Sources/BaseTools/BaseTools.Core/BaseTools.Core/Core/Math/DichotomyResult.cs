namespace BaseTools.Core.Math
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class DichotomyResult
    {
        public DichotomyResultType Type { get; set; }

        public double? IntersectionPointGreater { get; set; }

        public double? IntersectionPointLower { get; set; }
    }
}
