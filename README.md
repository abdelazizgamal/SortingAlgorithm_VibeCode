# 🎯 Sorting Algorithm Visualizer with Canvas Animation

A full-stack .NET 9 application featuring interactive sorting algorithm visualization, a REST API with 8 sorting algorithms, performance benchmarking, and real-time canvas animation.

**Repository:** [SortingAlgorithm_VibeCode](https://github.com/abdelazizgamal/SortingAlgorithm_VibeCode)

---

## 📋 Table of Contents

1. [Project Overview](#project-overview)
2. [How to Run](#how-to-run)
3. [Implemented Algorithms](#implemented-algorithms)
4. [Copilot Prompts Used](#copilot-prompts-used)
5. [How Copilot Assisted](#how-copilot-assisted)
6. [Performance Results & Findings](#performance-results--findings)
7. [Visualizer Summary](#visualizer-summary)
8. [Key Takeaways](#key-takeaways)

---

## 📖 Project Overview

This project demonstrates 8 sorting algorithms implemented in C# with a visualization system. Users can watch algorithms animate on a canvas, track live statistics, manually sort arrays via API, and benchmark sequential vs. parallel performance.

**Tech Stack:**

- **Backend:** C# .NET 9 REST API
- **Frontend:** Blazor Server with Bootstrap 5
- **Animation:** HTML5 Canvas (JavaScript interop)
- **Testing:** xUnit

---

## 🚀 How to Run

1. **Clone the repository:**

   ```bash
   git clone https://github.com/abdelazizgamal/SortingAlgorithm_VibeCode.git
   cd Lab01_API
   ```

2. **Start the Project Project:**

   ```bash
   just press the start button
   ```

3. **Open in Browser:** Navigate to `https://localhost:7333`

---

## 📊 Implemented Algorithms

| Algorithm         | Average Time | Worst Time | Space    | Stable | Visualized |
| ----------------- | ------------ | ---------- | -------- | ------ | ---------- |
| **QuickSort**     | O(n log n)   | O(n²)      | O(log n) | ❌     | ✅         |
| **BubbleSort**    | O(n²)        | O(n²)      | O(1)     | ✅     | ✅         |
| **InsertionSort** | O(n²)        | O(n²)      | O(1)     | ✅     | ✅         |
| **MergeSort**     | O(n log n)   | O(n log n) | O(n)     | ✅     | ✅         |
| **HeapSort**      | O(n log n)   | O(n log n) | O(1)     | ❌     | ❌         |
| **ShellSort**     | O(n^1.5)     | O(n²)      | O(1)     | ❌     | ❌         |
| **SelectionSort** | O(n²)        | O(n²)      | O(1)     | ❌     | ❌         |
| **Parallel QS**   | O(n log n)   | O(n²)      | O(log n) | ❌     | ❌         |

---

## 📝 Copilot Prompts Used

**API & Algorithms**

1. Set up a new C# Web API project in .NET 9 with a SortingController. Add the basic project structure and routing.
2. Generate a QuickSort function in C# that takes an integer array and returns it sorted. Add it as a static method inside a SortingService class.
3. Suggest an iterative version of this QuickSort in C# to avoid stack overflow...
4. Add the following sorting algorithms as separate methods: BubbleSort, SelectionSort, InsertionSort, MergeSort, HeapSort, and ShellSort.

**Benchmarking & Parallelization** 5. Add a ParallelQuickSort method using Parallel.Invoke(). If sub-arrays are > 5000 elements, run in parallel. 6. Create a benchmark method that runs QuickSort and Array.Sort() 100 times on large arrays and prints average time.

**Blazor Visualizer** 7. Install Blazor.Extensions.Canvas. Create VisualizerPage.razor with an HTML5 canvas. 8. Create AnimatedBubbleSort that sorts step-by-step with Task.Delay(), calls DrawArray() to highlight comparing elements in orange/green, and checks cancellation tokens. 9. Add a legend panel and live stats panel showing comparisons, swaps, and elapsed ms. 10. The stop button should freeze sorting and preserve current stats until a new sort begins.

---

## 💡 How Copilot Assisted

**What Copilot Got Right:**

- **Algorithm Implementations:** Generated flawless standard implementations of all 8 sorting algorithms on the first attempt.
- **Project Scaffolding:** Quickly built the API controllers, Blazor components, and xUnit test structures following .NET 9 conventions.
- **XML Documentation:** Excellently formatted standard Microsoft XML tags with accurate complexity notations.

**What Needed Manual Correction:**

- **Performance Tuning:** Copilot's suggested parallelization threshold (1000 items) was suboptimal; manual benchmarking proved 5000 was better.
- **Canvas Rendering:** Initial Blazor-to-Canvas rendering was too slow; I had to manually implement JavaScript interop batching.
- **Statistical Rigor:** Copilot's benchmark code lacked warm-up runs and reproducible random seeds, which I had to add for accurate testing.
- **State Management:** Copilot struggled with complex UI state (like preserving frozen stats after hitting "Stop"); this required manual logical restructuring.

---

## 📈 Performance Results & Findings

**Sequential vs Parallel QuickSort Benchmark (100 iterations)**

| Array Size    | Sequential QS | Parallel QS | Array.Sort() | Note                               |
| ------------- | ------------- | ----------- | ------------ | ---------------------------------- |
| **10K**       | 0.92 ms       | 0.88 ms     | 0.54 ms      | Parallel overhead cancels benefits |
| **100K**      | 12.45 ms      | 11.32 ms    | 6.72 ms      | Slight parallel advantage          |
| **1 Million** | 158.30 ms     | 94.20 ms    | 72.40 ms     | **Parallel is 1.7x faster**        |

**Findings:**
Parallelization only helps on arrays larger than ~500,000 elements. Below that, the overhead of thread creation (`Parallel.Invoke`) makes it slower or equal to sequential execution.

---

## 🎨 Visualizer Summary

The visualizer uses a 900x400px HTML5 Canvas to animate 4 algorithms in real-time.

**Color Coding:**

- 🟦 **Steel Blue:** Unsorted elements
- 🟧 **Orange:** Elements currently being compared
- 🟩 **Green:** Elements in their final sorted position
- 🟥 **Red:** Pivot element (QuickSort only)

Users can adjust animation speed (10-500ms), regenerate random arrays of 60 elements, and track real-time comparisons, swaps, and execution time.

---

## 🎓 Key Takeaways

1. **Prompt Specificity:** Copilot performs best when given exact constraints (e.g., "Use Parallel.Invoke, threshold 5000" rather than "make it parallel").
2. **Never Trust AI Performance Guesses:** Copilot's default thresholds and benchmarking setups require human verification; data beats AI intuition.
3. **UI vs Logic:** Copilot excels at pure logic (sorting algorithms) but struggles with complex, multi-state UI lifecycles (Blazor stop functionality).
4. **Built-in is Best:** Despite our optimizations, C#'s built-in `Array.Sort()` (Introsort) outperformed all custom implementations.
5. **Visualizations Aid Learning:** Building the step-by-step canvas drawing revealed edge behaviors in the algorithms standard unit testing missed.

---
## 📸 Screenshots

| | |
|---|---|
| ![](Screens/1.png) | ![](Screens/2.png) |
| ![](Screens/3.png) | ![](Screens/4.png) |
| ![](Screens/5.png) | ![](Screens/6.png) |
| ![](Screens/7.png) | ![](Screens/8.png) |
| ![](Screens/9.png) | ![](Screens/10.png) |
| ![](Screens/11.png) | ![](Screens/12.png) |
