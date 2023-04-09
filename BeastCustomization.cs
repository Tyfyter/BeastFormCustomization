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
			}
		}
		public override void Unload() {
			HeadFurTextures = null;
			HeadTeethTextures = null;
			BodyFurTextures = null;
			BodySecondaryFurTextures = null;
			BodyClawsTextures = null;
			LegsFurTextures = null;
			LegsClawsTextures = null;
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
		public override void OnActivate() {
			SoundEngine.PlaySound(SoundID.MenuOpen);
		}
		public override void OnInitialize() {
			if (!(Elements is null)) Elements.Clear();
			Left.Pixels = -16;
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
				Append(wrapper.Item1);
			}
			//Height.Set(Math.Min(totalHeight + (Width.Pixels / 8), Main.screenHeight * 0.9f), 0);
			//Left.Set(Width.Pixels * 0.1f, 1f);
			//Top.Set(Height.Pixels * -0.5f, 0.5f);
		}
		public override void OnDeactivate() {
			Main.LocalPlayer.GetModPlayer<BeastColorPlayer>().FinishCustomization(write);
		}
		public override void ScrollWheel(UIScrollWheelEvent evt) {
			this.Top.Pixels = MathHelper.Clamp(this.Top.Pixels + evt.ScrollWheelValue, Main.screenHeight - this.Height.Pixels, 0);
			this.Recalculate();
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
			//spriteBatch.Draw(BoardGames.SelectorEndTexture, topRect, new Rectangle(0, 0, 208, 26), color, 0, default, SpriteEffects.None, 0);
			//spriteBatch.Draw(BoardGames.SelectorMidTexture, midRect, new Rectangle(0, 0, 208, 1), color, 0, default, SpriteEffects.None, 0);
			//spriteBatch.Draw(BoardGames.SelectorEndTexture, bottomRect, new Rectangle(0, 0, 208, 26), color, 0, default, SpriteEffects.FlipVertically, 0);
			//spriteBatch.Draw(TextureAssets.InventoryBack2.Value, dimensions, null, color, 0, default, SpriteEffects.None, 0);

			Rectangle labelRect = new Rectangle(dimensions.X, dimensions.Y - (dimensions.Width / 10), dimensions.Width, endHeight);
			string labelText = Language.GetTextValue("Mods.BoardGames.GameSettings");
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, labelText, labelRect.Center.ToVector2(), Color.White, 0f, FontAssets.MouseText.Value.MeasureString(labelText) * 0.5f, Vector2.One);
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