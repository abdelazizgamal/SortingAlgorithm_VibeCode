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
    /// </summary>
    /// <param name="input">The array to sort.</param>
    /// <param name="algorithmName">The name of the sorting algorithm (case-insensitive).</param>
    /// <returns>A sorted copy of the input array.</returns>
    /// <exception cref="ArgumentException">Thrown when the algorithm name is not recognized.</exception>
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
    /// Sorts a copy of <paramref name="numbers"/> using recursive QuickSort (Lomuto partition, last-element pivot).
    /// Complexity: average O(n log n), worst O(n^2), extra space O(log n) average via recursion.
    /// </summary>
    /// <param name="numbers">The array to sort.</param>
    /// <returns>A sorted copy of the input array.</returns>
    public static int[] QuickSort(int[] numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        var result = (int[])numbers.Clone();
        QuickSortInternal(result, 0, result.Length - 1);
        return result;
    }

    /// <summary>
    /// Sorts a copy of <paramref name="numbers"/> using iterative QuickSort with an explicit stack.
    /// Complexity: average O(n log n), worst O(n^2), extra space O(log n) average and O(n) worst.
    /// </summary>
    /// <param name="numbers">The array to sort.</param>
    /// <returns>A sorted copy of the input array.</returns>
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
    /// Benchmarks QuickSort and Array.Sort on the same large array for 100 runs.
    /// </summary>
    /// <returns>Average execution times in milliseconds for each algorithm.</returns>
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

    public sealed record SortBenchmarkResult(
        int Iterations,
        int ArraySize,
        double QuickSortAverageMilliseconds,
        double ArraySortAverageMilliseconds);

    /// <summary>
    /// Sorts a copy of the input using Bubble Sort.
    /// Complexity: O(n^2) average/worst, O(n) best.
    /// </summary>
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
    /// Sorts a copy of the input using Selection Sort.
    /// Complexity: O(n^2) in all cases.
    /// </summary>
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
    /// Sorts a copy of the input using Insertion Sort.
    /// Complexity: O(n^2) average/worst, O(n) best for nearly sorted.
    /// </summary>
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
    /// Sorts a copy of the input using Merge Sort.
    /// Complexity: O(n log n) in all cases.
    /// </summary>
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
    /// Sorts a copy of the input using Heap Sort.
    /// Complexity: O(n log n) in all cases.
    /// </summary>
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
    /// Sorts a copy of the input using Shell Sort.
    /// Complexity: ~O(n^1.5) typical, O(n^2) worst case depending on gap sequence.
    /// </summary>
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
