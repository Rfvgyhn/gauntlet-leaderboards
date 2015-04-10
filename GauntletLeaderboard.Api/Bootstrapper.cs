﻿namespace GauntletLeaderboard.Api
{
    using GauntletLeaderboard.Api.Data;
    using GauntletLeaderboard.Api.Services;
    using Nancy;
    using Nancy.Bootstrapper;
    using Nancy.Json;
    using Nancy.TinyIoc;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System.IO;
    using System.Runtime.Caching;
    using System.Web.Configuration;
    using System.Linq;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            container.Register<ObjectCache, MemoryCache>(MemoryCache.Default);
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            var steamApiKey = WebConfigurationManager.AppSettings["steamApiKey"];
            var leaderboardUrl = WebConfigurationManager.AppSettings["leaderboardUrl"];
            var profileUrl = WebConfigurationManager.AppSettings["profileUrl"];

            container.Register<IInterestedLeaderboardRepository>((c, p) => new FileInterestedLeaderboardRepository(Path.Combine(container.Resolve<IRootPathProvider>().GetRootPath(), "leaderboards.json"), c.Resolve<ObjectCache>()));
            container.Register<IGroupService, GroupService>();
            container.Register<ILeaderboardService>((c, p) => new LeaderboardService(steamApiKey, profileUrl, leaderboardUrl, c.Resolve<IInterestedLeaderboardRepository>(), c.Resolve<ObjectCache>()));
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            pipelines.AfterRequest += (ctx) =>
            {
                var allowedOrigins = WebConfigurationManager.AppSettings["allowedOrigins"];
                ctx.Response.WithHeader("Access-Control-Allow-Origin", allowedOrigins)
                            .WithHeader("Access-Control-Allow-Methods", "GET OPTIONS")
                            .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type");
            };
        }
    }
}