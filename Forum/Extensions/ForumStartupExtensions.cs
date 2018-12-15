﻿using Forum.Contexts;
using Forum.Interfaces.Services;
using Forum.Middleware;
using Forum.Services;
using Jdenticon;
using Jdenticon.Rendering;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// REMINDER -
// Transient: created each time they are requested. This lifetime works best for lightweight, stateless services.
// Scoped: created once per request.
// Singleton: created the first time they are requested (or when ConfigureServices is run if you specify an instance there) and then every subsequent request will use the same instance.

namespace Forum.Extensions {
	public static class ForumStartupExtensions {
		public static IApplicationBuilder UseForum(this IApplicationBuilder builder) {
			Identicon.DefaultStyle = new IdenticonStyle {
				BackColor = Color.Transparent,
			};

			builder.UseMiddleware<HttpStatusCodeHandler>();
			builder.UseMiddleware<PageTimer>();
			builder.UseMiddleware<UserContextLoader>();

			return builder;
		}

		public static IServiceCollection AddForum(this IServiceCollection services, IConfiguration configuration) {
			RegisterRepositories(services, configuration);

			services.AddScoped<UserContext>();
			services.AddTransient<Sidebar>();
			services.AddTransient<IForumViewResult, ForumViewResult>();
			services.AddTransient<GzipWebClient>();

			services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
			services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();

			services.AddSingleton((serviceProvider) => {
				return BBCParserFactory.GetParser();
			});

			services.AddTransient<SetupService>();

			return services;
		}

		static void RegisterRepositories(IServiceCollection services, IConfiguration configuration) {
			services.AddScoped<Repositories.AccountRepository>();
			services.AddScoped<Repositories.BoardRepository>();
			services.AddScoped<Repositories.MessageRepository>();
			services.AddScoped<Repositories.NotificationRepository>();
			services.AddScoped<Repositories.PinRepository>();
			services.AddScoped<Repositories.QuoteRepository>();
			services.AddScoped<Repositories.RoleRepository>();
			services.AddScoped<Repositories.SettingsRepository>();
			services.AddScoped<Repositories.SmileyRepository>();
			services.AddScoped<Repositories.TopicRepository>();
		}
	}
}