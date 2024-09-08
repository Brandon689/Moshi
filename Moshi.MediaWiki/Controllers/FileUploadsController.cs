using Microsoft.AspNetCore.Mvc;
using Moshi.MediaWiki.Models;
using Moshi.MediaWiki.Services;

namespace Moshi.MediaWiki.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadsController : ControllerBase
{
    private readonly FileUploadService _fileUploadService;

    public FileUploadsController(FileUploadService fileUploadService)
    {
        _fileUploadService = fileUploadService;
    }

    [HttpGet("{id}")]
    public ActionResult<FileUpload> GetFileUpload(int id)
    {
        var fileUpload = _fileUploadService.GetFileUploadById(id);
        if (fileUpload == null)
        {
            return NotFound();
        }
        return fileUpload;
    }

    [HttpPost]
    public ActionResult<FileUpload> CreateFileUpload(FileUpload fileUpload)
    {
        try
        {
            _fileUploadService.CreateFileUpload(fileUpload);
            return CreatedAtAction(nameof(GetFileUpload), new { id = fileUpload.FileId }, fileUpload);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}