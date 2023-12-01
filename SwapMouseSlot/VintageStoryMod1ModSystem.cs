using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace VintageStoryMod1
{
    public class VintageStoryMod1ModSystem : ModSystem
    {
        private ICoreClientAPI capi;

        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return forSide == EnumAppSide.Client;
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);

            capi = api;
            capi.Input.RegisterHotKey("swapmouseitemslot", "Swap Mouse And Active Hotbat Slot", GlKeys.X,
                HotkeyType.InventoryHotkeys);
            capi.Input.SetHotKeyHandler("swapmouseitemslot", Handler);
        }

        private bool Handler(KeyCombination t1)
        {
            try
            {
                InventoryUtil.SwapMouseSlotAndActiveHotbarSlot(capi);
            }
            catch (System.Exception e)
            {
                capi.Logger.Error(e);
                capi.World.Player.ShowChatNotification("An eldritch horror is keeping you from swapping items!");
            }

            return true;
        }
    }
}