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
    public class DepartmentRepo : BaseRepo<DepartmentEntity>, IDepartmentRepo
    {
        public DepartmentRepo(IConfiguration config) : base(config)
        {
        }
    }
}
