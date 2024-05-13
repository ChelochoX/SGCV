using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Persistence
{
    public class DbConnections
    {
        private readonly string sqlConnectionStringLocalDB;

        public DbConnections(IConfiguration configuration)        {

            sqlConnectionStringLocalDB = configuration.GetConnectionString("Local");

        }
        //Conexion a mi base local
        public IDbConnection CreateSqlConnection() => new SqlConnection(sqlConnectionStringLocalDB);

    }
}
