using System;
using System.Windows;
using System.Windows.Media;

namespace Lab1.Core.Geometry
{
    public class Arc2D : GeometryObject
    {
        public Point2D Start { get; set; }

        public Point2D End { get; set; }

        public Point2D Center { get; set; }

        public double RadiusX { get; set; }

        public double RadiusY { get; set; }

        public double Rotation { get; set; }

        public bool Clockwise { get; set; }

        public bool IsFull
        {
            get
            {
                return IsFullCircle;
            }
        }

        public bool IsFullCircle { get; set; }

        public Arc2D(
            Point2D start,
            Point2D end,
            Point2D center,
            double radiusX,
            double radiusY,
            double rotation,
            bool clockwise)
        {
            Start = start;
            End = end;

            Center = center;

            RadiusX = radiusX;
            RadiusY = radiusY;

            Rotation = rotation;

            Clockwise = clockwise;

            IsFullCircle = false;
        }

        // Коло
        public Arc2D(
            Point2D center,
            double radius)
        {
            Center = center;

            RadiusX = radius;
            RadiusY = radius;

            Rotation = 0;

            Start =
                new Point2D(
                    center.X + radius,
                    center.Y
                );

            End =
                new Point2D(
                    center.X + radius,
                    center.Y
                );

            Clockwise = true;

            IsFullCircle = true;
        }

        // Повний елліпс
        public Arc2D(
            Point2D center,
            double radiusX,
            double radiusY)
        {
            Center = center;

            RadiusX = radiusX;
            RadiusY = radiusY;

            Rotation = 0;

            Start =
                new Point2D(
                    center.X + radiusX,
                    center.Y
                );

            End =
                new Point2D(
                    center.X + radiusX,
                    center.Y
                );

            Clockwise = true;

            IsFullCircle = true;
        }

        public override Point2D SetParameter(
            string name,
            double value)
        {
            if (name == "RadiusX")
            {
                return SetRadiusX(value);
            }

            if (name == "RadiusY")
            {
                return SetRadiusY(value);
            }

            return null;
        }

        private Point2D SetRadiusX(
            double radius)
        {
            if (radius <= 0)
            {
                return null;
            }

            RadiusX = radius;

            return Start;
        }

        private Point2D SetRadiusY(
            double radius)
        {
            if (radius <= 0)
            {
                return null;
            }

            RadiusY = radius;

            return Start;
        }

        public override bool ContainsPoint(
            Point2D point)
        {
            return Start == point ||
                   End == point ||
                   Center == point;
        }

        public override void OnPointChanged(
            Point2D point)
        {
        }

    }
}