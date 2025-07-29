using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using News.Backend.Articles.Controllers;
using News.Backend.Articles.Db;
using News.Backend.Articles.Models;
using News.Backend.Articles.Services;
using News.Backend.Articles.Services.Interfaces;
using System.Threading.Tasks;

namespace ArticlesTest;

public class ArticleTest 
{
    private readonly DbContextOptions<ArticlesContext> _dbOptions;


    public ArticleTest()
    {
        _dbOptions = new DbContextOptionsBuilder<ArticlesContext>()
            .UseSqlServer("Server=Piskarev;Database=ArticlesDb;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;
    }


    [Fact]
    public async Task DB_should_be_connected()
    {
        await using var context = new ArticlesContext(_dbOptions);
        var isConnected = await context.Database.CanConnectAsync();

        Assert.True(isConnected);
    }

    [Fact]
    public async Task DB_should_be_result_equal_true_and_value_is_list()
    {
        await using var context = new ArticlesContext(_dbOptions);
        var mockCache = new Mock<IDistributedCache>();
        mockCache.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])null);

        var service = new ArticleService(context, mockCache.Object);
        var controller = new ArticleController(service, Mock.Of<ILogger<ArticleController>>());


        var result = await controller.All();

        Assert.IsType<OkObjectResult>(result.Result);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsAssignableFrom<IEnumerable<Article>>(okResult.Value);

    }

    [Fact]
    public async Task DB_should_return_first_element()
    {
        await using var context = new ArticlesContext(_dbOptions);
        var mockCache = new Mock<IDistributedCache>();
        mockCache.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])null);
        var service = new ArticleService(context, mockCache.Object);
        var controller = new ArticleController(service, Mock.Of<ILogger<ArticleController>>());

        var result = await controller.GetById(1);


        Assert.IsType<OkObjectResult>(result.Result);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsAssignableFrom<Article>(okResult.Value);

    }
}