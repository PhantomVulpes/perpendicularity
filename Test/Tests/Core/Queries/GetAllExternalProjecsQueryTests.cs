using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Core.Queries;
using Vulpes.Perpendicularity.Test.Doubles.Electrum.Domain.Data;

namespace Vulpes.Perpendicularity.Test.Tests.Core.Queries;

[TestClass]
public class GetAllExternalProjecsQueryTests
{
    private readonly TestQueryProvider<ExternalProject> testQueryProvider;
    private readonly GetAllExternalProjectsQueryHandler underTest;

    public GetAllExternalProjecsQueryTests()
    {
        testQueryProvider = new();
        underTest = new(testQueryProvider);
    }

    private static GetAllExternalProjectsQuery BuildQuery() => new();

    [TestMethod]
    public async Task QueryReturnsEmptyResultWhenNoProjectsExist()
    {
        var query = BuildQuery();

        var result = await underTest.RequestAsync(query);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public async Task QueryReturnsAllProjectsFromProvider()
    {
        var project1 = ExternalProject.Default with { ProjectName = "Project 1" };
        var project2 = ExternalProject.Default with { ProjectName = "Project 2" };
        var project3 = ExternalProject.Default with { ProjectName = "Project 3" };

        testQueryProvider.Response.AddRange([project1, project2, project3]);

        var query = BuildQuery();
        var result = await underTest.RequestAsync(query);

        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count());
        CollectionAssert.Contains(result.ToList(), project1);
        CollectionAssert.Contains(result.ToList(), project2);
        CollectionAssert.Contains(result.ToList(), project3);
    }

    [TestMethod]
    public async Task AllUsersHaveAccess()
    {
        var query = BuildQuery();

        var access = await underTest.ValidateAccessAsync(query);

        Assert.IsTrue(access);
    }
}