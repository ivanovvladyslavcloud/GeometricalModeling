using System;

namespace Lab1.Core.Geometry
{
    public class Line2D : GeometryObject
    {
        public Point2D Start { get; set; }

        public Point2D End { get; set; }

        public Line2D(
            Point2D start,
            Point2D end)
        {
            Start = start;
            End = end;
        }

        public double Length
        {
            get
            {
                double dx =
                    End.X - Start.X;

                double dy =
                    End.Y - Start.Y;

                return Math.Sqrt(
                    dx * dx +
                    dy * dy
                );
            }
        }

        public void TrimStart(
            Point2D point)
        {
            Start = point;
        }

        public void TrimEnd(
            Point2D point)
        {
            End = point;
        }

        public Point2D SetLength(double length)
        {
            double dx = End.X - Start.X;
            double dy = End.Y - Start.Y;

            double currentLength =
                Math.Sqrt(dx * dx + dy * dy);

            if (currentLength < 0.000001)
            {
                return null;
            }

            double scale =
                length / currentLength;

            End.X =
                Start.X + dx * scale;

            End.Y =
                Start.Y + dy * scale;

            return End;
        }

        public override Point2D SetParameter(
        string name,
        double value)
            {
                if (name == "Length")
                {
                    return SetLength(value);
                }

                return null;
            }

        public override bool ContainsPoint(Point2D point)
        {
            return Start == point || End == point;
        }

        public override void OnPointChanged(Point2D point)
        {
            
        }
    }
}