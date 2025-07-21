using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigmaTestApp2.Models
{
    public class AddressTreeElement
    {
        public string OwnSegment { get; }
        public string FullPath { get; }
        public int Level { get; }
        public Dictionary<string, string> Metadata { get; } = new();

        public AddressTreeElement(string ownSegment, string fullPath, int level)
        {
            OwnSegment = ownSegment;
            FullPath = fullPath;
            Level = level;
        }
    }
}
