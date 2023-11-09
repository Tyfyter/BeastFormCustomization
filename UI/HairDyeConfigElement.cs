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
			base.TextDisplayFunction = TextDisplayOverride ?? base.TextDisplayFunction;
			pendingChanges = true;
			SetupList();
		}
		public Func<string> TextDisplayOverride { get; set; }
		protected void SetupList() {
			RemoveAllChildren();
			Main.UIScaleMatrix.Decompose(out Vector3 scale, out _, out _);
			float left = MathF.Max(FontAssets.ItemStack.Value.MeasureString(TextDisplayFunction()).X * scale.X - 8, 4);
			float top = 4;
			float width = 408f * scale.X;
			for (int i = 0; i < BeastCustomization.HairDyes.Count; i++) {
				if (left + 26 + 4 > width) {
					left = 0;
					top += 30;
					Height.Pixels += 30;
				}
				int type = BeastCustomization.HairDyes[i];
				HairDyeElement element = new HairDyeElement(type) {
					Left = new StyleDimension(left, 0),
					Top = new StyleDimension(top, 0)
				};
				element.OnLeftClick += (_, _) => {
					Value.SetDefaults(type == ItemID.HairDyeRemover ? 0 : type);
				};
				element.getSelectedItem = () => Value;
				Append(element);
				left += 26 + 4;
			}
			Height.Pixels += 6;
			Recalculate();
		}
	}
	public class HairDyeElement : UIElement {
		internal Func<Item> getSelectedItem;
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
			Main.inventoryScale = 0.525f;
			Rectangle dimensions = this.GetDimensions().ToRectangle();
			Color backColor = new Color(200, 200, 200);
			if (!PlayerInput.IgnoreMouseInterface && dimensions.Contains(Main.mouseX, Main.mouseY)) {
				ItemSlot.MouseHover(ref item, ItemSlot.Context.CraftingMaterial);
				backColor = Color.White;
				if (Main.mouseLeft && Main.mouseLeftRelease) {
					LeftClick(new UIMouseEvent(this, Main.MouseScreen));
				}
			}
			bool selectedBack = false;
			if (getSelectedItem is not null) {
				int selectedDye = getSelectedItem().hairDye;
				if (selectedDye == -1) selectedDye = 0;
				selectedBack = selectedDye == item.hairDye;
			}
			spriteBatch.Draw(
				selectedBack ? TextureAssets.InventoryBack14.Value : TextureAssets.InventoryBack.Value,
				dimensions,
				backColor
			);
			Main.inventoryScale *= 1.2f;
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
