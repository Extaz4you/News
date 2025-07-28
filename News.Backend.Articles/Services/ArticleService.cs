using Microsoft.EntityFrameworkCore;
using News.Backend.Articles.Db;
using News.Backend.Articles.Models;
using System.Collections;
using System.Reflection.Metadata.Ecma335;

namespace News.Backend.Articles.Services;

public class ArticleService
{
    private ArticlesContext context;
    public ArticleService(ArticlesContext articlesContext)
    {
        context = articlesContext;
    }

    public async Task<IEnumerable<Article>> All(CancellationToken ct = default)
    {
        return await context.Articles.ToListAsync(ct);
    }

    public async Task<Article?> Get(int id)
    {
        return await context.Articles.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<OperationResult> Delete(int id)
    {
        var article = await context.Articles.FirstOrDefaultAsync(x=>x.Id == id);
        if(article == null) return new OperationResult(false, "Article not found");
        try
        {
            context.Articles.Remove(article);
            await context.SaveChangesAsync();
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
            return new OperationResult(true);
        }
        catch (Exception ex) { return new OperationResult(false, ex.Message); }
    }
}
public record OperationResult(bool Success, string? ErrorMessage = null);
