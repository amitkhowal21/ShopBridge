using ShopBridge.Model;
using ShopBridge.ViewModels.Products;

namespace ShopBridge.Services
{
    public class EntityMapper
    {
        private static readonly object sync = new object();
        private static AutoMapper.IMapper mapper = null;
        private static AutoMapper.MapperConfiguration config = null;

        /// <summary>
        /// Method to Instantiate Auto Mapper
        /// </summary>
        public static void InitializeMapper()
        {
            if (mapper == null)
            {
                lock (sync)
                {
                    if (mapper == null)
                    {
                        config = new AutoMapper.MapperConfiguration(cfg =>
                                                {
                                                    cfg.CreateMap<Product, ProductStockVM>().ReverseMap();
                                                    cfg.CreateMap<Product, ProductVM>().ReverseMap();
                                                    cfg.CreateMap<Product, ProductListVM>().ReverseMap();
                                                });

                        mapper = config.CreateMapper();
                    }
                }
            }
        }

        /// <summary>
        /// Method to get the destination object from source object
        /// </summary>
        /// <typeparam name="TDestination">Type of destination entity</typeparam>
        /// <param name="source">Source Object instance</param>
        /// <returns>returns the detination object instance</returns>
        public static TDestination Map<TDestination>(object source) where TDestination : class
        {
            if (mapper != null)
            {
                return mapper.Map<TDestination>(source);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Method to create mapping between given entities
        /// </summary>
        /// <typeparam name="TSource">Type of source entity</typeparam>
        /// <typeparam name="TDestination">Type of destination entity</typeparam>
        /// <param name="source">Source data</param>
        /// <returns>Destination data</returns>
        public static TDestination Map<TSource, TDestination>(TSource source) where TDestination : class
        {
            if (mapper != null)
            {
                return mapper.Map<TDestination>(source);
            }
            else
            {
                return null;
            }
        }
    }
}
