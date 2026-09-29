using System;
using System.Collections.Generic;
using System.Windows;

namespace Lab2.Core.Geometry
{
    public class Strophoid2D : GeometryObject
    {
        public double A { get; set; }
        public double TMax { get; set; }
        public int Steps { get; set; }

        public double OffsetX { get; set; } = 0.0;
        public double OffsetY { get; set; } = 0.0;
        public double RotationAngleDegrees { get; set; } = 0.0;

        public Strophoid2D(double a = 50.0, double tMax = 5.0, int steps = 300)
        {
            A = a;
            TMax = tMax;
            Steps = steps;
        }

        #region Mathematics of coordinate transformations

        public Point TransformPoint(Point localPoint)
        {
            double angleRad = RotationAngleDegrees * Math.PI / 180.0;
            double cos = Math.Cos(angleRad);
            double sin = Math.Sin(angleRad);

            double rx = localPoint.X * cos - localPoint.Y * sin;
            double ry = localPoint.X * sin + localPoint.Y * cos;

            return new Point(rx + OffsetX, ry + OffsetY);
        }
        private Point GetLocalPointAt(double t)
        {
            double denom = t * t + 1;
            if (Math.Abs(denom) < 1e-9) return new Point(0, 0);

            double x = A * (t * t - 1) / denom;
            double y = A * t * (t * t - 1) / denom;
            return new Point(x, y);
        }

        #endregion

        #region Euclidean transformations (methods of state change)
        public void Translate(double dx, double dy)
        {
            OffsetX += dx;
            OffsetY += dy;
        }

        public void Rotate(double centerX, double centerY, double angleDegrees)
        {
            double angleRad = angleDegrees * Math.PI / 180.0;
            double cos = Math.Cos(angleRad);
            double sin = Math.Sin(angleRad);

            double dx = OffsetX - centerX;
            double dy = OffsetY - centerY;

            OffsetX = centerX + dx * cos - dy * sin;
            OffsetY = centerY + dx * sin + dy * cos;

            RotationAngleDegrees += angleDegrees;

        }

        #endregion

        #region Geometric calculations (taking into account transformation)

        public Point GetPointAt(double t)
        {
            Point localP = GetLocalPointAt(t);
            return TransformPoint(localP);
        }

        public Vector GetTangentAt(double t)
        {
            double t2 = t * t;
            double denom = (t2 + 1) * (t2 + 1);
            if (Math.Abs(denom) < 1e-9) return new Vector(1, 0);

            double dx = A * (4 * t) / denom;
            double dy = A * (t2 * t2 + 4 * t2 - 1) / denom; 

            double angleRad = RotationAngleDegrees * Math.PI / 180.0;
            double cos = Math.Cos(angleRad);
            double sin = Math.Sin(angleRad);

            double tx = dx * cos - dy * sin;
            double ty = dx * sin + dy * cos;

            Vector v = new Vector(tx, ty);
            if (v.Length > 1e-9) v.Normalize();
            return v;
        }

        public Vector GetNormalAt(double t)
        {
            Vector tan = GetTangentAt(t);
            return new Vector(-tan.Y, tan.X);
        }
        public (Point p1, Point p2) GetAsymptotePoints(double height = 2000.0)
        {
            Point p1Local = new Point(A, -height);
            Point p2Local = new Point(A, height);

            return (TransformPoint(p1Local), TransformPoint(p2Local));
        }

        public List<Point> GetPoints()
        {
            List<Point> points = new List<Point>();
            if (Steps <= 0) return points;

            double dt = (2 * TMax) / Steps;
            for (int i = 0; i <= Steps; i++)
            {
                double t = -TMax + i * dt;
                points.Add(GetPointAt(t));
            }
            return points;
        }

        #endregion

        #region Analytical characteristics (independent of the Euclidean transformation)

        public (double dx, double dy) GetDerivatives(double t)
        {
            double t2 = t * t;
            double denom = (t2 + 1) * (t2 + 1);
            if (Math.Abs(denom) < 1e-9) return (0, 0);

            double dx = A * (4.0 * t) / denom;
            double dy = A * (t2 * t2 + 4.0 * t2 - 1.0) / denom; 

            return (dx, dy);
        }

        public (double ddx, double ddy) GetSecondDerivatives(double t)
        {
            double t2 = t * t;
            double denom = Math.Pow(t2 + 1, 3);
            if (Math.Abs(denom) < 1e-9) return (0, 0);

            double ddx = 4.0 * A * (1.0 - 3.0 * t2) / denom;
            double ddy = 2.0 * A * t * (5.0 - t2) / denom; 

            return (ddx, ddy);
        }

        public double GetRadiusOfCurvatureAt(double t)
        {
            var (dx, dy) = GetDerivatives(t);
            var (ddx, ddy) = GetSecondDerivatives(t);

            double speedSq = dx * dx + dy * dy;
            double num = Math.Pow(speedSq, 1.5);
            double denom = Math.Abs(dx * ddy - dy * ddx);

            if (Math.Abs(denom) < 1e-9) return double.PositiveInfinity;
            return num / denom;
        }

        public double GetLoopLength()
        {
            return A * (Math.Sqrt(2.0) + Math.Log(1.0 + Math.Sqrt(2.0)));
        }

        public double GetArcLength(double t1, double t2, int n = 200)
        {

            double dt = (t2 - t1) / n;
            double length = 0;

            for (int i = 0; i < n; i++)
            {
                double tA = t1 + i * dt;
                double tB = tA + dt;

                var (dxA, dyA) = GetDerivatives(tA);
                var (dxB, dyB) = GetDerivatives(tB);

                double vA = Math.Sqrt(dxA * dxA + dyA * dyA);
                double vB = Math.Sqrt(dxB * dxB + dyB * dyB);

                length += (vA + vB) * 0.5 * dt;
            }

            return length;
        }

        public double GetLoopArea()
        {
            return A * A * (2.0 - Math.PI / 2.0); 
        }

        public double GetAsymptoteArea()
        {
            return A * A * (2.0 + Math.PI / 2.0); 
        }

        #endregion
    }
}