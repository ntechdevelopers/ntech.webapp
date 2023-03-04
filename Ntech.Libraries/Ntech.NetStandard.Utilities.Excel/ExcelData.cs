using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntech.NetStandard.Utilities.Excel
{
    public abstract class ExcelData
    {
        protected ConcurrentBag<dynamic> _data;

        protected ExcelData()
        {
            _data = new ConcurrentBag<dynamic>();
        }
    }
}
