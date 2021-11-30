using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
	partial class Program : MyGridProgram
	{
		IMyTextPanel logOutput;
		IMyBlockGroup teyItemGroup;
		List<IMyCargoContainer> teyComponentCargos = new List<IMyCargoContainer>();
		List<IMyCargoContainer> teyIngotCargos = new List<IMyCargoContainer>();
		List<IMyCargoContainer> teyOreCargos = new List<IMyCargoContainer>();
		List<IMyCargoContainer> teyToolCargos = new List<IMyCargoContainer>();
		List<string> itemTypes = new List<string>();
		List<string> teyNameList = new List<string>();

		public Program()
		{
			try
			{
				logOutput = GridTerminalSystem.GetBlockWithName("Log LCD") as IMyTextPanel;

				itemTypeListInit();
				teyListInit();
				initTEYGroup();
				searchAllContainer();
			}
			catch (Exception e)
			{
				EchoToLCD(e.StackTrace, true);
			}
		}


		public void Main(string argument, UpdateType updateSource)
		{
			try
			{
				searchAllContainer();
			}
			catch (Exception e)
			{
				EchoToLCD(e.StackTrace, true);
			}
		}

		private void searchAllContainer()
		{
			IMyCargoContainer srcContainer;
			var containerList = new List<IMyCargoContainer>();
			teyItemGroup.GetBlocksOfType<IMyCargoContainer>(containerList);
			foreach (var container in containerList)
			{
				//EchoToLCD("Bool : " + container.DisplayNameText.Contains("INGOT"), true);
				if (container.DisplayNameText.Contains(itemTypes[0]))
				{
					teyOreCargos.Add(container);
				}
				else if (container.DisplayNameText.Contains(itemTypes[1]))
				{
					teyIngotCargos.Add(container);
				}
				else if (container.DisplayNameText.Contains(itemTypes[2]))
				{
					teyComponentCargos.Add(container);
				}
				else if (container.DisplayNameText.Contains(itemTypes[3]))
				{
					teyToolCargos.Add(container);
				}
				else
				{
					EchoToLCD("İtem Container'ı bulunamadı : " + container.DisplayNameText, true);
				}
			}
			var items = new List<MyInventoryItem>();
			foreach (var blk in containerList)
			{
				blk.GetInventory().GetItems(items);
				srcContainer = (IMyCargoContainer)blk;
				foreach (var item in items)
				{
					if (item.Type.GetItemInfo().IsComponent)
					{
						addItemContainer(item, srcContainer, teyComponentCargos);
						EchoToLCD("Gönderilen : " + item.ToString() + " Alınan Depo : " + srcContainer.DisplayNameText, true);
					}
					else if (item.Type.GetItemInfo().IsIngot)
					{
						addItemContainer(item, srcContainer, teyIngotCargos);
						EchoToLCD("Gönderilen : " + item.ToString() + " Alınan Depo : " + srcContainer.DisplayNameText, true);

					}
					else if (item.Type.GetItemInfo().IsOre)
					{
						addItemContainer(item, srcContainer, teyOreCargos);
						EchoToLCD("Gönderilen : " + item.ToString() + " Alınan Depo : " + srcContainer.DisplayNameText, true);

					}
					else if (item.Type.GetItemInfo().IsTool)
					{
						addItemContainer(item, srcContainer, teyToolCargos);
						EchoToLCD("Gönderilen : " + item.ToString() + " Alınan Depo : " + srcContainer.DisplayNameText, true);

					}
					else if (item.Type.GetItemInfo().IsAmmo)
					{
						addItemContainer(item, srcContainer, teyToolCargos);
						EchoToLCD("Gönderilen : " + item.ToString() + " Alınan Depo : " + srcContainer.DisplayNameText, true);
					}
				}
			}
		}

		private void addItemContainer(MyInventoryItem item, IMyCargoContainer srcContainer, List<IMyCargoContainer> teyCargos)
		{
			IMyCargoContainer dstContainer;
			IMyInventory srcInventory, dstInventory;

			foreach (var container in teyCargos)
			{
				dstContainer = container;
				srcInventory = srcContainer.GetInventory();
				dstInventory = dstContainer.GetInventory();
				if (!dstInventory.IsFull)
				{
					srcInventory.TransferItemTo(dstInventory, item);
					break;
				}
				else
				{
					continue;
				}
			}
		}


		private void initTEYGroup()
		{
			var groupList = new List<IMyBlockGroup>();
			GridTerminalSystem.GetBlockGroups(groupList, group => group.Name.Contains(teyNameList[0]));
			foreach (var group in groupList)
			{
				if (group.Name.Contains(teyNameList[0]))
				{
					teyItemGroup = group;
				}
				else
				{
					EchoToLCD("TEY-item Bulunamadı", true);
				}
			}
		}

		public void itemTypeListInit()
		{
			itemTypes.Add("ORE");
			itemTypes.Add("INGOT");
			itemTypes.Add("COMPONENT");
			itemTypes.Add("TOOL");
		}

		public void teyListInit()
		{
			teyNameList.Add("TEY");
			teyNameList.Add("TEY-Container");
			teyNameList.Add("TEY-Assembler");
			teyNameList.Add("TEY-Refinery");
		}

		public void EchoToLCD(string text, bool append)
		{
			// Append the text and a newline to the logging LCD
			// A nice little C# trick here:
			// - The ?. after _logOutput means "call only if _logOutput is not null".
			logOutput?.WriteText($"{text}\n", append);
		}
	}
}
