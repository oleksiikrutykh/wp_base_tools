namespace BaseTools.UI.ControlExtensions.ImageLoader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using BaseTools.UI;
    using BaseTools.UI.Common;
    using Microsoft.Phone.Controls;

    public class ImageControlHandler
    {
        public Dictionary<PhoneApplicationPage, Entity> pagesDictionary = new Dictionary<PhoneApplicationPage, Entity>();

        public Dictionary<FrameworkElement, Entity> controlsDictionary = new Dictionary<FrameworkElement, PhoneApplicationPage>();

        public void AttachControl(FrameworkElement element)
        {
            Entity entity = null;
            var controlFounded = this.controlsDictionary.TryGetValue(element, out entity);
            if (!controlFounded)
            {
                var page = element.GetVisualAncestors().FirstOrDefault(e => e is PhoneApplicationPage);
                var pageFounded = this.pagesDictionary.TryGetValue(page, out entity);
                if (!pageFounded)
                {
                    entity = new Entity();
                    entity.ControlReference.C
                }
            }
            var loaderEntry = new Entity
            {
                Page = page,
                ControlReference = new WeakReference<FrameworkElement>(element);
            }
            var page = element.GetVisualAncestors().FirstOrDefault(e => e is PhoneApplicationPage);
            if (lastParent != null)
            {

            }

            element.Loaded += element_Loaded;
            element.Unloaded += element_Unloaded;
           
        }

        void element_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        void element_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }

    public class Entity
    {
        public Entity()
        {
            this.ControlReference = new List<WeakReference<FrameworkElement>>();
        }

        public Page Page { get; set; }

        public List<FrameworkElement> ControlReference { get; set; }


    }
}
