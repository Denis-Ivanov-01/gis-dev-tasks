using SpatialAnalysisMiniTool;
using GeoJSON.Net.Feature;

internal class Program
{
    private static void Main(string[] args)
    {
        try
        {
            ConsoleWriter.EnterPointsPathMessage();
            string? pointsJsonPath = Console.ReadLine()?.Trim('\"');

            if (string.IsNullOrWhiteSpace(pointsJsonPath) || !File.Exists(pointsJsonPath))
            {
                ConsoleWriter.InvalidGeoJsonMessage();
                return;
            }

            FeatureCollection points = Parser.LoadFromFile(pointsJsonPath);

            ConsoleWriter.EnterPolygonsPathMessage();
            string? polygonsJsonPath = Console.ReadLine()?.Trim('\"');

            if (string.IsNullOrWhiteSpace(polygonsJsonPath) || !File.Exists(polygonsJsonPath))
            {
                ConsoleWriter.InvalidGeoJsonMessage();
                return;
            }

            FeatureCollection polygons = Parser.LoadFromFile(polygonsJsonPath);

            SpatialAnalysis sa = new(polygons, points);
            List<CsvRow> results = sa.Analyze(polygons, points);

            CsvGenerator generator = new(results);
            string outDirectory = Path.GetDirectoryName(pointsJsonPath)!;
            string resultPath = generator.Generate(outDirectory);

            ConsoleWriter.AnalysisCompleteMessage(resultPath);
        }
        catch (Exception ex)
        {
            ConsoleWriter.ErrorMessage(ex.Message);
        }
    }
}
