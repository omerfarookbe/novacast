using NovaCast.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NovaCast.Contracts.Events
{
    public class NotificationEvent
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Type { get; set; } = default!;
        public NotificationChannel Channel { get; set; }
        public string Recipient { get; set; } = default!;
        public Dictionary<string, object> Payload { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
