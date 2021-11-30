using System.Reflection;
using HarmonyLib;
using Verse;

namespace RelationshipsReworked
{
    [StaticConstructorOnStartup]
    public static class RelationshipsReworked
    {
        static RelationshipsReworked()
        {
            Log.Message("RelationshipsReworked initialized!");
            Harmony harmony = new Harmony("RelationshipsReworked.Patches");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
    