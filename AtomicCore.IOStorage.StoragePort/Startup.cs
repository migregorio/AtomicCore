using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
        /// ���캯��
        /// </summary>
        /// <param name="configuration">ϵͳ����</param>
        /// <param name="env">WebHost����</param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            WebHostEnvironment = env;
        }

        #endregion

        #region Propertys

        /// <summary>
        /// ϵͳ����
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// WebHost����
        /// </summary>
        public IWebHostEnvironment WebHostEnvironment { get; }

        #endregion

        #region Public Methods

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            #region AtomicCore�����ʼ��

            AtomicCore.AtomicKernel.Initialize();

            #endregion

            #region ���л�������Linux or IIS��

            ///* ���������linuxϵͳ�ϣ���Ҫ������������ã� */
            //services.Configure<KestrelServerOptions>(options => 
            //{
            //    options.AllowSynchronousIO = true;
            //});

            ///* ���������IIS�ϣ���Ҫ������������ã� */
            //services.Configure<IISServerOptions>(options => 
            //{
            //    options.AllowSynchronousIO = true;
            //    options.AutomaticAuthentication = false;
            //});
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
            /* ����DEBUGģʽ */
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
