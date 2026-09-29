using System;
using Lab2.Core.Geometry;

namespace Lab2.Core
{
    public class Part
    {
        public Figure Figure { get; set; }

        public Part(Figure figure)
        {
            Figure = figure;
        }

        public void Translate(double dx, double dy)
        {

            if (Figure.Points != null)
            {
                foreach (Point2D point in Figure.Points)
                {
                    point.X += dx;
                    point.Y += dy;
                }
            }

            foreach (var obj in Figure.GeometryObjects)
            {
                if (obj is Strophoid2D strophoid)
                {
                    strophoid.Translate(dx, dy);
                }
            }
        }

        public void Rotate(double centerX, double centerY, double angleDegrees)
        {

            if (Figure.Points != null)
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
            }

            foreach (var obj in Figure.GeometryObjects)
            {
                if (obj is Strophoid2D strophoid)
                {
                    strophoid.Rotate(centerX, centerY, angleDegrees);
                }
            }
        }
    }
}