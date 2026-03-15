namespace Lab01_API.Services;

/// <summary>
/// Interface for sorting algorithms.
/// </summary>
public interface ISortingService
{
    /// <summary>
    /// Sorts an array using the specified algorithm.
    /// </summary>
    /// <param name="input">The array to sort.</param>
    /// <param name="algorithmName">The name of the sorting algorithm (case-insensitive).</param>
    /// <returns>A sorted copy of the input array.</returns>
    /// <exception cref="ArgumentException">Thrown when the algorithm name is not recognized.</exception>
    int[] Sort(int[] input, string algorithmName);
}
