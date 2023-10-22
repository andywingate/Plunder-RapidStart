using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XMLToCSV
{
    class Program
    {
        static void Main()
        {
            var xml = File.ReadAllText("med.xml"); // Assuming XML is saved in "input.xml"
            var xmlDoc = XDocument.Parse(xml);

            foreach (var element in xmlDoc.Root.Elements())
            {
                // Create CSV content for each repeating element
                var csv = ConvertToCSV(element);
                if (csv != null)
                {
                    File.WriteAllText($"{element.Name}.csv", csv);
                }
            }
        }

        private static string ConvertToCSV(XElement element)
        {
            // This function will convert an XML element to CSV format

            var subElements = element.Elements();
            if (!subElements.Any())
            {
                return null;
            }

            var header = string.Join(",", subElements.First().Elements().Select(e => e.Name));
            var rows = from se in subElements
                       let row = string.Join(",", se.Elements().Select(e => EscapeCsvValue(e.Value)))
                       select row;

            return $"{header}\n{string.Join("\n", rows)}";
        }

        private static string EscapeCsvValue(string value)
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
    }
}
