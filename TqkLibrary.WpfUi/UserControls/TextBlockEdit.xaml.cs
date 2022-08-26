using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace TqkLibrary.WpfUi.UserControls
{
    public partial class TextBlockEdit : UserControl
    {
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
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
          nameof(Text),
          typeof(string),
          typeof(TextBlockEdit),
          new FrameworkPropertyMetadata(nameof(TextBlockEdit), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        //public static readonly DependencyProperty FontFamilyProperty;
        //public static readonly DependencyProperty FontStyleProperty;
        //public static readonly DependencyProperty FontWeightProperty;
        //public static readonly DependencyProperty FontStretchProperty;
        //public static readonly DependencyProperty FontSizeProperty;
        //public static readonly DependencyProperty ForegroundProperty;
        //public static readonly DependencyProperty BackgroundProperty;
        /// <summary>
        /// 
        /// </summary>
        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }


        ///// <summary>
        ///// 
        ///// </summary>
        //public new FontFamily FontFamily
        //{
        //    get
        //    {
        //        return (FontFamily)GetValue(FontFamilyProperty);
        //    }
        //    set
        //    {
        //        SetValue(FontFamilyProperty, value);
        //    }
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        //public new FontStyle FontStyle
        //{
        //    get
        //    {
        //        return (FontStyle)GetValue(FontStyleProperty);
        //    }
        //    set
        //    {
        //        SetValue(FontStyleProperty, value);
        //    }
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        //public new FontWeight FontWeight
        //{
        //    get
        //    {
        //        return (FontWeight)GetValue(FontWeightProperty);
        //    }
        //    set
        //    {
        //        SetValue(FontWeightProperty, value);
        //    }
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        //public new FontStretch FontStretch
        //{
        //    get
        //    {
        //        return (FontStretch)GetValue(FontStretchProperty);
        //    }
        //    set
        //    {
        //        SetValue(FontStretchProperty, value);
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        //public new double FontSize
        //{
        //    get
        //    {
        //        return (double)GetValue(FontSizeProperty);
        //    }
        //    set
        //    {
        //        SetValue(FontSizeProperty, value);
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        //public new Brush Foreground
        //{
        //    get
        //    {
        //        return (Brush)GetValue(ForegroundProperty);
        //    }
        //    set
        //    {
        //        SetValue(ForegroundProperty, value);
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        //public new Brush Background
        //{
        //    get
        //    {
        //        return (Brush)GetValue(BackgroundProperty);
        //    }
        //    set
        //    {
        //        SetValue(BackgroundProperty, value);
        //    }
        //}



        /// <summary>
        /// 
        /// </summary>
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