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

  
        protected override string Generate(ColumnModel op)
        {
            var sql = base.Generate(op);
            if (op.IsUnicode == false)
            {
                sql = sql.Replace(")", ") character set latin1 collate latin1_general_ci");
            }

            return sql;
        }
        
        protected override MigrationStatement Generate(CreateTableOperation op)
        {
            var result = base.Generate(op);
            result.Sql += ";";
            return result;
        }


        protected override MigrationStatement Generate(CreateIndexOperation op)
        {
            var result = base.Generate(op);
            result.Sql += ";";
            return result;
        }

        protected override MigrationStatement Generate(AddForeignKeyOperation op)
        {
            var result = base.Generate(op);
            result.Sql += ";";
            return result;
        }
    }
}
