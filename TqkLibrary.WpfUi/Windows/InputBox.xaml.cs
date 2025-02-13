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
            get { return this._Text; }
            set { this._Text = value; this.NotifyPropertyChange(); }
        }

        TextAlignment _TextAlignment = TextAlignment.Left;
        public TextAlignment TextAlignment
        {
            get { return this._TextAlignment; }
            set { this._TextAlignment = value; this.NotifyPropertyChange(); }
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
        public event ValidateEventHandler? Validate;

        /// <summary>
        /// 
        /// </summary>
        public TextAlignment TextAlignment
        {
            get { return this.inputBoxVM.TextAlignment; }
            set { this.inputBoxVM.TextAlignment = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get { return this.inputBoxVM.Text; }
            set { this.inputBoxVM.Text = value; }
        }

        readonly InputBoxVM inputBoxVM;
        /// <summary>
        /// 
        /// </summary>
        public InputBox()
        {
            this.InitializeComponent();
            this.inputBoxVM = this.DataContext as InputBoxVM ?? throw new InvalidOperationException();
        }

        private void Add_Click(object? sender, RoutedEventArgs? e)
        {
            if (Validate is not null)
            {
                ValidateEventArgs validateEventArgs = new();
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
                this.Add_Click(null, null);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Tb.Focus();
            this.Tb.SelectAll();
            Keyboard.Focus(this.Tb);
        }
    }
}
