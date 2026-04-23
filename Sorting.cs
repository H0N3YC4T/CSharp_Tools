public class Sorting
{
  private const int PARALLEL_THRESHOLD = 2048;

  public static void QuickSort<T>(T[] arr, int low, int high)
  {
    while (low < high)
    {
      int p = Partition(arr, low, high);
      bool leftSmaller = (p - low < high - p);

      int nextLow = leftSmaller ? low : p + 1;
      int nextHigh = leftSmaller ? p : high;

      // If the chunk is big enough, branch off to another thread
      if (high - low > PARALLEL_THRESHOLD)
      {
        Parallel.Invoke(
            () => QuickSort(arr, nextLow, nextHigh),
            () => QuickSort(arr, leftSmaller ? p + 1 : low, leftSmaller ? high : p));
        return;
      }
      // Single thread for smaller chunks with TCO
      else
      {
        QuickSort(arr, nextLow, nextHigh);
        if (leftSmaller) low = p + 1;
        else high = p;
      }
    }
  }

  // Hoare Partitioning
  private static int Partition<T>(T[] arr, int inLow, int inHigh)
  {
    T pivot = arr[inLow + ((inHigh - inLow) >> 1)];
    int low = inLow - 1;
    int high = inHigh + 1;
    var comparer = Comparer<T>.Default;

    while (true)
    {
      while (comparer.Compare(arr[++low], pivot) < 0) ;
      while (comparer.Compare(arr[--high], pivot) > 0) ;
      if (low >= high) return high;
      (arr[low], arr[high]) = (arr[high], arr[low]);
    }
  }
}
