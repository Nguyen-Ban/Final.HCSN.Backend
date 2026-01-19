using Final.HCSN.Core.DTOs;
using Final.HCSN.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.HCSN.Core.Interface.Repository
{
    public interface IFixedAssetRepo : IBaseRepo<FixedAssetEntity>
    {
        /// <summary>
        /// Kiểm tra mã tài sản cố định đã tồn tại hay chưa
        /// </summary>
        /// <param name="assetCode"></param>
        /// <returns></returns>
        Task<bool> IsCodeExistsAsync(string assetCode);

        /// <summary>
        /// Lấy mã tài sản cố định mới khi tự động tăng hoặc nhân bản
        /// </summary>
        /// <returns></returns>
        Task<string> GetNewFixedAssetCodeAsync();

        /// <summary>
        /// Dữ liệu bảng tài sản cố định hiển thị trên grid
        /// </summary>
        /// <returns></returns>
        Task<List<FixedAssetDto>> GetFixedAssetGridAsync();

        /// <summary>
        /// Chi tiết tài sản cố định theo Id, bao gồm cả tên phòng ban và tên loại tài sản cố định
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<FixedAssetDto> GetFixedAssetDetailByIdAsync(string id);

        /// <summary>
        /// Lây danh sách tài sản cố định theo phân trang và lọc
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<PagingResult<FixedAssetDto>> GetFixedAssetsByFilterAsync(FixedAssetFilterDto filter);
    }
}
