using News.Backend.Articles.Models;

namespace News.Backend.Articles.Services.Interfaces;

public interface IArticleRepository
{
    public Task<IEnumerable<Article>> All(CancellationToken ct = default);
    public Task<Article?> Get(int id);
    public Task<OperationResult> Delete(int id);
    public Task<OperationResult> Create(Article article);
    public  Task<OperationResult> Update(Article article);
}
