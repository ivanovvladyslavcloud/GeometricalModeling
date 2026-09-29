using System.Windows;
using System.Windows.Media;
using Lab1.Core;
using Lab1.Core.Geometry;

namespace Lab1.Rendering
{
    public class PointRenderer
    {
        public void Draw(
            DrawingContext dc,
            Point2D point,
            Transform2D transform)
        {
            Point screenPoint =
                transform.ToScreen(point.X, point.Y);

            double radius = 2;

            dc.DrawEllipse(
                Brushes.Red,
                null,
                screenPoint,
                radius,
                radius
            );
        }
    }
}