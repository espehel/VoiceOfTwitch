using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using VoiceOfTwitch.Models;

namespace VoiceOfTwitch.Tools
{
    public class StatementComparer
    {
        public static IComparer<Statement> OrderByTop
        {
            get
            {
                return new OrderByTopImpl();
            }
        }
        public static IComparer<Statement> OrderByCount
        {
            get
            {
                return new OrderByCountImpl();
            }
        }
        public static IComparer<Statement> OrderByHot
        {
            get
            {
                return new OrderByHotImpl();
            }
        }
        internal class OrderByTopImpl : IComparer<Statement>
        {
            public int Compare(Statement x, Statement y)
            {
                if (!x.score.HasValue)
                    if (!y.score.HasValue)
                        return 0;
                    else
                        return 1;
                else if (!y.score.HasValue)
                    return -1;

                return Math.Sign(y.score.Value - x.score.Value);

            }
        }
        internal class OrderByCountImpl : IComparer<Statement>
        {
            public int Compare(Statement x, Statement y)
            {
                if (!x.occurrences.HasValue)
                    if (!y.occurrences.HasValue)
                        return 0;
                    else
                        return 1;
                else if (!y.occurrences.HasValue)
                    return -1;

                return Math.Sign(y.occurrences.Value - x.occurrences.Value);

            }
        }

        internal class OrderByHotImpl : IComparer<Statement>
        {
            public int Compare(Statement x, Statement y)
            {
                double xRating = 0;
                if (x.lastUpdated.HasValue && x.score.HasValue)
                   xRating = x.score.Value*x.lastUpdated.Value.Subtract(DateTime.Now).Milliseconds;
                
                double yRating = 0;
                if (y.lastUpdated.HasValue && y.score.HasValue)
                    yRating = y.score.Value * y.lastUpdated.Value.Subtract(DateTime.Now).Milliseconds;

                return Math.Sign(yRating - xRating);

            }
        }
    }

    
}