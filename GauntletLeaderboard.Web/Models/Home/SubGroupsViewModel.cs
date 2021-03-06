﻿using GauntletLeaderboard.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GauntletLeaderboard.Web.Models.Home
{
    public class SubGroupsViewModel : GroupsViewModel
    {
        public Group Group { get; set; }
        public IEnumerable<SubGroup> SubGroups { get; set; }
    }
}