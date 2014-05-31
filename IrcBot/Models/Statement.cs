using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrcBot.Models
{
    class Statement : IComparable<String>
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string[] Terms { get; private set; }
        public double Score { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdated { get; set; }

        public Statement(int id, string text, DateTime createdAt)
        {
            this.Id = id;
            this.Text = text;
            this.CreatedAt = createdAt;
            this.LastUpdated = createdAt;
            this.Score = 1;
            GenerateTerms();
        }

        private void GenerateTerms()
        {
            //TODO: use the text to make terms for finding similar texts
            Terms = Text.Split(new char[] {' '});
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

        public bool SimilarTo(string message)
        {
            // TODO: make a algorithm to check similarity
            return false;
        }
    }
}
