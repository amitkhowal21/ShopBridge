using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatePass.ViewModels.Common
{
    public class BaseSearchVM
    {
        public int PageSize { get; set; } = 10;
        public int OffSet { get; set; }
        public string[] SortNames { get; set; }
        public string[] SortDirections { get; set; }
        public string Keywords { get; set; }

        public int Id { get; set; }
    }
    public class BaseSearchVM<T> : BaseSearchVM
    {
        public T Params { get; set; }
    }
}
