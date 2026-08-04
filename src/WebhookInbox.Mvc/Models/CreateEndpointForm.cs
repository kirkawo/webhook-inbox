using System.ComponentModel.DataAnnotations;

namespace WebhookInbox.Mvc.Models;

public sealed class CreateEndpointForm
{
    [Required]
    [MaxLength(100)]
    [Display(Name = "Name")]
    public string? Name { get; set; }

    [Range(1, 3650)]
    [Display(Name = "Expiration (days, optional)")]
    public int? ExpiresInDays { get; set; }
}
