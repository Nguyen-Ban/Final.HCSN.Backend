using Dapper;
using Final.HCSN.Core.Attributes;
using Final.HCSN.Core.Interface.Repository;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Final.HCSN.Infrastructure.Repository
{
    public class BaseRepo<T> : IBaseRepo<T>
    {
        protected readonly string _connectionString;
        private readonly string _tableName;
        MySqlConnection connection;
        public BaseRepo(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
            _tableName = GetTableName();
        }

        private string GetTableName ()
        {
            var tableNameAttr = typeof(T).GetCustomAttributes(typeof(TableNameAttribute), true).FirstOrDefault() as TableNameAttribute;
            return tableNameAttr != null ? tableNameAttr.TableName : string.Empty;
        }

        public async Task<int> CreateAsync(T entity)
        {
            var tableName = _tableName;

            // Tên cột trong bảng ví dụ ColumnName
            var columns = new List<string>();

            // Tên tham số ví dụ @ColumnName
            var paramNames = new List<string>();

            // Tham số Dapper ví dụ new { ColumnName = value }
            var parameters = new DynamicParameters();

            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                // Ưu tiên lấy tên cột từ Attribute ColumnName, nếu không có thì lấy tên thuộc tính
                // Ví dụ: [ColumnName("db_column_name")] columnName = "db_column_name"
                var columnAttr = prop.GetCustomAttribute<ColumnNameAttribute>();
                var columnName = columnAttr != null ? columnAttr.ColumnName : prop.Name;

                // Lấy giá trị của thuộc tính ví dụ entity.ColumnName
                var value = prop.GetValue(entity);

                var isPrimaryKey = prop.GetCustomAttribute<PrimaryKeyAttribute>() != null;
                if (isPrimaryKey && (value == null || (prop.PropertyType == typeof(string) && string.IsNullOrEmpty((string)value))))
                {
                    // Nếu kiểu dữ liệu là Guid hoặc Guid? thì gán Guid
                    if (prop.PropertyType == typeof(Guid) || prop.PropertyType == typeof(Guid?))
                    {
                        value = Guid.NewGuid();
                    }
                    // Nếu là string thì gán string
                    else
                    {
                        value = Guid.NewGuid().ToString();
                    }

                    prop.SetValue(entity, value);
                }

                columns.Add(columnName);
                paramNames.Add($"@{columnName}");
                parameters.Add($"@{columnName}", value);
            }

            var sql = $"INSERT INTO {tableName} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", paramNames)})";

            using (connection = new MySqlConnection(_connectionString))
            {
                var result = await connection.ExecuteAsync(sql, parameters);
                return result;
            }

        }

        public Task<List<T>> GetAllAsync()
        {
            var sql = $"SELECT * FROM {_tableName}";
            using (connection = new MySqlConnection(_connectionString))
            {
                var result = connection.Query<T>(sql).ToList();
                return Task.FromResult(result);
            }
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            var properties = typeof(T).GetProperties();
            var primaryKeyProp = properties.FirstOrDefault(prop => prop.GetCustomAttribute<PrimaryKeyAttribute>() != null);

            if (primaryKeyProp == null)
            {
                throw new InvalidOperationException($"Bảng {typeof(T).Name} không có thuộc tính PrimaryKey attribute định nghĩa.");
            }

            var columnAttr = primaryKeyProp.GetCustomAttribute<ColumnNameAttribute>();
            var primaryKeyColumn = columnAttr != null ? columnAttr.ColumnName : primaryKeyProp.Name;

            var sql = $"SELECT * FROM {_tableName} WHERE {primaryKeyColumn} = @Id";

            using (connection = new MySqlConnection(_connectionString))
            {
                var result = await connection.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(T entity)
        {
            var tableName = _tableName;
            var setClauses = new List<string>();
            var parameters = new DynamicParameters();
            var primaryKeyName = string.Empty;
            object primaryKeyValue = null;

            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                var columnAttr = prop.GetCustomAttribute<ColumnNameAttribute>();
                var columnName = columnAttr != null ? columnAttr.ColumnName : prop.Name;
                var value = prop.GetValue(entity);

                var isPrimaryKey = prop.GetCustomAttribute<PrimaryKeyAttribute>() != null;

                if (isPrimaryKey)
                {
                    primaryKeyName = columnName;
                    primaryKeyValue = value;
                    parameters.Add($"@{columnName}", value);
                }
                else
                {
                    setClauses.Add($"{columnName} = @{columnName}");
                    parameters.Add($"@{columnName}", value);
                }
            }

            if (string.IsNullOrEmpty(primaryKeyName) || primaryKeyValue == null)
            {
                throw new Exception("Không tìm thấy khóa chính hoặc giá trị khóa chính để cập nhật.");
            }

            // Câu lệnh Update: UPDATE table_name SET col1=@col1, col2=@col2 WHERE id=@id
            var sql = $"UPDATE {tableName} SET {string.Join(", ", setClauses)} WHERE {primaryKeyName} = @{primaryKeyName}";

            using (connection = new MySqlConnection(_connectionString))
            {
                var result = await connection.ExecuteAsync(sql, parameters);
                return result;
            }
        }

        public async Task<int> DeleteAsync(string id)
        {
            var properties = typeof(T).GetProperties();
            var primaryKeyProp = properties.FirstOrDefault(prop => prop.GetCustomAttribute<PrimaryKeyAttribute>() != null);

            // Kiểm tra nếu không tìm thấy thuộc tính PrimaryKey
            if (primaryKeyProp == null)
            {
                throw new InvalidOperationException($"Bảng {typeof(T).Name} không có thuộc tính PrimaryKey attribute định nghĩa.");
            }
            var columnAttr = primaryKeyProp.GetCustomAttribute<ColumnNameAttribute>();

            var primaryKeyColumn = columnAttr != null ? columnAttr.ColumnName : primaryKeyProp.Name;

            var sql = $"DELETE FROM {_tableName} WHERE {primaryKeyColumn} = @Id";

            using (connection = new MySqlConnection(_connectionString))
            {
                return await connection.ExecuteAsync(sql, new { Id = id });
            }

        }

        public async Task<int> DeleteManyAsync(List<string> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return 0;
            }

            var properties = typeof(T).GetProperties();
            var primaryKeyProp = properties.FirstOrDefault(prop => prop.GetCustomAttribute<PrimaryKeyAttribute>() != null);
            var columnAttr = primaryKeyProp.GetCustomAttribute<ColumnNameAttribute>();
            var primaryKeyColumn = columnAttr != null ? columnAttr.ColumnName : primaryKeyProp.Name;

            var sql = $"DELETE FROM {_tableName} WHERE {primaryKeyColumn} IN @Ids";

            using (connection = new MySqlConnection(_connectionString))
            {
                return await connection.ExecuteAsync(sql, new { Ids = ids });
            }
        }

    }
}
