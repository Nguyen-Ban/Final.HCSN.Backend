using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.HCSN.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableNameAttribute : Attribute
    {
        public string TableName { get; }
        public TableNameAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
