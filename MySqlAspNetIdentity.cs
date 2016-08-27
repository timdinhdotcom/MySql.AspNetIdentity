using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Migrations.History;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;

namespace MySql.AspNetIdentity
{
    public class MySqlEFConfiguration : MySql.Data.Entity.MySqlEFConfiguration
    {
        public MySqlEFConfiguration()
            : base()
        {
            base.SetMigrationSqlGenerator(
                MySql.Data.Entity.MySqlProviderInvariantName.ProviderName,
                () => new MySql.AspNetIdentity.MySqlMigrationSqlGenerator());

            SetHistoryContext(
                MySql.Data.Entity.MySqlProviderInvariantName.ProviderName,
                (conn, schema) => new MySql.AspNetIdentity.HistoryContext(conn, schema));
        }
    }

    public class HistoryContext : System.Data.Entity.Migrations.History.HistoryContext
    {
        public HistoryContext(
          DbConnection existingConnection,
          string defaultSchema)
        : base(existingConnection, defaultSchema)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<HistoryRow>().Property(h => h.MigrationId).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<HistoryRow>().Property(h => h.ContextKey).HasMaxLength(200).IsRequired();
        }
    }

    public class MySqlMigrationSqlGenerator : MySql.Data.Entity.MySqlMigrationSqlGenerator
    {
        public MySqlMigrationSqlGenerator()
            : base()
        {

        }


        public override IEnumerable<MigrationStatement> Generate(IEnumerable<MigrationOperation> migrationOperations, string providerManifestToken)
        {
            var res = base.Generate(migrationOperations, providerManifestToken);

            // remove 'dbo.' from statements and add sql terminate char ';'
            foreach(var statement in res)
            {
                statement.Sql = statement.Sql.Replace("dbo.", "") + (!statement.Sql.EndsWith(";") ? ";" : string.Empty);
            }

            return res;
        }

        protected override string Generate(ColumnModel op)
        {
            var sql = base.Generate(op);
            if (op.IsUnicode == false)
            {
                sql = sql.Replace(")", ") character set latin1 collate latin1_general_ci");
            }

            return sql;
        }
        
    }
}
