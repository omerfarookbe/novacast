using System;
using System.Collections.Generic;
using System.Text;

namespace NovaCast.Contracts.Enums
{
    public enum NotificationStatus
    {
        Received,
        Published,
        Retrying,
        Delivered,
        Failed,
        DeadLettered
    }
}
