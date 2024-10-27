using HarmonyLib;
using MoonScrapLimitsPatch.Util;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MoonScrapLimitsPatch.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    internal static class RoundManagerPatcher
    {
        [HarmonyPatch(nameof(RoundManager.SpawnScrapInLevel))]
        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> RoundManagerSpawnScrapInLevel_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new(instructions);
            int index = 0;
            /*
            * Before: int num3 = 0;
            * After: int num3 = AnomalyRandom.Next(currentLevel.minTotalScrapValue, currentLevel.maxTotalScrapValue);
            */
            Tools.FindLocalField(ref index, ref codes, localIndex: 4, skip: true, store: true, errorMessage: "Couldn't find the local field used to store minTotalScrapValue");
            codes.RemoveAt(index - 2);
            codes.Insert(index - 2, new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(System.Random), nameof(System.Random.Next), [typeof(int), typeof(int)])));
            codes.Insert(index - 2, new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(SelectableLevel), nameof(SelectableLevel.maxTotalScrapValue))));
            codes.Insert(index - 2, new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(RoundManager), nameof(RoundManager.currentLevel))));
            codes.Insert(index - 2, new CodeInstruction(OpCodes.Ldarg_0));
            codes.Insert(index - 2, new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(SelectableLevel), nameof(SelectableLevel.minTotalScrapValue))));
            codes.Insert(index - 2, new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(RoundManager), nameof(RoundManager.currentLevel))));
            codes.Insert(index - 2, new CodeInstruction(OpCodes.Ldarg_0));
            codes.Insert(index - 2, new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(RoundManager), nameof(RoundManager.AnomalyRandom))));
            codes.Insert(index - 2, new CodeInstruction(OpCodes.Ldarg_0));

            Tools.FindLocalField(ref index, ref codes, localIndex: 4, skip: true, store: true, errorMessage: "Couldn't find the local field used to store minTotalScrapValue");
            Tools.FindLocalField(ref index, ref codes, localIndex: 4, skip: true, store: true, errorMessage: "Couldn't find the local field used to store minTotalScrapValue");
            codes.RemoveAt(index - 2);
            codes.Insert(index - 2, new CodeInstruction(OpCodes.Sub));
            for(; index < codes.Count; index++)
            {
                if (codes[index].opcode == OpCodes.Blt) break;
            }
            codes[index].opcode = OpCodes.Brtrue;
            codes.Insert(index, new CodeInstruction(OpCodes.And));
            codes.Insert(index, new CodeInstruction(OpCodes.Cgt));
            codes.Insert(index, new CodeInstruction(OpCodes.Ldc_I4_0));
            codes.Insert(index, new CodeInstruction(OpCodes.Ldloc_S, operand: 4));
            codes.Insert(index, new CodeInstruction(OpCodes.Clt));

            return codes;
        }
    }
}
