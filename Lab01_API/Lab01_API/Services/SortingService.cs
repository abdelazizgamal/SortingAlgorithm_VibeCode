using System.Diagnostics;

namespace Lab01_API.Services;

public static class SortingService
{
    /// <summary>
    /// List of all available sorting algorithms.
    /// </summary>
    public static readonly string[] AvailableAlgorithms = new[]
    {
        "QuickSort",
        "QuickSort-Iterative",
        "BubbleSort",
        "SelectionSort",
        "InsertionSort",
        "MergeSort",
        "HeapSort",
        "ShellSort"
    };

    /// <summary>
    /// Sorts an array using the specified algorithm.
    /// Delegates to the appropriate sorting algorithm implementation based on the algorithm name.
    /// </summary>
    /// <param name="input">The array of integers to sort. Cannot be null.</param>
    /// <param name="algorithmName">The name of the sorting algorithm to use (case-insensitive).
    /// Valid options: "quicksort", "quicksort-iterative", "bubblesort", "selectionsort", 
    /// "insertionsort", "mergesort", "heapsort", "shellsort".</param>
    /// <returns>A new sorted copy of the input array in ascending order.</returns>
    /// <remarks>
    /// This method acts as a router to delegate to the appropriate sorting algorithm.
    /// All underlying algorithms return a new sorted copy without modifying the original array.
    /// Time complexity varies by algorithm: O(n log n) average for QuickSort/MergeSort/HeapSort, 
    /// O(n^2) for BubbleSort/SelectionSort/InsertionSort.
    /// Space complexity: O(1) to O(n) depending on algorithm choice.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="algorithmName"/> is null, empty, or represents an unknown algorithm.</exception>
    public static int[] Sort(int[] input, string algorithmName)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (string.IsNullOrWhiteSpace(algorithmName))
        {
            throw new ArgumentException("Algorithm name cannot be null or empty.", nameof(algorithmName));
        }

        return algorithmName.ToLowerInvariant() switch
        {
            "quicksort" => QuickSort(input),
            "quicksort-iterative" => QuickSortIterative(input),
            "bubblesort" => BubbleSort(input),
            "selectionsort" => SelectionSort(input),
            "insertionsort" => InsertionSort(input),
            "mergesort" => MergeSort(input),
            "heapsort" => HeapSort(input),
            "shellsort" => ShellSort(input),
            _ => throw new ArgumentException(
                $"Unknown algorithm '{algorithmName}'. Valid options are: quicksort, quicksort-iterative, " +
                "bubblesort, selectionsort, insertionsort, mergesort, heapsort, shellsort.",
                nameof(algorithmName))
        };
    }

    /// <summary>
    /// Sorts a copy of the input array using the recursive QuickSort algorithm with Lomuto partition scheme.
    /// Uses the last element as the pivot for each partition.
    /// </summary>
    /// <param name="numbers">The array of integers to sort. Cannot be null.</param>
    /// <returns>A new sorted copy of the input array in ascending order.</returns>
    /// <remarks>
    /// QuickSort is a divide-and-conquer algorithm that recursively partitions the array around a pivot.
    /// Time Complexity: O(n log n) average case, O(n²) worst case (when pivot is consistently extreme).
    /// Space Complexity: O(log n) average case (recursion stack), O(n) worst case.
    /// This implementation uses the Lomuto partition scheme, which is stable when used with stable data types.
    /// The algorithm sorts in-place but returns a copy of the original array.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="numbers"/> is null.</exception>
    public static int[] QuickSort(int[] numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        var result = (int[])numbers.Clone();
        QuickSortInternal(result, 0, result.Length - 1);
        return result;
    }

    /// <summary>
    /// Sorts a copy of the input array using iterative QuickSort with an explicit stack.
    /// Eliminates recursion by using a stack-based approach to manage partition ranges.
    /// </summary>
    /// <param name="numbers">The array of integers to sort. Cannot be null.</param>
    /// <returns>A new sorted copy of the input array in ascending order.</returns>
    /// <remarks>
    /// Iterative QuickSort avoids potential stack overflow issues that can occur with recursive QuickSort on very large arrays.
    /// Time Complexity: O(n log n) average case, O(n²) worst case.
    /// Space Complexity: O(log n) average case (explicit stack), O(n) worst case.
    /// Uses an explicit stack data structure to maintain partition ranges instead of recursion.
    /// Performance is generally comparable to recursive QuickSort but with better memory safety.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="numbers"/> is null.</exception>
    public static int[] QuickSortIterative(int[] numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        var result = (int[])numbers.Clone();

        if (result.Length <= 1)
            return result;

        var stack = new Stack<(int, int)>();
        stack.Push((0, result.Length - 1));

        while (stack.Count > 0)
        {
            var (low, high) = stack.Pop();

            if (low >= high)
                continue;

            var pivotIndex = Partition(result, low, high);

            if (pivotIndex - 1 > low)
                stack.Push((low, pivotIndex - 1));

            if (pivotIndex + 1 < high)
                stack.Push((pivotIndex + 1, high));
        }

        return result;
    }

    /// <summary>
    /// Benchmarks recursive QuickSort against the built-in Array.Sort method on identical large arrays.
    /// Runs both algorithms 100 times on an array of 10,000 random integers and compares performance.
    /// </summary>
    /// <returns>A <see cref="SortBenchmarkResult"/> containing average execution times in milliseconds
    /// for QuickSort and Array.Sort, along with the number of iterations and array size used.</returns>
    /// <remarks>
    /// This benchmark provides insight into the relative performance of the custom QuickSort implementation
    /// versus the optimized built-in Array.Sort (which uses an adaptive sorting algorithm).
    /// The benchmark uses a fixed random seed (12345) to ensure reproducible results across runs.
    /// Array.Sort typically outperforms naive QuickSort due to extensive optimizations like:
    /// - Introsort (switching to HeapSort for worst-case safety)
    /// - Cache-locality improvements
    /// - Multiple algorithm selection based on array characteristics
    /// Results may vary based on system load and hardware characteristics.
    /// </remarks>
    /// <exception cref="OutOfMemoryException">Thrown if the system cannot allocate memory for the 10,000-element test arrays.</exception>
    public static SortBenchmarkResult BenchmarkQuickSortVsArraySort()
    {
        const int iterations = 100;
        const int arraySize = 10000;

        var random = new Random(12345);
        var baseArray = Enumerable.Range(0, arraySize)
            .Select(_ => random.Next(-100000, 100001))
            .ToArray();

        long quickSortTotalTicks = 0;
        long arraySortTotalTicks = 0;

        for (var i = 0; i < iterations; i++)
        {
            var quickSortInput = (int[])baseArray.Clone();
            var quickSortWatch = Stopwatch.StartNew();
            QuickSortInternal(quickSortInput, 0, quickSortInput.Length - 1);
            quickSortWatch.Stop();
            quickSortTotalTicks += quickSortWatch.ElapsedTicks;

            var arraySortInput = (int[])baseArray.Clone();
            var arraySortWatch = Stopwatch.StartNew();
            Array.Sort(arraySortInput);
            arraySortWatch.Stop();
            arraySortTotalTicks += arraySortWatch.ElapsedTicks;
        }

        var quickSortAverageMilliseconds = TimeSpan.FromTicks(quickSortTotalTicks).TotalMilliseconds / iterations;
        var arraySortAverageMilliseconds = TimeSpan.FromTicks(arraySortTotalTicks).TotalMilliseconds / iterations;

        return new SortBenchmarkResult(iterations, arraySize, quickSortAverageMilliseconds, arraySortAverageMilliseconds);
    }

    /// <summary>
    /// Bubble Sort algorithm result record containing benchmark statistics.
    /// </summary>
    /// <param name="Iterations">The number of iterations performed in the benchmark.</param>
    /// <param name="ArraySize">The size of arrays used in the benchmark.</param>
    /// <param name="QuickSortAverageMilliseconds">Average execution time of QuickSort in milliseconds.</param>
    /// <param name="ArraySortAverageMilliseconds">Average execution time of Array.Sort in milliseconds.</param>
    public sealed record SortBenchmarkResult(
        int Iterations,
        int ArraySize,
        double QuickSortAverageMilliseconds,
        double ArraySortAverageMilliseconds);

    /// <summary>
    /// Sorts a copy of the input array using Bubble Sort algorithm.
    /// Repeatedly steps through the list, compares adjacent elements, and swaps them if they are in the wrong order.
    /// </summary>
    /// <param name="numbers">The array of integers to sort. Cannot be null.</param>
    /// <returns>A new sorted copy of the input array in ascending order.</returns>
    /// <remarks>
    /// Bubble Sort is a simple comparison-based sorting algorithm suitable for educational purposes and small datasets.
    /// It is stable, meaning equal elements maintain their relative order after sorting.
    /// Time Complexity: O(n²) average and worst case, O(n) best case (when array is already sorted with early termination).
    /// Space Complexity: O(1) - sorts in-place with no additional space needed (excluding the output copy).
    /// Performance: Generally slower than QuickSort, MergeSort, or HeapSort for large arrays due to quadratic complexity.
    /// Optimization: Includes early termination when no swaps occur in a pass (best case detection).
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="numbers"/> is null.</exception>
    public static int[] BubbleSort(int[] numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        var result = (int[])numbers.Clone();

        for (var i = 0; i < result.Length - 1; i++)
        {
            var swapped = false;
            for (var j = 0; j < result.Length - 1 - i; j++)
            {
                if (result[j] > result[j + 1])
                {
                    (result[j], result[j + 1]) = (result[j + 1], result[j]);
                    swapped = true;
                }
            }
            if (!swapped) break;
        }

        return result;
    }

    /// <summary>
    /// Sorts a copy of the input array using Selection Sort algorithm.
    /// Divides the array into a sorted portion and an unsorted portion, repeatedly selecting the minimum element 
    /// from the unsorted portion and moving it to the sorted portion.
    /// </summary>
    /// <param name="numbers">The array of integers to sort. Cannot be null.</param>
    /// <returns>A new sorted copy of the input array in ascending order.</returns>
    /// <remarks>
    /// Selection Sort is a simple, in-place comparison-based sorting algorithm suitable for small datasets.
    /// It is not stable in its basic form, as swaps can alter the relative order of equal elements.
    /// Time Complexity: O(n²) in all cases (best, average, and worst) - very predictable performance.
    /// Space Complexity: O(1) - sorts in-place with no additional space needed (excluding the output copy).
    /// Performance: Generally slower than QuickSort, MergeSort, or HeapSort but faster than BubbleSort in practice.
    /// Advantage: Minimizes the number of writes to memory (only n-1 swaps required), useful for scenarios with expensive writes.
    /// Use Case: Suitable when write operations are costly compared to read operations.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="numbers"/> is null.</exception>
    public static int[] SelectionSort(int[] numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        var result = (int[])numbers.Clone();

        for (var i = 0; i < result.Length - 1; i++)
        {
            var minIndex = i;
            for (var j = i + 1; j < result.Length; j++)
            {
                if (result[j] < result[minIndex])
                    minIndex = j;
            }

            if (minIndex != i)
                (result[i], result[minIndex]) = (result[minIndex], result[i]);
        }

        return result;
    }

    /// <summary>
    /// Sorts a copy of the input array using Insertion Sort algorithm.
    /// Builds the sorted array one item at a time by inserting each element into its correct position 
    /// within the already-sorted portion of the array.
    /// </summary>
    /// <param name="numbers">The array of integers to sort. Cannot be null.</param>
    /// <returns>A new sorted copy of the input array in ascending order.</returns>
    /// <remarks>
    /// Insertion Sort is a stable, in-place comparison-based sorting algorithm that performs well on small datasets 
    /// and nearly-sorted arrays. It is widely used in practice as part of hybrid algorithms like Timsort.
    /// Time Complexity: O(n²) average and worst case, O(n) best case (when array is already sorted).
    /// Space Complexity: O(1) - sorts in-place with no additional space needed (excluding the output copy).
    /// Stability: Stable - equal elements maintain their relative order.
    /// Performance: Excellent for small arrays or nearly-sorted data; generally faster than BubbleSort and SelectionSort.
    /// Adaptive: Performs better as the input becomes more sorted; approximately O(n) for nearly-sorted arrays.
    /// Use Case: Preferred for small arrays, linked lists, or as the base case in hybrid sorting algorithms.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="numbers"/> is null.</exception>
    public static int[] InsertionSort(int[] numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        var result = (int[])numbers.Clone();

        for (var i = 1; i < result.Length; i++)
        {
            var key = result[i];
            var j = i - 1;

            while (j >= 0 && result[j] > key)
            {
                result[j + 1] = result[j];
                j--;
            }

            result[j + 1] = key;
        }

        return result;
    }

    /// <summary>
    /// Sorts a copy of the input array using Merge Sort algorithm.
    /// A divide-and-conquer algorithm that recursively divides the array into halves, sorts them, and merges the results.
    /// </summary>
    /// <param name="numbers">The array of integers to sort. Cannot be null.</param>
    /// <returns>A new sorted copy of the input array in ascending order.</returns>
    /// <remarks>
    /// Merge Sort is a stable, comparison-based sorting algorithm with guaranteed O(n log n) performance in all cases.
    /// It is not in-place due to the merge operation but offers predictable and reliable performance.
    /// Time Complexity: O(n log n) in all cases (best, average, and worst) - very consistent and predictable.
    /// Space Complexity: O(n) - requires additional space for merging operations.
    /// Stability: Stable - equal elements maintain their relative order after sorting.
    /// Performance: Excellent for large datasets and when consistent performance is required.
    /// Use Case: Preferred for linked lists, external sorting, and scenarios where O(n log n) guarantee is needed.
    /// Not in-place: Requires O(n) auxiliary space for the merge process.
    /// Cache Efficiency: Less cache-efficient than QuickSort due to scattered memory access patterns.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="numbers"/> is null.</exception>
    /// <exception cref="OutOfMemoryException">Thrown if insufficient memory is available for the merge buffer.</exception>
    public static int[] MergeSort(int[] numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        var result = (int[])numbers.Clone();

        if (result.Length <= 1)
            return result;

        var buffer = new int[result.Length];
        MergeSortInternal(result, buffer, 0, result.Length - 1);
        return result;
    }

    /// <summary>
    /// Sorts a copy of the input array using Heap Sort algorithm.
    /// Uses a heap (binary search tree) data structure to efficiently sort the array by repeatedly extracting 
    /// the maximum element and placing it at the end.
    /// </summary>
    /// <param name="numbers">The array of integers to sort. Cannot be null.</param>
    /// <returns>A new sorted copy of the input array in ascending order.</returns>
    /// <remarks>
    /// Heap Sort is an in-place, comparison-based sorting algorithm with guaranteed O(n log n) performance.
    /// It is not stable, as the heap operations can alter the relative order of equal elements.
    /// Time Complexity: O(n log n) in all cases (best, average, and worst) - very consistent and predictable.
    /// Space Complexity: O(1) - sorts in-place with no additional space needed (excluding the output copy).
    /// Stability: Not stable - equal elements may not maintain their relative order.
    /// Performance: Consistent O(n log n) performance; generally slower than QuickSort in practice due to poor cache locality.
    /// Use Case: Preferred when worst-case O(n log n) guarantee is required and in-place sorting is necessary.
    /// Advantage: Never requires more than O(n log n) comparisons, making it suitable for real-time systems.
    /// Not adaptive: Performance does not improve for partially sorted data.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="numbers"/> is null.</exception>
    public static int[] HeapSort(int[] numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        var result = (int[])numbers.Clone();
        var n = result.Length;

        for (var i = n / 2 - 1; i >= 0; i--)
            Heapify(result, n, i);

        for (var i = n - 1; i > 0; i--)
        {
            (result[0], result[i]) = (result[i], result[0]);
            Heapify(result, i, 0);
        }

        return result;
    }

    /// <summary>
    /// Sorts a copy of the input array using Shell Sort algorithm.
    /// A generalization of Insertion Sort that allows the exchange of elements far apart. 
    /// Uses a decreasing sequence of gaps to sort subarrays, eventually using a gap of 1 (final Insertion Sort pass).
    /// </summary>
    /// <param name="numbers">The array of integers to sort. Cannot be null.</param>
    /// <returns>A new sorted copy of the input array in ascending order.</returns>
    /// <remarks>
    /// Shell Sort is an in-place, comparison-based sorting algorithm that is more efficient than Insertion Sort for larger arrays.
    /// Performance depends significantly on the gap sequence used; this implementation uses the gap sequence: n/2, n/4, ..., 1.
    /// Time Complexity: ~O(n^1.5) with the n/2 gap sequence (typical), O(n²) worst case depending on gap sequence chosen.
    /// Alternative gap sequences (Knuth, Sedgewick) can improve performance to O(n log²n) or better.
    /// Space Complexity: O(1) - sorts in-place with no additional space needed (excluding the output copy).
    /// Stability: Not stable - equal elements may not maintain their relative order.
    /// Performance: Good practical performance on medium-sized arrays; faster than Insertion Sort but slower than QuickSort/MergeSort.
    /// Use Case: Suitable for medium-sized arrays when a simple in-place algorithm is preferred over more complex alternatives.
    /// Adaptive: Performance improves for partially sorted arrays compared to Insertion Sort.
    /// Cache Efficiency: Better cache locality than MergeSort; worse than QuickSort on average.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="numbers"/> is null.</exception>
    public static int[] ShellSort(int[] numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        var result = (int[])numbers.Clone();

        for (var gap = result.Length / 2; gap > 0; gap /= 2)
        {
            for (var i = gap; i < result.Length; i++)
            {
                var temp = result[i];
                var j = i;

                while (j >= gap && result[j - gap] > temp)
                {
                    result[j] = result[j - gap];
                    j -= gap;
                }

                result[j] = temp;
            }
        }

        return result;
    }

    private static void QuickSortInternal(int[] numbers, int low, int high)
    {
        if (low >= high)
            return;

        var pivotIndex = Partition(numbers, low, high);
        QuickSortInternal(numbers, low, pivotIndex - 1);
        QuickSortInternal(numbers, pivotIndex + 1, high);
    }

    private static int Partition(int[] numbers, int low, int high)
    {
        var pivot = numbers[high];
        var smallerElementIndex = low - 1;

        for (var i = low; i < high; i++)
        {
            if (numbers[i] <= pivot)
            {
                smallerElementIndex++;
                (numbers[smallerElementIndex], numbers[i]) = (numbers[i], numbers[smallerElementIndex]);
            }
        }

        (numbers[smallerElementIndex + 1], numbers[high]) = (numbers[high], numbers[smallerElementIndex + 1]);
        return smallerElementIndex + 1;
    }

    private static void MergeSortInternal(int[] numbers, int[] buffer, int left, int right)
    {
        if (left >= right)
            return;

        var mid = left + (right - left) / 2;
        MergeSortInternal(numbers, buffer, left, mid);
        MergeSortInternal(numbers, buffer, mid + 1, right);
        Merge(numbers, buffer, left, mid, right);
    }

    private static void Merge(int[] numbers, int[] buffer, int left, int mid, int right)
    {
        var i = left;
        var j = mid + 1;
        var k = left;

        while (i <= mid && j <= right)
        {
            if (numbers[i] <= numbers[j])
                buffer[k++] = numbers[i++];
            else
                buffer[k++] = numbers[j++];
        }

        while (i <= mid)
            buffer[k++] = numbers[i++];

        while (j <= right)
            buffer[k++] = numbers[j++];

        for (var index = left; index <= right; index++)
            numbers[index] = buffer[index];
    }

    private static void Heapify(int[] numbers, int heapSize, int rootIndex)
    {
        var largest = rootIndex;
        var left = 2 * rootIndex + 1;
        var right = 2 * rootIndex + 2;

        if (left < heapSize && numbers[left] > numbers[largest])
            largest = left;

        if (right < heapSize && numbers[right] > numbers[largest])
            largest = right;

        if (largest != rootIndex)
        {
            (numbers[rootIndex], numbers[largest]) = (numbers[largest], numbers[rootIndex]);
            Heapify(numbers, heapSize, largest);
        }
    }
}
