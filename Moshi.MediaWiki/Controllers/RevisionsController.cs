using Microsoft.AspNetCore.Mvc;
using Moshi.MediaWiki.Models;
using Moshi.MediaWiki.Services;

namespace Moshi.MediaWiki.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RevisionsController : ControllerBase
{
    private readonly RevisionService _revisionService;

    public RevisionsController(RevisionService revisionService)
    {
        _revisionService = revisionService;
    }

    [HttpGet("{id}")]
    public ActionResult<Revision> GetRevision(int id)
    {
        var revision = _revisionService.GetRevisionById(id);
        if (revision == null)
        {
            return NotFound();
        }
        return revision;
    }

    [HttpPost]
    public ActionResult<Revision> CreateRevision(Revision revision)
    {
        try
        {
            _revisionService.CreateRevision(revision);
            return CreatedAtAction(nameof(GetRevision), new { id = revision.RevisionId }, revision);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}