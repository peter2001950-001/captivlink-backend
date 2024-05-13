using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivlink.Infrastructure.Events
{
    public class ClickEvent : BaseEvent
    {
        public override string Type => "ClickEvent";

    }
}
