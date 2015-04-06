namespace BaseTools.UI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using Microsoft.Phone.Controls;

    /// <summary>
    /// Use for display text which display size more than size limit of single element 
    /// </summary>
    [TemplatePart(Name = "StackPanel", Type = typeof(StackPanel))]
    public class LongTextBlock : Control
    {
        /// <summary>
        /// Represent plain text property 
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(LongTextBlock), new PropertyMetadata(String.Empty, OnTextPropertyChanged));

        ///// <summary>
        ///// Represent formatted text property.
        ///// </summary>
        //public static readonly DependencyProperty FormattedTextProperty =
        //    DependencyProperty.Register("FormattedText", typeof(IList<ParagraphText>), typeof(LongTextBlock), new PropertyMetadata(OnTextPropertyChanged));

        ///// <summary>
        ///// Represent linkable text.
        ///// </summary>
        //public static readonly DependencyProperty LinkableTextProperty =
        //    DependencyProperty.Register("LinkableText", typeof(LinkableText), typeof(LongTextBlock), new PropertyMetadata(OnTextPropertyChanged));

        /// <summary>
        ///  Size limit of single element.
        /// </summary>
        private const double SingleElementSizeLimit = 2048;

        /// <summary>
        /// Initial value of defaultWidth field 
        /// </summary>
        private const double DefaultWidthInitialValue = 300;
        
        /// <summary>
        /// Panel which contains textblocks.
        /// </summary>
        private Panel itemsPanel;

        /// <summary>
        /// Initializes a new instance of the LongTextBlock class.
        /// </summary>
        public LongTextBlock()
        {
            // Get the style from generic.xaml
            this.DefaultStyleKey = typeof(LongTextBlock);
        }

        ///// <summary>
        ///// Gets or sets linkable text
        ///// </summary>
        //public LinkableText LinkableText
        //{
        //    get
        //    {
        //        return (LinkableText)GetValue(LinkableTextProperty);
        //    }

        //    set
        //    {
        //        SetValue(LinkableTextProperty, value);
        //    }
        //}

        ///// <summary>
        ///// Gets or sets formatted text. 
        ///// </summary>
        //public IList<ParagraphText> FormattedText
        //{
        //    get
        //    {
        //        return (IList<ParagraphText>)GetValue(FormattedTextProperty);
        //    }

        //    set
        //    {
        //        SetValue(FormattedTextProperty, value);
        //    }
        //}

        /// <summary>
        /// Gets or sets plain text. 
        /// </summary>
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }

            set
            {
                SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Method is called just before a UI element displays in an application.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.itemsPanel = this.GetTemplateChild("panel") as Panel;
            this.DisplayText();
        }

        /// <summary>
        /// Handle change of Text or FormattedText dependency property change
        /// </summary>
        /// <param name="d">LongTextBlock which property was changed</param>
        /// <param name="e">Info about property change</param>
        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LongTextBlock textBlock = (LongTextBlock)d;
            textBlock.DisplayText();
        }

        /// <summary>
        /// Display text
        /// </summary>
        private void DisplayText()
        {
            //if (this.FormattedText != null && this.FormattedText.Count > 0)
            //{
            //    this.DisplayFormattedText(this.FormattedText);
            //}
            //else if (this.LinkableText != null)
            //{
            //    this.DisplayLinkableText(this.LinkableText);
            //}
            if (!String.IsNullOrEmpty(this.Text))
            {
                this.DisplayPlainText(this.Text);
            }
            else
            {
                if (this.itemsPanel != null)
                {
                    this.itemsPanel.Children.Clear();
                }
            }
        }

        ///// <summary>
        ///// Display text that can contains links.
        ///// </summary>
        ///// <param name="linkableText">Text which can contains links</param>
        //private void DisplayLinkableText(LinkableText linkableText)
        //{
        //    if (this.itemsPanel == null)
        //    {
        //        return;
        //    }

        //    this.itemsPanel.Children.Clear();

        //    if (linkableText.Words != null)
        //    {
        //        if (linkableText.ContainsLinks)
        //        {
        //            foreach (var item in linkableText.Words)
        //            {
        //                if (item is Word)
        //                {
        //                    if (item.DisplayedText.Contains("\n"))
        //                    {
        //                        var lineBreaker = this.CreateLineBreaker();
        //                        this.itemsPanel.Children.Add(lineBreaker);
        //                    }
        //                    else
        //                    {
        //                        var textBlock = this.CreateTextBlock();
        //                        textBlock.Text = item.DisplayedText;
        //                        textBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
        //                        textBlock.VerticalAlignment = System.Windows.VerticalAlignment.Top;
        //                        this.itemsPanel.Children.Add(textBlock);
        //                    }
        //                }
        //                else
        //                {
        //                    var link = (LinksInText)item;
        //                    var hyperlinkButton = this.CreateHyperlinkButton(link);
        //                    this.itemsPanel.Children.Add(hyperlinkButton);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            var plainText = linkableText.Words[0].DisplayedText;
        //            this.DisplayPlainText(plainText);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Display formatted text property.
        ///// </summary>
        ///// <param name="paragraphs">paragraphs for displaying</param>
        //private void DisplayFormattedText(IList<ParagraphText> paragraphs)
        //{
        //    if (this.itemsPanel == null)
        //    {
        //        return;
        //    }

        //    this.itemsPanel.Children.Clear();

        //    // TODO: Max count of symbols in one textblock. Must depends on font, font size.
        //    TextBlock textBlock = this.CreateTextBlock();

        //    // Получает количество символов, которое может вместить в себя один текст блок.
        //    int symbolsCount = this.GetSymbolsCount(FontWeights.Normal, FontStyles.Normal);

        //    // Количество символов, которые еще можно вместить в текст блок.
        //    int lessSize = symbolsCount;

        //    for (int i = 0; i < paragraphs.Count; i++)
        //    {
        //        string text = paragraphs[i].Text;
        //        TextBlockType type = paragraphs[i].Type;
        //        while (text.Length > 0)
        //        {
        //            Run run = this.CreateRun(type);

        //            // Если текст параграфа помещается полностью в оставшееся место в текст блоке - то его добавляем.
        //            if (text.Length <= lessSize)
        //            {
        //                run.Text = text;
        //                textBlock.Inlines.Add(run);

        //                // Просчитываем количество оставшихся символов.
        //                lessSize -= text.Length;

        //                // Очищаем текст, чтобы выйти из цикла.
        //                text = String.Empty;
        //            }
        //            else
        //            {
        //                // Если текст не вмещается, берем столько символов, сколько еще может вместиться
        //                run.Text = text.Substring(0, lessSize);
        //                textBlock.Inlines.Add(run);

        //                // Добавляем заполненный текст блок в стек панель.
        //                this.itemsPanel.Children.Add(textBlock);

        //                // Создаем новый текст блок.
        //                textBlock = this.CreateTextBlock();

        //                // Удаляем тот текст, который уже добавили.
        //                text = text.Remove(0, lessSize);

        //                // Задаем оставшееся количество символов.
        //                lessSize = symbolsCount;                        
        //            }
        //        }

        //        // Если текущий параграф последний и текстблок еще не был добавлен в стек панель, то добавляем его.
        //        if (i == paragraphs.Count - 1 && !this.itemsPanel.Children.Contains(textBlock))
        //        {
        //            this.itemsPanel.Children.Add(textBlock);
        //        }
        //    }
        //}

        /// <summary>
        /// Calculate symbols count which can be fitted to single textblock.
        /// </summary>
        /// <param name="weight">Font weight</param>
        /// <param name="style">Font style</param>
        /// <returns>count of symbols that can be placed to single textblock</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Controls.TextBlock.set_Text(System.String)",
                                                         Justification = "Used only for measurment")]
        private int GetSymbolsCount(FontWeight weight, FontStyle style)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.FontSize = this.FontSize;
            textBlock.FontStyle = style;
            textBlock.FontFamily = this.FontFamily;
            textBlock.FontWeight = weight;
            textBlock.Text = "A";
            Size size = new Size(textBlock.ActualWidth, textBlock.ActualHeight);
            double width = DefaultWidthInitialValue;
            if (!Double.IsNaN(this.Width))
            {
                width = this.Width;
            }

            // Not all lines contains lineSymbolsCount symbols.
            int lineSymbolsCount = (int)(width * 0.66 / size.Width);
            int symbolsCount = (int)(SingleElementSizeLimit / size.Height) * lineSymbolsCount;
            return symbolsCount;
        }

        /// <summary>
        /// Measure height on text line.
        /// </summary>
        /// <returns>Height on text line.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Controls.TextBlock.set_Text(System.String)", 
                                                         Justification = "Used for text measurments")]
        private double MeasureLineHeight()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.FontSize = this.FontSize;
            textBlock.FontFamily = this.FontFamily;
            textBlock.Text = "A";
            return textBlock.ActualHeight;
        }

        /// <summary>
        /// Display plain text
        /// </summary>
        /// <param name="value">text for displayng</param>
        private void DisplayPlainText(string value)
        {
            if (this.itemsPanel == null)
            {
                return;
            }

            this.itemsPanel.Children.Clear();

            // Получает количество символов, которое может вместить в себя один текст блок.
            int symbolsPerTextBlock = this.GetSymbolsCount(FontWeights.Normal, FontStyles.Normal);
            int startPosition = 0;
            do
            {
                var remainingLength = value.Length - startPosition;
                var substringLength = Math.Min(remainingLength, symbolsPerTextBlock);
                var textPart = value.Substring(startPosition, substringLength);
                var textBlock = this.CreateTextBlock();
                textBlock.Text = textPart;
                this.itemsPanel.Children.Add(textBlock);
                startPosition += textPart.Length;
            }
            while (startPosition < value.Length);
        }

        /// <summary>
        /// Create textblock with specified font.
        /// </summary>
        /// <returns>New texblock.</returns>
        private TextBlock CreateTextBlock()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.Foreground = this.Foreground;
            return textBlock;
        }

        ///// <summary>
        ///// Create run. 
        ///// </summary>
        ///// <param name="type">type of text that must be placed in run</param>
        ///// <returns>New instance of System.Windows.Documents.Run</returns>
        //private Run CreateRun(TextBlockType type)
        //{
        //    Run run = new Run();
        //    switch (type)
        //    {
        //        case TextBlockType.Bold:
        //            run.FontWeight = FontWeights.Bold;
        //            break;
        //        case TextBlockType.Italic:
        //            run.FontStyle = FontStyles.Italic;
        //            break;
        //    }

        //    return run;
        //}

        ///// <summary>
        ///// Create Hyperlink button control.
        ///// </summary>
        ///// <param name="link">hyperlink button data</param>
        ///// <returns>New Hyperlink button</returns>
        //private HyperlinkButton CreateHyperlinkButton(LinksInText link)
        //{
        //    var hyperlinkButton = new HyperlinkButton
        //    {
        //        Foreground = (Brush)Application.Current.Resources["PhoneAccentBrush"],
        //        NavigateUri = link.Uri,
        //        Content = link.DisplayedText,
        //        FontFamily = this.FontFamily,
        //        FontWeight = this.FontWeight,
        //        FontSize = this.FontSize,
        //        HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left,
        //        Margin = new Thickness(-12, 0, -12, 0)
        //    };

        //    if (link.IsUsualLink)
        //    {
        //        hyperlinkButton.TargetName = "_blank";
        //    }

        //    return hyperlinkButton;
        //}
    }
}