using Lab1.Core.Constraints;
using Lab1.Core.Geometry;
using Lab1.Core.Solver;
using System.Collections.Generic;

namespace Lab1.Core
{
    public class Figure
    {
        public List<Point2D> Points { get; private set; }

        public List<GeometryObject> GeometryObjects
        {
            get;
            private set;
        }

        public List<Constraint> Constraints
        {
            get;
            private set;
        }

        public GeometrySolver Solver { get; private set; }

        private Dictionary<Point2D, Point2D> originalPoints;
        private Dictionary<Arc2D, (double Rx, double Ry)> originalRadii;

        public Figure()
        {
            Points =
                new List<Point2D>();

            GeometryObjects =
                new List<GeometryObject>();

            Constraints =
                new List<Constraint>();

            Solver =
                new GeometrySolver();

            originalPoints =
                new Dictionary<Point2D, Point2D>();

            originalRadii = new Dictionary<Arc2D, (double Rx, double Ry)>();

        }

        public void AddPoint(Point2D point)
        {
            Points.Add(point);

            originalPoints[point] =
                new Point2D(
                    point.X,
                    point.Y
                );
        }

        public void AddObject(
        GeometryObject geometryObject)
            {
                GeometryObjects.Add(
                    geometryObject
                );

            if (geometryObject is Arc2D arc)
            {
                originalRadii[arc] = (arc.RadiusX, arc.RadiusY);
            }
        }

        public void AddConstraint(
            Constraint constraint)
        {
            Constraints.Add(constraint);

            Solver.AddConstraint(
                constraint
            );
        }

        public void Solve()
        {
            Solver.Solve();
        }

        private void NotifyPointChanged(
        Point2D point,
        GeometryObject source)
            {
                foreach (
                    GeometryObject geometryObject
                    in GeometryObjects)
                {
                    if (geometryObject == source)
                    {
                        continue;
                    }

                    if (geometryObject.ContainsPoint(point))
                    {
                        geometryObject.OnPointChanged(point);
                    }
                }
        }
        public void SetParameter(
        GeometryObject geometryObject,
        string parameter,
        double value)
            {
                Point2D changedPoint =
                    geometryObject.SetParameter(
                        parameter,
                        value
                    );

                if (changedPoint != null)
                {
                    NotifyPointChanged(
                        changedPoint,
                        geometryObject
                    );
                }

            Solve();
        }

        public void Reset()
        {
            foreach (var item in originalRadii)
            {
                item.Key.RadiusX = item.Value.Rx;
                item.Key.RadiusY = item.Value.Ry;
            }

            foreach (Point2D point in Points)
            {
                if (!originalPoints.TryGetValue(point, out Point2D originalPoint))
                {
                    continue;
                }

                point.X = originalPoint.X;
                point.Y = originalPoint.Y;
            }

            Solve();
        }
    }
}