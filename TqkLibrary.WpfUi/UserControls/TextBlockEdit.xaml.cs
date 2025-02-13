using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace TqkLibrary.WpfUi.UserControls
{
    public partial class TextBlockEdit : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsEditingOnLMouseDownProperty = DependencyProperty.Register(
            nameof(IsEditingOnLMouseDown),
            typeof(bool),
            typeof(TextBlockEdit),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsEditingProperty = DependencyProperty.Register(
            nameof(IsEditing),
            typeof(bool),
            typeof(TextBlockEdit),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsSellectAllOnEditingProperty = DependencyProperty.Register(
            nameof(IsSellectAllOnEditing),
            typeof(bool),
            typeof(TextBlockEdit),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(TextBlockEdit),
            new FrameworkPropertyMetadata(nameof(TextBlockEdit), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register(
            nameof(TextAlignment),
            typeof(TextAlignment),
            typeof(TextBlockEdit),
            new FrameworkPropertyMetadata(TextAlignment.Left, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AcceptsReturnProperty = DependencyProperty.Register(
            nameof(AcceptsReturn),
            typeof(bool),
            typeof(TextBlockEdit),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AcceptsTabProperty = DependencyProperty.Register(
            nameof(AcceptsTab),
            typeof(bool),
            typeof(TextBlockEdit),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 
        /// </summary>
        public bool IsEditingOnLMouseDown
        {
            get { return (bool)this.GetValue(IsEditingOnLMouseDownProperty); }
            set { this.SetValue(IsEditingOnLMouseDownProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsEditing
        {
            get { return (bool)this.GetValue(IsEditingProperty); }
            set { this.SetValue(IsEditingProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsSellectAllOnEditing
        {
            get { return (bool)this.GetValue(IsSellectAllOnEditingProperty); }
            set { this.SetValue(IsSellectAllOnEditingProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)this.GetValue(TextAlignmentProperty); }
            set { this.SetValue(TextAlignmentProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AcceptsReturn
        {
            get { return (bool)this.GetValue(AcceptsReturnProperty); }
            set { this.SetValue(AcceptsReturnProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool AcceptsTab
        {
            get { return (bool)this.GetValue(AcceptsTabProperty); }
            set { this.SetValue(AcceptsTabProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public TextBlockEdit()
        {
            this.InitializeComponent();
        }

        private void tb_name_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox is not null)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Input, () =>
                {
                    Keyboard.Focus(textBox);
                    if (this.IsSellectAllOnEditing) textBox.SelectAll();
                });
            }
        }

        private void tb_name_LostFocus(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
            this.IsEditing = false;
        }

        private void tb_name_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.ClearFocus();
                this.IsEditing = false;
            }
        }

        private void root_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && this.IsEditingOnLMouseDown)
            {
                this.IsEditing = true;
            }
        }
    }
}