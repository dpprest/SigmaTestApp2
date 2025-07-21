using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigmaTestApp2.Models
{
    public class AddressTree
    {
        private readonly AddressTreeNode _root = new("Root", "Root", 0);

        public void ImportFromCsv(string filePath)
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Read();
            while (csv.Read())
            {
                var fullAddress = csv.GetField("Полный адрес: ");
                var addressParts = fullAddress.Split(',').Select(x => x.Trim()).ToArray();
                AddToTree(addressParts, csv);
            }
        }
        private void AddToTree(string[] addressParts,  CsvReader csv)
        {
            var currentNode = _root;
            string currentPath = "Root";
            for (int i = 0; i < addressParts.Length; i++)
            {
                var segment = addressParts[i];
                currentPath += $", {segment}";
                if (i  < addressParts.Length - 1)
                {
                    var element = new AddressTreeElement (currentPath, segment, i+1);
                    foreach (var header in csv.HeaderRecord)
                    {
                        if (header != "Полный адрес: ")
                            element.Metadata[header] = csv.GetField(header);
                    }
                    currentNode.Elements[segment] = element;
                }
                else //если промежуточный узел
                {
                    if (!currentNode.Nodes.ContainsKey(segment))
                    {
                        var newNode = new AddressTreeNode(segment, currentPath, i + 1);
                        currentNode.Nodes[segment] = newNode;
                    }
                    currentNode = currentNode.Nodes[segment];
                }
            }
        }
        //public AddressTreeElement FindElement(string[] addressPath);
        //public List<AddressTreeElement> GetAllElements();
        //public List<AddressTreeElement> SearchElementsBySubstring(string substring);
        //public AddressTreeNode FindCommonAncestor(IEnumerable<AddressTreeElement> elements);
        //public int GetNodeLevel(AddressTreeNode node);
        //public int GetElementLevel(AddressTreeElement element);
    }   

}
