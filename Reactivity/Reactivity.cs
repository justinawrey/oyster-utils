using System;
using System.Collections.Generic;

namespace OysterUtils
{
  public class Reactive<T>
  {
    private T val;
    private Dictionary<Func<T, bool>, List<Action<T>>> predicates = new Dictionary<Func<T, bool>, List<Action<T>>>();

    public Reactive(T _val)
    {
      val = _val;
    }

    public void Set(T to)
    {
      T prevValue = val;
      val = to;

      if (prevValue.Equals(val))
      {
        return;
      }

      foreach (var pair in predicates)
      {
        Func<T, bool> predicate = pair.Key;
        List<Action<T>> cbs = pair.Value;

        if (!predicate(val))
        {
          continue;
        }

        cbs.ForEach(cb => cb(val));
      }
    }

    public void When(Func<T, bool> predicate, Action<T> cb)
    {
      AddPredicate(predicate, cb);
    }

    public void OnChange(Action<T> cb)
    {
      Func<T, bool> predicate = val => true;
      AddPredicate(predicate, cb);
    }

    public T Get()
    {
      return val;
    }

    private void AddPredicate(Func<T, bool> predicate, Action<T> cb)
    {
      if (predicates.ContainsKey(predicate))
      {
        predicates[predicate].Add(cb);
      }
      else
      {
        predicates[predicate] = new List<Action<T>>() { cb };
      }
    }
  }
}
