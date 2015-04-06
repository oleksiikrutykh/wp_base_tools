namespace BaseTools.Core.Common
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class DateTimeProvider
    {
        public virtual DateTime Now 
        {
            get
            {
                return DateTime.Now;
            }
        }

        public virtual DateTime UtcNow
        {
            get
            {
                return DateTime.UtcNow;
            }
        }
    }


}
