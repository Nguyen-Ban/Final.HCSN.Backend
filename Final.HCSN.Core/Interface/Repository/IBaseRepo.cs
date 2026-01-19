using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.HCSN.Core.Interface.Repository
{
    public interface IBaseRepo<T>
    {
        /// <summary>
        /// Lấy tất cả bản ghi
        /// </summary>
        /// <returns></returns>
        Task<List<T>> GetAllAsync();

        /// <summary>
        /// Lay ban ghi theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T?> GetByIdAsync(string id);

        /// <summary>
        /// Tao moi 1 ban ghi
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> CreateAsync(T entity);

        /// <summary>
        /// Chỉnh sửa 1 bản ghi
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(T entity);

        /// <summary>
        /// Xóa 1 bản ghi
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(string id);

        /// <summary>
        /// Xóa nhiều bản ghi
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<int> DeleteManyAsync(List<string> ids);
    }
}
