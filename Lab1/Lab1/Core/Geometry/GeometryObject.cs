using Lab1.Core.Geometry;

namespace Lab1.Core
{
    public abstract class GeometryObject
    {
        public string Name { get; set; }

        public abstract Point2D SetParameter(
            string name,
            double value);

        public abstract bool ContainsPoint(
            Point2D point);

        public abstract void OnPointChanged(
            Point2D point);
    }

}

