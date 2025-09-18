using System.Text;
    
namespace SpatialAnalysisMiniTool
{
    internal class CsvGenerator
    {
        private const string extension = ".csv";
        private const string nameNoExtension = "analysis_result";

        private const string CsvHeader = "PolygonId,PointCount";
        private List<CsvRow> csvRows;

        public CsvGenerator(List<CsvRow> rows)
        {
            csvRows = rows;
        }

        public string Generate(string directory)
        {
            string filePath = GetAvailableFilePath(directory);
            string csvContent = GenerateCsvContent();
            File.WriteAllText(filePath, csvContent);
            return filePath;
        }

        private string GenerateCsvContent()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(CsvHeader);
            foreach (CsvRow row in csvRows)
            {
                sb.AppendLine($"{row.PolygonId},{row.PointCount}");
            }
            return sb.ToString();
        }

        private string GetAvailableFilePath(string directory)
        {
            string filePath = Path.Combine(directory, $"{nameNoExtension}{extension}");

            int counter = 1;
            while (File.Exists(filePath))
            {
                counter++;
                filePath = Path.Combine(directory, $"{nameNoExtension}_{counter}{extension}");
            }

            return filePath;
        }
    }
}
