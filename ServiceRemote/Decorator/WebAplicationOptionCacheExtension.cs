using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ServiceRemote.Config;

namespace ServiceRemote.Decorator;

public static class WebAplicationOptionCacheExtension
{
    public static void GetCache(this WebApplicationBuilder app, AppUserConfig config)
    {
        if (config.GetCacheType == "Memory")
        {
            app.Services.AddMemoryCache();
        }
        else if(config.GetCacheType == "Distributed")
        {
            app.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = ConfigRedis.ConnectionString;
                options.InstanceName = "UsersCache_";
            });
        }
    }
}