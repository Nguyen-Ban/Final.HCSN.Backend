using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Final.HCSN.Core.DTOs
{
    public class FixedAssetCreateUpdateDto
    {
        public string fixed_asset_code { get; set; } // Không được trùng 
        public string fixed_asset_name { get; set; }
        public string department_id { get; set; }
        public string fixed_asset_category_id { get; set; }

        public DateTime purchase_date { get; set; } // Mặc định ngày hiện tại 
        public int production_year { get; set; } // Tự động theo năm của purchase_date 
        public int tracked_year { get; set; } // Tự động theo năm của purchase_date 

        public decimal quantity { get; set; } // Số nguyên dương 
        public decimal cost { get; set; } // Nguyên giá 

        // Các trường phục vụ tính toán
        public decimal depreciation_rate { get; set; } // Lấy từ Loại tài sản 
        public decimal depreciation_value_year { get; set; } // Tự động tính = cost * rate / 100
    }
}
