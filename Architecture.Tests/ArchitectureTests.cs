using NetArchTest.Rules;
using Xunit;

namespace Architecture.Tests;

public class ArchitectureTests
{
    private const string DomainNamespace = "Domain";
    private const string ApplicationNamespace = "Application";
    private const string InfrastructureNamespace = "Infrastructure";
    private const string PresentationNamespace = "DataCapture";
    
    [Fact]
    public void Domain_Should_Not_DependOnOtherLayers()
    {
        // Arrange
        var assembly = typeof(Domain.SampleEntity).Assembly;

        var otherLayers = new[]
        {
            ApplicationNamespace,
            InfrastructureNamespace,
            PresentationNamespace
        };

        // Act
        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherLayers)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, $"Domain should not depend on other layers. Violations: {(result.FailingTypeNames != null ? string.Join(", ", result.FailingTypeNames) : "none")}");
    }

    [Fact]
    public void Application_Should_DependOnDomain_Only()
    {
        // Arrange
        var assembly = typeof(Application.DependencyInjection).Assembly;

        var forbiddenLayers = new[]
        {
            InfrastructureNamespace,
            PresentationNamespace
        };

        // Act
        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(forbiddenLayers)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, $"Application should not depend on Infrastructure or Presentation. Violations: {(result.FailingTypeNames != null ? string.Join(", ", result.FailingTypeNames) : "none")}");
    }

    [Fact]
    public void Infrastructure_Should_Not_DependOnPresentation()
    {
        // Arrange
        var assembly = typeof(Infrastructure.DependencyInjection).Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOn(PresentationNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, $"Infrastructure should not depend on Presentation. Violations: {(result.FailingTypeNames != null ? string.Join(", ", result.FailingTypeNames) : "none")}");
    }

    [Fact]
    public void Presentation_Should_Not_HaveDependencyOnInfrastructureImplementations()
    {
        var assembly = System.Reflection.Assembly.Load("DataCapture");
        var infrastructureImplementationPattern = $"{InfrastructureNamespace}.*";
        
        // Act
        var result = Types
            .InAssembly(assembly)
            .That()
            .ResideInNamespace(PresentationNamespace)
            .And()
            .DoNotImplementInterface(typeof(Application.Abstraction.IApplicationDbContext))
            .Should()
            .OnlyHaveDependenciesOn($"{PresentationNamespace}.*", $"{ApplicationNamespace}.*", $"{DomainNamespace}.*", "System.*", "Microsoft.*")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, $"Presentation should only depend on abstractions, not implementations. Violations: {(result.FailingTypeNames != null ? string.Join(", ", result.FailingTypeNames) : "none")}");
    }
}
