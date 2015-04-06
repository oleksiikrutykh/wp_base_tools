namespace BaseTools.UI.ControlExtensions
{
#if SILVERLIGHT
    using System.Windows;
    
#endif

#if WINRT
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
#endif

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using BaseTools.Core.Utility;
using System.Text.RegularExpressions;
    using System.Globalization;

    /// <summary>
    /// Allow save the control proportions 
    /// </summary>
    public static class ProportionalSizeManager
    {
        /// <summary>
        /// Control proportional width.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency property pattern")]
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.RegisterAttached("Width", typeof(GridLength), typeof(ProportionalSizeManager), new PropertyMetadata(new GridLength(), OnSizePropertyChanged));

        /// <summary>
        /// Control proportional width.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency property pattern")]
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.RegisterAttached("Height", typeof(GridLength), typeof(ProportionalSizeManager), new PropertyMetadata(new GridLength(), OnSizePropertyChanged));




        public static string GetHeightFormula(DependencyObject obj)
        {
            return (string)obj.GetValue(HeightFormulaProperty);
        }

        public static void SetHeightFormula(DependencyObject obj, string value)
        {
            obj.SetValue(HeightFormulaProperty, value);
        }

        // Using a DependencyProperty as the backing store for HeightFormula.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeightFormulaProperty =
            DependencyProperty.RegisterAttached("HeightFormula", typeof(string), typeof(ProportionalSizeManager), new PropertyMetadata(null, OnSizePropertyChanged));

        


        /// <summary>
        /// Gets value of proportional width
        /// </summary>
        /// <param name="control">Associated object</param>
        /// <returns>Width restrictions</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by Guard")]
        public static GridLength GetWidth(DependencyObject control)
        {
            Guard.CheckIsNotNull(control, "control");
            return (GridLength)control.GetValue(WidthProperty);
        }

        /// <summary>
        /// Sets value of proportional width.
        /// </summary>
        /// <param name="control">Associated object</param>
        /// <param name="value">New value of width restrictions</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by Guard")]
        public static void SetWidth(DependencyObject control, GridLength value)
        {
            Guard.CheckIsNotNull(control, "control");
            control.SetValue(WidthProperty, value);
        }

        /// <summary>
        /// Gets value of proportional height
        /// </summary>
        /// <param name="control">Associated object</param>
        /// <returns>Height restrictions</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by Guard")]
        public static GridLength GetHeight(DependencyObject control)
        {
            Guard.CheckIsNotNull(control, "control");
            return (GridLength)control.GetValue(HeightProperty);
        }

        /// <summary>
        /// Sets value of proportional height.
        /// </summary>
        /// <param name="control">Associated object</param>
        /// <param name="value">New value of height restrictions</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by Guard")]
        public static void SetHeight(DependencyObject control, GridLength value)
        {
            Guard.CheckIsNotNull(control, "control");
            control.SetValue(HeightProperty, value);
        }
        
        /// <summary>
        /// Handle changes of width property change 
        /// </summary>
        /// <param name="sender"><see cref="FrameworkElement"/> which WidthProperty property was chaned</param>
        /// <param name="e">Info about property changes</param>
        private static void OnSizePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var inspectedUIElement = (FrameworkElement)sender;
            inspectedUIElement.SizeChanged -= OnElementSizeChanged;
            inspectedUIElement.SizeChanged += OnElementSizeChanged;
        }

        /// <summary>
        /// Handle change of control size.
        /// </summary>
        /// <param name="sender">Control which size was changed</param>
        /// <param name="e">Info about size change</param>
        private static void OnElementSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            var heightFormula = GetHeightFormula(element);
            if (heightFormula != null)
            {
                var currentWidth = e.NewSize.Width;
                var formula = Equation.FromExpression(heightFormula);
                var height = formula.CalculateValue(currentWidth);
                element.Height = height;
            }
            else
            {
                var widthProperty = GetWidth(element);
                if (widthProperty.IsStar)
                {
                    var currentHeigth = e.NewSize.Height;
                    element.Width = currentHeigth * widthProperty.Value;
                }
                else
                {
                    var heightProperty = GetHeight(element);
                    if (heightProperty.IsStar)
                    {
                        var currentWidth = e.NewSize.Width;
                        element.Height = currentWidth * heightProperty.Value;
                    }
                }
            }
        }
    }

    internal abstract class Equation
    {
        private static Dictionary<string, Equation> cachedEquations = new Dictionary<string, Equation>();

        public static Equation FromExpression(string value)
        {
            Equation formula = null;
            var isExist = cachedEquations.TryGetValue(value, out formula);
            if (!isExist)
            {
                formula = LineEquation.TryCreate(value);
                if (formula != null)
                {
                    cachedEquations[value] = formula;
                }
                else
                {
                    //TODO: add Reverse Polish Notation logic (in future).
                    throw new ArgumentException("Only line equation are supported: kx + b");
                }
            }

            return formula;
        }

        public abstract double CalculateValue(double argument);
    }

    internal class LineEquation : Equation
    {
        public double K { get; private set; }

        public double B { get; private set; }

        public static LineEquation TryCreate(string expression)
        {
            expression = Regex.Replace(expression, "\\s", string.Empty);
            LineEquation result = null;
            var match = Regex.Match(expression, "([\\d\\.]*)x([+-][\\d\\.]*)");
            if (match.Success)
            {
                var kString = match.Groups[1].Captures[0].Value;
                var k = Double.Parse(kString, CultureInfo.InvariantCulture);

                var bString = match.Groups[2].Captures[0].Value;
                var b = Double.Parse(bString, CultureInfo.InvariantCulture);

                result = new LineEquation(k, b);
            }

            return result;
        }

        private LineEquation(double k, double b)
        {
            this.K = k;
            this.B = b;
        }

        public override double CalculateValue(double argument)
        {
            return this.K * argument + this.B;
        }
    }
}
