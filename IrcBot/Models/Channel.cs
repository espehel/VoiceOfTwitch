using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrcBot.Models
{
    public class Channel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public Channel(string name, DateTime startedAt, DateTime createdAt)
        {
            this.Name = name;
            this.StartedAt = startedAt;
            this.CreatedAt = createdAt;
        }
         public Channel(object id, object name, object startedAt, object createdAt)
        {
            this.Id = Convert.ToInt64(id);
            this.Name = (string) name;
            this.StartedAt = (DateTime) startedAt;
            this.CreatedAt = (DateTime) createdAt;
        }
    }

}
