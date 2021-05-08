using System;
using System.Windows;
using System.Windows.Navigation;

namespace TqkLibrary.WpfUi.Windows
{
  /// <summary>
  /// Interaction logic for OauthWindow.xaml
  /// </summary>
  public partial class OauthWindow : Window
  {
    private readonly Uri uri;
    private readonly Uri redirect_uri;

    public OauthWindow(Uri uri, Uri redirect_uri)
    {
      this.uri = uri ?? throw new ArgumentNullException(nameof(uri));
      this.redirect_uri = redirect_uri ?? throw new ArgumentNullException(nameof(redirect_uri));
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      WB.Navigate(uri);
    }

    private void WB_Navigating(object sender, NavigatingCancelEventArgs e)
    {
      if (e.Uri.AbsoluteUri.StartsWith(redirect_uri.AbsoluteUri))
      {
        UriResult = e.Uri;
        this.Close();
      }
    }

    public Uri UriResult { get; private set; }
  }
}