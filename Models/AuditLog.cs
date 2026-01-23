using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Models
{
    [Table("AuditLogs", Schema = "Healthcare")]
    public class AuditLog
    {
        [Key]
        public int AuditLogId { get; set; }

        public string EntityName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        public DateTime ActionDate { get; set; } = DateTime.Now;
        public string? UserId { get; set; }
    }
}
