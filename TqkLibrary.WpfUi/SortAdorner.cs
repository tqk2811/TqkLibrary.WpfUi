﻿using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace TqkLibrary.WpfUi
{
    public class SortAdorner : Adorner
    {
        private static readonly Geometry ascGeometry = Geometry.Parse("M 0 4 L 3.5 0 L 7 4 Z");

        private static readonly Geometry descGeometry = Geometry.Parse("M 0 0 L 3.5 4 L 7 0 Z");

        public ListSortDirection Direction { get; private set; }

        public SortAdorner(UIElement element, ListSortDirection dir) : base(element)
        {
            this.Direction = dir;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (this.AdornedElement.RenderSize.Width < 20) return;
            TranslateTransform transform = new(
                this.AdornedElement.RenderSize.Width - 15,
                (this.AdornedElement.RenderSize.Height - 5) / 2
            );
            drawingContext.PushTransform(transform);
            Geometry geometry = ascGeometry;
            if (this.Direction == ListSortDirection.Descending) geometry = descGeometry;
            drawingContext.DrawGeometry(Brushes.Black, null, geometry);
            drawingContext.Pop();
        }
    }
}