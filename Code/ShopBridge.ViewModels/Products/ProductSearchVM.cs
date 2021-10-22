using GatePass.ViewModels.Common;
namespace ShopBridge.ViewModels.Products
{
    public class ProductSearchVM : BaseSearchVM
    {
        public string SKU { get; set; }
        public string Name { get; set; }
        public bool? OnlyFinished { get; set; }
        public bool? OnlyActive { get; set; }
    }
}
