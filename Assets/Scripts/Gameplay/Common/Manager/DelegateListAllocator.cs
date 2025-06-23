using UnityEngine;

public class DelegateListAllocator
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void InitializeDelegateCache()
    {
#if UNITY_EDITOR
        ResetFastPlayMode();
#endif
        AllocateDelegateLists();
    }
    private static void ResetFastPlayMode()
    {
        DelegateList<bool>.CreateWithGlobalCache();
    }

    private static void AllocateDelegateLists()
    {
        DelegateList<bool>.InitGlobalCache(100);
    }
}