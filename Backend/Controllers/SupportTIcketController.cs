using Microsoft.AspNetCore.Mvc;
using Backend.Domain;
using Backend.DbConnection; 

namespace Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SupportTicketController : ControllerBase
{
    private readonly ILogger<SupportTicketController> _logger;
    private readonly NHibernate.ISession _session;
    private readonly IUnitOfWorkFactory _uowFactory;

    public SupportTicketController(ILogger<SupportTicketController> logger, NHibernate.ISession session, IUnitOfWorkFactory uowFactory)
    {
        _logger = logger;
        _session = session;
        _uowFactory = uowFactory;
    }

    [HttpGet("execute")]
    public async Task<IActionResult> ExecutePlan()
    {
        _logger.LogInformation("Executing plan...");

        List<Organization> organizations = [.. _session.Query<Organization>()];

        Organization org = new Organization
        {
            Name = "New Organization",
            Slug = "new-organization"
        };

        using var uow = _uowFactory.Create();

        uow.BeginTransaction();

        await _session.SaveAsync(org);
            
        // auto generated date and bool have problems--have to check
        await uow.CommitAsync();


        return Ok(organizations);
    }
}
