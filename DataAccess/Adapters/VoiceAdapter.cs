using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities;
using Shared.Models;


namespace DataAccess.Adapters
{
    public class VoiceAdapter
    {
        public List<StatementModel> GetStatements()
        {
            var statements = new List<StatementModel>();
            using (var ef = new VoiceDatabaseEntities())
            {
               statements = ef.Statements.Select(x => new StatementModel()
                {
                    Id = x.id,
                    ChannelId = x.channelId,
                    CreatedAt = (DateTime) x.createdAt,
                    LastUpdated = (DateTime) x.lastUpdated,
                    Occurrences = (long) x.occurrences,
                    Score = (double) x.score,
                    Text = x.text
                }).ToList();
            }
            return statements;
        }

        public void DeleteAllStatements()
        {
            using (var ef = new VoiceDatabaseEntities())
            {
                ef.Database.ExecuteSqlCommand("DELETE Statement");
                ef.SaveChanges();
            }
        }

        public void AddOrUpdateChannel(ChannelModel channelModel)
        {
            using (var ef = new VoiceDatabaseEntities())
            {
                var channel = ef.Channels.SingleOrDefault(c => c.name == channelModel.Name);

                if (channel == null)
                    ef.Channels.Add(new Channel()
                    {
                        createdAt = DateTime.Now,
                        name = channelModel.Name,
                        startedAt = DateTime.Now
                    });
                else
                {
                    channel.startedAt = DateTime.Now;
                }
                ef.SaveChanges();
            }
        }

        public void UpdateStatements(List<StatementModel> statementModels, int counter )
        {
            using (var ef = new VoiceDatabaseEntities())
            {
                statementModels.ForEach(sm => UpdateStatement(ef.Statements.FirstOrDefault(s => s.id == sm.Id),sm));
                ef.SaveChanges();
            }
        }

        private void UpdateStatement(Statement statement, StatementModel model)
        {
            statement.occurrences = model.Occurrences;
            statement.score = (float?) model.Score;
            statement.lastUpdated = DateTime.Now;
        }
        public void DeleteRareStatements(int threshold)
        {
            using (var ef = new VoiceDatabaseEntities())
            {
                foreach (var statement in ef.Statements)
                {
                    if (statement.score < threshold)
                        ef.Statements.Remove(statement);
                }
                ef.SaveChanges();
            }
        }
    }
}
