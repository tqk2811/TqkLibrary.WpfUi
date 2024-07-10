using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;

namespace TqkLibrary.WpfUi.UserControls
{
    public partial class TokenizingControl : RichTextBox
    {
        static readonly ResourceDictionary _resourceDictionary = new ResourceDictionary()
        {
            Source = new Uri("pack://application:,,,/TqkLibrary.WpfUi;component/UserControls/TokenizingControlDictionary.xaml")
        };

        public static readonly DependencyProperty TokenTemplateProperty =
            DependencyProperty.Register(
                nameof(TokenTemplate),
                typeof(DataTemplate),
                typeof(TokenizingControl),
                new FrameworkPropertyMetadata(_resourceDictionary["NameTokenTemplate"])
                );
        public DataTemplate TokenTemplate
        {
            get { return (DataTemplate)GetValue(TokenTemplateProperty); }
            set { SetValue(TokenTemplateProperty, value); }
        }


        public static readonly DependencyProperty TokenMatcherProperty =
            DependencyProperty.Register(
                nameof(TokenMatcher),
                typeof(string),
                typeof(TokenizingControl),
                new FrameworkPropertyMetadata(";")
                );
        public string TokenMatcher
        {
            get { return (string)GetValue(TokenMatcherProperty); }
            set { SetValue(TokenMatcherProperty, value); }
        }

        public TokenizingControl()
        {
            TextChanged += OnTokenTextChanged;
        }

        string? ReplaceMatcher(string text)
        {
            if (text.EndsWith(TokenMatcher))
            {
                // Remove the ';'
                return text.Substring(0, text.Length - 1).Trim();
            }
            return null;
        }


        private void OnTokenTextChanged(object sender, TextChangedEventArgs e)
        {
            string? text = CaretPosition.GetTextInRun(LogicalDirection.Backward);
            if(!string.IsNullOrWhiteSpace(text) && text!.Contains(TokenMatcher))
            {
                ReplaceTextWithToken(text!);
            }
        }

        private void ReplaceTextWithToken(string inputText)
        {
            string token = string.Empty;
            // Remove the handler temporarily as we will be modifying tokens below, causing more TextChanged events
            TextChanged -= OnTokenTextChanged;

            var para = CaretPosition.Paragraph;

            Run? matchedRun = para.Inlines.FirstOrDefault(inline =>
            {
                var run = inline as Run;
                return (run is not null && run.Text.Contains(inputText));
            }) as Run;
            if (matchedRun is not null) // Found a Run that matched the inputText
            {
                var tokenContainer = CreateTokenContainer(inputText, token);
                para.Inlines.InsertBefore(matchedRun, tokenContainer);

                // Remove only if the Text in the Run is the same as inputText, else split up
                if (matchedRun.Text == inputText)
                {
                    para.Inlines.Remove(matchedRun);
                }
                else // Split up
                {
                    var index = matchedRun.Text.IndexOf(inputText) + inputText.Length;
                    var tailEnd = new Run(matchedRun.Text.Substring(index));
                    para.Inlines.InsertAfter(matchedRun, tailEnd);
                    para.Inlines.Remove(matchedRun);
                }
            }

            TextChanged += OnTokenTextChanged;
        }

        private InlineUIContainer CreateTokenContainer(string inputText, string token)
        {
            // Note: we are not using the inputText here, but could be used in future

            var presenter = new ContentPresenter()
            {
                Content = token,
                ContentTemplate = TokenTemplate,
            };

            // BaselineAlignment is needed to align with Run
            return new InlineUIContainer(presenter) { BaselineAlignment = BaselineAlignment.TextBottom };
        }


    }

}
