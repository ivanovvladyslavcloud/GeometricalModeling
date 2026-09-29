using System;
using System.Collections.Generic;
using System.Windows;

namespace Lab2.Core
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

    }
}