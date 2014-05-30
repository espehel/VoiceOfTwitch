using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrcBot.Models
{
    class Statement : IComparable<String>
    {
        private int Id { get; set; }
        string Text { get; set; }
        public int Score { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime LastUpdated { get; set; }

        public Statement(int id, string text, DateTime createdAt)
        {
            this.Id = id;
            this.Text = text;
            this.CreatedAt = createdAt;
            this.LastUpdated = createdAt;
            this.Score = 1;
        }

        public int CompareTo(string other)
        {
            return System.String.CompareOrdinal(Text, other);
        }

        public bool Equals(string other)
        {
            return Text.Equals(other);
        }

        public override string ToString()
        {
            return Text + "[" + Score + "]";
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof (Statement))
                return Text.Equals(((Statement) obj).Text);
            return false;

        }
    }
}
