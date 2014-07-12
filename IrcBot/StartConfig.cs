using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrcBot
{
    class StartConfig
    {
        public bool IsEvent { get; set; };
        public Boolean Delete { get; set; }
        public List<string> ChannesList { get; set; }
    }
}
