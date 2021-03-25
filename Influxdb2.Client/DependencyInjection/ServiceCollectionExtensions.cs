﻿using Influxdb2.Client;
using Microsoft.Extensions.Options;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// ServiceCollection扩展
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加IInfuxdbClient
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddInfuxdbClient(this IServiceCollection services, Action<InfuxdbOptions> configureOptions)
        {
            var builder = services.AddHttpApi<IInfuxdbClient>();
            var name = builder.Name;

            services.Configure(name, configureOptions);
            return builder.ConfigureHttpClient((sp, httpClient) =>
            {
                var infuxdb = sp.GetRequiredService<IOptionsMonitor<InfuxdbOptions>>().Get(name);
                httpClient.BaseAddress = infuxdb.Host;
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Token {infuxdb.Token}");
            });
        }
    }
}
