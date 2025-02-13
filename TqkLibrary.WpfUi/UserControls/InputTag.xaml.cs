using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TqkLibrary.WpfUi.UserControls
{
    /// <summary>
    /// Interaction logic for InputTag.xaml
    /// </summary>
    public partial class InputTag : UserControl
    {
#if DEBUG
        class DebugData : ObservableCollection<string>
        {
            public DebugData(int count)
            {
                for (int i = 0; i < count; i++)
                {
                    this.Add($"Item {i}");
                }
            }

        }
#endif
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsEditingOnLMouseDownProperty = DependencyProperty.Register(
            nameof(Items),
            typeof(ObservableCollection<string>),
            typeof(InputTag),
            new FrameworkPropertyMetadata(

#if DEBUG
                new DebugData(3)
#else
                null
#endif
                ));
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<string> Items
        {
            get { return (ObservableCollection<string>)GetValue(IsEditingOnLMouseDownProperty); }
            set { SetValue(IsEditingOnLMouseDownProperty, value); }
        }




        public InputTag()
        {
            InitializeComponent();
        }

        private void ButtonDetailRemove_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            string? context = button?.DataContext as string;
            if (!string.IsNullOrWhiteSpace(context))
                Items?.Remove(context!);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter &&
                sender is TextBox textBox &&
                !string.IsNullOrWhiteSpace(textBox.Text))
            {
                Items?.Add(textBox.Text);
                textBox.Clear();
            }
        }

        private void Button_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.HeightChanged && sender is Button button)
            {
                button.Width = button.ActualHeight;
            }
        }
    }
}
