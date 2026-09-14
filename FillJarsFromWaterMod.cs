using System;
using HarmonyLib;
using UnityEngine;

public class FillJarsFromWaterMod : IModApi
{
    public void InitMod(Mod _modInstance)
    {
        new Harmony("local.filljarsfromwater").PatchAll();
        Debug.Log("[FillJarsFromWater] loaded");
    }

    public static bool LookingAtWater(ItemActionData data)
    {
        try
        {
            var world = data?.invData?.world;
            if (world == null) return false;
            var hit = data.hitInfo;
            if (hit != null && hit.bHitValid)
            {
                if (world.IsWater(hit.hit.pos)) return true;
                if (world.IsWater(hit.hit.blockPos)) return true;
                if (world.IsWater(hit.lastBlockPos)) return true;
                try
                {
                    var name = world.GetBlock(hit.hit.blockPos).Block?.GetBlockName() ?? "";
                    if (name.IndexOf("water", StringComparison.OrdinalIgnoreCase) >= 0)
                        return true;
                }
                catch { }
            }
            var ent = data.invData.holdingEntity;
            if (ent != null)
            {
                var look = ent.GetLookVector();
                var origin = ent.position + new Vector3(0f, 1.6f, 0f);
                for (int i = 1; i <= 6; i++)
                {
                    var p = origin + look * i;
                    if (world.IsWater(p)) return true;
                    if (world.IsWater(Vector3i.FromVector3Rounded(p))) return true;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("[FillJarsFromWater] LookingAtWater: " + e.Message);
        }
        return false;
    }

    public static bool TryFillJar(ItemActionData data)
    {
        try
        {
            var invData = data.invData;
            var entity = invData.holdingEntity as EntityPlayer;
            if (entity == null || entity.inventory == null) return false;
            var empty = ItemClass.GetItem("drinkJarEmpty", true);
            var cur = invData.itemStack;
            if (cur == null || cur.count < 1) return false;
            if (cur.itemValue.type != empty.type) return false;
            if (!LookingAtWater(data)) return false;

            var slot = invData.slotIdx;
            var filled = new ItemStack(ItemClass.GetItem("drinkJarRiverWater", true), 1);
            if (cur.count <= 1)
            {
                entity.inventory.SetItem(slot, filled);
            }
            else
            {
                cur.count--;
                entity.inventory.SetItem(slot, cur);
                if (!GiveToPlayer(entity, filled, slot))
                    GameManager.Instance.ItemDropServer(filled, entity.position, Vector3.zero, entity.entityId);
            }
            data.lastUseTime = Time.time;
            Debug.Log("[FillJarsFromWater] filled a jar into inventory");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning("[FillJarsFromWater] TryFillJar: " + e);
            return false;
        }
    }

    static bool GiveToPlayer(EntityPlayer entity, ItemStack stack, int preferSlot)
    {
        try
        {
            var local = entity as EntityPlayerLocal;
            if (local != null && local.bag != null && local.bag.AddItem(stack))
                return true;
        }
        catch { }
        try
        {
            if (entity.inventory.AddItem(stack))
                return true;
        }
        catch { }
        try
        {
            for (int i = 0; i < 10; i++)
            {
                if (i == preferSlot) continue;
                var s = entity.inventory.GetItem(i);
                if (s == null || s.IsEmpty())
                {
                    entity.inventory.SetItem(i, stack);
                    return true;
                }
            }
        }
        catch { }
        return false;
    }
}

[HarmonyPatch(typeof(ItemActionCollectWater), "ExecuteAction")]
static class Patch_CollectWater
{
    static bool Prefix(ItemActionData _actionData, bool _bReleased)
    {
        if (_bReleased) return true;
        if (FillJarsFromWaterMod.TryFillJar(_actionData))
            return false;
        return true;
    }
}
