using System;
using System.Xml;
using System.IO;
using System.Text;

class Program
{
    static void Main()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load("med.xml");

        XmlNode dataList = doc.SelectSingleNode("/DataList");
        if (dataList == null)
        {
            Console.WriteLine("Invalid XML format. The <DataList> element was not found.");
            return;
        }

        foreach (XmlNode list in dataList.ChildNodes)
        {
            if (list.NodeType == XmlNodeType.Element)
            {
                string listName = list.Name;
                string fileName = listName + ".csv";

                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    bool isFirstRow = true;

                    foreach (XmlNode item in list.ChildNodes)
                    {
                        if (item.NodeType == XmlNodeType.Element)
                        {
                            if (isFirstRow)
                            {
                                // Write the header row using element names
                                var header = new StringBuilder();
                                foreach (XmlNode element in item.ChildNodes)
                                {
                                    if (element.NodeType == XmlNodeType.Element)
                                    {
                                        header.Append(element.Name).Append(",");
                                    }
                                }
                                writer.WriteLine(header.ToString().TrimEnd(','));
                                isFirstRow = false;
                            }

                            // Write data row
                            var dataRow = new StringBuilder();
                            foreach (XmlNode element in item.ChildNodes)
                            {
                                if (element.NodeType == XmlNodeType.Element)
                                {
                                    dataRow.Append(element.InnerText).Append(",");
                                }
                            }
                            writer.WriteLine(dataRow.ToString().TrimEnd(','));
                        }
                    }
                }
            }
        }
    }
}
