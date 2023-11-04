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
    /// 
    /// </summary>
    public interface IInputBox
    {
        /// <summary>
        /// 
        /// </summary>
        string Text { get; set; }
        /// <summary>
        /// 
        /// </summary>
        TextAlignment TextAlignment { get; set; }
    }

    internal class InputBoxVM : BaseViewModel, IInputBox
    {
        string _Text = string.Empty;
        public string Text
        {
            get { return _Text; }
            set { _Text = value; NotifyPropertyChange(); }
        }

        TextAlignment _TextAlignment = TextAlignment.Left;
        public TextAlignment TextAlignment
        {
            get { return _TextAlignment; }
            set { _TextAlignment = value; NotifyPropertyChange(); }
        }
    }
    /// <summary>
    /// Interaction logic for InputBox.xaml
    /// </summary>
    public partial class InputBox : Window, IInputBox
    {
        /// <summary>
        /// 
        /// </summary>
        public event ValidateEventHandler Validate;

        /// <summary>
        /// 
        /// </summary>
        public TextAlignment TextAlignment
        {
            get { return inputBoxVM.TextAlignment; }
            set { inputBoxVM.TextAlignment = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get { return inputBoxVM.Text; }
            set { inputBoxVM.Text = value; }
        }

        readonly InputBoxVM inputBoxVM;
        /// <summary>
        /// 
        /// </summary>
        public InputBox()
        {
            InitializeComponent();
            this.inputBoxVM = this.DataContext as InputBoxVM;
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
