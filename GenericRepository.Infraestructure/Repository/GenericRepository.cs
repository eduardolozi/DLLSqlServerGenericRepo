using System.Text;
using Infra.Caching;
using Infra.Context;
using Infra.Interfaces;
using Microsoft.Data.SqlClient;

namespace Infra.Repository;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private DbContext _context;
    public GenericRepository(DbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var objectList = new List<T>();
        var className = typeof(T).Name;

        await using var connection = _context.CreateSqlConnection();
        var query = $"SELECT * FROM {className}";
        
        var command = new SqlCommand(query, connection);
        try
        {
            connection.Open();
            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                objectList.Add(MapSqlRecordToObject(reader));
            }
            reader.Close();
            
            return objectList;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    public async Task<T> GetByIdAsync(int id)
    {
        var type = typeof(T);
        var obj = Activator.CreateInstance<T>();

        await using var connection = _context.CreateSqlConnection();
        var query = $"SELECT * FROM {type.Name} WHERE Id = @Id";
        var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);
        try
        {
            connection.Open();
            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                obj = MapSqlRecordToObject(reader);
            }
            connection.Close();

            return obj;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    public async Task AddAsync(T entity)
    {
        var type = typeof(T);
        var properties = ReflectionCache.GetPropertiesByType(type);

        var query = new StringBuilder($"INSERT INTO {type.Name} (");
        var propertyNames = properties.Select(x => x.Name).ToList();
        query.Append(string.Join(",", propertyNames));
        query.Append(") VALUES (");
        var parameters = properties.Select(x => $"@{x.Name}").ToList();
        query.Append(string.Join(",", parameters));
        query.Append(')');

        await using var connection = _context.CreateSqlConnection();
        var command = new SqlCommand(query.ToString(), connection);
        for (var i = 0; i < parameters.Count; i++)
        {
            command.Parameters.AddWithValue(parameters[i], properties[i].GetValue(entity));
        }

        try
        {
            connection.Open();
            await command.ExecuteNonQueryAsync();
            connection.Close();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    public async Task UpdateAsync(int id, T entity)
    {
        var type = typeof(T);
        var properties = ReflectionCache.GetPropertiesByType(type);
        
        var query = new StringBuilder($"UPDATE {type.Name} SET ");
        query.Append(string.Join(",", properties.Where(x => x.Name != "Id").Select(x => $"{x.Name} = @{x.Name}")));
        query.Append(" WHERE Id = @Id");
         
        await using var connection = _context.CreateSqlConnection();
        var command = new SqlCommand(query.ToString(), connection);
        foreach (var parameter in properties)
        {
            if(parameter.Name != "Id")
                command.Parameters.AddWithValue($"@{parameter.Name}", parameter.GetValue(entity));
        }
        command.Parameters.AddWithValue("@Id", id);
        
        try
        {
            connection.Open();
            await command.ExecuteNonQueryAsync();
            connection.Close();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    public async Task DeleteAsync(int id)
    {
        var type = typeof(T);

        var query = $"DELETE FROM {type.Name} WHERE Id = @Id";
        await using var connection = _context.CreateSqlConnection();
        var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);
        try 
        {
            connection.Open();
            await command.ExecuteNonQueryAsync();
            connection.Close();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }
    
    private T MapSqlRecordToObject(SqlDataReader reader)
    {
        var type = typeof(T);
        var properties = ReflectionCache.GetPropertiesByType(type);
        var obj = Activator.CreateInstance<T>();
        foreach (var property in properties)
        {
            var x = reader[property.Name];
            property.SetValue(obj, reader[property.Name]);
        }

        return obj;
    }
}