using Lab1.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Lab1.Core
{
    public class Transform2D
    {
        public double Scale { get; set; } = 20;

        public double OffsetX { get; set; }
        public double OffsetY { get; set; }

        public Point ToScreen(double x, double y)
        {
            return new Point(
                OffsetX + x * Scale,
                OffsetY - y * Scale
            );
        }

        public Point ToScreen(Point point)
        {
            return ToScreen(point.X, point.Y);
        }

        public Point ToWorld(Point screenPoint)
        {
            return new Point(
                (screenPoint.X - OffsetX) / Scale,
                (OffsetY - screenPoint.Y) / Scale
            );
        }

        public void CenterOrigin(double canvasWidth, double canvasHeight)
        {
            OffsetX = canvasWidth / 2.0;
            OffsetY = canvasHeight / 2.0;
        }

        public void Zoom(double factor, double canvasWidth, double canvasHeight)
        {
            if (Scale * factor < 0.1 || Scale * factor > 2000)
                return;

            double cx = canvasWidth / 2.0;
            double cy = canvasHeight / 2.0;

            Point worldCenter = ToWorld(new Point(cx, cy));

            Scale *= factor;

            OffsetX = cx - worldCenter.X * Scale;
            OffsetY = cy + worldCenter.Y * Scale;
        }

        public void FitToView(IEnumerable<Point2D> points, double canvasWidth, double canvasHeight, double margin = 40)
        {
            double minX = double.MaxValue, maxX = double.MinValue;
            double minY = double.MaxValue, maxY = double.MinValue;
            bool hasPoints = false;

            foreach (var p in points)
            {
                minX = Math.Min(minX, p.X);
                maxX = Math.Max(maxX, p.X);
                minY = Math.Min(minY, p.Y);
                maxY = Math.Max(maxY, p.Y);
                hasPoints = true;
            }

            if (!hasPoints)
            {
                CenterOrigin(canvasWidth, canvasHeight);
                Scale = 20;
                return;
            }

            double w = maxX - minX;
            double h = maxY - minY;

            if (w < 0.0001) w = 2;
            if (h < 0.0001) h = 2;

            double scaleX = (canvasWidth - 2 * margin) / w;
            double scaleY = (canvasHeight - 2 * margin) / h;

            Scale = Math.Min(scaleX, scaleY);
            if (Scale <= 0) Scale = 20;

            double centerX = (minX + maxX) / 2.0;
            double centerY = (minY + maxY) / 2.0;

            OffsetX = (canvasWidth / 2.0) - centerX * Scale;
            OffsetY = (canvasHeight / 2.0) + centerY * Scale;
        }
    }
}