using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LMS.Data;
using LMS.Models;
using LMS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Http;

namespace LMS
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {

      services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("IdentityConnection")));

      services.AddIdentity<ApplicationUser, IdentityRole>(options =>
      {
        // Password settings
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 1;
        options.Password.RequiredUniqueChars = 1;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;

      })
      .AddEntityFrameworkStores<ApplicationDbContext>()
      .AddDefaultTokenProviders();

      // Add application services.
      services.AddTransient<IEmailSender, EmailSender>();

      services.AddMvc();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
    {
      
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      app.UseHttpsRedirection();

      app.UseStaticFiles();

      app.UseAuthentication();

      app.UseMvc(routes =>
      {
        routes.MapRoute(
                  name: "default",
                  template: "{controller=Home}/{action=Index}/{id?}");
      });

      CreateUserRoles(services).Wait();
    }

    private async Task CreateUserRoles(IServiceProvider serviceProvider)
    {
      var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
      var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();


      IdentityResult roleResult;
      // Make sure admin, prof, and student roles exist
      // This should only happen the first time the server starts up
      var hasAdmin = await RoleManager.RoleExistsAsync("Administrator");
      if (!hasAdmin)
      {
        roleResult = await RoleManager.CreateAsync(new IdentityRole("Administrator"));
      }

      var hasProfessor = await RoleManager.RoleExistsAsync("Professor");
      if (!hasProfessor)
      {
        roleResult = await RoleManager.CreateAsync(new IdentityRole("Professor"));
      }

      var hasStudent = await RoleManager.RoleExistsAsync("Student");
      if (!hasStudent)
      {
        roleResult = await RoleManager.CreateAsync(new IdentityRole("Student"));
      }
    }

  }
}

