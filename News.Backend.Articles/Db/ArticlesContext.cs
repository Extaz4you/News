using Microsoft.EntityFrameworkCore;
using News.Backend.Articles.Models;

namespace News.Backend.Articles.Db;

public class ArticlesContext : DbContext
{
    public ArticlesContext(DbContextOptions<ArticlesContext> opt) : base(opt)
    {

    }
    public DbSet<Article> Articles { get; set; }
}

public interface Deletable
{
    public bool Deleted {  get; set; }
}

public static class Ext
{
    public static IQueryable<T> NotDeleted<T>(this IQueryable<T> query) where T : Deletable
        => query.Where(i => i.Deleted);
}
