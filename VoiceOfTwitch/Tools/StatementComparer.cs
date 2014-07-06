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
    }

    
}