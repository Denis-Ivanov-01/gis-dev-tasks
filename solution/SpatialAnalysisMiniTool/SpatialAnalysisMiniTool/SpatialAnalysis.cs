using System.Diagnostics.Metrics;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;

namespace SpatialAnalysisMiniTool
{
    internal struct CsvRow
    {
        public string PolygonId { get; set; }
        public int PointCount { get; set; }
        public CsvRow(string polygonId, int pointCount)
        {
            PolygonId = polygonId;
            PointCount = pointCount;
        }
    }

    internal class SpatialAnalysis
    {
        public SpatialAnalysis(FeatureCollection polygons, FeatureCollection points)
        {

        }

        public List<CsvRow> Analyze(FeatureCollection polygons, FeatureCollection points)
        {
            List<CsvRow> csvRows = new();
            foreach(Feature polygonFeature in polygons.Features)
            {
                Polygon poly = (polygonFeature.Geometry as Polygon)!;
                string polyId = polygonFeature.Properties["PolygonId"].ToString()!;
                csvRows.Add(new CsvRow(polyId, IntersectedPointsCount(poly, points)));
                ConsoleWriter.ProcessedPolygonMessage(polyId);
            }
            return csvRows;
        }

        private int IntersectedPointsCount(Polygon polygon, FeatureCollection points)
        {
            int counter = 0;
            foreach(Feature pointFeature in points.Features)
            {
                Point p = (pointFeature.Geometry as Point)!; 
                if (polygon.Contains(p))
                {
                    counter++;
                }
            }
            return counter;
        }
    }
}
