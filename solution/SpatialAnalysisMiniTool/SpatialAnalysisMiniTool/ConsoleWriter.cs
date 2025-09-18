using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialAnalysisMiniTool
{
    public static class ConsoleWriter
    {
        public static void ProcessedPolygonMessage(string polygonId)
        {
            Console.WriteLine($"Processed Polygon with ID: {polygonId}");
        }

        public static void EnterPointsPathMessage()
        {
            Console.WriteLine("Please enter the path to the Points GeoJSON file:");
        }

        public static void EnterPolygonsPathMessage()
        {
            Console.WriteLine("Please enter the path to the Polygons GeoJSON file:");
        }

        public static void InvalidGeoJsonMessage()
        {
            Console.WriteLine("The GeoJSON file is invalid!");
        }

        public static void AnalysisCompleteMessage(string resultPath)
        {
            Console.WriteLine($"Analysis complete. Results saved to:\n{resultPath}");
        }

        public static void ErrorMessage(string message)
        {
            Console.WriteLine($"Error: {message}");
        }
    }
}
