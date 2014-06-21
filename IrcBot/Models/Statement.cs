using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrcBot.Models
{
    internal class Statement : IComparable<String>
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public string[] Terms { get; private set; }
        public double Score { get; private set; }
        public long Occurrences { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdated { get; set; }

        public Statement(string text)
        {
            this.Text = text;
            this.CreatedAt = DateTime.Now;
            this.LastUpdated = DateTime.Now;
            this.Score = 1;
            this.Occurrences = 1;
            GenerateTerms();
        }

        public Statement(object id, object text, object createdAt, object lastUpdated, object score, object occurences)
        {
            this.Id = Convert.ToInt64(id);
            this.Text = (string) text;
            this.CreatedAt = (DateTime) createdAt;
            this.LastUpdated = (DateTime) lastUpdated;
            this.Score = Convert.ToDouble(score);
            this.Occurrences = Convert.ToInt64(occurences);
            GenerateTerms();
        }

        private void GenerateTerms()
        {
            //TODO: use the text to make terms for finding similar texts
            Terms = IndexingUtils.extractTerms(Text);
        }

        public void IncrementScore(double i)
        {
            Score += i;
            LastUpdated = DateTime.Now;
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

        public void SimilarTo(Statement other)
        {
            //https://fuzzystring.codeplex.com/
            //algorithms to check for similarities
            double simScore = 0;
            simScore = checkCase(other.Text);
            simScore = checkHammingDistance(other.Terms);

            
            IncrementScore(simScore);
            other.IncrementScore(simScore);
        }
        private double calcNewScore(double score)
        {
            if (score == 0)
                return 0.5;
            else
            {
                return score + (1 - score) * (1 - score);
            }
        }

        private double checkCase(string message)
        {
            return Text.Equals(message, StringComparison.OrdinalIgnoreCase) ? 0.5 : 0;
        }

        private double checkHammingDistance(string[] otherTerms)
        {
            double score = 0;
            if (this.Terms.Count() == otherTerms.Count())
                for (int i = 0; i < this.Terms.Count(); i++)
                {
                    int lenght = this.Terms[i].Length;
                    int upperLimit;
                    if(lenght<2)
                        continue;
                    else if (lenght == 2)
                        upperLimit = 1;
                    else
                        upperLimit = 2;
                    
                    if (checkHammingDistance(this.Terms[i], this.Terms[i], upperLimit))
                        score += 0.1;
                }

            return score;
        }

        private bool checkHammingDistance(string source, string target, int upperLimit)
        {
            int distance = 0;

            if (source.Length == target.Length)
            {
                for (int i = 0; i < source.Length; i++)
                {
                    if (!source[i].Equals(target[i]))
                    {
                        distance++;
                    }
                }
                return distance <= upperLimit;
            }
            else { return false; }
        }
    }
}
