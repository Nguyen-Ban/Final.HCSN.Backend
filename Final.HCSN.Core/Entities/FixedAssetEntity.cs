using Final.HCSN.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.HCSN.Core.Entities
{
    /// <summary>
    /// Bảng Tài sản cố định
    /// </summary>
    [TableName("fixed_asset")]
    public class FixedAssetEntity
    {
        [PrimaryKey]
        [ColumnName("fixed_asset_id")]
        public Guid? fixed_asset_id { get; set; }
        public string fixed_asset_code { get; set; }
        public string fixed_asset_name { get; set; }
        public Guid department_id { get; set; }
        public Guid fixed_asset_category_id { get; set; }
        public DateTime purchase_date { get; set; }
        public int production_year { get; set; }
        public int tracked_year { get; set; }
        public decimal quantity { get; set; } // Decimal(18,4)
        public decimal cost { get; set; } // Decimal(22,4)
        public decimal depreciation_rate { get; set; }
        public decimal depreciation_value_year { get; set; }
    }
}
