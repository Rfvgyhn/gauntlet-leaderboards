﻿using Autofac;
using GauntletLeaderboard.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac.Integration.Mvc;
using GauntletLeaderboard.Core.Data;
using System.IO;
using System.Web.Hosting;
using System.Runtime.Caching;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GauntletLeaderboard.Web.App_Start
{
    public static class Bootstrapper
    {
        public static void Run()
        {
            var container = ConfigureContainer();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        private static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            var steamApiKey = WebConfigurationManager.AppSettings["steamApiKey"];
            var leaderboardUrl = WebConfigurationManager.AppSettings["leaderboardUrl"];
            var leaderboardForProfileUrl = WebConfigurationManager.AppSettings["leaderboardForProfileUrl"];
            var profileUrl = WebConfigurationManager.AppSettings["profileUrl"];
            var achievementsUrl = WebConfigurationManager.AppSettings["achievementsUrl"];
            var badgesUrl = WebConfigurationManager.AppSettings["badgesUrl"];
            var vanityUrl = WebConfigurationManager.AppSettings["vanityUrl"];
            var currentPlayersUrl = WebConfigurationManager.AppSettings["currentPlayersUrl"];
            var appId = int.Parse(WebConfigurationManager.AppSettings["appId"]);
            var leaderboardPath = HostingEnvironment.MapPath("~/leaderboards.json");

            builder.Register(c => MemoryCache.Default)
                   .As<ObjectCache>()
                   .SingleInstance();

            builder.RegisterType<FileInterestedLeaderboardRepository>()
                   .WithParameter("filePath", leaderboardPath)
                   .As<IInterestedLeaderboardRepository>()
                   .InstancePerRequest();

            builder.RegisterType<SteamProfileRepository>()
                   .WithParameters(new[]
                   {
                       new NamedParameter("steamApiKey", steamApiKey),
                       new NamedParameter("profileUrl", profileUrl),
                       new NamedParameter("achievementsUrl", achievementsUrl),
                       new NamedParameter("badgesUrl", badgesUrl),
                       new NamedParameter("vanityUrl", vanityUrl),
                       new NamedParameter("appId", appId),
                   })
                   .As<IProfileRepository>()
                   .InstancePerRequest();

            builder.RegisterType<SteamLeaderboardRepository>()
                   .WithParameters(new[]
                   {
                       new NamedParameter("leaderboardUrl", leaderboardUrl),
                       new NamedParameter("leaderboardForProfileUrl", leaderboardForProfileUrl),
                   })
                   .As<ILeaderboardRepository>()
                   .InstancePerRequest();

            builder.RegisterType<PlayerService>()
                   .As<IPlayerService>()
                   .InstancePerRequest();

            builder.RegisterType<GameService>()
                   .WithParameters(new[]
                      {
                          new NamedParameter("steamApiKey", steamApiKey),
                          new NamedParameter("currentPlayersUrl", currentPlayersUrl),
                      })
                   .As<IGameService>()
                   .InstancePerRequest();

            builder.RegisterType<LeaderboardService>()
                   .As<ILeaderboardService>()
                   .InstancePerRequest();

            builder.RegisterType<GroupService>()
                   .As<IGroupService>()
                   .InstancePerRequest();

            return builder.Build();
        }
    }
}