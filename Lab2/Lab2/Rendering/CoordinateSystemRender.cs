using Lab2.Core;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Lab2.Rendering
{
    public class CoordinateSystemRenderer
    {
        private readonly Transform2D transform;

        public CoordinateSystemRenderer(Transform2D transform)
        {
            this.transform = transform;
        }


        public void Draw(DrawingContext dc, double width, double height)
        {
            DrawGrid(dc, width, height);
            DrawAxes(dc, width, height);
        }

        private void DrawGrid(DrawingContext dc, double width, double height)
        {
            double scale = transform.Scale;

            double minX = (0 - transform.OffsetX) / scale;
            double maxX = (width - transform.OffsetX) / scale;

            double minY = (transform.OffsetY - height) / scale;
            double maxY = transform.OffsetY / scale;

            Pen gridPen = new Pen(
                new SolidColorBrush(Color.FromRgb(225, 225, 225)),
                1
            );

            int startX = (int)Math.Floor(minX);
            int endX = (int)Math.Ceiling(maxX);

            for (int x = startX; x <= endX; x++)
            {

                Point p1 = transform.ToScreen(x, minY);
                Point p2 = transform.ToScreen(x, maxY);
                dc.DrawLine(gridPen, p1, p2);

            }

            int startY = (int)Math.Floor(minY);
            int endY = (int)Math.Ceiling(maxY);

            for(int y = startY; y <= endY; y++)

            {

                Point p1 = transform.ToScreen(minX, y);
                Point p2 = transform.ToScreen(maxX, y);
                dc.DrawLine(gridPen, p1, p2);

            }
        }

        private void DrawAxes(
            DrawingContext dc, double width, double height)
        {

            Point origin = transform.ToScreen(0, 0);
            double scale = transform.Scale;

            double minX = (0 - transform.OffsetX) / scale;
            double maxX = (width - transform.OffsetX) / scale;

            double minY = (transform.OffsetY - height) / scale;
            double maxY = transform.OffsetY / scale;

            Pen axisPen = new Pen(Brushes.Black, 2);


            dc.DrawLine(axisPen, new Point(0, origin.Y), new Point(width, origin.Y));


            dc.DrawLine(axisPen, new Point(origin.X, 0), new Point(origin.X, height));

            DrawArrows(dc, width, height, origin);
            DrawLabels(dc, width, height);
        }

        private void DrawArrows( DrawingContext dc, double width, double height, Point origin)
        {
            double size = 8;
            Pen pen = new Pen(Brushes.Black, 2);

            Point xArrow = new Point(width, origin.Y);

            dc.DrawLine(pen, xArrow, new Point(xArrow.X - size, xArrow.Y - size / 2));   
            dc.DrawLine(pen, xArrow, new Point(xArrow.X - size, xArrow.Y + size / 2));

            Point yArrow = new Point(origin.X, 0);

            dc.DrawLine(pen, yArrow, new Point(yArrow.X - size / 2, yArrow.Y + size));
            dc.DrawLine(pen, yArrow, new Point(yArrow.X + size / 2, yArrow.Y + size));
        }

        private void DrawLabels(DrawingContext dc, double width, double height)
        {
            Point origin = transform.ToScreen(0, 0);
            double scale = transform.Scale;

            int minX = (int)Math.Floor((0 - transform.OffsetX) / scale);
            int maxX = (int)Math.Ceiling((width - transform.OffsetX) / scale);

            int minY = (int)Math.Floor((transform.OffsetY - height) / scale);
            int maxY = (int)Math.Ceiling(transform.OffsetY / scale);

            for (int x = minX; x <= maxX; x++)

            { 
                if (x == 0)
                  continue;
                
                Point position = transform.ToScreen(x, 0);
                DrawText(dc, x.ToString(), position.X - 5, position.Y + 5);

            }

            for (int y = minY; y <= maxY; y++)

            {
                if (y == 0)
                  continue;
                
                Point position = transform.ToScreen(0, y);
                DrawText(dc, y.ToString(), position.X + 5, position.Y - 10);

            }

            DrawText(dc, "X", origin.X - 15, origin.Y + 10);
            DrawText(dc, "Y", origin.X + 10, origin.Y + 5);
            DrawText(dc, "0", origin.X + 5, origin.Y + 5);
        }

        private void DrawText(
            DrawingContext dc,
            string text,
            double x,
            double y)
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