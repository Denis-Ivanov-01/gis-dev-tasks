using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;

namespace SpatialAnalysisMiniTool
{
    internal class Parser
    {
        public static FeatureCollection LoadFromFile(string p)
        {
            string path = p.Trim('\"');
            if (!IsPathValid(path))
            {
                throw new ArgumentException("Invalid file path or file type. Please provide a valid .geojson file path.");
            }

            string json = File.ReadAllText(path);
            return ParseJson(json);
        }

        private static FeatureCollection ParseJson(string json)
        {
            return JsonConvert.DeserializeObject<FeatureCollection>(json)!;
        }

        private static bool IsPathValid(string path)
        {
            return File.Exists(path) && !string.IsNullOrWhiteSpace(path);
        }
    }
}
