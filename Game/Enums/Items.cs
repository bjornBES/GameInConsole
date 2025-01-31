using GameInConsole.Game.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInConsole.Game.Enums
{
    public enum Items
    {
        none,

        wood,
        stick,

        stone,
        flint,
    }

    public static class ItemData
    {
        public static Dictionary<Items, ItemDataEntry> itemData = new Dictionary<Items, ItemDataEntry>()
        {
            { Items.none, new ItemDataEntry() },
            { Items.wood, new ItemDataEntry(Items.wood, 25, "img_wood").IsMaterialItem() },
            { Items.stick, new ItemDataEntry(Items.stick, 50, "img_stick").IsMaterialItem().IsAttackItem(new Stats(0, 2), AttackFunctions.AttackStick) },
            { Items.stone, new ItemDataEntry(Items.stone, 10, "img_stone").IsMaterialItem() },
            { Items.flint, new ItemDataEntry(Items.flint, 20, "img_flint").IsMaterialItem() },
        };
    }
    public class ItemDataEntry
    {
        public Items Item;
        public int MaxItemStack;
        public string ResourceKey;

        public bool IsAttackingItem = false;
        public AttackFunction AttackingFunction;
        public Stats ItemStats;

        public bool IsMaterial = false;

        public ItemDataEntry()
        {
            Item = Items.none;
            MaxItemStack = 0;
        }
        public ItemDataEntry(Items item, int maxItemStack, string itemImgKey)
        {
            ResourceKey = itemImgKey;
            Item = item;
            MaxItemStack = maxItemStack;
        }

        public ItemDataEntry IsAttackItem(Stats itemStats, AttackFunction attackFunction)
        {
            ItemDataEntry result = this;

            result.ItemStats = itemStats;
            result.AttackingFunction = attackFunction;
            result.IsAttackingItem = true;

            return result;
        }
        public ItemDataEntry IsMaterialItem()
        {
            ItemDataEntry result = this;

            result.IsMaterial = true;

            return result;
        }
    }
}
