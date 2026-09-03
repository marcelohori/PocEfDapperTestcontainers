using System.Data;

namespace PocEfDapper.Application.Common;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}