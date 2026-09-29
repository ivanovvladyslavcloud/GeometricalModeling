using System;
using System.Windows;
using System.Windows.Media;
using Lab1.Core;
using Lab1.Core.Geometry;

namespace Lab1.Rendering
{
    public class ArcRenderer
    {
        public void DrawAffine(
            DrawingContext dc,
            Arc2D arc,
            Transform2D transform,
            double a00, double a01, double a02,
            double a10, double a11, double a12,
            int segmentsCount = 72)
        {
            RenderArc(dc, arc, transform, a00, a01, a02, a10, a11, a12, 0, 0, 1, isProjective: false, segmentsCount);
        }

        public void DrawProjective(
            DrawingContext dc,
            Arc2D arc,
            Transform2D transform,
            double h00, double h01, double h02,
            double h10, double h11, double h12,
            double h20, double h21, double h22,
            int segmentsCount = 72)
        {
            RenderArc(dc, arc, transform, h00, h01, h02, h10, h11, h12, h20, h21, h22, isProjective: true, segmentsCount);
        }

        private void RenderArc(
            DrawingContext dc,
            Arc2D arc,
            Transform2D transform,
            double m00, double m01, double m02,
            double m10, double m11, double m12,
            double m20, double m21, double m22,
            bool isProjective,
            int segmentsCount)
        {
            if (arc == null) return;

            double startAngle = 0;
            double sweepAngle = 2.0 * Math.PI;

            if (!arc.IsFullCircle && arc.Start != null && arc.End != null)
            {
                startAngle = GetAngleFromCenter(arc, arc.Start);
                double endAngle = GetAngleFromCenter(arc, arc.End);

                sweepAngle = endAngle - startAngle;

                if (arc.Clockwise && sweepAngle < 0)
                {
                    sweepAngle += 2.0 * Math.PI;
                }
                else if (!arc.Clockwise && sweepAngle > 0)
                {
                    sweepAngle -= 2.0 * Math.PI;
                }
            }

            double radRotation = arc.Rotation;
            double cosRot = Math.Cos(radRotation);
            double sinRot = Math.Sin(radRotation);

            StreamGeometry geometry = new StreamGeometry();

            using (StreamGeometryContext ctx = geometry.Open())
            {
                bool isFirst = true;

                for (int i = 0; i <= segmentsCount; i++)
                {
                    double progress = (double)i / segmentsCount;
                    double t = startAngle + sweepAngle * progress;

                    double localX = arc.RadiusX * Math.Cos(t);
                    double localY = arc.RadiusY * Math.Sin(t);

                    double worldX = arc.Center.X + (localX * cosRot - localY * sinRot);
                    double worldY = arc.Center.Y + (localX * sinRot + localY * cosRot);

                    double transformedX;
                    double transformedY;

                    if (isProjective)
                    {

                        double w = m20 * worldX + m21 * worldY + m22;
                        if (Math.Abs(w) < 1e-9) w = (w < 0 ? -1e-9 : 1e-9);

                        transformedX = (m00 * m20 * worldX + m01 * m21 * worldY + m02 * m22) / w;
                        transformedY = (m10 * m20 * worldX + m11 * m21 * worldY + m12 * m22) / w;
                    }
                    else
                    {
                        transformedX = m00 * worldX + m01 * worldY + m02;
                        transformedY = m10 * worldX + m11 * worldY + m12;
                    }

                    Point screenPt = transform.ToScreen(transformedX, transformedY);

                    if (isFirst)
                    {
                        ctx.BeginFigure(screenPt, false, arc.IsFullCircle);
                        isFirst = false;
                    }
                    else
                    {
                        ctx.LineTo(screenPt, true, false);
                    }
                }
            }

            geometry.Freeze();

            Pen pen = new Pen(Brushes.Red, 2);
            dc.DrawGeometry(null, pen, geometry);
        }

        private double GetAngleFromCenter(Arc2D arc, Point2D point)
        {
            double dx = point.X - arc.Center.X;
            double dy = point.Y - arc.Center.Y;

            if (Math.Abs(arc.Rotation) > 1e-6)
            {
                double rad = -arc.Rotation;
                double rx = dx * Math.Cos(rad) - dy * Math.Sin(rad);
                double ry = dx * Math.Sin(rad) + dy * Math.Cos(rad);
                dx = rx;
                dy = ry;
            }

            return Math.Atan2(dy / arc.RadiusY, dx / arc.RadiusX);
        }
    }
}