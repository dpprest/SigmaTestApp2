using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigmaTestApp2.Models
{
    public class AddressTree
    {
        private readonly AddressTreeNode _root = new("Root", "Root", 0);

        public void ImportFromCsv(string filePath);
        public AddressTreeElement FindElement(string[] addressPath);
        public List<AddressTreeElement> GetAllElements();
        public List<AddressTreeElement> SearchElementsBySubstring(string substring);
        public AddressTreeNode FindCommonAncestor(IEnumerable<AddressTreeElement> elements);
        public int GetNodeLevel(AddressTreeNode node);
        public int GetElementLevel(AddressTreeElement element);
    }
}
