using System;
using System.Windows;
using System.Windows.Media;
using Lab1.Core;
using Lab1.Core.Geometry;

namespace Lab1.Rendering
{
    public class LineRenderer
    {
        public void DrawAffine(
            DrawingContext dc,
            Line2D line,
            Transform2D transform,
            double a00, double a01, double a02,
            double a10, double a11, double a12)
        {
            Point start = ProjectPoint(line.Start, transform, a00, a01, a02, a10, a11, a12, 0, 0, 1, isProjective: false);
            Point end = ProjectPoint(line.End, transform, a00, a01, a02, a10, a11, a12, 0, 0, 1, isProjective: false);

            Pen pen = new Pen(Brushes.Blue, 2);
            dc.DrawLine(pen, start, end);
        }

        public void DrawProjective(
            DrawingContext dc,
            Line2D line,
            Transform2D transform,
            double h00, double h01, double h02,
            double h10, double h11, double h12,
            double h20, double h21, double h22)
        {
            Point start = ProjectPoint(line.Start, transform, h00, h01, h02, h10, h11, h12, h20, h21, h22, isProjective: true);
            Point end = ProjectPoint(line.End, transform, h00, h01, h02, h10, h11, h12, h20, h21, h22, isProjective: true);

            Pen pen = new Pen(Brushes.Blue, 2);
            dc.DrawLine(pen, start, end);
        }

        private Point ProjectPoint(
            Point2D pt,
            Transform2D transform,
            double m00, double m01, double m02,
            double m10, double m11, double m12,
            double m20, double m21, double m22,
            bool isProjective)
        {
            double px, py;

            if (isProjective)
            {
                double w = m20 * pt.X + m21 * pt.Y + m22;
                if (Math.Abs(w) < 1e-9) w = (w < 0 ? -1e-9 : 1e-9);

                px = (m00 * m20 * pt.X + m01 * m21 * pt.Y + m22 * m02) / w;
                py = (m10 * m20 * pt.X + m11 * m21 * pt.Y + m22 * m12) / w;
            }
            else
            {
                px = m00 * pt.X + m01 * pt.Y + m02;
                py = m10 * pt.X + m11 * pt.Y + m12;
            }

            return transform.ToScreen(px, py);
        }
    }
}