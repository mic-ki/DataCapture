using Application.Abstraction;
using Application.Abstraction.Mediator;
using Application.Features.Generic;
using Application.Pipeline;
using Moq;
using Xunit;

namespace Infrastructure.Tests.Mediator;

public class CacheBehaviorTests
{
    private class TestCachableQuery : ICachableQuery<string>
    {
        public string CacheKey => "test-key";
        public IEnumerable<string>? Tags => new[] { "test-tag" };
    }
    
    [Fact]
    public async Task Handle_WithCachedResponse_ShouldReturnCachedValue()
    {
        // Arrange
        var cacheService = new Mock<ICacheService>();
        cacheService.Setup(c => c.GetAsync<string>("test-key", It.IsAny<CancellationToken>()))
            .ReturnsAsync("cached-value");
        
        var behavior = new CacheBehavior<TestCachableQuery, string>(cacheService.Object);
        var request = new TestCachableQuery();
        
        // Tento delegát by neměl být volán, protože hodnota je v cache
        RequestHandlerDelegate<string> next = () => 
        {
            Assert.True(false, "Next delegate should not be called");
            return Task.FromResult("handler-value");
        };
        
        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);
        
        // Assert
        Assert.Equal("cached-value", result);
        cacheService.Verify(c => c.GetAsync<string>("test-key", It.IsAny<CancellationToken>()), Times.Once);
        cacheService.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task Handle_WithoutCachedResponse_ShouldCallNextAndCacheResult()
    {
        // Arrange
        var cacheService = new Mock<ICacheService>();
        cacheService.Setup(c => c.GetAsync<string>("test-key", It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);
        
        var behavior = new CacheBehavior<TestCachableQuery, string>(cacheService.Object);
        var request = new TestCachableQuery();
        
        // Tento delegát by měl být volán, protože hodnota není v cache
        RequestHandlerDelegate<string> next = () => Task.FromResult("handler-value");
        
        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);
        
        // Assert
        Assert.Equal("handler-value", result);
        cacheService.Verify(c => c.GetAsync<string>("test-key", It.IsAny<CancellationToken>()), Times.Once);
        cacheService.Verify(c => c.SetAsync(
            "test-key", 
            "handler-value", 
            It.IsAny<TimeSpan>(), 
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}
