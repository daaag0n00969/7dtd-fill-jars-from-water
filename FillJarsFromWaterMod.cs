using System;
using HarmonyLib;
using UnityEngine;

public class FillJarsFromWaterMod : IModApi
{
    // One RMB press = one fill. Vanilla CollectWater must not run afterwards
    // or it drops drinkJarRiverWater next to the player.
    public static bool FilledThisHold;

    public void InitMod(Mod _modInstance)
    {
        new Harmony("local.filljarsfromwater").PatchAll();
        Debug.Log("[FillJarsFromWater] loaded 1.0.2");
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
                try { entity.inventory.Changed(); } catch { }
            }
            else
            {
                cur.count--;
                entity.inventory.SetItem(slot, cur);
                try { entity.inventory.Changed(); } catch { }
                if (!GiveToPlayer(entity, filled, slot))
                {
                    GameManager.Instance.ItemDropServer(filled, entity.GetPosition(), Vector3.zero, entity.entityId);
                    Debug.LogWarning("[FillJarsFromWater] inventory full, dropped murky water");
                }
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
            if (local != null)
            {
                try
                {
                    var ui = LocalPlayerUI.GetUIForPlayer(local);
                    var pinv = ui != null ? ui.xui.PlayerInventory : null;
                    if (pinv != null)
                    {
                        if (pinv.AddItem(stack, true)) return true;
                        if (pinv.AddItemToBackpack(stack)) return true;
                        if (pinv.AddItemToToolbelt(stack)) return true;
                    }
                }
                catch { }
                try
                {
                    if (local.bag != null && local.bag.AddItem(stack))
                        return true;
                }
                catch { }
            }
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
        if (_bReleased)
        {
            bool skipVanilla = FillJarsFromWaterMod.FilledThisHold;
            FillJarsFromWaterMod.FilledThisHold = false;
            return !skipVanilla;
        }

        if (FillJarsFromWaterMod.FilledThisHold)
            return false;

        if (FillJarsFromWaterMod.TryFillJar(_actionData))
        {
            FillJarsFromWaterMod.FilledThisHold = true;
            return false;
        }
        return true;
    }
}
