using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class ChannelModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public Boolean Live { get; set; }
    }
}
