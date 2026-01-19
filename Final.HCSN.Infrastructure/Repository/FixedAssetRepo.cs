using Dapper;
using Final.HCSN.Core.DTOs;
using Final.HCSN.Core.Entities;
using Final.HCSN.Core.Interface.Repository;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.HCSN.Infrastructure.Repository
{
    public class FixedAssetRepo : BaseRepo<FixedAssetEntity>, IFixedAssetRepo
    {
        public FixedAssetRepo(IConfiguration config) : base(config)
        {
            
        }

        public async Task<bool> IsCodeExistsAsync(string assetCode)
        {
            var sql = "SELECT COUNT(*) FROM fixed_asset WHERE fixed_asset_code = @Code";

            using (var connection = new MySqlConnection(_connectionString))
            {
                var count = await connection.ExecuteScalarAsync<int>(sql, new { Code = assetCode });
                return count > 0;
            }
        }

        public async Task<List<FixedAssetDto>> GetFixedAssetGridAsync()
        {
            var sql = @"
                SELECT 
                    CAST(fa.fixed_asset_id AS CHAR) AS fixed_asset_id,
                    fa.fixed_asset_code AS fixed_asset_code,
                    fa.fixed_asset_name AS fixed_asset_name,

                    CAST(fa.department_id AS CHAR) AS department_id,
                    CAST(fa.fixed_asset_category_id AS CHAR) AS fixed_asset_category_id,

                    d.department_name AS department_name,
                    d.department_code AS department_code,

                    fac.fixed_asset_category_code AS fixed_asset_category_code,
                    fac.fixed_asset_category_name AS fixed_asset_category_name,
                    fa.quantity AS quantity,
                    fa.cost AS cost,
                    fa.depreciation_value_year AS depreciation_value_year,
                    fa.depreciation_rate AS depreciation_rate
                FROM 
                    fixed_asset fa
                JOIN 
                    department d ON fa.department_id = d.department_id
                JOIN 
                    fixed_asset_category fac ON fa.fixed_asset_category_id = fac.fixed_asset_category_id
                ORDER BY fa.modified_date DESC;";
            using (var connection = new MySqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<FixedAssetDto>(sql);
                return result.ToList();
            }
        }

        public async Task<FixedAssetDto> GetFixedAssetDetailByIdAsync(string id)
        {
            var sql = @"
                SELECT 
                    CAST(fa.fixed_asset_id AS CHAR) AS fixed_asset_id,
                    fa.fixed_asset_code AS fixed_asset_code,
                    fa.fixed_asset_name AS fixed_asset_name,
                    CAST(fa.department_id AS CHAR) AS department_id,
                    CAST(fa.fixed_asset_category_id AS CHAR) AS fixed_asset_category_id,
                    d.department_name AS department_name,
                    d.department_code AS department_code,
                    fac.fixed_asset_category_code AS fixed_asset_category_code,
                    fac.fixed_asset_category_name AS fixed_asset_category_name,
                    fa.quantity AS quantity,
                    fa.cost AS cost,
                    fa.depreciation_value_year AS depreciation_value_year,
                    fa.depreciation_rate AS depreciation_rate,
                    fa.purchase_date AS purchase_date,  
                    fa.tracked_year AS tracked_year,
                    fa.production_year AS production_year
                FROM 
                    fixed_asset fa
                JOIN 
                    department d ON fa.department_id = d.department_id
                JOIN 
                    fixed_asset_category fac ON fa.fixed_asset_category_id = fac.fixed_asset_category_id
                WHERE 
                    fa.fixed_asset_id = @Id;";
            using (var connection = new MySqlConnection(_connectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<FixedAssetDto>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<string> GetNewFixedAssetCodeAsync()
        {
            // Logic: Lấy mã tài sản lớn nhất hiện tại để tăng lên 1
            // Giả sử mã có dạng TSxxxxx (TS00001)
            // Cắt bỏ phần chữ, lấy phần số, tăng lên 1 rồi ghép lại

            // 1. Lấy mã lớn nhất
            var sql = "SELECT fixed_asset_code FROM fixed_asset ORDER BY fixed_asset_code DESC LIMIT 1";
            string maxCode = "";

            using (var connection = new MySqlConnection(_connectionString))
            {
                maxCode = await connection.QueryFirstOrDefaultAsync<string>(sql);
            }

            // 2. Xử lý sinh mã
            if (string.IsNullOrEmpty(maxCode))
            {
                return "TS00001"; // Mã mặc định đầu tiên
            }

            // Giả sử format luôn là TS + số
            // Cách đơn giản: Tách số và tăng
            // Lưu ý: Cần xử lý kỹ hơn nếu mã không theo quy tắc, ở đây demo quy tắc TS...
            string prefix = "TS";
            string numberPart = maxCode.Substring(prefix.Length); // Lấy phần số sau "TS"

            if (long.TryParse(numberPart, out long number))
            {
                number++;
                // PadLeft để giữ định dạng số 0 (ví dụ 00002)
                return $"{prefix}{number.ToString().PadLeft(numberPart.Length, '0')}";
            }

            return "TS00001";
        }

        public async Task<PagingResult<FixedAssetDto>> GetFixedAssetsByFilterAsync(FixedAssetFilterDto filter)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                var whereConditions = new List<string>();

                // 1. Xây dựng điều kiện lọc
                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    whereConditions.Add("(fa.fixed_asset_code LIKE @Keyword OR fa.fixed_asset_name LIKE @Keyword)");
                    parameters.Add("@Keyword", $"%{filter.Keyword}%");
                }
                if (filter.DepartmentId.HasValue)
                {
                    whereConditions.Add("fa.department_id = @DepartmentId");
                    parameters.Add("@DepartmentId", filter.DepartmentId);
                }
                if (filter.FixedAssetCategoryId.HasValue)
                {
                    whereConditions.Add("fa.fixed_asset_category_id = @CategoryId");
                    parameters.Add("@CategoryId", filter.FixedAssetCategoryId);
                }

                string whereClause = whereConditions.Count > 0 ? "WHERE " + string.Join(" AND ", whereConditions) : "";

                // 2. QUERY 1: Đếm tổng số bản ghi (Để tính Total Pages)
                string countSql = $"SELECT COUNT(*) FROM fixed_asset fa {whereClause};";
                var totalRecords = await connection.ExecuteScalarAsync<int>(countSql, parameters);

                // 3. QUERY 2: Lấy dữ liệu phân trang (Limit/Offset)
                int offset = (filter.PageNumber - 1) * filter.PageSize;
                parameters.Add("@Limit", filter.PageSize);
                parameters.Add("@Offset", offset);

                string dataSql = $@"
                    SELECT 
                        CAST(fa.fixed_asset_id AS CHAR) AS fixed_asset_id,
                        fa.fixed_asset_code, fa.fixed_asset_name,
                        d.department_name, fac.fixed_asset_category_name,
                        fa.quantity, fa.cost, fa.depreciation_rate, fa.depreciation_value_year,
                        fa.tracked_year, fa.purchase_date
                    FROM fixed_asset fa
                    LEFT JOIN department d ON fa.department_id = d.department_id
                    LEFT JOIN fixed_asset_category fac ON fa.fixed_asset_category_id = fac.fixed_asset_category_id
                    {whereClause}
                    ORDER BY fa.modified_date DESC
                    LIMIT @Limit OFFSET @Offset;"; // Chỉ lấy số lượng = PageSize

                var data = await connection.QueryAsync<FixedAssetDto>(dataSql, parameters);

                // 4. Trả về kết quả gộp
                return new PagingResult<FixedAssetDto>
                {
                    TotalRecords = totalRecords,
                    Data = data.ToList()
                };
            }
        }
    }
}
