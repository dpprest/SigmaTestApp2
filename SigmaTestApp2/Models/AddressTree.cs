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
        private void AddToTree(string[] addressParts, CsvReader csv)
        {
            var currentNode = _root;
            string currentPath = "Root";
            for (int i = 0; i < addressParts.Length; i++)
            {
                var segment = addressParts[i];
                currentPath += $", {segment}";
                if (i < addressParts.Length - 1)
                {
                    var element = new AddressTreeElement(currentPath, segment, i + 1);
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
        public AddressTreeElement FindElement(string[] addressPath)
        {
            var currentNode = _root;
            for (int i = 0; i < addressPath.Length; i++)
            {
                var segment = addressPath[i];
                if (i == addressPath.Length - 1)
                {
                    return currentNode.Elements.TryGetValue(segment, out var element) ? element : null;
                }
                if (!currentNode.Nodes.TryGetValue(segment, out currentNode))
                {
                    return null;
                }
            }
            return null;
        }
        public List<AddressTreeElement> SearchElementsBySubstring(string substring)
        {
            var results = new List<AddressTreeElement>();
            SearchSubstring(_root, substring, results);
            return results;
        }
        private void SearchSubstring(AddressTreeNode node, string substring, List<AddressTreeElement> results)
        {
            foreach (var element in node.Elements.Values)
            {
                if (element.OwnSegment.Contains(substring) || element.FullPath.Contains(substring))
                    results.Add(element);
            }
            foreach (var childNode in node.Nodes.Values)
                SearchSubstring(childNode, substring, results);
        }
        public List<AddressTreeElement> GetAllElements()
        {
           var elements = new List<AddressTreeElement>();
           CollectElements(_root, elements);
           return elements;
        }
        private void CollectElements(AddressTreeNode node, List<AddressTreeElement> elements)
        {
            elements.AddRange(node.Elements.Values);
            foreach (var childNode in node.Nodes.Values )
            {
                CollectElements(childNode, elements);
            }
        }
        public AddressTreeNode FindCommonAncestor(IEnumerable<AddressTreeElement> elements)
        {
            if (elements == null || !elements.Any())
            {
                return null;
            }
            var paths = elements.Select(e => e.FullPath.Split(", ")).ToList();
            var minLength = paths.Min(p => p.Length);
            AddressTreeNode commonAncestor = _root;
            for (int i = 0; i < minLength; i++)
            {
                var currentSegment = paths[0][i];
                if (paths.Any(p => p[i] != currentSegment))
                    break;
                commonAncestor = commonAncestor.Nodes.TryGetValue(currentSegment, out var node) ? node : commonAncestor;
            }
                return commonAncestor;
        }
        public int GetNodeLevel(AddressTreeNode node)
        {
            return node?.Level ?? -1;
        }
        public int GetElementLevel(AddressTreeElement element)
        {
            return element?.Level ?? -1;
        }
    }   

}
