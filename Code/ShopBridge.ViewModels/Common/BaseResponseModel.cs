using ShopBridge.BuldingBlocks.AppEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopBridge.ViewModels.Common
{
    public class BaseResponseModel<T>
    {
        public int Status { get; set; } = (int)ResponseStatus.Success;
        public bool IsSuccess
        {
            get { return Status == (int)ResponseStatus.Success; }
        }
        //RESPONSE MESSAGE
        public string Message { get; set; }
        //RESPONSE DATA OBJECT
        public T Data { get; set; }
        //RESPONSE TOTAL ITEMS COUNT
        public int TotalItems { get; set; }
    }

}
