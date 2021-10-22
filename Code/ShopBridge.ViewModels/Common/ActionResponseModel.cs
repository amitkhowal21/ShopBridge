using ShopBridge.BuldingBlocks.AppEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopBridge.ViewModels.Common
{
   public class ActionResponseModel
    {
		public int Id { get; set; } //USED TO GET NEWLY INSERTED ENTITY ID
        public int Status { get; set; } = (int)ResponseStatus.Success;
        public bool IsSuccess { get { return Status == (int)ResponseStatus.Success; } }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
