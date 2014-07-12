using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities;
using Shared.Models;

namespace IrcBot
{
    public class StatementWrapper
    {
        public StatementModel Statement { get; set; }
        public string[] Terms { get; set; }

        public StatementWrapper(StatementModel model)
        {
            Statement = model;
            GenerateTerms();
        }

        private void GenerateTerms()
        {
            //TODO: use the text to make terms for finding similar texts
            Terms = IndexingUtils.ExtractTerms(Statement.Text);
        }

        public void IncrementScore(double i)
        {
            Statement.Score += i;
        }

        public int CompareTo(string other)
        {
            return System.String.CompareOrdinal(Statement.Text, other);
        }

        public bool Equals(string other)
        {
            return Statement.Text.Equals(other);
        }


        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(StatementWrapper))
                return Statement.Equals(((StatementWrapper)obj).Statement.Text);
            return false;

        }

        public void SimilarTo(StatementWrapper other)
        {
            //https://fuzzystring.codeplex.com/
            //algorithms to check for similarities
            double simScore = 0;
            simScore = checkCase(other.Statement.Text);
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
            return Statement.Text.Equals(message, StringComparison.OrdinalIgnoreCase) ? 0.5 : 0;
        }

        private double checkHammingDistance(string[] otherTerms)
        {
            double score = 0;
            if (this.Terms.Count() == otherTerms.Count())
                for (int i = 0; i < this.Terms.Count(); i++)
                {
                    int lenght = this.Terms[i].Length;
                    int upperLimit;
                    if (lenght < 2)
                        continue;
                    else if (lenght == 2)
                        upperLimit = 1;
                    else
                        upperLimit = 2;

                    if (checkHammingDistance(this.Terms[i], this.Terms[i], upperLimit))
                        score += 0.01;
                    else
                        return 0;
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
