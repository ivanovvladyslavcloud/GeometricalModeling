using Lab2.Core;
using Lab2.Core.Geometry;
using System.Windows;
using System.Windows.Media;

namespace Lab2.Rendering
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