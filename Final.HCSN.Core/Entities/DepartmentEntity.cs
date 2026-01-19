using Final.HCSN.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.HCSN.Core.Entities
{
    [Attributes.TableName("department")]
    public class DepartmentEntity
    {
        [PrimaryKey]
        public Guid? department_id { get; set; }
        public string department_code { get; set; }
        public string department_name { get; set; }
    }
}
