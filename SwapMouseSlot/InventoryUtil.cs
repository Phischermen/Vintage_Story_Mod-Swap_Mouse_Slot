using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace VintageStoryMod1;

public class InventoryUtil
{
    public static void SwapMouseSlotAndActiveHotbarSlot(ICoreClientAPI capi)
    {
        var player = capi.World.Player;
        var im = player.InventoryManager;
        var mSlot = im.MouseItemSlot;
        var aSlot = im.ActiveHotbarSlot;
        ItemSlot tSlot = null;

        if (mSlot == null || aSlot == null)
        {
            return;
        }

        if (mSlot.Empty && aSlot.Empty)
        {
            return;
        }

        if (mSlot.Empty == false && aSlot.Empty)
        {
            TryTransferTo(mSlot, aSlot);
        }
        else if (aSlot.Empty == false && mSlot.Empty)
        {
            TryTransferTo(aSlot, mSlot);
        }
        else
        {
            // Note: tSlot will be assigned in Matcher
            var find = im.Find(Matcher);

            if (!find)
            {
                // Show the player a message
                capi.ShowChatMessage("No empty slot found!");
                return;
            }

            TryTransferTo(mSlot, tSlot);
            TryTransferTo(aSlot, mSlot);
            TryTransferTo(tSlot, aSlot);
        }
        mSlot.MarkDirty();
        aSlot.MarkDirty();
        tSlot?.MarkDirty();
        
        return;

        bool Matcher(ItemSlot slot)
        {
            if (slot == null)
            {
                return false;
            }
            var b1 = slot != mSlot && slot != aSlot;
            var b2 = !slot.Inventory.InventoryID.StartsWith("ground");
            var b3 = slot.Empty;
            var b4 = slot.CanHold(mSlot);
            if (b1 && b2 && b3 && b4)
            {
                tSlot = slot;
                return true;
            }

            return false;
        }

        void TryTransferTo(ItemSlot source, ItemSlot target)
        {
            ItemStackMoveOperation op =
                new ItemStackMoveOperation(capi.World, EnumMouseButton.Left, 0, EnumMergePriority.AutoMerge,
                    source.Itemstack?.StackSize ?? 0);
            var packet = im.TryTransferTo(source, target, ref op);
            capi.Network.SendPacketClient(packet);
        }
    }
}