using GeoJSON.Net.Geometry;

namespace SpatialAnalysisMiniTool
{
    public static class PolygonExtensions
    {
        public static bool Contains(this Polygon polygon, Point point)
        {
            // Create a horizontal line to the right from the point
            var lineEnd = new Position(point.Coordinates.Latitude, double.MaxValue);
            var line = new Line(point.Coordinates, lineEnd);

            int intersectionCount = 0;
            bool onBoundary = false;

            foreach (LineString ring in polygon.Coordinates)
            {
                var coords = ring.Coordinates;

                for (int i = 0; i < coords.Count - 1; i++)
                {
                    var edge = new Line(coords[i], coords[i + 1]);

                    // Check if point is exactly on the edge
                    if (edge.IsPointOnLine(point))
                    {
                        onBoundary = true;
                        break;
                    }

                    if (line.Intersects(edge))
                        intersectionCount++;
                }

                if (onBoundary) break;
            }

            // If point is on boundary, it's considered inside
            if (onBoundary)
                return true;

            // Odd number of intersections means point is inside
            return intersectionCount % 2 == 1;
        }
    }
}