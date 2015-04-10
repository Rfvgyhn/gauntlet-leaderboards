﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GauntletLeaderboard.Api.Model
{
    [XmlRoot("entry")]
    public class Entry
    {
        [XmlElement("steamid")]  
        public string SteamId { get; set; }
        public SteamProfile Player { get; set; }
        [XmlElement("score")]
        public int Score { get; set; }
        [XmlElement("rank")]
        public int Rank { get; set; }
    }
}