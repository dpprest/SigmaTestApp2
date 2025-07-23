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
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл не найден!");
                return;
            }
            try
            {
                using var reader = new StreamReader(filePath);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                csv.Read();
                while (csv.Read())
                {
                    string fullAddress = csv.GetField("Полный адрес: ");
                    string[] addressParts = fullAddress.Split(',');
                    for (int i = 0; i < addressParts.Length; i++)
                    {
                        addressParts[i] = addressParts[i].Trim();
                    }
                    AddToTree(addressParts, csv);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла {ex.Message}");
            }
        }
        private void AddToTree(string[] addressParts, CsvReader csv)
        {
            AddressTreeNode currentNode = _root;
            string currentPath = "Root";
            for (int i = 0; i < addressParts.Length; i++)
            {
                string segment = addressParts[i];
                currentPath += $", {segment}";
                if (i == addressParts.Length - 1)
                {
                    var element = new AddressTreeElement(segment, currentPath, i + 1);
                    currentNode.Elements[segment] = element;
                }
                else
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

        public AddressTreeElement? FindElement(string[] addressPath)
        {
            AddressTreeNode currentNode = _root;
            for (int i = 0; i < addressPath.Length; i++)
            {
                string segment = addressPath[i];
                if (i == addressPath.Length - 1)
                {
                    if (currentNode.Elements.TryGetValue(segment, out var element))
                    {
                        return element;
                    }
                    return null;
                }
            }
            return null;
        }
        public List<AddressTreeElement> GetAllElements()
        {
            List<AddressTreeElement> allElements = new List<AddressTreeElement>();
            CollectElements(_root, allElements);
            return allElements;
        }

        private void CollectElements(AddressTreeNode node, List<AddressTreeElement> elements)
        {
            foreach (var element in node.Elements.Values)
            {
                elements.Add(element);
            }
        }
        public List<AddressTreeElement> SearchElementsBySubstring(string substring)
        {
            List<AddressTreeElement> results = new List<AddressTreeElement>();
            SearchInNode(_root, substring, results);
            return results;
        }
        private void SearchInNode(AddressTreeNode node, string substring, List<AddressTreeElement> results)
        {
            foreach (var element in node.Elements.Values)
            {
                if (element.OwnSegment.Contains(substring) ||
                    element.FullPath.Contains(substring))
                {
                    results.Add(element);
                }
            }
        }
        public int GetNodeLevel(AddressTreeNode node)
        {
            if (node == null) return -1;
            return node.Level;
        }
        public int GetElementLevel(AddressTreeElement element)
        {
            if (element == null) return -1;
            return element.Level;
        }
    }
}