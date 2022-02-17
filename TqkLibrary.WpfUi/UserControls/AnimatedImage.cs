using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            get { return (int)GetValue(FrameIndexProperty); }
            set { SetValue(FrameIndexProperty, value); }
        }

        /// <summary>
        /// Gets / sets the image that will be drawn.
        /// </summary>
        public new ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Gets / sets running or pause
        /// </summary>
        public bool IsAnimationWorking
        {
            get { return (bool)GetValue(IsAnimationWorkingProperty); }
            set { SetValue(IsAnimationWorkingProperty, value); }
        }
        #endregion

        #region Protected interface

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Source property.
        /// </summary>
        protected virtual void OnSourceChanged(DependencyPropertyChangedEventArgs aEventArgs)
        {
            ClearAnimation();

            BitmapImage lBitmapImage = aEventArgs.NewValue as BitmapImage;

            if (lBitmapImage == null)
            {
                ImageSource lImageSource = aEventArgs.NewValue as ImageSource;
                base.Source = lImageSource;
                return;
            }

            if (!IsAnimatedGifImage(lBitmapImage))
            {
                base.Source = lBitmapImage;
                return;
            }

            PrepareAnimation(lBitmapImage);
        }

        #endregion

        #region Private properties

        private Int32Animation Animation { get; set; }
        private GifBitmapDecoder Decoder { get; set; }

        #endregion

        #region Private methods

        private void ClearAnimation()
        {
            if (Animation != null)
            {
                BeginAnimation(FrameIndexProperty, null);
            }

            //IsAnimationWorking = false;
            Animation = null;
            Decoder = null;
        }

        private void PrepareAnimation(BitmapImage aBitmapImage)
        {
            Debug.Assert(aBitmapImage != null);

            if (aBitmapImage.UriSource != null)
            {
                Decoder = new GifBitmapDecoder(
                    aBitmapImage.UriSource,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.Default);
            }
            else
            {
                aBitmapImage.StreamSource.Position = 0;
                Decoder = new GifBitmapDecoder(
                    aBitmapImage.StreamSource,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.Default);
            }

            Animation =
                new Int32Animation(
                    0,
                    Decoder.Frames.Count - 1,
                    new Duration(
                        new TimeSpan(
                            0,
                            0,
                            0,
                            Decoder.Frames.Count / 10,
                            (int)((Decoder.Frames.Count / 10.0 - Decoder.Frames.Count / 10) * 1000))))
                {
                    RepeatBehavior = RepeatBehavior.Forever
                };

            base.Source = Decoder.Frames[0];
            BeginAnimation(FrameIndexProperty, Animation);
            //IsAnimationWorking = true;
        }

        private bool IsAnimatedGifImage(BitmapImage aBitmapImage)
        {
            Debug.Assert(aBitmapImage != null);

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
                        new GifBitmapDecoder(
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

        private static void ChangingFrameIndex
            (DependencyObject aObject, DependencyPropertyChangedEventArgs aEventArgs)
        {
            AnimatedImage lAnimatedImage = aObject as AnimatedImage;

            if (lAnimatedImage == null || !lAnimatedImage.IsAnimationWorking)
            {
                return;
            }

            int lFrameIndex = (int)aEventArgs.NewValue;
            ((Image)lAnimatedImage).Source = lAnimatedImage.Decoder.Frames[lFrameIndex];
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
