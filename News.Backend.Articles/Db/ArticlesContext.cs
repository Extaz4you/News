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
