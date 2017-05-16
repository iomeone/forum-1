﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Forum3.Data;
using Forum3.Helpers;
using Forum3.Annotations;
using Forum3.Models.DataModels;

namespace Forum3 {
	public class Startup {
		public Startup(IHostingEnvironment env) {
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

			if (env.IsDevelopment()) {
				builder.AddUserSecrets<Startup>();
			}

			builder.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		public void ConfigureServices(IServiceCollection services) {
			// Loads from the user-secrets store
			var connectionString = Configuration["DefaultConnection"];

			// secrets store is empty, use the one defined in appsettings.json
			if (string.IsNullOrEmpty(connectionString))
				connectionString = Configuration.GetConnectionString("DefaultConnection");

			// Add framework services.
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString)
			);

			services.AddIdentity<ApplicationUser, IdentityRole>(o => {
				o.Password.RequireDigit = false;
				o.Password.RequireLowercase = false;
				o.Password.RequireUppercase = false;
				o.Password.RequireNonAlphanumeric = false;
				o.Password.RequiredLength = 3;
				o.Cookies.ApplicationCookie.LoginPath = "/Authentication/Login";
			})
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			services.Configure<MvcOptions>(options => {
				options.Filters.Add(new RequireRemoteHttpsAttribute());
			});

			services.AddMvc();

			services.AddDistributedMemoryCache();
			services.AddSession();
			services.AddForum();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
				app.UseBrowserLink();
			}

			app.UseStaticFiles();

			app.UseIdentity();

			app.UseSession();

			app.UseMvc(routes => {
				routes.MapRoute(
					name: "default",
					template: "{controller=Boards}/{action=Index}/{id?}/{page?}/{target?}");
			});
		}
	}
}
