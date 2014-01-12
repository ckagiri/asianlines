﻿using System;
using Ligi.Core.Model;

namespace Ligi.Core.Events.Domain
{
    public class SeasonAccountUpdated : VersionedEvent
    {
        public Guid BookieId { get; set; }
        public SeasonAccount SeasonAccount { get; set; }
    }
}
