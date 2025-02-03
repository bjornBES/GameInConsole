using GameInConsole.Game.Data;
using GameInConsole.Game.Enums;
using GameInConsoleEngine.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInConsole.Game.PlayerSystem
{
    public class Player : ThreadMono
    {
        private List<InventorySlotEntry> PlayerInventory = new List<InventorySlotEntry>();
        private Stats PlayerStats;
        private int counter = 0;
        public override void Start()
        {
            PlayerStats = new Stats(10,0);

            PlayerInventory.Clear();

            AddItem(Items.stick, 50);
            AddItem(Items.stick, 1);
        }
        public override void Stop()
        {
        }
        public override void Update()
        {
            counter++;
        }
        public override void Render()
        {
            Engine.WriteText(new Point(0, 20),$"Player = {counter}", 15);
        }

        public void AddItem(Items item, int quantity)
        {
            int index;
            if (IsItemInInv(item, out index, true))
            {
                PlayerInventory[index].Quantity += quantity;
            }
            else
            {
                index = PlayerInventory.Count;
                PlayerInventory.Add(new InventorySlotEntry(item, quantity));
            }

            if (PlayerInventory[index].Quantity > ItemData.itemData[item].MaxItemStack)
            {
                int diff = PlayerInventory[index].Quantity - ItemData.itemData[item].MaxItemStack;
                PlayerInventory[index].Quantity -= diff;
                AddItem(item, diff);
            }
        }

        public void RemoveItem(Items item, int quantity)
        {
            if (IsItemInInv(item, out int index))
            {
                if (quantity <= PlayerInventory[index].Quantity)
                {
                    PlayerInventory[index].Quantity -= quantity;
                }

                if (PlayerInventory[index].Quantity == 0)
                {
                    PlayerInventory[index].Item = Items.none;
                }
            }
            else
            {
                return;
            }
        }

        private bool IsItemInInv(Items item, out int index, bool toAdd = false)
        {
            index = -1;
            for (int i = 0; i < PlayerInventory.Count; i++)
            {
                if (PlayerInventory[i].Item == item)
                {
                    if (toAdd)
                    {
                        if (PlayerInventory[i].Quantity >= ItemData.itemData[item].MaxItemStack)
                        {
                            break;
                        }
                    }
                    index = i;
                    return true;
                }
            }
            return false;
        }
    }
    public class InventorySlotEntry
    {
        public Items Item { get; set; } = Items.none;
        public int Quantity { get; set; } = 0;
        public InventorySlotEntry(Items item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }
    }
}
