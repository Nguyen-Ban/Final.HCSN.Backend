using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.HCSN.Core.Attributes
{
    public class ColumnNameAttribute : Attribute
    {
        public string ColumnName { get; }
        public ColumnNameAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : Attribute { }
}
