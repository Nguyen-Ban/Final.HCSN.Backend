using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.HCSN.Core.DTOs
{
    public class FixedAssetFilterDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Keyword { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? FixedAssetCategoryId { get; set; }
    }

    public class PagingResult<T>
    {
        public int TotalRecords { get; set; }
        public List<T> Data { get; set; }
    }
}
