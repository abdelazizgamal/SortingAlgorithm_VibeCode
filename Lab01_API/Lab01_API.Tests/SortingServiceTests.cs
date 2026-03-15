using Lab01_API.Services;
using Xunit;

namespace Lab01_API.Tests;

public class SortingServiceTests
{
    [Fact]
    public void QuickSort_WhenArrayIsEmpty_ReturnsEmptyArray()
    {
        var input = Array.Empty<int>();

        var result = SortingService.QuickSort(input);

        Assert.Empty(result);
    }

    [Fact]
    public void QuickSort_WhenArrayHasSingleElement_ReturnsSameElement()
    {
        var input = new[] { 42 };

        var result = SortingService.QuickSort(input);

        Assert.Equal(new[] { 42 }, result);
    }

    [Fact]
    public void QuickSort_WhenArrayIsAlreadySorted_ReturnsSortedArray()
    {
        var input = new[] { 1, 2, 3, 4, 5 };

        var result = SortingService.QuickSort(input);

        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, result);
    }

    [Fact]
    public void QuickSort_WhenArrayIsReverseSorted_ReturnsSortedArray()
    {
        var input = new[] { 5, 4, 3, 2, 1 };

        var result = SortingService.QuickSort(input);

        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, result);
    }

    [Fact]
    public void QuickSort_WhenArrayContainsDuplicates_ReturnsSortedArrayWithDuplicates()
    {
        var input = new[] { 4, 2, 5, 2, 1, 4, 3, 2 };

        var result = SortingService.QuickSort(input);

        Assert.Equal(new[] { 1, 2, 2, 2, 3, 4, 4, 5 }, result);
    }

    [Fact]
    public void QuickSort_WhenArrayIsLargeRandom_ReturnsArraySortedLikeArraySort()
    {
        var random = new Random(12345);
        var input = Enumerable.Range(0, 10000)
            .Select(_ => random.Next(-100000, 100001))
            .ToArray();

        var expected = (int[])input.Clone();
        Array.Sort(expected);

        var result = SortingService.QuickSort(input);

        Assert.Equal(expected, result);
    }
}
