using Lab2.Core;
using Lab2.Core.Geometry;
using System.Collections.Generic;
using System.Data;

namespace Lab2.Core.Geometry
{
    public class Figure
    {
        public List<Point2D> Points { get; private set; }

        public List<GeometryObject> GeometryObjects
        {
            get;
            private set;
        }

        public Figure()
        {
            Points =
                new List<Point2D>();

            GeometryObjects =
                new List<GeometryObject>();

        }

        public void AddPoint(Point2D point)
        {
            Points.Add(point);
        }

        public void AddObject(
        GeometryObject geometryObject)
        {
            GeometryObjects.Add(
                geometryObject
            );

        }

    }
}