namespace BaseTools.Sample.WpSilverlight.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class LightItemsControlViewModel
    {
        public LightItemsControlViewModel()
        {
            this.Data = Enumerable.Range(0, 100).ToList();
        }

        public List<int> Data { get; set; }


    }
}
