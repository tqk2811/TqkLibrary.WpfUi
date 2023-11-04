using System;
using System.Windows;
using System.Windows.Input;

namespace TqkLibrary.WpfUi.Windows
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public delegate void ValidateEventHandler(object sender, ValidateEventArgs e);
    /// <summary>
    /// 
    /// </summary>
    public class ValidateEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsValid { get; set; } = false;
        /// <summary>
        /// 
        /// </summary>
        public string ErrorMessage { get; set; } = nameof(ErrorMessage);
        /// <summary>
        /// 
        /// </summary>
        public string ErrorTitle { get; set; } = "Error";
    }
    /// <summary>
    /// Interaction logic for InputBox.xaml
    /// </summary>
    public partial class InputBox : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public event ValidateEventHandler Validate;



        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(InputBox),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register(
            nameof(TextAlignment),
            typeof(TextAlignment),
            typeof(InputBox),
            new FrameworkPropertyMetadata(TextAlignment.Left, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        /// <summary>
        /// 
        /// </summary>
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }


        /// <summary>
        /// 
        /// </summary>
        public InputBox()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (Validate is not null)
            {
                ValidateEventArgs validateEventArgs = new ValidateEventArgs();
                Validate?.Invoke(this, validateEventArgs);
                if (!validateEventArgs.IsValid)
                {
                    MessageBox.Show(validateEventArgs.ErrorMessage, validateEventArgs.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Add_Click(null, null);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Tb.Focus();
            Tb.SelectAll();
            Keyboard.Focus(Tb);
        }
    }
}
