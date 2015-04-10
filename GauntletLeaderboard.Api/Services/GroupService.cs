﻿using GauntletLeaderboard.Api.Data;
using GauntletLeaderboard.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GauntletLeaderboard.Api.Services
{
    public class GroupService : IGroupService
    {
        readonly IInterestedLeaderboardRepository leaderboardRepository;

        public GroupService(IInterestedLeaderboardRepository leaderboardRepository)
        {
            this.leaderboardRepository = leaderboardRepository;
        }

        public IEnumerable<Group> All()
        {
            return this.leaderboardRepository
                       .GetLeaderboards()
                       .GroupBy(l => l.Group)
                       .Select(g => new Group
                       {
                           Name = g.Key,
                           IsActive = g.Any(l => l.IsActive)
                       })
                       .ToArray();
        }

        public Group GetByName(string groupName)
        {
            return this.leaderboardRepository
                       .GetLeaderboards()
                       .GroupBy(l => l.Group)
                       .Where(g => g.Key.Equals(groupName, StringComparison.OrdinalIgnoreCase))
                       .Select(g => new Group
                       {
                           Name = g.Key,
                           IsActive = g.Any(l => l.IsActive)
                       })
                       .Single();
        }

        public IEnumerable<SubGroup> GetSubGroupsByName(string subGroupName)
        {
            return this.leaderboardRepository
                       .GetLeaderboards()
                       .Where(l => l.SubGroup.Equals(subGroupName, StringComparison.OrdinalIgnoreCase))
                       .Select(l => new SubGroup
                       {
                           Group = l.Group,
                           Name = l.SubGroup,
                           IsActive = l.IsActive
                       })
                       .GroupBy(l => l.Name)
                       .Select(g => g.First())
                       .ToArray();
        }

        public IEnumerable<SubGroup> GetSubGroupsByGroup(string groupName)
        {
            return this.leaderboardRepository
                       .GetLeaderboards()
                       .Where(l => l.Group.Equals(groupName, StringComparison.OrdinalIgnoreCase))
                       .Select(l => new SubGroup
                       {
                           Group = l.Group,
                           Name = l.SubGroup,
                           IsActive = l.IsActive
                       })
                       .GroupBy(l => l.Name)
                       .Select(g => g.First())
                       .ToArray();
        }
    }
}