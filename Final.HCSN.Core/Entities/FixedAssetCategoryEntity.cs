using Final.HCSN.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.HCSN.Core.Entities
{
    [Attributes.TableName("fixed_asset_category")]
    public class FixedAssetCategoryEntity
    {
        [PrimaryKey]
        [Attributes.ColumnName("fixed_asset_category_id")]
        public Guid? fixed_asset_category_id { get; set; }

        [Attributes.ColumnName("fixed_asset_category_code")]
        public string fixed_asset_category_code { get; set; }

        [Attributes.ColumnName("fixed_asset_category_name")]
        public string fixed_asset_category_name { get; set; }

        [Attributes.ColumnName("life_time")]
        public int life_time { get; set; } // Số năm sử dụng

        [Attributes.ColumnName("depreciation_rate")]
        public decimal depreciation_rate { get; set; } // Tỷ lệ hao mòn
    }
}
