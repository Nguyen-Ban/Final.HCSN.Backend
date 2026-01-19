using Final.HCSN.Core.Entities;
using Final.HCSN.Core.Interface.Repository;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.HCSN.Infrastructure.Repository
{
    public class FixedAssetCategoryRepo : BaseRepo<FixedAssetCategoryEntity>, IFixedAssetCategoryRepo
    {
        public FixedAssetCategoryRepo(IConfiguration config) : base(config)
        {
        }
    }
}
