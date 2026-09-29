using System;
using System.Collections.Generic;
using Lab1.Core.Geometry;

namespace Lab1.Core.Instrsections
{
    public static class Intersection2D
    {
        public static List<Point2D> LineCircleIntersection(
            Line2D line,
            Arc2D ellipse)
        {
            List<Point2D> points = new List<Point2D>();

            double dx = line.End.X - line.Start.X;
            double dy = line.End.Y - line.Start.Y;

            double fx = line.Start.X - ellipse.Center.X;
            double fy = line.Start.Y - ellipse.Center.Y;

            if (Math.Abs(ellipse.Rotation) > 0.000001)
            {
                double cos = Math.Cos(-ellipse.Rotation);
                double sin = Math.Sin(-ellipse.Rotation);

                double rdx = dx * cos - dy * sin;
                double rdy = dx * sin + dy * cos;
                dx = rdx; dy = rdy;

                double rfx = fx * cos - fy * sin;
                double rfy = fx * sin + fy * cos;
                fx = rfx; fy = rfy;
            }

            double rx2 = ellipse.RadiusX * ellipse.RadiusX;
            double ry2 = ellipse.RadiusY * ellipse.RadiusY;

            double a = (dx * dx) / rx2 + (dy * dy) / ry2;
            double b = 2 * ((fx * dx) / rx2 + (fy * dy) / ry2);
            double c = (fx * fx) / rx2 + (fy * fy) / ry2 - 1.0;

            if (Math.Abs(a) < 0.000001)
                return points;

            double discriminant = b * b - 4 * a * c;
            const double epsilon = 0.000001;

            if (discriminant < -epsilon)
                return points;

            void AddPointFromT(double t)
            {
                if (t >= 0 && t <= 1)
                {

                    points.Add(new Point2D(
                        line.Start.X + t * (line.End.X - line.Start.X),
                        line.Start.Y + t * (line.End.Y - line.Start.Y)
                    ));
                }
            }

            if (Math.Abs(discriminant) <= epsilon)
            {
                AddPointFromT(-b / (2 * a));
            }
            else
            {
                double sqrt = Math.Sqrt(discriminant);
                AddPointFromT((-b - sqrt) / (2 * a));
                AddPointFromT((-b + sqrt) / (2 * a));
            }

            return points;
        }
        public static Point2D TrimLineByCircle(
        Line2D line,
        Arc2D circle,
        TrimSide side)
            {
            List<Point2D> intersections =
                LineCircleIntersection(
                    line,
                    circle
                );

            if (intersections.Count == 0)
            {
                return null;
            }

            Point2D intersection;

            if (side == TrimSide.Start)
            {
                intersection =
                    intersections[0];

                line.Start =
                    intersection;
            }
            else
            {
                intersection =
                    intersections[
                        intersections.Count - 1
                    ];

                line.End =
                    intersection;
            }

            return intersection;
        }


        public static Point2D LineLineIntersection(
    Line2D line1,
    Line2D line2)
        {
            double x1 = line1.Start.X;
            double y1 = line1.Start.Y;

            double x2 = line1.End.X;
            double y2 = line1.End.Y;

            double x3 = line2.Start.X;
            double y3 = line2.Start.Y;

            double x4 = line2.End.X;
            double y4 = line2.End.Y;

            double denominator =
                (x1 - x2) * (y3 - y4) -
                (y1 - y2) * (x3 - x4);

            const double epsilon = 0.000001;

            if (Math.Abs(denominator) < epsilon)
            {
                return null;
            }

            double t =
                (
                    (x1 - x3) * (y3 - y4) -
                    (y1 - y3) * (x3 - x4)
                )
                / denominator;

            double u =
                -(
                    (x1 - x2) * (y1 - y3) -
                    (y1 - y2) * (x1 - x3)
                )
                / denominator;


            if (t < 0 || t > 1 ||
                u < 0 || u > 1)
            {
                return null;
            }

            return new Point2D(
                x1 + t * (x2 - x1),
                y1 + t * (y2 - y1)
            );
        }

        public static Point2D TrimLineByLine(
         Line2D line,
         Line2D cuttingLine,
         TrimSide side)
        {
            Point2D intersection =
                LineLineIntersection(
                    line,
                    cuttingLine
                );

            if (intersection == null)
            {
                return null;
            }

            if (side == TrimSide.Start)
            {
                line.Start = intersection;
            }
            else
            {
                line.End = intersection;
            }

            return intersection;
        }
    }

}