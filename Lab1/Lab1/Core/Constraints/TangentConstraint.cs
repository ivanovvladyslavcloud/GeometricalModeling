using System;
using Lab1.Core.Geometry;

namespace Lab1.Core.Constraints
{
    public class TangentConstraint : Constraint
    {
        private readonly Arc2D arc;
        private readonly Line2D line;
        private readonly Point2D tangentPoint;

        public TangentConstraint(
            Arc2D arc,
            Line2D line,
            Point2D tangentPoint)
        {
            this.arc = arc;
            this.line = line;
            this.tangentPoint = tangentPoint;
        }

        public override bool Solve()
        {
            if (arc == null || line == null || tangentPoint == null || arc.Center == null)
            {
                return false;
            }

            double dx = tangentPoint.X - arc.Center.X;
            double dy = tangentPoint.Y - arc.Center.Y;

            double cos = Math.Cos(-arc.Rotation);
            double sin = Math.Sin(-arc.Rotation);

            double localX = dx * cos - dy * sin;
            double localY = dx * sin + dy * cos;

            double rx2 = arc.RadiusX * arc.RadiusX;
            double ry2 = arc.RadiusY * arc.RadiusY;

            double val = Math.Sqrt((localX * localX) / rx2 + (localY * localY) / ry2);
            if (val < 0.000001) return false;

            localX /= val;
            localY /= val;

            double normX = localX / rx2;
            double normY = localY / ry2;

            double localTangX = -normY;
            double localTangY = normX;

            double cosR = Math.Cos(arc.Rotation);
            double sinR = Math.Sin(arc.Rotation);

            tangentPoint.X = arc.Center.X + (localX * cosR - localY * sinR);
            tangentPoint.Y = arc.Center.Y + (localX * sinR + localY * cosR);

            double tangentX = localTangX * cosR - localTangY * sinR;
            double tangentY = localTangX * sinR + localTangY * cosR;

            double tangLen = Math.Sqrt(tangentX * tangentX + tangentY * tangentY);
            if (tangLen < 0.000001) return false;

            tangentX /= tangLen;
            tangentY /= tangLen;

            double lineX = line.End.X - line.Start.X;
            double lineY = line.End.Y - line.Start.Y;
            double lineLength = Math.Sqrt(lineX * lineX + lineY * lineY);

            if (lineLength < 0.000001) return false;

            if (lineX * tangentX + lineY * tangentY < 0)
            {
                tangentX = -tangentX;
                tangentY = -tangentY;
            }

            if (line.End == tangentPoint)
            {
                line.Start.X = tangentPoint.X - tangentX * lineLength;
                line.Start.Y = tangentPoint.Y - tangentY * lineLength;
            }
            else if (line.Start == tangentPoint)
            {
                line.End.X = tangentPoint.X + tangentX * lineLength;
                line.End.Y = tangentPoint.Y + tangentY * lineLength;
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}