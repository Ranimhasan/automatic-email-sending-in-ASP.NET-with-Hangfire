using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using Hangfire;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Owin;
using Owin;

using Sozlesme_Takip_Sistemi1.Controllers;

[assembly: OwinStartupAttribute(typeof(Sozlesme_Takip_Sistemi1.Startup))]

namespace Sozlesme_Takip_Sistemi1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            // Hangfire için depolama ayarları
            GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection");

            // Hangfire dashboard ve server'ı kullanma
            app.UseHangfireDashboard("/myJobDashboard");
            app.UseHangfireServer();

            // Özel bir BackgroundJobServerOptions kullanarak Hangfire server'ı başlatma
            
          
            RecurringJob.AddOrUpdate(() => new TestController().SendEmail(), Cron.Monthly(1));
        }

        private void ConfigureAuth(IAppBuilder app)
        {
            // Kimlik doğrulama 
        }
    }
}
