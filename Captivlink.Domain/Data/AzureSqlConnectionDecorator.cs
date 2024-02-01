using System.Data.Common;
using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Captivlink.Infrastructure.Data
{
    public static class AzureSqlConnectionDecorator
    {
        public static void ApplyAzureManagedIdentityToken(this DbContext dbContext, bool useAccessToken)
        {
            var conn = (SqlConnection)dbContext.Database.GetDbConnection();

            if (!UseToken(conn, useAccessToken)) return;

            var tokenCredential = new DefaultAzureCredential();
            var accessToken = tokenCredential.GetTokenAsync(
                new TokenRequestContext(scopes: new string[] { "https://database.windows.net" + "/.default" }) { }
            ).Result;


            conn.AccessToken = accessToken.Token;
        }

        private static bool UseToken(DbConnection conn, bool useAccessToken)
        {
            if (conn?.DataSource == null || conn.DataSource.Contains("localhost")) return false;

            return useAccessToken;
        }
    }
}