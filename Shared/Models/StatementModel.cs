using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class StatementModel
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public string[] Terms { get; private set; }
        public double Score { get; set; }
        public long Occurrences { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdated { get; set; }
        public long ChannelId { get; set; }
    }
}
