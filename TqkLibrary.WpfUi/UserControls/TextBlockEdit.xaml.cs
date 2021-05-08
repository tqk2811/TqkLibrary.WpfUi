using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace TqkLibrary.WpfUi.UserControls
{
  public sealed partial class TextBlockEdit : UserControl
  {
    public static readonly DependencyProperty IsEditingProperty = DependencyProperty.Register(
      nameof(IsEditing),
      typeof(bool),
      typeof(TextBlockEdit),
      new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
      nameof(Text),
      typeof(string),
      typeof(TextBlockEdit),
      new FrameworkPropertyMetadata("TextBlockEdit", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


    public bool IsEditing
    {
      get { return (bool)GetValue(IsEditingProperty); }
      set { SetValue(IsEditingProperty, value); }
    }

    public string Text
    {
      get { return (string)GetValue(TextProperty); }
      set { SetValue(TextProperty, value); }
    }


    public TextBlockEdit()
    {
      InitializeComponent();
    }

    private void root_Loaded(object sender, RoutedEventArgs e)
    {
      Window window = Window.GetWindow(this);
      if (window != null) //fix design
        window.MouseDown += Window_MouseDown;
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
      this.Focus();
    }

    private void tb_name_GotFocus(object sender, RoutedEventArgs e)
    {
      TextBox textBox = sender as TextBox;
      Dispatcher.BeginInvoke(DispatcherPriority.Input,
          new Action(delegate ()
          {
            Keyboard.Focus(textBox);
            textBox.SelectAll();
          }));
    }

    private void tb_name_LostFocus(object sender, RoutedEventArgs e)
    {
      Keyboard.ClearFocus();
      IsEditing = false;
    }

    private void tb_name_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        Keyboard.ClearFocus();
        IsEditing = false;
      }
    }
  }
}