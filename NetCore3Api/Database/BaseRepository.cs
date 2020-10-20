using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NetCore3Api.Etc;

namespace NetCore3Api.Database
{
    public abstract class BaseRepository
    {
        protected string _connectionString;
        // protected IConnectionMultiplexer _connectionMultiplexer;

        protected BaseRepository(IOptions<Configuracoes> configuracoes)
        {
            _connectionString = configuracoes.Value.ConnectionStrings.Conn1;
        }

        protected async Task<T> UsarSqlConnection<T>(Func<IDbConnection, Task<T>> getData)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var data = await getData(connection);

                    return data;
                }
            }
            catch (TimeoutException ex)
            {
                throw new Exception(String.Format("{0}.WithConnection() experienced a SQL timeout", GetType().FullName), ex);
            }
            catch (SqlException ex)
            {
                throw new Exception(String.Format("{0}.WithConnection() experienced a SQL exception (not a timeout)", GetType().FullName), ex);
            }
        }

        // protected async Task<T> UsarCacheRedis<T>(string key, Func<IDbConnection, Task<T>> getData, TimeSpan? expiry = null)
        // {
        //     if (expiry == null)
        //         expiry = DateTime.Now.AddSeconds(30) - DateTime.Now;

        //     IDatabase redisDb = _connectionMultiplexer.GetDatabase();

        //     if (redisDb.KeyExists(key))
        //         return redisDb.GetAs<T>(key);

        //     var data = await UsarSqlConnection(getData);

        //     redisDb.StringSet(key, JsonConvert.SerializeObject(data), expiry, flags: CommandFlags.FireAndForget);

        //     return data;
        // }
    }
}
