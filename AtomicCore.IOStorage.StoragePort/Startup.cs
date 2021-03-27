using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AtomicCore.IOStorage.StoragePort
{
    /// <summary>
    /// NetCore Startup
    /// </summary>
    public class Startup
    {
        #region Constructor

        /// <summary>
        /// ע�빹�캯��
        /// </summary>
        /// <param name="configuration">���ýӿ�ע��</param>
        public Startup(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion

        #region Propertys

        /// <summary>
        /// ������Ϣ����
        /// </summary>
        public Microsoft.Extensions.Configuration.IConfiguration Configuration { get; }

        #endregion

        #region Public Methods

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            #region ����IIS�����ں˳���ģ�ͣ�https://docs.microsoft.com/zh-cn/aspnet/core/host-and-deploy/iis/?view=aspnetcore-3.1��

            //services.Configure<IISServerOptions>(options =>
            //{
            //    options.AutomaticAuthentication = false;
            //});

            #endregion

            #region ����IIS���������ģ��

            //services.Configure<IISOptions>(options =>
            //{
            //    options.ForwardClientCertificate = false;
            //});

            #endregion

            #region ���ض�ȡ�����AppSettings��

            IConfigurationSection appSettings = Configuration.GetSection("AppSettings");
            services.Configure<BizAppSettings>(appSettings);
            services.AddSingleton<IBizPathSrvProvider, BizPathSrvProvider>();

            #endregion

            #region �����ϴ�������Ʒ�ֵ���޸�Ĭ�Ϸ�ֵ��

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = long.MaxValue;
            });

            #endregion

            #region ���MVC����

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            /* ����MVC·�� */
            app.UseRouting();

            /* ����������� */
            app.UseResponseCaching();

            /* ����MVCĬ�Ϸ��� */
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );
            });
        }

        #endregion
    }
}
