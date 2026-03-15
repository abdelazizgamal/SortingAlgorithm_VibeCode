using Lab01_API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Lab01_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SortingController : ControllerBase
{
    /// <summary>
    /// Returns a simple health status for API connectivity checks.
    /// </summary>
    [HttpGet("health")]
    public ActionResult<object> Health()
    {
        return Ok(new { status = "Healthy" });
    }

    /// <summary>
    /// Benchmarks QuickSort and Array.Sort using the same large input over 100 runs.
    /// </summary>
    /// <returns>Average execution time for each algorithm.</returns>
    [HttpGet("benchmark")]
    public ActionResult<SortingService.SortBenchmarkResult> Benchmark()
    {
        var result = SortingService.BenchmarkQuickSortVsArraySort();
        return Ok(result);
    }

    /// <summary>
    /// Returns all available sorting algorithm names.
    /// </summary>
    /// <returns>Array of algorithm names.</returns>
    [HttpGet("algorithms")]
    public ActionResult<string[]> GetAlgorithms()
    {
        return Ok(SortingService.AvailableAlgorithms);
    }

    /// <summary>
    /// Sorts a list of integers using QuickSort.
    /// </summary>
    /// <param name="numbers">The list of integers to sort.</param>
    /// <returns>The sorted list of integers.</returns>
    [HttpPost("quicksort")]
    public ActionResult<int[]> QuickSort([FromBody] int[] numbers)
    {
        if (numbers is null)
        {
            return BadRequest("Input array is required.");
        }

        var sorted = SortingService.QuickSort(numbers);
        return Ok(sorted);
    }

    /// <summary>
    /// Sorts a list of integers using the specified algorithm.
    /// </summary>
    /// <param name="request">Request containing numbers array and algorithm name.</param>
    /// <returns>Sorted array with algorithm name and execution time in milliseconds.</returns>
    [HttpPost("sort")]
    public ActionResult<SortResponse> Sort([FromBody] SortRequest request)
    {
        // Validation
        if (request is null)
        {
            return BadRequest("Request body is required.");
        }

        if (request.Numbers is null || request.Numbers.Length == 0)
        {
            return BadRequest("Numbers array cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(request.Algorithm))
        {
            return BadRequest("Algorithm name is required.");
        }

        try
        {
            var watch = Stopwatch.StartNew();
            var sorted = SortingService.Sort(request.Numbers, request.Algorithm);
            watch.Stop();

            return Ok(new SortResponse
            {
                SortedArray = sorted,
                Algorithm = request.Algorithm,
                ExecutionTimeMilliseconds = watch.Elapsed.TotalMilliseconds
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

/// <summary>
/// Request model for the Sort endpoint.
/// </summary>
public class SortRequest
{
    /// <summary>
    /// The array of integers to sort.
    /// </summary>
    public int[]? Numbers { get; set; }

    /// <summary>
    /// The name of the sorting algorithm to use.
    /// </summary>
    public string? Algorithm { get; set; }
}

/// <summary>
/// Response model for the Sort endpoint.
/// </summary>
public class SortResponse
{
    /// <summary>
    /// The sorted array.
    /// </summary>
    public int[]? SortedArray { get; set; }

    /// <summary>
    /// The algorithm name used for sorting.
    /// </summary>
    public string? Algorithm { get; set; }

    /// <summary>
    /// The execution time in milliseconds.
    /// </summary>
    public double ExecutionTimeMilliseconds { get; set; }
}
