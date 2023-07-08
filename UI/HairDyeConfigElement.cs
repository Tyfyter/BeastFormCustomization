using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace BeastCustomization.UI {
	public class HairDyeConfigElement : ConfigElement<Item> {
		protected bool pendingChanges = false;
		public override void OnBind() {
			base.OnBind();
			pendingChanges = true;
			SetupList();
		}
		protected void SetupList() {
			RemoveAllChildren();
			Main.UIScaleMatrix.Decompose(out Vector3 scale, out _, out _);
			float left = FontAssets.ItemStack.Value.MeasureString(TextDisplayFunction()).X * scale.X - 8;
			float top = 4;
			float width = 408f * scale.X;
			for (int i = 0; i < BeastCustomization.HairDyes.Count; i++) {
				int type = BeastCustomization.HairDyes[i];
				UIElement element = new HairDyeElement(type) {
					Left = new StyleDimension(left, 0),
					Top = new StyleDimension(top, 0)
				};
				element.OnLeftClick += (_, _) => {
					Value.SetDefaults(type == ItemID.HairDyeRemover ? 0 : type);
				};
				Append(element);
				left += 26 + 4;
				if (left + 26 + 4 > width) {
					left = 0;
					top += 30;
					Height.Pixels += 30;
				}
			}
			Height.Pixels += 6;
			Recalculate();
		}
	}
	public class HairDyeElement : UIElement {
		Item item;
		public HairDyeElement(int type) : this(new Item(type)) { }
		public HairDyeElement(Item item) : base() {
			this.item = item;
		}
		public override void OnInitialize() {
			Width.Set(26, 0);
			Height.Set(26, 0);
		}
		public override void Draw(SpriteBatch spriteBatch) {
			float inventoryScale = Main.inventoryScale;
			Main.inventoryScale *= 0.75f;
			Rectangle dimensions = this.GetDimensions().ToRectangle();
			Color backColor = new Color(200, 200, 200);
			if (!PlayerInput.IgnoreMouseInterface && dimensions.Contains(Main.mouseX, Main.mouseY)) {
				ItemSlot.MouseHover(ref item, ItemSlot.Context.CraftingMaterial);
				backColor = Color.White;
			}
			spriteBatch.Draw(
				TextureAssets.InventoryBack.Value,
				dimensions,
				backColor
			);
			ItemSlot.Draw(
				spriteBatch,
				ref item,
				ItemSlot.Context.ChatItem,
				this.GetInnerDimensions().Position() - new Vector2(4)
			);
			Main.inventoryScale = inventoryScale;
		}
	}
}
