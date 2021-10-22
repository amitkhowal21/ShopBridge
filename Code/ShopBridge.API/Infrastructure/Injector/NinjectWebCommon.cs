
[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(ShopBridge.API.Infrastructure.Injector.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(ShopBridge.API.Infrastructure.Injector.NinjectWebCommon), "Stop")]

namespace ShopBridge.API.Infrastructure.Injector
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Modules;
    using Ninject.Web.Common;
    using Ninject.Web.Common.WebHost;
    using ShopBridge.Services.Products.Brand;
    using ShopBridge.Services.Products.Stock;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {


            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

            RegisterServices(kernel);

            NinjectDependencyResolver ninjectResolver = new NinjectDependencyResolver(kernel);
            //DependencyResolver.SetResolver(ninjectResolver); //MVC 
            GlobalConfiguration.Configuration.DependencyResolver = ninjectResolver; //Web API

            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {

            kernel.Bind<IProductStock>().To<ProductStockService>();
            kernel.Bind<IProductBrand>().To<ProductBrandService>();

            //var modules = new List<INinjectModule>
            //    {
            //        new ServiceModule()
            //    };
            //kernel.Load(modules);
        }
    }
}