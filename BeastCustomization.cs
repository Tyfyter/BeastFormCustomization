using log4net;
using log4net.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using ReLogic.Content;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.UI.Chat;

namespace BeastCustomization {
	public class BeastCustomization : Mod {
#if DEBUG
		public static ILog DebugLogger => ModContent.GetInstance<BeastCustomization>().Logger;
#else
		public static ILog DebugLogger => new Nonlogger();
#endif
		static BeastCustomization Instance => ModContent.GetInstance<BeastCustomization>();
		public static AutoCastingAsset<Texture2D> SelectorEndTexture { get; private set; }
		public static AutoCastingAsset<Texture2D> SelectorMidTexture { get; private set; }
		public static AutoCastingAsset<Texture2D> ButtonEndTexture { get; private set; }
		public static AutoCastingAsset<Texture2D> ButtonMidTexture { get; private set; }
		static List<int> beastPlayers;
		public static List<int> BeastPlayers {
			get => beastPlayers ??= new();
			private set => beastPlayers = value;
		}
		static Dictionary<Type, int> beastPlayersByType;
		public static Dictionary<Type, int> BeastPlayersByType {
			get => beastPlayersByType ??= new();
			private set => beastPlayersByType = value;
		}
		static List<int> hairDyes;
		public static List<int> HairDyes {
			get => hairDyes ??= new();
			private set => hairDyes = value;
		}
		public static ModKeybind OpenMenuHotkey { get; private set; }
		internal static FastFieldInfo<RangeElement, RangeElement> _rightLock;
		internal static FastFieldInfo<RangeElement, RangeElement> _rightHover;
		internal static HashSet<int> modMoonCharms;
		public override void Load() {
			if (Main.netMode != NetmodeID.Server) {
				SelectorEndTexture = Assets.Request<Texture2D>("Textures/UI/Selector_Back_End");
				SelectorMidTexture = Assets.Request<Texture2D>("Textures/UI/Selector_Back_Mid");
				ButtonEndTexture = Assets.Request<Texture2D>("Textures/UI/Button_Back_End");
				ButtonMidTexture = Assets.Request<Texture2D>("Textures/UI/Button_Back_Mid");
			}
			OpenMenuHotkey = KeybindLoader.RegisterKeybind(this, "Open Customization Menu", "NumPad7");
			_rightLock = new("rightLock", BindingFlags.NonPublic | BindingFlags.Static, true);
			_rightHover = new("rightHover", BindingFlags.NonPublic | BindingFlags.Static, true);
			modMoonCharms = new();
		}
		public override void Unload() {
			BeastPlayers = null;
			HairDyes = null;
			SelectorEndTexture = null;
			SelectorMidTexture = null;
			ButtonEndTexture = null;
			ButtonMidTexture = null;
			OpenMenuHotkey = null;
			_rightLock = null;
			_rightHover = null;
			modMoonCharms = null;
			BeastPlayerBase.ExportDatas = null;
			BeastPlayerBase.ImportDatas = null;
			BeastPlayerBase.NetSends = null;
			BeastPlayerBase.NetReceives = null;
		}
		public static void FillSpriteList(List<AutoCastingAsset<Texture2D>> list, string baseName, int startIndex = 1) {
			while (Instance.RequestAssetIfExists($"{baseName}_{startIndex}", out Asset<Texture2D> sprite)) {
				list.Add(sprite);
				startIndex++;
			}
		}
		public override void HandlePacket(BinaryReader reader, int whoAmI) {
			byte mode = reader.ReadByte();
			if (Main.netMode == NetmodeID.Server) {
				switch (mode) {
					case 0: {
						short targetPlayer = reader.ReadInt16();
						WolfColorPlayer syncPlayer = new();
						ushort beastPlayerIndex = reader.ReadUInt16();
						syncPlayer.NetReceive(reader);

						ModPacket packet = GetPacket();
						packet.Write((byte)0);
						packet.Write((short)whoAmI);
						packet.Write(beastPlayerIndex);
						syncPlayer.NetSend(packet);
						packet.Send(targetPlayer, whoAmI);
						break;
					}
				}
			} else {
				switch (mode) {
					case 0:
					(Main.player[reader.ReadInt16()].ModPlayers[BeastPlayers[reader.ReadUInt16()]] as BeastPlayerBase).NetReceive(reader);
					break;

					case 1:
					(Main.LocalPlayer.ModPlayers[BeastPlayers[reader.ReadUInt16()]] as BeastPlayerBase).SendData(reader.ReadInt16());
					break;
				}
			}
		}
		public override void PostSetupContent() {
			HairDyes.Add(ItemID.HairDyeRemover);
			foreach (var item in ContentSamples.ItemsByType.Values) {
				if (item.hairDye > -1 && item.type != ItemID.HairDyeRemover) {
					HairDyes.Add(item.type);
				}
			}
		}
	}
	internal class Nonlogger : ILog {
		public bool IsDebugEnabled { get; }
		public bool IsInfoEnabled { get; }
		public bool IsWarnEnabled { get; }
		public bool IsErrorEnabled { get; }
		public bool IsFatalEnabled { get; }
		public ILogger Logger { get; }

		public void Debug(object message) {}

		public void Debug(object message, Exception exception) {}

		public void DebugFormat(string format, params object[] args) {}

		public void DebugFormat(string format, object arg0) {}

		public void DebugFormat(string format, object arg0, object arg1) {}

		public void DebugFormat(string format, object arg0, object arg1, object arg2) {}

		public void DebugFormat(IFormatProvider provider, string format, params object[] args) {}

		public void Error(object message) {}

		public void Error(object message, Exception exception) {}

		public void ErrorFormat(string format, params object[] args) {}

		public void ErrorFormat(string format, object arg0) {}

		public void ErrorFormat(string format, object arg0, object arg1) {}

		public void ErrorFormat(string format, object arg0, object arg1, object arg2) {}

		public void ErrorFormat(IFormatProvider provider, string format, params object[] args) {}

		public void Fatal(object message) {}

		public void Fatal(object message, Exception exception) {}

		public void FatalFormat(string format, params object[] args) {}

		public void FatalFormat(string format, object arg0) {}

		public void FatalFormat(string format, object arg0, object arg1) {}

		public void FatalFormat(string format, object arg0, object arg1, object arg2) {}

		public void FatalFormat(IFormatProvider provider, string format, params object[] args) {}

		public void Info(object message) {}

		public void Info(object message, Exception exception) {}

		public void InfoFormat(string format, params object[] args) {}

		public void InfoFormat(string format, object arg0) {}

		public void InfoFormat(string format, object arg0, object arg1) {}

		public void InfoFormat(string format, object arg0, object arg1, object arg2) {}

		public void InfoFormat(IFormatProvider provider, string format, params object[] args) {}

		public void Warn(object message) {}

		public void Warn(object message, Exception exception) {}

		public void WarnFormat(string format, params object[] args) {}

		public void WarnFormat(string format, object arg0) {}

		public void WarnFormat(string format, object arg0, object arg1) {}

		public void WarnFormat(string format, object arg0, object arg1, object arg2) {}

		public void WarnFormat(IFormatProvider provider, string format, params object[] args) {}
	}
	public struct AutoCastingAsset<T> where T : class {
		public bool IsLoaded => asset?.IsLoaded ?? false;
		public T Value => asset?.Value;

		readonly Asset<T> asset;
		AutoCastingAsset(Asset<T> asset) {
			this.asset = asset;
		}
		public static implicit operator AutoCastingAsset<T>(Asset<T> asset) => new(asset);
		public static implicit operator T(AutoCastingAsset<T> asset) => asset.Value;
	}

	public class CustomizationMenuState : UIState {
		readonly int index;
		internal CustomizationMenu customizationMenu;
		internal static float scrollbarStartPos = 0;
		public CustomizationMenuState(int index) : base() {
			this.index = index;
		}
		public override void OnInitialize() {
			Main.UIScaleMatrix.Decompose(out Vector3 scale, out Quaternion _, out Vector3 _);
			customizationMenu = new CustomizationMenu(index);
			Append(customizationMenu);
			PresetsMenu presetsMenu = new PresetsMenu(index);
			presetsMenu.Left.Pixels += (416f + 10) * scale.X;
			Append(presetsMenu);
		}
		public override void Update(GameTime gameTime) {
			base.Update(gameTime);
			if (scrollbarStartPos != 0) {
				customizationMenu.scrollbar.ViewPosition = scrollbarStartPos;
				scrollbarStartPos = 0;
			}
		}
	}
	public class CustomizationMenu : UIElement {
		public float totalHeight;
		CustomizationMenuList listWrapper;
		CustomizationMenuList listWrapper2;
		internal UIScrollbar scrollbar;
		readonly int index;
		public CustomizationMenu(int index) : base() {
			this.index = index;
		}
		public override void OnActivate() {
			SoundEngine.PlaySound(SoundID.MenuOpen);
		}
		public override void OnInitialize() {
			if (!(Elements is null)) Elements.Clear();
			Top.Pixels = 0;
			Main.UIScaleMatrix.Decompose(out Vector3 scale, out Quaternion _, out Vector3 _);
			totalHeight = 39 * scale.Y;
			UIElement element;
			int top = 6;
			BeastPlayerBase beastPlayer = Main.LocalPlayer.ModPlayers[BeastCustomization.BeastPlayers[index]] as BeastPlayerBase;
			if (beastPlayer is null) {
				this.Deactivate();
				Remove();
				return;
			}
			beastPlayer.StartCustomization();
			PropertyFieldWrapper[] settingList = beastPlayer.GetType()
				.GetFields().Where(v => v.IsPublic && v.GetCustomAttribute<JsonIgnoreAttribute>() is null)
				.Select(v => new PropertyFieldWrapper(v)
			)
			.ToArray();
			Width.Set((416f + 10) * scale.X, 0);
			listWrapper = new CustomizationMenuList();
			listWrapper2 = new CustomizationMenuList();
			Tuple<UIElement, UIElement> wrapper = null;
			for (int i = 0; i < settingList.Length; i++) {
				int sliderMax = -1;
				if (settingList[i].MemberInfo.GetCustomAttribute<ListRangeAttribute>() is ListRangeAttribute rangeAttribute) {
					sliderMax = ((IList)beastPlayer.ResourceCacheType.
						GetProperty(rangeAttribute.ListName, BindingFlags.Public | BindingFlags.Static)
						.GetValue(null)).Count;
				}
				if (sliderMax == 1) continue;
				wrapper = ConfigManager.WrapIt(this, ref top, settingList[i], beastPlayer, i, index: i);
				element = wrapper.Item2;
				if (sliderMax > 0 && element is PrimitiveRangeElement<int> rangeElement) {
					rangeElement.Min = 0;
					rangeElement.Max = sliderMax - 1;
				}
				//if (element is RangeElement) Width.Set(416f * scale.X, 0);
				//element.Top.Set(element.Top.Pixels + top, element.Top.Percent);
				totalHeight += top;
				wrapper.Item1.Width.Pixels -= 10;
				listWrapper.Append(wrapper.Item1);
			}
			listWrapper.Height.Set(Height.Pixels, 0);
			listWrapper2.Append(listWrapper);

			listWrapper2.Height.Set(-(52 + 16), 1);
			listWrapper2.OverflowHidden = true;
			listWrapper2.Left.Pixels = -12;
			listWrapper2.Top.Pixels += 8;
			Append(listWrapper2);

			UIButton cancelButton = new UIButton() {
				Text = Language.GetTextValue("tModLoader.Exit"),
				Left = new(0, 0.20f),
				Top = new(-16, 1),
				Scale = 1.15f
			};
			cancelButton.Left.Pixels -= cancelButton.Width.Pixels * 0.5f;
			cancelButton.Top.Pixels -= cancelButton.Height.Pixels;
			cancelButton.OnClick += (el) => {
				IngameFancyUI.Close();
			};
			Append(cancelButton);

			UIButton resetButton = new UIButton() {
				Text = Language.GetTextValue("tModLoader.ModConfigRevertChanges"),
				Left = new(0, 0.50f),
				Top = new(-16, 1),
				Scale = 1.15f
			};
			resetButton.Left.Pixels -= resetButton.Width.Pixels * 0.5f;
			resetButton.Top.Pixels -= resetButton.Height.Pixels;
			Type colorElement = typeof(ConfigElement).Assembly.GetType("Terraria.ModLoader.Config.UI.ColorElement");
			Type colorObject = typeof(ConfigElement).Assembly.GetType("Terraria.ModLoader.Config.UI.ColorElement+ColorObject");
			FieldInfo _c = colorElement.GetField("c", BindingFlags.NonPublic | BindingFlags.Instance);
			FieldInfo _current = colorObject.GetField("current", BindingFlags.NonPublic | BindingFlags.Instance);
			PropertyInfo _MemberInfo = typeof(ConfigElement).GetProperty("MemberInfo", BindingFlags.NonPublic | BindingFlags.Instance);

			resetButton.OnClick += (el) => {
				beastPlayer.FinishCustomization(false);
				//this.Reinitialize();
				CustomizationMenuState.scrollbarStartPos = scrollbar.ViewPosition;
				IngameFancyUI.OpenUIState(new CustomizationMenuState(index));
			};
			Append(resetButton);

			UIButton saveButton = new UIButton() {
				Text = Language.GetTextValue("UI.Save"),
				Left = new(0, 0.80f),
				Top = new(-16, 1),
				Scale = 1.15f
			};
			saveButton.Left.Pixels -= saveButton.Width.Pixels * 0.5f;
			saveButton.Top.Pixels -= saveButton.Height.Pixels;
			saveButton.OnClick += (el) => {
				beastPlayer.FinishCustomization(true);
				beastPlayer.StartCustomization();
			};
			Append(saveButton);

			Append(listWrapper2);
			if (listWrapper.Height.Pixels > Main.screenHeight - (52 + 16)) {
				scrollbar = new();
				scrollbar.Top.Set(8, 0);
				scrollbar.Height.Set(-16, 1);
				scrollbar.Left.Set(-20, 1);
				scrollbar.SetView(Main.screenHeight - (52 + 16), listWrapper.Height.Pixels);
				Append(scrollbar);
			}
			Height.Set(0, 1);
			//Height.Set(Math.Min(totalHeight + (Width.Pixels / 8), Main.screenHeight * 0.9f), 0);
			//Left.Set(Width.Pixels * 0.1f, 1f);
			//Top.Set(Height.Pixels * -0.5f, 0.5f);
		}
		public override void Update(GameTime gameTime) {
			if (scrollbar is not null) {
				float oldTop = listWrapper.Top.Pixels;
				listWrapper.Top.Pixels = -scrollbar.ViewPosition;
				if (listWrapper.Top.Pixels != oldTop) this.Recalculate();
			}
		}

		public override void OnDeactivate() {
			(Main.LocalPlayer.ModPlayers[BeastCustomization.BeastPlayers[index]] as BeastPlayerBase).FinishCustomization(false);
		}
		public override void ScrollWheel(UIScrollWheelEvent evt) {
			if (scrollbar is not null) {
				scrollbar.ViewPosition -= evt.ScrollWheelValue;
			}
			//listWrapper.Top.Pixels = MathHelper.Clamp(listWrapper.Top.Pixels + evt.ScrollWheelValue, (Main.screenHeight - (52 + 16)) - listWrapper.Height.Pixels, 0);
			//this.Recalculate();
		}
		public override void Draw(SpriteBatch spriteBatch) {
			base.Draw(spriteBatch);
		}
		protected override void DrawSelf(SpriteBatch spriteBatch) {
			Rectangle dimensions = this.GetDimensions().ToRectangle();
			if (Main.mouseY < dimensions.Width && !PlayerInput.IgnoreMouseInterface) {
				Main.LocalPlayer.mouseInterface = true;
			}
			int endHeight = dimensions.Width / 8;
			Color color = Color.IndianRed;
			Rectangle topRect = new Rectangle(dimensions.X, dimensions.Y, dimensions.Width - 10, endHeight);
			Rectangle midRect = new Rectangle(dimensions.X, dimensions.Y + endHeight, dimensions.Width - 10, dimensions.Height - (endHeight * 2));
			Rectangle bottomRect = new Rectangle(dimensions.X, dimensions.Y + dimensions.Height - endHeight, dimensions.Width - 10, endHeight);
			spriteBatch.Draw(BeastCustomization.SelectorEndTexture, topRect, new Rectangle(0, 0, 208, 26), color, 0, default, SpriteEffects.None, 0);
			spriteBatch.Draw(BeastCustomization.SelectorMidTexture, midRect, new Rectangle(0, 0, 208, 1), color, 0, default, SpriteEffects.None, 0);
			spriteBatch.Draw(BeastCustomization.SelectorEndTexture, bottomRect, new Rectangle(0, 0, 208, 26), color, 0, default, SpriteEffects.FlipVertically, 0);
			//spriteBatch.Draw(TextureAssets.InventoryBack2.Value, dimensions, null, color, 0, default, SpriteEffects.None, 0);

		}
		protected override void DrawChildren(SpriteBatch spriteBatch) {
			base.DrawChildren(spriteBatch);
			if (Main.hoverItemName != null && Main.hoverItemName != "") {
				Main.LocalPlayer.cursorItemIconEnabled = false;
				if (Main.SettingsEnabled_OpaqueBoxBehindTooltips) {
					Main.instance.MouseText(Main.hoverItemName, Main.rare, 0, Main.mouseX + 6, Main.mouseY + 6);
				} else {
					Main.instance.MouseText(Main.hoverItemName, Main.rare, 0);
				}
				Main.mouseText = true;
				Main.hoverItemName = null;
			}
		}
	}
	public class CustomizationMenuList : UIElement {
		public override void OnInitialize() {
			Width.Set(0, 1);
		}
	}
	public class PresetsMenu : UIElement {
		float totalHeight = 8;
		readonly int index;
		public PresetsMenu(int index) : base() {
			this.index = index;
		}
		public override void OnInitialize() {
			if (!(Elements is null)) Elements.Clear();
			Top.Pixels = 0;
			Main.UIScaleMatrix.Decompose(out Vector3 scale, out Quaternion _, out Vector3 _);
			BeastPlayerBase beastPlayer = Main.LocalPlayer.ModPlayers[BeastCustomization.BeastPlayers[index]] as BeastPlayerBase;
			if (beastPlayer is null) {
				this.Deactivate();
				Remove();
				return;
			}
			int i = 0;
			const float marginedButtonHeight = 52 + 8;
			int snapPoints = 0;
			int snapPointsPerPreset = 0;
			void AddButton(TagCompound item, int index, bool inConfig) {
				string name = item.TryGet("presetName", out string presetName) ? presetName : ("Preset #" + index);
				snapPointsPerPreset = 0;
				UIButton applyButton = new UIButton() {
					Text = name,
					Left = new(8, 0),
					Top = new(0, 0),
					Scale = 1.15f
				};
				applyButton.OnClick += (el) => {
					TagCompound tag = item;
					beastPlayer.ImportData(tag);
					beastPlayer.FinishCustomization(true);
					beastPlayer.StartCustomization();
					//customizationMenu.Reinitialize();
					CustomizationMenuState.scrollbarStartPos = (this.Parent as CustomizationMenuState).customizationMenu.scrollbar.ViewPosition;
					IngameFancyUI.OpenUIState(new CustomizationMenuState(this.index));
					//beastPlayer.StartCustomization();
				};
				applyButton.SetSnapPoint($"{name}_{nameof(applyButton)}", snapPoints++);
				snapPointsPerPreset++;

				UIButton renameButton = new UIButton() {
					Text = Language.GetTextValue("UI.Rename"),
					Left = new(8, 0),
					Top = new(0, 0),
					Scale = 1.15f
				};
				renameButton.OnClick += (el) => {
					Main.LocalPlayer.GetModPlayer<SharedModPlayer>().renamingPreset = new(applyButton, item, name);
				};
				renameButton.SetSnapPoint($"{name}_{nameof(renameButton)}", snapPoints++);
				snapPointsPerPreset++;

				UIButton overwriteButton = new UIButton(15, 600) {
					Text = Language.GetTextValue("Overwrite"),
					Left = new(8, 0),
					Top = new(0, 0),
					Scale = 1.15f
				};
				overwriteButton.OnClick += (el) => {
					if (el.Confirm(Language.GetTextValue("CLI.DeleteConfirmation", name)
						.Replace(Language.GetTextValue("UI.Delete").ToLower(), "overwrite"), "Overwritten", 300)) {
						beastPlayer.ExportData(item);
						//if (inConfig) BeastCustomizationSavedPresets.Instance.Save();
					}
				};
				overwriteButton.SetSnapPoint($"{name}_{nameof(overwriteButton)}", snapPoints++);
				snapPointsPerPreset++;

				UIButton deleteButton = new UIButton(15, 600) {
					Text = Language.GetTextValue("UI.Delete"),
					Left = new(8, 0),
					Top = new(0, 0),
					Scale = 1.15f
				};
				deleteButton.OnClick += (el) => {
					if (el.Confirm(Language.GetTextValue("CLI.DeleteConfirmation", name))) {
						float ownTop = el.Parent.Top.Pixels;
						List<TagCompound> currentLocation;
						if (inConfig) {
							currentLocation = beastPlayer.ConfigPresets;
						} else {
							currentLocation = beastPlayer.Presets;
						}
						currentLocation.Remove(item);
						if (inConfig) BeastCustomizationSavedPresets.Instance.Save();
						totalHeight -= marginedButtonHeight;
						var grandparent = el.Parent.Parent;
						foreach (var sibling in grandparent.Children) {
							if (sibling.Top.Pixels > ownTop) {
								sibling.Top.Pixels -= marginedButtonHeight;
							}
						}

						if (el.Parent is UIPresetWrapper wrapper) {
							wrapper.Remove();
						}
						grandparent.RecalculateChildren();
						snapPoints -= snapPointsPerPreset;
					}
				};
				deleteButton.SetSnapPoint($"{name}_{nameof(deleteButton)}", snapPoints++);
				snapPointsPerPreset++;

				UIButton moveButton = new UIButton(15, 600) {
					Text = $"Move to {(inConfig ? "Player" : "Config")}",
					Left = new(8, 0),
					Top = new(0, 0),
					Scale = 1.15f
				};
				moveButton.OnClick += (el) => {
					float ownTop = el.Parent.Top.Pixels;
					beastPlayer.ConfigPresets ??= new();
					List<TagCompound> currentLocation;
					List<TagCompound> newLocation;
					if (inConfig) {
						currentLocation = beastPlayer.ConfigPresets;
						newLocation = beastPlayer.Presets;
					} else {
						currentLocation = beastPlayer.Presets;
						newLocation = beastPlayer.ConfigPresets;
					}
					try {
						currentLocation.Remove(item);
					} finally {
						newLocation.Add(item);
						BeastCustomizationSavedPresets.Instance.Save();
					}
					totalHeight -= marginedButtonHeight;
					var grandparent = el.Parent.Parent;
					foreach (var sibling in grandparent.Children) {
						if (sibling.Top.Pixels > ownTop) {
							sibling.Top.Pixels -= marginedButtonHeight;
						}
					}

					if (el.Parent is UIPresetWrapper wrapper) {
						wrapper.Remove();
					}

					AddButton(item, index, !inConfig);
					grandparent.RecalculateChildren();
				};
				moveButton.SetSnapPoint($"{name}_{nameof(moveButton)}", snapPoints++);
				snapPointsPerPreset++;
				if (index == -1) {
					renameButton = null;
					deleteButton = null;
					moveButton = null;
					snapPoints -= 3;
				}
				//renameButton.Append(deleteButton);
				//applyButton.Append(renameButton);
				//Append(applyButton);
				var presetWrapper = new UIPresetWrapper(applyButton, renameButton, overwriteButton, deleteButton, moveButton) {
					Top = new(totalHeight, 0)
				};
				this.Append(presetWrapper);
				presetWrapper.Initialize();
				Recalculate();
				totalHeight += marginedButtonHeight;
			}
			foreach (var item in beastPlayer.Presets) {
				AddButton(item, ++i, false);
			}
			i = 0;
			beastPlayer.ConfigPresets ??= new();
			foreach (var item in beastPlayer.ConfigPresets) {
				AddButton(item, ++i, true);
			}

			UIButton saveButton = new UIButton() {
				Text = "New Preset",
				Left = new(8, 0),
				Top = new(totalHeight, 0),
				Scale = 1.15f
			};
			saveButton.OnClick += (el) => {
				TagCompound tag = new();
				beastPlayer.ExportData(tag);
				tag.Remove("SavedPresets");
				beastPlayer.Presets.Add(tag);
				AddButton(tag, ++i, false);
				//el.Top.Pixels += marginedButtonHeight;
				el.Recalculate();
			};
			Append(saveButton);
			totalHeight += marginedButtonHeight;

			TagCompound defaultTag = new();
			beastPlayer.CreateNew().ExportData(defaultTag);
			defaultTag["presetName"] = "Default";
			AddButton(defaultTag, -1, false);

			Width.Set(0, 1);
			Height.Set(0, 1);
		}
	}
	public class CustomizationMenuSelectorState : UIState {
		public override void OnInitialize() {
			CustomizationMenuSelector presetsMenu = new CustomizationMenuSelector();
			Append(presetsMenu);
		}
	}
	public class CustomizationMenuSelector : UIElement {
		float totalHeight = 8;
		public override void OnInitialize() {
			if (!(Elements is null)) Elements.Clear();
			Top.Pixels = 0;
			Main.UIScaleMatrix.Decompose(out Vector3 scale, out Quaternion _, out Vector3 _);
			const float marginedButtonHeight = 52 + 8;
			void AddButton(string name, int index) {
				UIButton button = new UIButton() {
					Text = name,
					Left = new(8, 0),
					Top = new(totalHeight, 0),
					Scale = 1.15f
				};
				button.OnClick += (el) => {
					IngameFancyUI.OpenUIState(new CustomizationMenuState(index));
				};

				this.Append(button);
				Recalculate();
				totalHeight += marginedButtonHeight;
			}
			for (int i = 0; i < BeastCustomization.BeastPlayers.Count; i++) {
				AddButton(Language.GetTextValue((Main.LocalPlayer.ModPlayers[BeastCustomization.BeastPlayers[i]] as BeastPlayerBase).DisplayName), i);
			}

			Width.Set(0, 1);
			Height.Set(0, 1);
		}
	}
	public sealed class SharedModPlayer : ModPlayer {
		public BeastPlayerBase current;
		public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo) {
			current = null;
			if (drawInfo.drawPlayer.ModPlayers.Length <= 0) return;
			int specificity = 0;
			foreach (int i in BeastCustomization.BeastPlayers) {
				if (drawInfo.drawPlayer.ModPlayers[i] is BeastPlayerBase beastPlayer && beastPlayer.IsActive && beastPlayer.Specificity > specificity) {
					current = beastPlayer;
					specificity = beastPlayer.Specificity;
				}
			}
			if (current is not null) {
				current.ApplyVanillaDrawLayers(drawInfo, out bool applyHead, out bool applyBody, out bool applyCloaks, out bool applyLegs);
				if (applyHead) {
					int slot = current.GetSlot(0);
					if (slot >= 0) {
						drawInfo.drawPlayer.head = slot;
						if (slot > 0 && slot < ArmorIDs.Head.Count) {
							Main.instance.LoadArmorHead(slot);
							int backID = ArmorIDs.Head.Sets.FrontToBackID[slot];
							if (backID >= 0) {
								Main.instance.LoadArmorHead(backID);
							}
						}
						drawInfo.drawsBackHairWithoutHeadgear = ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[drawInfo.drawPlayer.head];
						drawInfo.fullHair = ArmorIDs.Head.Sets.DrawFullHair[drawInfo.drawPlayer.head];
						drawInfo.hatHair = ArmorIDs.Head.Sets.DrawHatHair[drawInfo.drawPlayer.head];
					}
				}
				if (applyBody) {
					int slot = current.GetSlot(1);
					if (slot >= 0) {
						drawInfo.drawPlayer.body = slot;
						if (slot > 0 && slot < ArmorIDs.Body.Count) {
							Main.instance.LoadArmorBody(slot);
						}
						drawInfo.armorHidesHands = ArmorIDs.Body.Sets.HidesHands[slot];
						drawInfo.armorHidesArms = ArmorIDs.Body.Sets.HidesArms[slot];
					}
				}
				if (!applyCloaks) {
					drawInfo.drawPlayer.back = 0;
					drawInfo.drawPlayer.front = 0;
				}
				if (applyLegs) {
					int slot = current.GetSlot(2);
					if (slot >= 0) {
						drawInfo.drawPlayer.legs = slot;
						if (slot > 0 && slot < ArmorIDs.Legs.Count) {
							Main.instance.LoadArmorLegs(slot);
						}
					}
				}
			}
		}
		public override void HideDrawLayers(PlayerDrawSet drawInfo) {
			HandleKeybind();
			if (current is not null) {
				current.HideVanillaDrawLayers(drawInfo, out bool hideHead, out bool hideBody, out bool hideLegs);
				if (hideHead) {
					PlayerDrawLayers.Head.Hide();
				}
				if (hideBody) {
					PlayerDrawLayers.Skin.Hide();
					PlayerDrawLayers.Torso.Hide();
					PlayerDrawLayers.ArmOverItem.Hide();
				}
				if (hideLegs) {
					PlayerDrawLayers.Leggings.Hide();
				}
			}
		}
		void HandleKeybind() {
			if (BeastCustomization.OpenMenuHotkey.JustPressed) {
				if (BeastCustomizationConfig.Instance.openToActive && current is not null) {
					IngameFancyUI.OpenUIState(new CustomizationMenuState(current.ModIndex));
					return;
				}
				IngameFancyUI.OpenUIState(new CustomizationMenuSelectorState());
			}
		}
		internal Tuple<UIButton, TagCompound, string> renamingPreset;
		public override void PreUpdate() {
			if (renamingPreset is not null) {
				if (!IngameFancyUI.CanShowVirtualKeyboard(2) || UIVirtualKeyboard.KeyboardContext != 2) {
					string text = renamingPreset.Item1.Text;
					string oldText = text;
					if (text == " ") text = "";
					PlayerInput.WritingText = true;
					Main.instance.HandleIME();
					text = Main.GetInputText(text);
					if (text == "") text = " ";
					if (text != oldText) {
						renamingPreset.Item1.Text = text;
					}
					if (Main.inputTextEnter) {
						renamingPreset.Item2["presetName"] = text;
						renamingPreset = null;
					} else if (Main.inputTextEscape) {
						renamingPreset.Item1.Text = renamingPreset.Item3;
						renamingPreset = null;
					}
				}
				Main.editSign = true;
			}
		}
	}
	public class UIButton : UIElement {
		public new event Action<UIButton> OnClick;
		private string _text;
		public string Text {
			get => _text;
			set {
				_text = value;
				Vector2 size = FontAssets.MouseText.Value.MeasureString(Text) * Scale;
				Width.Pixels = size.X + 8;
				Height.Pixels = size.Y + 8;
				Recalculate();
			}
		}
		private float _scale = 1;
		public float Scale {
			get => _scale;
			set {
				float scaleDiff = value / _scale;
				_scale = value;
				Width.Pixels = (Width.Pixels - 8) * scaleDiff + 8;
				Height.Pixels = (Height.Pixels - 8) * scaleDiff + 8;
				Recalculate();
			}
		}
		int confirmationTime = 0;
		int confirmationDelayTime = 0;
		int confirmationResetTime = 0;
		int confirmationAltResetTime = 0;
		string confirmationOldText;
		public Color NormalColor { get; set; } = new Color(255, 220, 220);
		public Color HoverColor { get; set; } = new Color(255, 180, 180);
		public UIButton() :
			this(0, 0){ }
		public UIButton(int confirmationDelayTime, int confirmationResetTime) :
			this(new Color(255, 220, 220), new Color(255, 180, 180), confirmationDelayTime, confirmationResetTime) { }
		public UIButton(Color normalColor, Color hoverColor) :
			this(normalColor, hoverColor, 0, 0) { }
		public UIButton(Color normalColor, Color hoverColor, int confirmationDelayTime, int confirmationResetTime) {
			NormalColor = normalColor;
			HoverColor = hoverColor;
			this.confirmationDelayTime = confirmationDelayTime;
			this.confirmationResetTime = confirmationResetTime;
		}
		public bool Confirm(string confirmationText, string confirmedText = null, int confirmedResetTime = -1) {
			if (confirmationResetTime == 0) return true;
			if (confirmationTime == 0) {
				confirmationOldText = Text;
				confirmationTime = 1;
				Text = confirmationText;
			} else if (confirmationTime > confirmationDelayTime) {
				Text = confirmedText ?? confirmationOldText;
				confirmationAltResetTime = confirmedResetTime;
				return true;
			}
			return false;
		}
		public override void Update(GameTime gameTime) {
			if (confirmationTime > 0) {
				if (++confirmationTime >= confirmationResetTime
					|| (confirmationAltResetTime != -1 && confirmationTime >= confirmationAltResetTime)) {
					confirmationTime = 0;
					confirmationAltResetTime = -1;
					Text = confirmationOldText;
				}
			}
		}
		protected override void DrawSelf(SpriteBatch spriteBatch) {
			Rectangle dimensions = this.GetDimensions().ToRectangle();
			int endWidth = dimensions.Height / 2;
			Color color = NormalColor;
			if (ContainsPoint(Main.MouseScreen) && !PlayerInput.IgnoreMouseInterface && (confirmationTime <= 0 || confirmationTime > confirmationDelayTime)) {
				Main.LocalPlayer.mouseInterface = true;
				if (Main.mouseLeft && Main.mouseLeftRelease) {
					OnClick(this);
				}
				color = HoverColor;
				BeastCustomization._rightLock.SetValue(null, null);
				BeastCustomization._rightHover.SetValue(null, null);
			}

			if (confirmationResetTime > 0) {

			}
			Rectangle topRect = new Rectangle(dimensions.X, dimensions.Y, endWidth, dimensions.Height);
			Rectangle midRect = new Rectangle(dimensions.X + endWidth, dimensions.Y, dimensions.Width - (endWidth * 2), dimensions.Height);
			Rectangle bottomRect = new Rectangle(dimensions.X + dimensions.Width - endWidth, dimensions.Y, endWidth, dimensions.Height);
			spriteBatch.Draw(BeastCustomization.ButtonEndTexture, topRect, new Rectangle(0, 0, 26, 52), color, 0, default, SpriteEffects.None, 0);
			spriteBatch.Draw(BeastCustomization.ButtonMidTexture, midRect, new Rectangle(0, 0, 1, 52), color, 0, default, SpriteEffects.None, 0);
			spriteBatch.Draw(BeastCustomization.ButtonEndTexture, bottomRect, new Rectangle(0, 0, 26, 52), color, 0, default, SpriteEffects.FlipHorizontally, 0);
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, Text, dimensions.Center.ToVector2(), Color.White, 0f, ((dimensions.Size() - new Vector2(8, 16)) / Scale) * 0.5f, new Vector2(Scale));
		}
	}
	public class UIPresetWrapper : UIElement {
		UIButton applyButton;
		UIButton renameButton;
		UIButton overwriteButton;
		UIButton deleteButton;
		UIButton moveButton;
		public UIPresetWrapper(UIButton applyButton, UIButton renameButton, UIButton overwriteButton, UIButton deleteButton, UIButton moveButton) {
			this.applyButton = applyButton;
			this.renameButton = renameButton;
			this.overwriteButton = overwriteButton;
			this.deleteButton = deleteButton;
			this.moveButton = moveButton;
		}
		public override void OnInitialize() {
			Width.Percent = 1;
			Height.Pixels = 100;
			Elements?.Clear();
			Append(applyButton);
			if (renameButton is not null) {
				Append(renameButton);
				Append(overwriteButton);
				Append(deleteButton);
				Append(moveButton);
			}
		}
		public override void Update(GameTime gameTime) {
			if (Width.Percent == 0) {
				Height.Pixels = applyButton.Height.Pixels;
				Recalculate();
			}
			if (renameButton is not null) {
				bool changed = false;//OwO
				float oldLeft = renameButton.Left.Pixels;
				float newLeft = applyButton.Left.Pixels + applyButton.Width.Pixels + 8;
				if (newLeft != oldLeft) {
					renameButton.Left.Pixels = newLeft;
					overwriteButton.Left.Pixels = newLeft + renameButton.Width.Pixels + 8;
					deleteButton.Left.Pixels = overwriteButton.Left.Pixels + overwriteButton.Width.Pixels + 8;
					changed = true;
				}

				oldLeft = deleteButton.Left.Pixels;
				newLeft = overwriteButton.Left.Pixels + overwriteButton.Width.Pixels + 8;
				if (newLeft != oldLeft) {
					deleteButton.Left.Pixels = newLeft;
					changed = true;
				}

				oldLeft = moveButton.Left.Pixels;
				newLeft = deleteButton.Left.Pixels + deleteButton.Width.Pixels + 8;
				if (newLeft != oldLeft) {
					moveButton.Left.Pixels = newLeft;
					changed = true;
				}

				if (changed) {
					Recalculate();
				}
			}
			base.Update(gameTime);
		}
	}
	[AttributeUsage(AttributeTargets.Field)]
	internal sealed class ListRangeAttribute : Attribute {
		// See the attribute guidelines at 
		//  http://go.microsoft.com/fwlink/?LinkId=85236
		readonly string listName;
		public ListRangeAttribute(string listName) {
			this.listName = listName;
		}
		public string ListName {
			get { return listName; }
		}
	}
	public class FastFieldInfo<TParent, T> {
		public readonly FieldInfo field;
		Func<TParent, T> getter;
		Action<TParent, T> setter;
		public FastFieldInfo(string name, BindingFlags bindingFlags, bool init = false) {
			field = typeof(TParent).GetField(name, bindingFlags);
			if (init) {
				getter = CreateGetter();
				setter = CreateSetter();
			}
		}
		public FastFieldInfo(FieldInfo field, bool init = false) {
			this.field = field;
			if (init) {
				getter = CreateGetter();
				setter = CreateSetter();
			}
		}
		public T GetValue(TParent parent) {
			return (getter ??= CreateGetter())(parent);
		}
		public void SetValue(TParent parent, T value) {
			(setter ??= CreateSetter())(parent, value);
		}
		private Func<TParent, T> CreateGetter() {
			if (field.FieldType != typeof(T)) throw new InvalidOperationException($"type of {field.Name} does not match provided type {typeof(T)}");
			string methodName = field.ReflectedType.FullName + ".get_" + field.Name;
			DynamicMethod getterMethod = new DynamicMethod(methodName, typeof(T), new Type[] { typeof(TParent) }, true);
			ILGenerator gen = getterMethod.GetILGenerator();

			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldfld, field);
			gen.Emit(OpCodes.Ret);

			return (Func<TParent, T>)getterMethod.CreateDelegate(typeof(Func<TParent, T>));
		}
		private Action<TParent, T> CreateSetter() {
			if (field.FieldType != typeof(T)) throw new InvalidOperationException($"type of {field.Name} does not match provided type {typeof(T)}");
			string methodName = field.ReflectedType.FullName + ".set_" + field.Name;
			DynamicMethod setterMethod = new DynamicMethod(methodName, null, new Type[] { typeof(TParent), typeof(T) }, true);
			ILGenerator gen = setterMethod.GetILGenerator();

			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldarg_1);
			gen.Emit(OpCodes.Stfld, field);
			gen.Emit(OpCodes.Ret);

			return (Action<TParent, T>)setterMethod.CreateDelegate(typeof(Action<TParent, T>));
		}
	}
	public class BeastCustomizationConfig : ModConfig {
		public override ConfigScope Mode => ConfigScope.ClientSide;
		public static BeastCustomizationConfig Instance;
		public bool openToActive = true;
	}
	public class BeastCustomizationSavedPresets : ILoadable {
		public static BeastCustomizationSavedPresets Instance => ModContent.GetInstance<BeastCustomizationSavedPresets>();
		internal List<TagCompound> wolfPresets;
		internal List<TagCompound> fishPresets;
		internal List<TagCompound> fishWolfPresets;
		static string SavePath => Path.Combine(ConfigManager.ModConfigPath, "BeastCustomizationSavedPresets" + ".nbt");
		public void Load(Mod mod) {
			Read();
		}
		public void Unload() {
			Save();
		}
		internal void Read() {
			if (File.Exists(SavePath)) {
				TagCompound tag = TagIO.FromFile(SavePath);
				tag.TryGet("WolfPresets", out wolfPresets);
				tag.TryGet("FishPresets", out fishPresets);
				tag.TryGet("FishWolfPresets", out fishWolfPresets);
			}
			wolfPresets ??= new();
			fishPresets ??= new();
			fishWolfPresets ??= new();
		}
		internal void Save() {
			Directory.CreateDirectory(ConfigManager.ModConfigPath);
			TagIO.ToFile(
				new TagCompound() {
					["WolfPresets"] = wolfPresets,
					["FishPresets"] = fishPresets,
					["FishWolfPresets"] = fishWolfPresets
				},
				SavePath
			);
		}
	}
}