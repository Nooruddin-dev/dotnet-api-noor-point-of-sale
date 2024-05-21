using DAL.DBContext;
using DAL.Repository.Services;
using DAL.Repository.IServices;
using PetaPoco;
using Microsoft.EntityFrameworkCore;
using Helpers.CommonHelpers;
using Helpers.AuthorizationHelpers;
using Helpers.ConversionHelpers;
using System.Configuration;
using Helpers.AuthorizationHelpers.JwtTokenHelper;
using DocumentFormat.OpenXml.Presentation;
using Entities.DBModels;
using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using Helpers.ConversionHelpers.IServices;
using Helpers.ConversionHelpers.Services;


namespace QRCode.Noor.API.Helpers
{
    public static class ServiceExtensions
    {

        public static string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public static void ConfigureIServiceCollection(this WebApplicationBuilder builder)
        {

            #region CORS setting for API

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.AllowAnyOrigin()
                                                          .AllowAnyHeader()
                                                          .AllowAnyMethod();

                                      //policy.WithOrigins("http://noornashad-001-site5.etempurl.com")
                                      //             .AllowAnyHeader()
                                      //             .AllowAnyMethod();
                                  });
            });
            #endregion


            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            #region main services registeration
            //---These are the services that we will use in our project for CRUD
            builder.Services.AddSingleton<IDatabase, Database>();
            builder.Services.AddSingleton<IDataContextHelper, DataContextHelper>();
            builder.Services.AddSingleton<IDapperConnectionHelper, DapperConnectionHelper>();
            builder.Services.AddSingleton<IConstants, Constants>();
            builder.Services.AddSingleton<ICalculationHelper, CalculationHelper>();
            builder.Services.AddSingleton<IFilesHelpers, FilesHelpers>();
            builder.Services.AddSingleton<ICommonServicesDAL, CommonServicesDAL>();
            builder.Services.AddSingleton<IUsersServicesDAL, UsersServicesDAL>();
            builder.Services.AddSingleton<IProductsCatalogServicesDAL, ProductsCatalogServicesDAL>();
            builder.Services.AddSingleton<ISalesManagementServicesDAL, SalesManagementServicesDAL>();
            builder.Services.AddSingleton<ISettingServicesDAL, SettingServicesDAL>();
            builder.Services.AddSingleton<ICashierMainServicesDAL, CashierMainServicesDAL>();
            builder.Services.AddSingleton<IDiscountsServicesDAL, DiscountsServicesDAL>();
            builder.Services.AddSingleton<IDataAnalyticsServicesDAL, DataAnalyticsServicesDAL>();
            builder.Services.AddSingleton<IShiftManagementServicesDAL, ShiftManagementServicesDAL>();
          




            //--For session and cookies purpose
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //--Different attributes registeration
            builder.Services.AddScoped<CustomerApiCallsAuthorization>();

            #endregion

            //--Configure IConfiguration Interface for static methods
            StaticMethodsDependencyInjctHelper.Initialize(builder.Configuration, null);

  
    
      

        }

        public static void ConfigureWebApplication(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

           
            app.UseRouting();

            app.UseAuthorization();
            app.MapControllers();


            app.UseCors(MyAllowSpecificOrigins);


            ConfigureRoutes(app);


         





        }

        public static void ConfigureRoutes(this WebApplication app)
        {



            #region Other Routes

            //app.MapControllerRoute(
            //    name: "ProductsList",
            //    pattern: "{langCode}/products-catalog/products-list",
            //    defaults: new { controller = "ProductsCatalog", action = "ProductsList" });

            #endregion

            #region Admin Panel Web Routes
            //app.MapControllerRoute(
            //  name: "default",
            //  pattern: "{controller=Home}/{action=Index}/{id?}");
            #endregion

            #region API Routes
            //app.MapControllerRoute(
            //     name: "V1",
            //     pattern: "{area:exists}/api/dynamic/dataoperation/{UrlName?}",
            //     new { action = "DataOperation", controller = "ApiDynamic" }
            //   );
            #endregion


        }
    }
}
