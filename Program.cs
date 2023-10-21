using System;
using System.IO;
using System.Xml.Linq;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        Console.Write("Enter the input XML filename: ");
        string inputFileName = Console.ReadLine();

        if (File.Exists(inputFileName))
        {
            // Load the XML file
            XDocument xDocument = XDocument.Load(inputFileName);

            // Extract the elements from DataList
            var dataList = xDocument.Root;

            if (dataList != null)
            {
                // Iterate through child elements of DataList
                foreach (var element in dataList.Elements())
                {
                    if (element.Name == "FixedAssetList" || element.Name == "FADepreciationBookList")
                    {
                        // Extract the TableID
                        string tableId = element.Element("TableID")?.Value;

                        if (!string.IsNullOrEmpty(tableId))
                        {
                            string csvFileName = $"{element.Name}_{tableId}.csv";

                            using (StreamWriter writer = new StreamWriter(csvFileName))
                            {
                                bool isFirstRow = true;

                                // Iterate through the child elements and write data rows
                                foreach (var dataElement in element.Elements())
                                {
                                    var dataValues = dataElement.Elements()
                                        .Select(e => e.Value);

                                    if (isFirstRow)
                                    {
                                        // Write the header row for the first element
                                        var headerRow = dataElement.Elements()
                                            .Select(e => e.Name.LocalName);
                                        writer.WriteLine(string.Join(",", headerRow));
                                        isFirstRow = false;
                                    }

                                    writer.WriteLine(string.Join(",", dataValues));
                                }
                            }

                            Console.WriteLine($"{csvFileName} file has been created.");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No valid elements found in FixedAssetList or FADepreciationBookList.");
            }
        }
        else
        {
            Console.WriteLine("The specified input XML file does not exist.");
        }
    }
}
