using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace XMLToCSV
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Enter the name of the GZip archive:");
            var gzipFileName = Console.ReadLine();

            // Check if file exists
            if (!File.Exists(gzipFileName))
            {
                Console.WriteLine("File not found.");
                return;
            }

            // Extract GZip
            var extractedFileName = ExtractGZip(gzipFileName);

            var xml = File.ReadAllText(extractedFileName);
            var xmlDoc = XDocument.Parse(xml);

            foreach (var element in xmlDoc.Root.Elements())
            {
                // Create CSV content for each repeating element
                var csv = ConvertToCSV(element);
                if (csv != null)
                {
                    var csvFileName = $"{element.Name}.csv";
                    File.WriteAllText(csvFileName, csv);
                    RemoveSecondRowFromCsv(csvFileName);
                }
            }

            // Cleanup: Delete the extracted XML file after processing
            File.Delete(extractedFileName);
        }

        private static string ExtractGZip(string gzipFileName)
        {
            var targetFileName = Path.GetFileNameWithoutExtension(gzipFileName);

            using (var gzipStream = new GZipStream(File.OpenRead(gzipFileName), CompressionMode.Decompress))
            {
                using (var fileStream = File.Create(targetFileName))
                {
                    gzipStream.CopyTo(fileStream);
                }
            }

            return targetFileName;
        }

        private static string ConvertToCSV(XElement element)
        {
            var subElements = element.Elements().ToList();
            if (!subElements.Any())
            {
                return null;
            }

            var headerElements = subElements
                .SelectMany(se => se.Elements().Select(e => e.Name.LocalName))
                .Distinct()
                .ToList();

            var header = string.Join(",", headerElements);
            var rows = from se in subElements
                       let row = string.Join(",", headerElements.Select(h => EscapeCsvValue(se.Element(h)?.Value ?? "")))
                       select row;

            return $"{header}\n{string.Join("\n", rows)}";
        }

        private static void RemoveSecondRowFromCsv(string filePath)
        {
            var allLines = File.ReadAllLines(filePath).ToList();
            if (allLines.Count > 1)
            {
                allLines.RemoveAt(1);
            }
            File.WriteAllLines(filePath, allLines);
        }

        private static string EscapeCsvValue(string value)
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
    }
}
