using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using News.Backend.Articles.Models;
using News.Backend.Articles.Services;
using News.Backend.Articles.Services.Interfaces;

namespace News.Backend.Articles.Controllers;

[Route("[controller]")]
[ApiController]
[Produces("application/json")]
public class ArticleController : ControllerBase
{
    private readonly IArticleRepository service;
    private readonly ILogger<ArticleController> logger;

    public ArticleController(IArticleRepository articleService, ILogger<ArticleController> log)
    {
        service = articleService;
        logger = log;
    }


    /// <summary>
    /// Получить все новости
    /// </summary>
    /// <response code="200">Возвращает список</response>
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<bool>> All(CancellationToken ct = default)
    {
        var result = await service.All(ct);
        if (result == null)
        {
            logger.LogInformation($"Not found");
            return NoContent();
        }
        else return Ok(result);
    }


    /// <summary>
    /// Получить новость по ID
    /// </summary>
    /// <response code="200">Возвращает новость</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<Article>>> GetById(int id)
    {

        var result = await service.Get(id);
        if (result == null)
        {
            logger.LogInformation($"Not found article by {id}");
            return NoContent();
        }
        else return Ok(result);
    }


    /// <summary>
    /// Создать новость
    /// </summary>
    /// <response code="201">Возвращает ID созданной новости</response>
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Article>> Create(Article article)
    {
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


    /// <summary>
    /// Обновить новость
    /// </summary>
    /// <response code="200">Возвращает bool</response>
    [HttpPut]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<bool>> Update(Article article)
    {
        var result = await service.Update(article);
        if (result.Success == false)
        {
            logger.LogInformation($"Error: {result.ErrorMessage}");
            return BadRequest(result.ErrorMessage);
        }
        return Ok(result.Success);
    }


    /// <summary>
    /// Удалить новость по ID
    /// </summary>
    /// <response code="200">Возвращает bool</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<bool>> Delete(int id)
    {
        var result = await service.Delete(id);
        if (result.Success == false)
        {
            logger.LogInformation($"Error: {result.ErrorMessage}");
            return BadRequest(result.ErrorMessage);
        }
        return Ok(result.Success);
    }
}
