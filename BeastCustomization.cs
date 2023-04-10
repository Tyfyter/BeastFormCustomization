using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using ReLogic.Content;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
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
				EyesIrisTexture = Assets.Request<Texture2D>("Textures/Head_Eyes_Iris");
				EyesScleraTexture = Assets.Request<Texture2D>("Textures/Head_Eyes_White");
				HeadTeethTextures.Add(Assets.Request<Texture2D>("Textures/Head_Teeth"));
				BodyFurTextures.Add(Assets.Request<Texture2D>("Textures/Body_Fur"));
				BodySecondaryFurTextures.Add(Assets.Request<Texture2D>("Textures/Body_Secondary_Fur_0"));
				BodySecondaryFurTextures.Add(Assets.Request<Texture2D>("Textures/Body_Secondary_Fur_1"));
				BodyClawsTextures.Add(Assets.Request<Texture2D>("Textures/Body_Claws"));
				LegsFurTextures.Add(Assets.Request<Texture2D>("Textures/Legs_Fur"));
				LegsClawsTextures.Add(Assets.Request<Texture2D>("Textures/Legs_Claws"));
				SelectorEndTexture = Assets.Request<Texture2D>("Textures/UI/Selector_Back_End");
				SelectorMidTexture = Assets.Request<Texture2D>("Textures/UI/Selector_Back_Mid");
				ButtonEndTexture = Assets.Request<Texture2D>("Textures/UI/Button_Back_End");
				ButtonMidTexture = Assets.Request<Texture2D>("Textures/UI/Button_Back_Mid");
			}
			OpenMenuHotkey = KeybindLoader.RegisterKeybind(this, "Open Customization Menu", "NumPad7");
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
			Append(new CustomizationMenu());
		}
	}
	public class CustomizationMenu : UIElement {
		public float totalHeight;
		bool write = false;
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
				wrapper = ConfigManager.WrapIt(this, ref top, settingList[i], beastPlayer, i, index: i);
				element = wrapper.Item2;
				if (settingList[i].MemberInfo.GetCustomAttribute<ListRangeAttribute>() is ListRangeAttribute rangeAttribute) {
					if (element is PrimitiveRangeElement<int> rangeElement) {
						int max = ((IList)typeof(BeastCustomization).
							GetProperty(rangeAttribute.ListName, BindingFlags.Public | BindingFlags.Static)
							.GetValue(null)).Count;
						rangeElement.Min = 0;
						rangeElement.Max = max - 1;
					}
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
			listWrapper2.Left.Pixels = -16;
			Append(listWrapper2);

			UIButton cancelButton = new UIButton() {
				Text = Language.GetTextValue("UI.Cancel"),
				Left = new(0, 0.20f),
				Top = new(-16, 1),
				Scale = 1.15f
			};
			cancelButton.Left.Pixels -= cancelButton.Width.Pixels * 0.5f;
			cancelButton.Top.Pixels -= cancelButton.Height.Pixels;
			cancelButton.OnClick += (evt, el) => {
				write = false;
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
			resetButton.OnClick += (evt, el) => {
				Main.LocalPlayer.GetModPlayer<BeastColorPlayer>().FinishCustomization(false);
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
			saveButton.OnClick += (evt, el) => {
				write = true;
				IngameFancyUI.Close();
			};
			Append(saveButton);

			Append(listWrapper2);
			Height.Set(0, 1);
			//Height.Set(Math.Min(totalHeight + (Width.Pixels / 8), Main.screenHeight * 0.9f), 0);
			//Left.Set(Width.Pixels * 0.1f, 1f);
			//Top.Set(Height.Pixels * -0.5f, 0.5f);
		}

		public override void OnDeactivate() {
			Main.LocalPlayer.GetModPlayer<BeastColorPlayer>().FinishCustomization(write);
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
	public class UIButton : UIElement {
		private string _text;
		public string Text {
			get => _text;
			set {
				_text = value;
				Vector2 size = FontAssets.MouseText.Value.MeasureString(Text);
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
		public Color NormalColor { get; set; } = new Color(255, 220, 220);
		public Color HoverColor { get; set; } = new Color(255, 180, 180);
		protected override void DrawSelf(SpriteBatch spriteBatch) {
			Rectangle dimensions = this.GetDimensions().ToRectangle();
			int endWidth = dimensions.Height / 2;
			Color color = NormalColor;
			if (ContainsPoint(Main.MouseScreen) && !PlayerInput.IgnoreMouseInterface) {
				Main.LocalPlayer.mouseInterface = true;
				color = HoverColor;
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
}