using Microsoft.AspNetCore.Mvc;
using WebhookInbox.Application.Endpoints;
using WebhookInbox.Mvc.Models;

namespace WebhookInbox.Mvc.Controllers;

[Route("[controller]")]
public class EndpointsController : Controller
{
    private readonly EndpointService _endpoints;

    public EndpointsController(EndpointService endpoints)
    {
        _endpoints = endpoints;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var endpoints = await _endpoints.ListAsync(cancellationToken);
        return View(endpoints.Select(ToViewModel).ToList());
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View(new CreateEndpointForm());
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEndpointForm form, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var lifetime = form.ExpiresInDays is > 0 ? TimeSpan.FromDays(form.ExpiresInDays.Value) : (TimeSpan?)null;
        var created = await _endpoints.CreateAsync(new CreateEndpointRequest(form.Name!, lifetime), cancellationToken);
        return RedirectToAction(nameof(Details), new { id = created.EndpointId });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Details(string id, CancellationToken cancellationToken)
    {
        var endpoint = await _endpoints.GetAsync(id, cancellationToken);
        if (endpoint is null)
        {
            return NotFound();
        }

        return View(ToViewModel(endpoint));
    }

    [HttpPost("{id}/deactivate")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(string id, CancellationToken cancellationToken)
    {
        await _endpoints.DeactivateAsync(id, cancellationToken);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost("{id}/expire")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Expire(string id, CancellationToken cancellationToken)
    {
        await _endpoints.ExpireAsync(id, cancellationToken);
        return RedirectToAction(nameof(Details), new { id });
    }

    private EndpointViewModel ToViewModel(EndpointDto dto)
    {
        var webhookUrl = $"{Request.Scheme}://{Request.Host}/in/{dto.PathToken}";
        return EndpointViewModel.From(dto, DateTimeOffset.UtcNow, webhookUrl);
    }
}
