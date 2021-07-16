using System.Windows;
using System.Windows.Input;

namespace TqkLibrary.WpfUi.Windows
{
  public delegate bool DelegateValidate(string Name);
  /// <summary>
  /// Interaction logic for InputBox.xaml
  /// </summary>
  public partial class InputBox : Window
  {
    public InputBox(string InputDefault = "")
    {
      InitializeComponent();
      this.Tb.Text = InputDefault;

    }

    public event DelegateValidate Validate;

    public string Result { get; private set; }
    private void Add_Click(object sender, RoutedEventArgs e)
    {
      if (Validate?.Invoke(Tb.Text) == true)
      {
        Result = Tb.Text;
        this.Close();
      }
      else MessageBox.Show("Item are existed", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
      Result = null;
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
