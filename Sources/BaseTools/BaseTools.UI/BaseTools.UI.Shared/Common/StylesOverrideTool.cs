namespace BaseTools.UI.Common
{
#if SILVERLIGHT
    using System.Windows;
    using System.Windows.Media;
#endif

    using BaseTools.Core.Info;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.ComponentModel;

    public class StylesOverrideTool
    {
        private string resourceDictionaryPath;

        public StylesOverrideTool()
        {

        }

        public string ResourceDictionaryPath
        {
            get
            {
                return this.resourceDictionaryPath;
            }

            set
            {
                this.resourceDictionaryPath = value;
                //var isDesingMode = DesignerProperties.IsInDesignTool;
                //if (!isDesingMode)
                //{
                    this.OverridePhoneStyles();
                //}
            }
        }


        /// <summary>
        /// Method override system brushes by brushes contained in Styles/OverridedPhoneStyles.xaml resourse dictionary
        /// </summary>
        private void OverridePhoneStyles()
        {
            var uri = new Uri(this.resourceDictionaryPath, UriKind.Relative);
            var themeStyles = new ResourceDictionary
            {
                Source = uri
            };

            ResourceDictionary appResources = Application.Current.Resources;
            foreach (DictionaryEntry newStyle in themeStyles)
            {
                if (newStyle.Value is SolidColorBrush)
                {
                    SolidColorBrush newBrush = (SolidColorBrush)newStyle.Value;
                    SolidColorBrush existingBrush = (SolidColorBrush)appResources[newStyle.Key];
                    existingBrush.Color = newBrush.Color;
                }
            }
        }
    }
}
