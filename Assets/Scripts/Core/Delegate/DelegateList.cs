using System;
using System.Collections.Generic;

public class DelegateList
{
    private readonly LinkedList<Action> _callbacks;
    private readonly LinkedList<Action> _pendingCallbacksToAdd;
    private bool _invoking;
    private bool _pendingCallbacksToRemove;

    private DelegateList()
    {
        _callbacks = new LinkedList<Action>();
        _pendingCallbacksToAdd = new LinkedList<Action>();
    }

    public static DelegateList CreateWithGlobalCache()
    {
        return new DelegateList();
    }

    public static void InitGlobalCache(int cacheSize)
    {
        GlobalLinkedListNodeCache<Action>.InitNodeCache(cacheSize);
    }

    public static void ResetGlobalCache()
    {
        GlobalLinkedListNodeCache<Action>.ResetNodeCache();
    }

    public Action GetAction(int at)
    {
        if (_callbacks == null)
            return null;

        int index = 0;
        foreach (Action callback in _callbacks)
        {
            if (index == at)
                return callback;
            index++;
        }

        return null;
    }

    public int Count => _callbacks?.Count ?? 0;

    public static DelegateList operator +(DelegateList left, Action right)
    {
        left.Add(right);
        return left;
    }

    public static DelegateList operator -(DelegateList left, Action right)
    {
        left.Remove(right);
        return left;
    }

    private void Add(Action action)
    {
        if (action == null)
            return;

        LinkedListNode<Action> node = GlobalLinkedListNodeCache<Action>.Acquire(action);
        if (_invoking)
            _pendingCallbacksToAdd.AddLast(node);
        else
        {
            if (!_callbacks.Contains(action))
                _callbacks.AddLast(node);
            else
                GlobalLinkedListNodeCache<Action>.Release(node);
        }
    }

    private void Remove(Action action)
    {
        if (action == null)
            return;

        if (!Remove(action, _callbacks, false) && _invoking)
        {
            Remove(action, _pendingCallbacksToAdd, true);
        }
    }

    private bool Remove(Action action, LinkedList<Action> list, bool isPending)
    {
        bool found = false;
        LinkedListNode<Action> node = list.First;
        while (node != null)
        {
            if (node.Value == action)
            {
                if (_invoking && !isPending)
                {
                    node.Value = null;
                    _pendingCallbacksToRemove = true;
                    found = true;
                }
                else
                {
                    list.Remove(node);
                    if (!isPending)
                    {
                        GlobalLinkedListNodeCache<Action>.Release(node);
                    }
                    return true;
                }
            }
            node = node.Next;
        }
        return found;
    }

    public void Invoke()
    {
        _invoking = true;
        LinkedListNode<Action> node = _callbacks.First;
        while (node != null)
        {
            if (node.Value != null)
            {
                try
                {
                    node.Value();
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogException(ex);
                }
            }
            node = node.Next;
        }
        _invoking = false;

        CleanPendingDuringInvoke();
    }

    private void CleanPendingDuringInvoke()
    {
        CleanRemovePendingDuringInvoke();
        CleanAddPendingDuringInvoke();
    }

    private void CleanAddPendingDuringInvoke()
    {
        LinkedListNode<Action> pendingCallback = _pendingCallbacksToAdd.First;
        while (pendingCallback != null)
        {
            LinkedListNode<Action> next = pendingCallback.Next;
            _pendingCallbacksToAdd.Remove(pendingCallback);
            if (!_callbacks.Contains(pendingCallback.Value))
                _callbacks.AddLast(pendingCallback);
            else
                GlobalLinkedListNodeCache<Action>.Release(pendingCallback);
            pendingCallback = next;
        }
    }

    private void CleanRemovePendingDuringInvoke()
    {
        if (_pendingCallbacksToRemove)
        {
            LinkedListNode<Action> r = _callbacks.First;
            while (r != null)
            {
                LinkedListNode<Action> next = r.Next;
                if (r.Value == null)
                {
                    _callbacks.Remove(r);
                    GlobalLinkedListNodeCache<Action>.Release(r);
                }
                r = next;
            }
            _pendingCallbacksToRemove = false;
        }
    }

    private static void ClearLinkedList(LinkedList<Action> l)
    {
        LinkedListNode<Action> node = l.First;
        while (node != null)
        {
            LinkedListNode<Action> next = node.Next;
            l.Remove(node);
            GlobalLinkedListNodeCache<Action>.Release(node);
            node = next;
        }
    }

    public void Clear()
    {
        if (_invoking)
        {
            for (LinkedListNode<Action> r = _callbacks.First; r != null; r = r.Next)
            {
                r.Value = null;
            }
            _pendingCallbacksToRemove = true;
            ClearLinkedList(_pendingCallbacksToAdd);
        }
        else
        {
            ClearLinkedList(_callbacks);
        }
    }

    public DelegateList Set(Action right)
    {
        Clear();
        Add(right);
        return this;
    }
}

public class DelegateList<T>
{
    private readonly LinkedList<Action<T>> _callbacks;
    private readonly LinkedList<Action<T>> _pendingCallbacksToAdd;
    private bool _invoking;
    private bool _pendingCallbacksToRemove;

    private DelegateList()
    {
        _callbacks = new LinkedList<Action<T>>();
        _pendingCallbacksToAdd = new LinkedList<Action<T>>();
    }

    public static DelegateList<T> CreateWithGlobalCache()
    {
        return new DelegateList<T>();
    }

    public static void InitGlobalCache(int cacheSize)
    {
        GlobalLinkedListNodeCache<Action<T>>.InitNodeCache(cacheSize);
    }

    public static void ResetGlobalCache()
    {
        GlobalLinkedListNodeCache<Action<T>>.ResetNodeCache();
    }

    public Action<T> GetAction(int at)
    {
        if (_callbacks == null)
            return null;

        int index = 0;
        foreach (Action<T> callback in _callbacks)
        {
            if (index == at)
                return callback;
            index++;
        }

        return null;
    }

    public int Count => _callbacks?.Count ?? 0;

    public static DelegateList<T> operator +(DelegateList<T> left, Action<T> right)
    {
        left.Add(right);
        return left;
    }

    public static DelegateList<T> operator -(DelegateList<T> left, Action<T> right)
    {
        left.Remove(right);
        return left;
    }

    private void Add(Action<T> action)
    {
        if (action == null)
            return;

        LinkedListNode<Action<T>> node = GlobalLinkedListNodeCache<Action<T>>.Acquire(action);
        if (_invoking)
            _pendingCallbacksToAdd.AddLast(node);
        else
        {
            if (!_callbacks.Contains(action))
                _callbacks.AddLast(node);
            else
                GlobalLinkedListNodeCache<Action<T>>.Release(node);
        }
    }

    private void Remove(Action<T> action)
    {
        if (action == null)
            return;

        if (!Remove(action, _callbacks, false) && _invoking)
        {
            Remove(action, _pendingCallbacksToAdd, true);
        }
    }

    private bool Remove(Action<T> action, LinkedList<Action<T>> list, bool isPending)
    {
        bool found = false;
        LinkedListNode<Action<T>> node = list.First;
        while (node != null)
        {
            if (node.Value == action)
            {
                if (_invoking && !isPending)
                {
                    node.Value = null;
                    _pendingCallbacksToRemove = true;
                    found = true;
                }
                else
                {
                    list.Remove(node);
                    if (!isPending)
                    {
                        GlobalLinkedListNodeCache<Action<T>>.Release(node);
                    }
                    return true;
                }
            }
            node = node.Next;
        }
        return found;
    }

    public void Invoke(T res)
    {
        _invoking = true;
        LinkedListNode<Action<T>> node = _callbacks.First;
        while (node != null)
        {
            if (node.Value != null)
            {
                try
                {
                    node.Value(res);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogException(ex);
                }
            }
            node = node.Next;
        }
        _invoking = false;

        CleanPendingDuringInvoke();
    }

    private void CleanPendingDuringInvoke()
    {
        CleanRemovePendingDuringInvoke();
        CleanAddPendingDuringInvoke();
    }

    private void CleanAddPendingDuringInvoke()
    {
        LinkedListNode<Action<T>> pendingCallback = _pendingCallbacksToAdd.First;
        while (pendingCallback != null)
        {
            LinkedListNode<Action<T>> next = pendingCallback.Next;
            _pendingCallbacksToAdd.Remove(pendingCallback);
            if (!_callbacks.Contains(pendingCallback.Value))
                _callbacks.AddLast(pendingCallback);
            else
                GlobalLinkedListNodeCache<Action<T>>.Release(pendingCallback);
            pendingCallback = next;
        }
    }

    private void CleanRemovePendingDuringInvoke()
    {
        if (_pendingCallbacksToRemove)
        {
            LinkedListNode<Action<T>> r = _callbacks.First;
            while (r != null)
            {
                LinkedListNode<Action<T>> next = r.Next;
                if (r.Value == null)
                {
                    _callbacks.Remove(r);
                    GlobalLinkedListNodeCache<Action<T>>.Release(r);
                }
                r = next;
            }
            _pendingCallbacksToRemove = false;
        }
    }

    private static void ClearLinkedList(LinkedList<Action<T>> l)
    {
        LinkedListNode<Action<T>> node = l.First;
        while (node != null)
        {
            LinkedListNode<Action<T>> next = node.Next;
            l.Remove(node);
            GlobalLinkedListNodeCache<Action<T>>.Release(node);
            node = next;
        }
    }

    public void Clear()
    {
        if (_invoking)
        {
            for (LinkedListNode<Action<T>> r = _callbacks.First; r != null; r = r.Next)
            {
                r.Value = null;
            }
            _pendingCallbacksToRemove = true;
            ClearLinkedList(_pendingCallbacksToAdd);
        }
        else
        {
            ClearLinkedList(_callbacks);
        }
    }

    public DelegateList<T> Set(Action<T> right)
    {
        Clear();
        Add(right);
        return this;
    }
}

public class DelegateList<T1, T2>
{
    private readonly LinkedList<Action<T1, T2>> _callbacks;
    private readonly LinkedList<Action<T1, T2>> _pendingCallbacksToAdd;
    private bool _invoking;
    private bool _pendingCallbacksToRemove;

    private DelegateList()
    {
        _callbacks = new LinkedList<Action<T1, T2>>();
        _pendingCallbacksToAdd = new LinkedList<Action<T1, T2>>();
    }

    public static DelegateList<T1, T2> CreateWithGlobalCache()
    {
        return new DelegateList<T1, T2>();
    }

    public static void InitGlobalCache(int cacheSize)
    {
        GlobalLinkedListNodeCache<Action<T1, T2>>.InitNodeCache(cacheSize);
    }

    public static void ResetGlobalCache()
    {
        GlobalLinkedListNodeCache<Action<T1, T2>>.ResetNodeCache();
    }

    public Action<T1, T2> GetAction(int at)
    {
        if (_callbacks == null)
            return null;

        int index = 0;
        foreach (Action<T1, T2> callback in _callbacks)
        {
            if (index == at)
                return callback;
            index++;
        }

        return null;
    }

    public int Count => _callbacks?.Count ?? 0;

    public static DelegateList<T1, T2> operator +(DelegateList<T1, T2> left, Action<T1, T2> right)
    {
        left.Add(right);
        return left;
    }

    public static DelegateList<T1, T2> operator -(DelegateList<T1, T2> left, Action<T1, T2> right)
    {
        left.Remove(right);
        return left;
    }

    private void Add(Action<T1, T2> action)
    {
        if (action == null)
            return;

        LinkedListNode<Action<T1, T2>> node = GlobalLinkedListNodeCache<Action<T1, T2>>.Acquire(action);
        if (_invoking)
            _pendingCallbacksToAdd.AddLast(node);
        else
        {
            if (!_callbacks.Contains(action))
                _callbacks.AddLast(node);
            else
                GlobalLinkedListNodeCache<Action<T1, T2>>.Release(node);
        }
    }

    private void Remove(Action<T1, T2> action)
    {
        if (action == null)
            return;

        if (!Remove(action, _callbacks, false) && _invoking)
        {
            Remove(action, _pendingCallbacksToAdd, true);
        }
    }

    private bool Remove(Action<T1, T2> action, LinkedList<Action<T1, T2>> list, bool isPending)
    {
        bool found = false;
        LinkedListNode<Action<T1, T2>> node = list.First;
        while (node != null)
        {
            if (node.Value == action)
            {
                if (_invoking && !isPending)
                {
                    node.Value = null;
                    _pendingCallbacksToRemove = true;
                    found = true;
                }
                else
                {
                    list.Remove(node);
                    if (!isPending)
                    {
                        GlobalLinkedListNodeCache<Action<T1, T2>>.Release(node);
                    }
                    return true;
                }
            }
            node = node.Next;
        }
        return found;
    }

    public void Invoke(T1 param1, T2 param2)
    {
        _invoking = true;
        LinkedListNode<Action<T1, T2>> node = _callbacks.First;
        while (node != null)
        {
            if (node.Value != null)
            {
                try
                {
                    node.Value(param1, param2);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogException(ex);
                }
            }
            node = node.Next;
        }
        _invoking = false;

        CleanPendingDuringInvoke();
    }

    private void CleanPendingDuringInvoke()
    {
        CleanRemovePendingDuringInvoke();
        CleanAddPendingDuringInvoke();
    }

    private void CleanAddPendingDuringInvoke()
    {
        LinkedListNode<Action<T1, T2>> pendingCallback = _pendingCallbacksToAdd.First;
        while (pendingCallback != null)
        {
            LinkedListNode<Action<T1, T2>> next = pendingCallback.Next;
            _pendingCallbacksToAdd.Remove(pendingCallback);
            if (!_callbacks.Contains(pendingCallback.Value))
                _callbacks.AddLast(pendingCallback);
            else
                GlobalLinkedListNodeCache<Action<T1, T2>>.Release(pendingCallback);
            pendingCallback = next;
        }
    }

    private void CleanRemovePendingDuringInvoke()
    {
        if (_pendingCallbacksToRemove)
        {
            LinkedListNode<Action<T1, T2>> r = _callbacks.First;
            while (r != null)
            {
                LinkedListNode<Action<T1, T2>> next = r.Next;
                if (r.Value == null)
                {
                    _callbacks.Remove(r);
                    GlobalLinkedListNodeCache<Action<T1, T2>>.Release(r);
                }
                r = next;
            }
            _pendingCallbacksToRemove = false;
        }
    }

    private static void ClearLinkedList(LinkedList<Action<T1, T2>> l)
    {
        LinkedListNode<Action<T1, T2>> node = l.First;
        while (node != null)
        {
            LinkedListNode<Action<T1, T2>> next = node.Next;
            l.Remove(node);
            GlobalLinkedListNodeCache<Action<T1, T2>>.Release(node);
            node = next;
        }
    }

    public void Clear()
    {
        if (_invoking)
        {
            for (LinkedListNode<Action<T1, T2>> r = _callbacks.First; r != null; r = r.Next)
            {
                r.Value = null;
            }
            _pendingCallbacksToRemove = true;
            ClearLinkedList(_pendingCallbacksToAdd);
        }
        else
        {
            ClearLinkedList(_callbacks);
        }
    }

    public DelegateList<T1, T2> Set(Action<T1, T2> right)
    {
        Clear();
        Add(right);
        return this;
    }
}

public class DelegateList<T1, T2, T3>
{
    private readonly LinkedList<Action<T1, T2, T3>> _callbacks;
    private readonly LinkedList<Action<T1, T2, T3>> _pendingCallbacksToAdd;
    private bool _invoking;
    private bool _pendingCallbacksToRemove;

    private DelegateList()
    {
        _callbacks = new LinkedList<Action<T1, T2, T3>>();
        _pendingCallbacksToAdd = new LinkedList<Action<T1, T2, T3>>();
    }

    public static DelegateList<T1, T2, T3> CreateWithGlobalCache()
    {
        return new DelegateList<T1, T2, T3>();
    }

    public static void InitGlobalCache(int cacheSize)
    {
        GlobalLinkedListNodeCache<Action<T1, T2, T3>>.InitNodeCache(cacheSize);
    }

    public static void ResetGlobalCache()
    {
        GlobalLinkedListNodeCache<Action<T1, T2, T3>>.ResetNodeCache();
    }

    public Action<T1, T2, T3> GetAction(int at)
    {
        if (_callbacks == null)
            return null;

        int index = 0;
        foreach (Action<T1, T2, T3> callback in _callbacks)
        {
            if (index == at)
                return callback;
            index++;
        }

        return null;
    }

    public int Count => _callbacks?.Count ?? 0;

    public static DelegateList<T1, T2, T3> operator +(DelegateList<T1, T2, T3> left, Action<T1, T2, T3> right)
    {
        left.Add(right);
        return left;
    }

    public static DelegateList<T1, T2, T3> operator -(DelegateList<T1, T2, T3> left, Action<T1, T2, T3> right)
    {
        left.Remove(right);
        return left;
    }

    private void Add(Action<T1, T2, T3> action)
    {
        if (action == null)
            return;

        LinkedListNode<Action<T1, T2, T3>> node = GlobalLinkedListNodeCache<Action<T1, T2, T3>>.Acquire(action);
        if (_invoking)
            _pendingCallbacksToAdd.AddLast(node);
        else
        {
            if (!_callbacks.Contains(action))
                _callbacks.AddLast(node);
            else
                GlobalLinkedListNodeCache<Action<T1, T2, T3>>.Release(node);
        }
    }

    private void Remove(Action<T1, T2, T3> action)
    {
        if (action == null)
            return;

        if (!Remove(action, _callbacks, false) && _invoking)
        {
            Remove(action, _pendingCallbacksToAdd, true);
        }
    }

    private bool Remove(Action<T1, T2, T3> action, LinkedList<Action<T1, T2, T3>> list, bool isPending)
    {
        bool found = false;
        LinkedListNode<Action<T1, T2, T3>> node = list.First;
        while (node != null)
        {
            if (node.Value == action)
            {
                if (_invoking && !isPending)
                {
                    node.Value = null;
                    _pendingCallbacksToRemove = true;
                    found = true;
                }
                else
                {
                    list.Remove(node);
                    if (!isPending)
                    {
                        GlobalLinkedListNodeCache<Action<T1, T2, T3>>.Release(node);
                    }
                    return true;
                }
            }
            node = node.Next;
        }
        return found;
    }

    public void Invoke(T1 param1, T2 param2, T3 param3)
    {
        _invoking = true;
        LinkedListNode<Action<T1, T2, T3>> node = _callbacks.First;
        while (node != null)
        {
            if (node.Value != null)
            {
                try
                {
                    node.Value(param1, param2, param3);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogException(ex);
                }
            }
            node = node.Next;
        }
        _invoking = false;

        CleanPendingDuringInvoke();
    }

    private void CleanPendingDuringInvoke()
    {
        CleanRemovePendingDuringInvoke();
        CleanAddPendingDuringInvoke();
    }

    private void CleanAddPendingDuringInvoke()
    {
        LinkedListNode<Action<T1, T2, T3>> pendingCallback = _pendingCallbacksToAdd.First;
        while (pendingCallback != null)
        {
            LinkedListNode<Action<T1, T2, T3>> next = pendingCallback.Next;
            _pendingCallbacksToAdd.Remove(pendingCallback);
            if (!_callbacks.Contains(pendingCallback.Value))
                _callbacks.AddLast(pendingCallback);
            else
                GlobalLinkedListNodeCache<Action<T1, T2, T3>>.Release(pendingCallback);
            pendingCallback = next;
        }
    }

    private void CleanRemovePendingDuringInvoke()
    {
        if (_pendingCallbacksToRemove)
        {
            LinkedListNode<Action<T1, T2, T3>> r = _callbacks.First;
            while (r != null)
            {
                LinkedListNode<Action<T1, T2, T3>> next = r.Next;
                if (r.Value == null)
                {
                    _callbacks.Remove(r);
                    GlobalLinkedListNodeCache<Action<T1, T2, T3>>.Release(r);
                }
                r = next;
            }
            _pendingCallbacksToRemove = false;
        }
    }

    private static void ClearLinkedList(LinkedList<Action<T1, T2, T3>> l)
    {
        LinkedListNode<Action<T1, T2, T3>> node = l.First;
        while (node != null)
        {
            LinkedListNode<Action<T1, T2, T3>> next = node.Next;
            l.Remove(node);
            GlobalLinkedListNodeCache<Action<T1, T2, T3>>.Release(node);
            node = next;
        }
    }

    public void Clear()
    {
        if (_invoking)
        {
            for (LinkedListNode<Action<T1, T2, T3>> r = _callbacks.First; r != null; r = r.Next)
            {
                r.Value = null;
            }
            _pendingCallbacksToRemove = true;
            ClearLinkedList(_pendingCallbacksToAdd);
        }
        else
        {
            ClearLinkedList(_callbacks);
        }
    }

    public DelegateList<T1, T2, T3> Set(Action<T1, T2, T3> right)
    {
        Clear();
        Add(right);
        return this;
    }
}

public static class GlobalLinkedListNodeCache<T>
{
    private static LinkedListNodeCache<T> _globalCache;

    public static int CreatedNodeCount => _globalCache.CreatedNodeCount;
    public static int CachedNodeCount => _globalCache.CachedNodeCount;

    public static LinkedListNode<T> Acquire(T val)
    {
        if (_globalCache == null)
        {
            _globalCache = new LinkedListNodeCache<T>();
        }
        LinkedListNode<T> node = _globalCache.Acquire(val);
        return node;
    }

    public static void Release(LinkedListNode<T> node)
    {
        if (_globalCache == null)
        {
            _globalCache = new LinkedListNodeCache<T>();
        }
        _globalCache.Release(node);
    }

    public static void InitNodeCache(int _amountToInit)
    {
        _globalCache ??= new LinkedListNodeCache<T>();
        _globalCache.InitNodeCache(_amountToInit);
    }

    public static void ResetNodeCache()
    {
        _globalCache?.ResetNodeCache();
        _globalCache = null;
    }
}

internal class LinkedListNodeCache<T>
{
    private int _nodesCreated = 0;
    private LinkedList<T> _nodeCache;

    public void InitNodeCache(int amountToInit)
    {
        _nodeCache ??= new LinkedList<T>();

        for (int i = 0; i < amountToInit; i++)
        {
            AddNewNodeToCache();
        }
    }

    public void ResetNodeCache()
    {
        _nodeCache?.Clear();
        _nodeCache = null;
    }

    public LinkedListNode<T> Acquire(T val)
    {
        if (_nodeCache != null)
        {
            LinkedListNode<T> first = _nodeCache.First;
            if (first != null)
            {
                _nodeCache.RemoveFirst();
                first.Value = val;
                return first;
            }
        }

        Realloc();
        return Acquire(val);
    }

    public void Release(LinkedListNode<T> node)
    {
        EnsureNodeCacheExists();
        node.Value = default(T);
        _nodeCache.AddLast(node);
    }

    internal int CreatedNodeCount => _nodesCreated;

    internal int CachedNodeCount => _nodeCache == null ? 0 : _nodeCache.Count;

    private void AddNewNodeToCache()
    {
        LinkedListNode<T> node = new LinkedListNode<T>(default(T));
        ++_nodesCreated;
        _nodeCache.AddLast(node);
    }

    private void Realloc()
    {
        EnsureNodeCacheExists();
        int oldSize = CreatedNodeCount;
        int newSize = (int)(CreatedNodeCount * 1.2f) + 1;
        if (newSize - oldSize > 100)
            newSize = oldSize + 100;

        for (int i = oldSize; i < newSize; i++)
        {
            AddNewNodeToCache();
        }
    }

    private void EnsureNodeCacheExists()
    {
        if (_nodeCache == null)
        {
            _nodeCache = new LinkedList<T>();
        }
    }
}