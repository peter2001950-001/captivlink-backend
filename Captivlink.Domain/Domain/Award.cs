﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Infrastructure.Domain.Enums;

namespace Captivlink.Infrastructure.Domain
{
    public class Award : Entity
    {
        public AwardType Type { get; set; }
        public decimal Amount { get; set; }
    }
}
