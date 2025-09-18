using System;
using GeoJSON.Net.Geometry;

namespace SpatialAnalysisMiniTool
{
    public class Line
    {
        public Point FromPoint { get; set; }
        public Point ToPoint { get; set; }

        public Line(IPosition from, IPosition to)
        {
            FromPoint = new Point(from);
            ToPoint = new Point(to);
        }

        /// <summary>
        /// Checks if this line intersects with another line.
        /// The algorithm is based on the one described at:
        /// https://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/
        /// </summary>
        public bool Intersects(Line other)
        {
            Point p1 = FromPoint;
            Point p2 = ToPoint;
            Point p3 = other.FromPoint;
            Point p4 = other.ToPoint;

            int o1 = Orientation(p1, p2, p3);
            int o2 = Orientation(p1, p2, p4);
            int o3 = Orientation(p3, p4, p1);
            int o4 = Orientation(p3, p4, p2);

            // General case
            if (o1 != o2 && o3 != o4)
                return true;

            // Special Cases
            if (o1 == 0 && OnSegment(p1, p3, p2)) return true;
            if (o2 == 0 && OnSegment(p1, p4, p2)) return true;
            if (o3 == 0 && OnSegment(p3, p1, p4)) return true;
            if (o4 == 0 && OnSegment(p3, p2, p4)) return true;

            return false;
        }

        private bool OnSegment(Point p, Point q, Point r)
        {
            double qx = q.Coordinates.Latitude;
            double qy = q.Coordinates.Longitude;
            double px = p.Coordinates.Latitude;
            double py = p.Coordinates.Longitude;
            double rx = r.Coordinates.Latitude;
            double ry = r.Coordinates.Longitude;

            return (qx <= Math.Max(px, rx) && qx >= Math.Min(px, rx) &&
                    qy <= Math.Max(py, ry) && qy >= Math.Min(py, ry));
        }

        /// <summary>
        /// Checks the orientation of the ordered triplet (p, q, r).
        /// </summary>
        /// <returns> 0 if collinear, 1 if clockwise, 2 if counterclockwise </returns>
        private int Orientation(Point p, Point q, Point r)
        {
            double px = p.Coordinates.Latitude;
            double py = p.Coordinates.Longitude;
            double qx = q.Coordinates.Latitude;
            double qy = q.Coordinates.Longitude;
            double rx = r.Coordinates.Latitude;
            double ry = r.Coordinates.Longitude;

            // There is a known edge case where if q has either lat or lon equal to those of r,
            // the function can return false collinearity. For production grade software this could be solved.
            // For the purposes of this mini-tool, this is sufficient.
            double val = (qy - py) * (rx - qx) - (qx - px) * (ry - qy);

            if (Math.Abs(val) < 1e-10) return 0;

            return (val > 0) ? 1 : 2;
        }

        public bool IsPointOnLine(Point point)
        {
            if (Orientation(FromPoint, ToPoint, point) != 0)
                return false;

            double px = point.Coordinates.Latitude;
            double py = point.Coordinates.Longitude;
            double x1 = FromPoint.Coordinates.Latitude;
            double y1 = FromPoint.Coordinates.Longitude;
            double x2 = ToPoint.Coordinates.Latitude;
            double y2 = ToPoint.Coordinates.Longitude;

            return (px <= Math.Max(x1, x2) && px >= Math.Min(x1, x2) &&
                    py <= Math.Max(y1, y2) && py >= Math.Min(y1, y2));
        }
    }
}