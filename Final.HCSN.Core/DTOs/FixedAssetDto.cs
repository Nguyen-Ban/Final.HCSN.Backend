using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.HCSN.Core.DTOs
{
    public class FixedAssetDto
    {
        public string fixed_asset_id { get; set; }
        public string fixed_asset_code { get; set; }
        public string fixed_asset_name { get; set; }
        public string department_id { get; set; }
        public string fixed_asset_category_id { get; set; }

        public string department_code { get; set; }
        public string fixed_asset_category_code { get; set; }

        public string department_name { get; set; } // Hiển thị thay vì ID 
        public string fixed_asset_category_name { get; set; } // Hiển thị thay vì ID 
        public decimal quantity { get; set; }
        public decimal cost { get; set; }
        public decimal depreciation_value_year { get; set; }
        public DateTime purchase_date { get; set; }
        // Thêm các trường khác nếu cần hiển thị trên Grid
        public decimal depreciation_rate { get; set; }
    }
}
