using Final.HCSN.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.HCSN.Core.Interface.Service
{
    public interface IFixedAssetService
    {
        Task<int> CreateFixedAssetAsync(FixedAssetCreateUpdateDto fixedAssetDto);
        Task<int> UpdateFixedAssetAsync(string id, FixedAssetCreateUpdateDto fixedAssetDto);

        /// <summary>
        /// Lấy danh sách tài sản cố định có phân trang
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<PagingResult<FixedAssetDto>> GetFixedAssetsPagingAsync(FixedAssetFilterDto filter);

        /// <summary>
        /// Lấy mã tài sản cố định mới khi tự động tăng cho thêm, nhân bản
        /// </summary>
        /// <returns></returns>
        Task<string> GetNewFixedAssetCodeAsync();

        /// <summary>
        /// Nhân bản tài sản cố định, mã tài sản là mã lớn nhất hiện tại + 1
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        Task<int> DuplicateFixedAssetAsync(string sourceId);
    }
}
