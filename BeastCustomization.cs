using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using ReLogic.Content;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
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
		public static List<AutoCastingAsset<Texture2D>> HeadFurTextures { get; private set; }
		public static AutoCastingAsset<Texture2D> EyesIrisTexture { get; private set; }
		public static AutoCastingAsset<Texture2D> EyesScleraTexture { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> HeadTeethTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> BodyFurTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> BodySecondaryFurTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> BodyClawsTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> LegsFurTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> LegsClawsTextures { get; private set; }
		public static AutoCastingAsset<Texture2D> SelectorEndTexture { get; private set; }
		public static AutoCastingAsset<Texture2D> SelectorMidTexture { get; private set; }
		public static AutoCastingAsset<Texture2D> ButtonEndTexture { get; private set; }
		public static AutoCastingAsset<Texture2D> ButtonMidTexture { get; private set; }
		public static ModKeybind OpenMenuHotkey { get; private set; }
		internal static FastFieldInfo<RangeElement, RangeElement> _rightLock;
		internal static FastFieldInfo<RangeElement, RangeElement> _rightHover;
		public override void Load() {
			if (Main.netMode != NetmodeID.Server) {
				HeadFurTextures = new();
				HeadTeethTextures = new();
				BodyFurTextures = new();
				BodySecondaryFurTextures = new();
				BodyClawsTextures = new();
				LegsFurTextures = new();
				LegsClawsTextures = new();
				HeadFurTextures.Add(Assets.Request<Texture2D>("Textures/Head_Fur"));
				HeadFurTextures.Add(Assets.Request<Texture2D>("Textures/Head_Fur_2"));
				EyesIrisTexture = Assets.Request<Texture2D>("Textures/Head_Eyes_Iris");
				EyesScleraTexture = Assets.Request<Texture2D>("Textures/Head_Eyes_White");
				HeadTeethTextures.Add(Assets.Request<Texture2D>("Textures/Head_Teeth"));
				BodyFurTextures.Add(Assets.Request<Texture2D>("Textures/Body_Fur"));
				BodyFurTextures.Add(Assets.Request<Texture2D>("Textures/Body_Fur_2"));
				BodySecondaryFurTextures.Add(Assets.Request<Texture2D>("Textures/Body_Secondary_Fur_0"));
				BodySecondaryFurTextures.Add(Assets.Request<Texture2D>("Textures/Body_Secondary_Fur_1"));
				BodyClawsTextures.Add(Assets.Request<Texture2D>("Textures/Body_Claws"));
				LegsFurTextures.Add(Assets.Request<Texture2D>("Textures/Legs_Fur"));
				LegsFurTextures.Add(Assets.Request<Texture2D>("Textures/Legs_Fur_2"));
				LegsClawsTextures.Add(Assets.Request<Texture2D>("Textures/Legs_Claws"));
				SelectorEndTexture = Assets.Request<Texture2D>("Textures/UI/Selector_Back_End");
				SelectorMidTexture = Assets.Request<Texture2D>("Textures/UI/Selector_Back_Mid");
				ButtonEndTexture = Assets.Request<Texture2D>("Textures/UI/Button_Back_End");
				ButtonMidTexture = Assets.Request<Texture2D>("Textures/UI/Button_Back_Mid");
			}
			OpenMenuHotkey = KeybindLoader.RegisterKeybind(this, "Open Customization Menu", "NumPad7");
			_rightLock = new("rightLock", BindingFlags.NonPublic | BindingFlags.Static, true);
			_rightHover = new("rightHover", BindingFlags.NonPublic | BindingFlags.Static, true);
		}
		public override void Unload() {
			HeadFurTextures = null;
			HeadTeethTextures = null;
			BodyFurTextures = null;
			BodySecondaryFurTextures = null;
			BodyClawsTextures = null;
			LegsFurTextures = null;
			LegsClawsTextures = null;
			SelectorEndTexture = null;
			SelectorMidTexture = null;
			ButtonEndTexture = null;
			ButtonMidTexture = null;
			OpenMenuHotkey = null;
			_rightLock = null;
			_rightHover = null;
		}
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
		public override void OnInitialize() {
			Main.UIScaleMatrix.Decompose(out Vector3 scale, out Quaternion _, out Vector3 _);
			CustomizationMenu customizationMenu = new CustomizationMenu();
			Append(customizationMenu);
			PresetsMenu presetsMenu = new PresetsMenu();
			presetsMenu.Left.Pixels += 416f * scale.X;
			Append(presetsMenu);
		}
	}
	public class CustomizationMenu : UIElement {
		public float totalHeight;
		CustomizationMenuList listWrapper;
		CustomizationMenuList listWrapper2;
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
			BeastColorPlayer beastPlayer = Main.LocalPlayer.GetModPlayer<BeastColorPlayer>();
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
			Width.Set(416f * scale.X, 0);
			listWrapper = new CustomizationMenuList();
			listWrapper2 = new CustomizationMenuList();
			Tuple<UIElement, UIElement> wrapper = null;
			for (int i = 0; i < settingList.Length; i++) {
				int sliderMax = -1;
				if (settingList[i].MemberInfo.GetCustomAttribute<ListRangeAttribute>() is ListRangeAttribute rangeAttribute) {
					sliderMax = ((IList)typeof(BeastCustomization).
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
				foreach (var childWrapper in this.Children.First(e => e is CustomizationMenuList).Children.First().Children) {
					ConfigElement child = childWrapper.Children.First() as ConfigElement;
					if (child?.GetType() == colorElement) {
						_current.SetValue(_c.GetValue(child), ((PropertyFieldWrapper)_MemberInfo.GetValue(child)).GetValue(beastPlayer));
					}
				}
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
			Height.Set(0, 1);
			//Height.Set(Math.Min(totalHeight + (Width.Pixels / 8), Main.screenHeight * 0.9f), 0);
			//Left.Set(Width.Pixels * 0.1f, 1f);
			//Top.Set(Height.Pixels * -0.5f, 0.5f);
		}

		public override void OnDeactivate() {
			Main.LocalPlayer.GetModPlayer<BeastColorPlayer>().FinishCustomization(false);
		}
		public override void ScrollWheel(UIScrollWheelEvent evt) {
			listWrapper.Top.Pixels = MathHelper.Clamp(listWrapper.Top.Pixels + evt.ScrollWheelValue, (Main.screenHeight - (52 + 16)) - listWrapper.Height.Pixels, 0);
			this.Recalculate();
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
			Rectangle topRect = new Rectangle(dimensions.X, dimensions.Y, dimensions.Width, endHeight);
			Rectangle midRect = new Rectangle(dimensions.X, dimensions.Y + endHeight, dimensions.Width, dimensions.Height - (endHeight * 2));
			Rectangle bottomRect = new Rectangle(dimensions.X, dimensions.Y + dimensions.Height - endHeight, dimensions.Width, endHeight);
			spriteBatch.Draw(BeastCustomization.SelectorEndTexture, topRect, new Rectangle(0, 0, 208, 26), color, 0, default, SpriteEffects.None, 0);
			spriteBatch.Draw(BeastCustomization.SelectorMidTexture, midRect, new Rectangle(0, 0, 208, 1), color, 0, default, SpriteEffects.None, 0);
			spriteBatch.Draw(BeastCustomization.SelectorEndTexture, bottomRect, new Rectangle(0, 0, 208, 26), color, 0, default, SpriteEffects.FlipVertically, 0);
			//spriteBatch.Draw(TextureAssets.InventoryBack2.Value, dimensions, null, color, 0, default, SpriteEffects.None, 0);

		}
	}
	public class CustomizationMenuList : UIElement {
		public override void OnInitialize() {
			Width.Set(0, 1);
		}
	}
	public class PresetsMenu : UIElement {
		float totalHeight = 8;
		public override void OnInitialize() {
			if (!(Elements is null)) Elements.Clear();
			Top.Pixels = 0;
			Main.UIScaleMatrix.Decompose(out Vector3 scale, out Quaternion _, out Vector3 _);
			BeastColorPlayer beastPlayer = Main.LocalPlayer.GetModPlayer<BeastColorPlayer>();
			if (beastPlayer is null) {
				this.Deactivate();
				Remove();
				return;
			}
			int i = 0;
			const float marginedButtonHeight = 52 + 8;
			void AddButton(TagCompound item, int index) {
				string name = item.TryGet("presetName", out string presetName) ? presetName : ("Preset #" + index);
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
				};

				UIButton renameButton = new UIButton() {
					Text = Language.GetTextValue("UI.Rename"),
					Left = new(8, 0),
					Top = new(0, 0),
					Scale = 1.15f
				};
				renameButton.OnClick += (el) => {
					beastPlayer.renamingPreset = new(applyButton, item, name);
				};

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
					}
				};

				UIButton deleteButton = new UIButton(15, 600) {
					Text = Language.GetTextValue("UI.Delete"),
					Left = new(8, 0),
					Top = new(0, 0),
					Scale = 1.15f
				};
				deleteButton.OnClick += (el) => {
					if (el.Confirm(Language.GetTextValue("CLI.DeleteConfirmation", name))) {
						float ownTop = el.Parent.Top.Pixels;
						beastPlayer.Presets.Remove(item);
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
					}
				};
				if (index == -1) {
					renameButton = null;
					deleteButton = null;
				}
				//renameButton.Append(deleteButton);
				//applyButton.Append(renameButton);
				//Append(applyButton);
				var presetWrapper = new UIPresetWrapper(applyButton, renameButton, overwriteButton, deleteButton) {
					Top = new(totalHeight, 0)
				};
				this.Append(presetWrapper);
				presetWrapper.Initialize();
				Recalculate();
				totalHeight += marginedButtonHeight;
			}
			foreach (var item in beastPlayer.Presets) {
				AddButton(item, ++i);
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
				AddButton(tag, ++i);
				el.Top.Pixels += marginedButtonHeight;
				el.Recalculate();
			};
			Append(saveButton);
			totalHeight += marginedButtonHeight;

			TagCompound defaultTag = new();
			new BeastColorPlayer().ExportData(defaultTag);
			defaultTag["presetName"] = "Default";
			AddButton(defaultTag, -1);

			Width.Set(0, 1);
			Height.Set(0, 1);
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
		public UIPresetWrapper(UIButton applyButton, UIButton renameButton, UIButton overwriteButton, UIButton deleteButton) {
			this.applyButton = applyButton;
			this.renameButton = renameButton;
			this.overwriteButton = overwriteButton;
			this.deleteButton = deleteButton;
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
}