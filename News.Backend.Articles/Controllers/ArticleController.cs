using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using News.Backend.Articles.Models;
using News.Backend.Articles.Services;

namespace News.Backend.Articles.Controllers;

[Route("[controller]")]
[ApiController]
[Produces("application/json")]
public class ArticleController : ControllerBase
{
    private readonly ArticleService service;
    private readonly ILogger<ArticleController> logger;
    public ArticleController(ArticleService articleService, ILogger<ArticleController> log)
    {
        service = articleService;
        logger = log;
    }

    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<bool>> All(CancellationToken ct = default)
    {
        logger.LogDebug("Getting all articles");
        var result = await service.All(ct);
        if (result == null) return NoContent();
        else return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<Article>>> GetById(int id)
    {
        logger.LogDebug("Getting by id article");
        var result = await service.Get(id);
        if (result == null)
        {
            logger.LogInformation($"Not found article by {id}");
            return NoContent();
        }
        else return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Article>> Create(Article article)
    {
        logger.LogDebug("Creating article");
        if (!ModelState.IsValid)
        {
            logger.LogInformation($"Model is not valid");
            return BadRequest("Model is not valid");
        }
        var result = await service.Create(article);
        if (result.Success == false)
        {
            logger.LogInformation($"Error: {result.ErrorMessage}");
            return BadRequest(result.ErrorMessage);
        }
        return CreatedAtAction(nameof(GetById), new { id = article.Id }, article);
    }

    [HttpPut]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<bool>> Update(Article article)
    {
        logger.LogDebug("Updating article");
        var result = await service.Update(article);
        if (result.Success == false)
        {
            logger.LogInformation($"Error: {result.ErrorMessage}");
            return BadRequest(result.ErrorMessage);
        }
        return Ok(result.Success);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<bool>> Delete(int id)
    {
        logger.LogDebug("Updating article");
        var result = await service.Delete(id);
        if (result.Success == false)
        {
            logger.LogInformation($"Error: {result.ErrorMessage}");
            return BadRequest(result.ErrorMessage);
        }
        return Ok(result.Success);
    }
}
