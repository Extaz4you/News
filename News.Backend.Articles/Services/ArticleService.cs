using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Identity.Client;
using News.Backend.Articles.Db;
using News.Backend.Articles.Models;
using News.Backend.Articles.Services.Interfaces;
using System.Collections;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace News.Backend.Articles.Services;

public class ArticleService : IArticleRepository
{
    private readonly ArticlesContext context;
    private readonly IDistributedCache cache;
    public ArticleService(ArticlesContext articlesContext, IDistributedCache distributedCache)
    {
        context = articlesContext;
        cache = distributedCache;
    }

    public async Task<IEnumerable<Article>> All(CancellationToken ct = default)
    {
        var cacheKey = $"AllNews";
        byte[] cached = await cache.GetAsync(cacheKey);
        if(cached != null) return JsonSerializer.Deserialize<List<Article>>(cached);
        var articles = await context.Articles.ToListAsync(ct);
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };
        await cache.SetAsync(cacheKey, JsonSerializer.SerializeToUtf8Bytes(articles), cacheOptions);
        return articles;
    }

    public async Task<Article?> Get(int id)
    {
        var cacheKey = $"News_{id}";

        byte[] cached = await cache.GetAsync(cacheKey);
        if (cached != null) return JsonSerializer.Deserialize<Article>(cached);

        var article = await context.Articles.FirstOrDefaultAsync(x => x.Id == id);
        if (article == null) return null;
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };
        await cache.SetAsync(cacheKey, JsonSerializer.SerializeToUtf8Bytes(article), cacheOptions);

        return article;
    }

    public async Task<OperationResult> Delete(int id)
    {
        var article = await context.Articles.FirstOrDefaultAsync(x=>x.Id == id);
        if(article == null) return new OperationResult(false, "Article not found");
        try
        {
            context.Articles.Remove(article);
            await context.SaveChangesAsync();
            await cache.RemoveAsync($"News_{id}");
            await cache.RemoveAsync($"AllNews");
            return new OperationResult(true);
        }
        catch(Exception ex) { return new OperationResult(false, ex.Message); }
    }

    public async Task<OperationResult> Create(Article article)
    {
        if (article == null) return new OperationResult(false);
        var foundArticle = await context.Articles.AnyAsync(x => x.Theme == article.Theme);
        if (foundArticle == true) return new OperationResult(false, "Already exist");
        try
        {
            await context.AddAsync(article);
            await context.SaveChangesAsync();
            await cache.RemoveAsync($"AllNews");
            return new OperationResult(true);
        }
        catch (Exception ex) { return new OperationResult(false, ex.Message); }
    }

    public async Task<OperationResult> Update(Article article)
    {
        if (article == null) return new OperationResult(false); ;

        var updatedArticle = await context.Articles.FirstOrDefaultAsync(x => x.Id == article.Id);
        if (updatedArticle == null) return new OperationResult(false, "Article not found");
        try
        {
            updatedArticle.Theme = article.Theme;
            updatedArticle.Content = article.Content;
            await context.SaveChangesAsync();
            await cache.RemoveAsync($"News_{article.Id}");
            await cache.RemoveAsync($"AllNews");
            return new OperationResult(true);
        }
        catch (Exception ex) { return new OperationResult(false, ex.Message); }
    }
}
public record OperationResult(bool Success, string? ErrorMessage = null);
