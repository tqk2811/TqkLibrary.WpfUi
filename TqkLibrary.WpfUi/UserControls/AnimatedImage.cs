using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace TqkLibrary.WpfUi.UserControls
{
    //https://stackoverflow.com/a/1660225/5034139
    /// <summary>
    /// Control the "Images", which supports animated GIF.
    /// </summary>
    public class AnimatedImage : Image
    {
        #region Public properties

        /// <summary>
        /// Gets / sets the number of the current frame.
        /// </summary>
        public int FrameIndex
        {
            get { return (int)this.GetValue(FrameIndexProperty); }
            set { this.SetValue(FrameIndexProperty, value); }
        }

        /// <summary>
        /// Gets / sets the image that will be drawn.
        /// </summary>
        public new ImageSource Source
        {
            get { return (ImageSource)this.GetValue(SourceProperty); }
            set { this.SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Gets / sets running or pause
        /// </summary>
        public bool IsAnimationWorking
        {
            get { return (bool)this.GetValue(IsAnimationWorkingProperty); }
            set { this.SetValue(IsAnimationWorkingProperty, value); }
        }
        #endregion

        #region Protected interface

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Source property.
        /// </summary>
        protected virtual void OnSourceChanged(DependencyPropertyChangedEventArgs aEventArgs)
        {
            this.ClearAnimation();

            BitmapImage? lBitmapImage = aEventArgs.NewValue as BitmapImage;

            if (lBitmapImage == null)
            {
                ImageSource? lImageSource = aEventArgs.NewValue as ImageSource;
                base.Source = lImageSource;
                return;
            }

            if (!this.IsAnimatedGifImage(lBitmapImage))
            {
                base.Source = lBitmapImage;
                return;
            }

            this.PrepareAnimation(lBitmapImage);
        }

        #endregion

        #region Private properties

        private Int32Animation? Animation { get; set; }
        private GifBitmapDecoder? Decoder { get; set; }

        #endregion

        #region Private methods

        private void ClearAnimation()
        {
            if (this.Animation != null)
            {
                this.BeginAnimation(FrameIndexProperty, null);
            }

            //IsAnimationWorking = false;
            this.Animation = null;
            this.Decoder = null;
        }

        private void PrepareAnimation(BitmapImage aBitmapImage)
        {
            if (aBitmapImage is null) throw new ArgumentNullException(nameof(aBitmapImage));

            if (aBitmapImage.UriSource != null)
            {
                this.Decoder = new GifBitmapDecoder(
                    aBitmapImage.UriSource,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.Default);
            }
            else
            {
                aBitmapImage.StreamSource.Position = 0;
                this.Decoder = new GifBitmapDecoder(
                    aBitmapImage.StreamSource,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.Default);
            }

            this.Animation =
                new Int32Animation(
                    0,
                    this.Decoder.Frames.Count - 1,
                    new Duration(
                        new TimeSpan(
                            0,
                            0,
                            0,
                            this.Decoder.Frames.Count / 10,
                            (int)((this.Decoder.Frames.Count / 10.0 - this.Decoder.Frames.Count / 10) * 1000))))
                {
                    RepeatBehavior = RepeatBehavior.Forever
                };

            base.Source = this.Decoder.Frames[0];
            this.BeginAnimation(FrameIndexProperty, this.Animation);
            //IsAnimationWorking = true;
        }

        private bool IsAnimatedGifImage(BitmapImage aBitmapImage)
        {
            if (aBitmapImage is null) throw new ArgumentNullException(nameof(aBitmapImage));

            bool lResult = false;
            if (aBitmapImage.UriSource != null)
            {
                BitmapDecoder lBitmapDecoder = BitmapDecoder.Create(
                    aBitmapImage.UriSource,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.Default);
                lResult = lBitmapDecoder is GifBitmapDecoder;
            }
            else if (aBitmapImage.StreamSource != null)
            {
                try
                {
                    long lStreamPosition = aBitmapImage.StreamSource.Position;
                    aBitmapImage.StreamSource.Position = 0;
                    GifBitmapDecoder lBitmapDecoder =
                        new(
                            aBitmapImage.StreamSource,
                            BitmapCreateOptions.PreservePixelFormat,
                            BitmapCacheOption.Default);
                    lResult = lBitmapDecoder.Frames.Count > 1;

                    aBitmapImage.StreamSource.Position = lStreamPosition;
                }
                catch
                {
                    lResult = false;
                }
            }

            return lResult;
        }

        private static void ChangingFrameIndex(DependencyObject aObject, DependencyPropertyChangedEventArgs aEventArgs)
        {
            AnimatedImage? lAnimatedImage = aObject as AnimatedImage;

            if (lAnimatedImage == null || !lAnimatedImage.IsAnimationWorking)
            {
                return;
            }

            int lFrameIndex = (int)aEventArgs.NewValue;
            ((Image)lAnimatedImage).Source = lAnimatedImage.Decoder?.Frames[lFrameIndex];
            lAnimatedImage.InvalidateVisual();
        }

        /// <summary>
        /// Handles changes to the Source property.
        /// </summary>
        private static void OnSourceChanged
            (DependencyObject aObject, DependencyPropertyChangedEventArgs aEventArgs)
        {
            ((AnimatedImage)aObject).OnSourceChanged(aEventArgs);
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// FrameIndex Dependency Property
        /// </summary>
        public static readonly DependencyProperty FrameIndexProperty =
            DependencyProperty.Register(
                nameof(FrameIndex),
                typeof(int),
                typeof(AnimatedImage),
                new UIPropertyMetadata(0, ChangingFrameIndex));

        /// <summary>
        /// Source Dependency Property
        /// </summary>
        public new static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(
                nameof(Source),
                typeof(ImageSource),
                typeof(AnimatedImage),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure,
                    OnSourceChanged));

        /// <summary>
        /// FrameIndex Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsAnimationWorkingProperty =
            DependencyProperty.Register(
                nameof(IsAnimationWorking),
                typeof(bool),
                typeof(AnimatedImage),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        #endregion
    }
}
