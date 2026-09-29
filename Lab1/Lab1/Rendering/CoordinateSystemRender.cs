using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Lab1.Core;

namespace Lab1.Rendering
{
    public class CoordinateSystemRenderer
    {
        private readonly Transform2D transform;

        public CoordinateSystemRenderer(Transform2D transform)
        {
            this.transform = transform;
        }

        public void DrawAffine(
            DrawingContext dc,
            double width,
            double height,
            double a00, double a01, double a02,
            double a10, double a11, double a12)
        {
            DrawGrid(dc, width, height, a00, a01, a02, a10, a11, a12, 0, 0, 1, isProjective: false);
            DrawAxes(dc, width, height, a00, a01, a02, a10, a11, a12, 0, 0, 1, isProjective: false);
        }

        public void DrawProjective(
            DrawingContext dc,
            double width,
            double height,
            double h00, double h01, double h02,
            double h10, double h11, double h12,
            double h20, double h21, double h22)
        {
            DrawGrid(dc, width, height, h00, h01, h02, h10, h11, h12, h20, h21, h22, isProjective: true);
            DrawAxes(dc, width, height, h00, h01, h02, h10, h11, h12, h20, h21, h22, isProjective: true);
        }

        private void DrawGrid(
            DrawingContext dc,
            double width,
            double height,
            double m00, double m01, double m02,
            double m10, double m11, double m12,
            double m20, double m21, double m22,
            bool isProjective)
        {
            double scale = transform.Scale;

            double minX = (0 - transform.OffsetX) / scale;
            double maxX = (width - transform.OffsetX) / scale;

            double minY = (transform.OffsetY - height) / scale;
            double maxY = transform.OffsetY / scale;

            Pen gridPen = new Pen(new SolidColorBrush(Color.FromRgb(225, 225, 225)), 1);

            int startX = (int)Math.Floor(minX);
            int endX = (int)Math.Ceiling(maxX);

            for (int x = startX; x <= endX; x++)
            {
                Point p1 = ProjectPoint(x, minY, m00, m01, m02, m10, m11, m12, m20, m21, m22, isProjective);
                Point p2 = ProjectPoint(x, maxY, m00, m01, m02, m10, m11, m12, m20, m21, m22, isProjective);

                dc.DrawLine(gridPen, p1, p2);
            }

            int startY = (int)Math.Floor(minY);
            int endY = (int)Math.Ceiling(maxY);

            for (int y = startY; y <= endY; y++)
            {
                Point p1 = ProjectPoint(minX, y, m00, m01, m02, m10, m11, m12, m20, m21, m22, isProjective);
                Point p2 = ProjectPoint(maxX, y, m00, m01, m02, m10, m11, m12, m20, m21, m22, isProjective);

                dc.DrawLine(gridPen, p1, p2);
            }
        }

        private void DrawAxes(
            DrawingContext dc,
            double width,
            double height,
            double m00, double m01, double m02,
            double m10, double m11, double m12,
            double m20, double m21, double m22,
            bool isProjective)
        {
            double scale = transform.Scale;

            double minX = (0 - transform.OffsetX) / scale;
            double maxX = (width - transform.OffsetX) / scale;

            double minY = (transform.OffsetY - height) / scale;
            double maxY = transform.OffsetY / scale;

            Pen axisPen = new Pen(Brushes.Black, 2);

            Point xStart = ProjectPoint(minX, 0, m00, m01, m02, m10, m11, m12, m20, m21, m22, isProjective);
            Point xEnd = ProjectPoint(maxX, 0, m00, m01, m02, m10, m11, m12, m20, m21, m22, isProjective);
            dc.DrawLine(axisPen, xStart, xEnd);

            Point yStart = ProjectPoint(0, minY, m00, m01, m02, m10, m11, m12, m20, m21, m22, isProjective);
            Point yEnd = ProjectPoint(0, maxY, m00, m01, m02, m10, m11, m12, m20, m21, m22, isProjective);
            dc.DrawLine(axisPen, yStart, yEnd);

            DrawArrows(dc, xStart, xEnd, yStart, yEnd);
            DrawLabels(dc, width, height, m00, m01, m02, m10, m11, m12, m20, m21, m22, isProjective);
        }

        private void DrawArrows(DrawingContext dc, Point xStart, Point xEnd, Point yStart, Point yEnd)
        {
            double size = 8;
            Pen pen = new Pen(Brushes.Black, 2);

            Vector xDir = xEnd - xStart;
            if (xDir.Length > 1e-5)
            {
                xDir.Normalize();
                Vector xNormal = new Vector(-xDir.Y, xDir.X);
                Point p1 = xEnd - xDir * size + xNormal * (size / 2);
                Point p2 = xEnd - xDir * size - xNormal * (size / 2);
                dc.DrawLine(pen, xEnd, p1);
                dc.DrawLine(pen, xEnd, p2);
            }

            Vector yDir = yEnd - yStart;
            if (yDir.Length > 1e-5)
            {
                yDir.Normalize();
                Vector yNormal = new Vector(-yDir.Y, yDir.X);
                Point p1 = yEnd - yDir * size + yNormal * (size / 2);
                Point p2 = yEnd - yDir * size - yNormal * (size / 2);
                dc.DrawLine(pen, yEnd, p1);
                dc.DrawLine(pen, yEnd, p2);
            }
        }

        private void DrawLabels(
            DrawingContext dc,
            double width,
            double height,
            double m00, double m01, double m02,
            double m10, double m11, double m12,
            double m20, double m21, double m22,
            bool isProjective)
        {
            Point origin = ProjectPoint(0, 0, m00, m01, m02, m10, m11, m12, m20, m21, m22, isProjective);

            double scale = transform.Scale;

            int minX = (int)Math.Floor((0 - transform.OffsetX) / scale);
            int maxX = (int)Math.Ceiling((width - transform.OffsetX) / scale);

            int minY = (int)Math.Floor((transform.OffsetY - height) / scale);
            int maxY = (int)Math.Ceiling(transform.OffsetY / scale);

            for (int x = minX; x <= maxX; x++)
            {
                if (x == 0) continue;
                Point position = ProjectPoint(x, 0, m00, m01, m02, m10, m11, m12, m20, m21, m22, isProjective);
                DrawText(dc, x.ToString(), position.X - 5, position.Y + 5);
            }

            for (int y = minY; y <= maxY; y++)
            {
                if (y == 0) continue;
                Point position = ProjectPoint(0, y, m00, m01, m02, m10, m11, m12, m20, m21, m22, isProjective);
                DrawText(dc, y.ToString(), position.X + 5, position.Y - 10);
            }

            Point xEndPos = ProjectPoint(maxX, 0, m00, m01, m02, m10, m11, m12, m20, m21, m22, isProjective);
            Point yEndPos = ProjectPoint(0, maxY, m00, m01, m02, m10, m11, m12, m20, m21, m22, isProjective);

            DrawText(dc, "X", xEndPos.X - 15, xEndPos.Y + 10);
            DrawText(dc, "Y", yEndPos.X + 10, yEndPos.Y + 5);
            DrawText(dc, "0", origin.X + 5, origin.Y + 5);
        }

        private Point ProjectPoint(
            double x, double y,
            double m00, double m01, double m02,
            double m10, double m11, double m12,
            double m20, double m21, double m22,
            bool isProjective)
        {
            double projX, projY;

            if (isProjective)
            {
                double w = m20 * x + m21 * y + m22;
                if (Math.Abs(w) < 1e-9) w = (w < 0 ? -1e-9 : 1e-9);

                projX = (m00 * m20 * x + m01 * m21 * y + m02 * m22) / w;
                projY = (m10 * m20 * x + m11 * m21 * y + m12 * m22) / w;
            }
            else
            {
                projX = m00 * x + m01 * y + m02;
                projY = m10 * x + m11 * y + m12;
            }

            return transform.ToScreen(projX, projY);
        }

        private void DrawText(DrawingContext dc, string text, double x, double y)
        {
            FormattedText formattedText = new FormattedText(
                text,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI"),
                12,
                Brushes.Black,
                1.0
            );

            dc.DrawText(formattedText, new Point(x, y));
        }
    }
}