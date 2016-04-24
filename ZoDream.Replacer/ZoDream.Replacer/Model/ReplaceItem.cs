using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Replacer.Model
{
    public class ReplaceItem
    {
        public string Search { get; set; }

        public string Replace { get; set; } = string.Empty;

        public ReplaceItem()
        {

        }

        public ReplaceItem(string search)
        {
            Search = search;
        }

        public ReplaceItem(string search, string replace)
        {
            Search = search;
            Replace = replace;
        }
    }
}
