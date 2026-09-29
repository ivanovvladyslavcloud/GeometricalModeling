using Lab1.Core.Geometry;
using System;

namespace Lab1.Core
{
    public class Part
    {
        public Figure Figure { get; private set; }

        public Part(Figure figure)
        {
            Figure = figure;
        }

        public void Translate(double dx, double dy)
        {
            foreach (Point2D point in Figure.Points)
            {
                point.X += dx;
                point.Y += dy;
            }

            Figure.Solve();
        }

        public void Rotate(double centerX, double centerY, double angleDegrees)
        {
            double angle = angleDegrees * Math.PI / 180.0;
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            foreach (Point2D point in Figure.Points)
            {
                double dx = point.X - centerX;
                double dy = point.Y - centerY;

                double newX = centerX + dx * cos - dy * sin;
                double newY = centerY + dx * sin + dy * cos;

                point.X = newX;
                point.Y = newY;
            }

            Figure.Solve();
        }

    }
}