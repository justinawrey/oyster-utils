using System.Threading.Tasks;
using UnityEngine;

namespace OysterUtils
{
  public static class TaskUtils
  {
    public static async Task TaskFromAwaitable(Awaitable awaitable)
    {
      await awaitable;
    }
  }
}
