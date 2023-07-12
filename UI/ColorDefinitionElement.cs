using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace BeastCustomization.UI {
	public class ColorDefinitionElement : ConfigElement {
		public class ColorObject {
			
			public readonly PropertyFieldWrapper memberInfo;

			public readonly object item;

			public readonly IList<Color> array;

			public readonly int index;

			public Color current;

			[LabelKey("$Config.Color.Red.Label")]
			public byte R {
				get {
					return current.R;
				}
				set {
					current.R = value;
					Update();
				}
			}

			[LabelKey("$Config.Color.Green.Label")]
			public byte G {
				get {
					return current.G;
				}
				set {
					current.G = value;
					Update();
				}
			}

			[LabelKey("$Config.Color.Blue.Label")]
			public byte B {
				get {
					return current.B;
				}
				set {
					current.B = value;
					Update();
				}
			}

			[LabelKey("$Config.Color.Hue.Label")]
			public float Hue {
				get {
					return Main.rgbToHsl(current).X;
				}
				set {
					byte a = A;
					current = Main.hslToRgb(value, Saturation, Lightness);
					current.A = a;
					Update();
				}
			}

			[LabelKey("$Config.Color.Saturation.Label")]
			public float Saturation {
				get {
					return Main.rgbToHsl(current).Y;
				}
				set {
					byte a = A;
					current = Main.hslToRgb(Hue, value, Lightness);
					current.A = a;
					Update();
				}
			}

			[LabelKey("$Config.Color.Lightness.Label")]
			public float Lightness {
				get {
					return Main.rgbToHsl(current).Z;
				}
				set {
					byte a = A;
					current = Main.hslToRgb(Hue, Saturation, value);
					current.A = a;
					Update();
				}
			}

			[LabelKey("$Config.Color.Alpha.Label")]
			public byte A {
				get {
					return current.A;
				}
				set {
					current.A = value;
					Update();
				}
			}

			public void Update() {
				if (array == null) {
					memberInfo.SetValue(item, current);
				} else {
					array[index] = current;
				}
			}

			public ColorObject(PropertyFieldWrapper memberInfo, object item) {
				this.item = item;
				this.memberInfo = memberInfo;
				current = (Color)memberInfo.GetValue(item);
			}

			public ColorObject(IList<Color> array, int index) {
				current = array[index];
				this.array = array;
				this.index = index;
			}
		}

		public int height;

		public ColorObject c;

		public IList<Color> ColorList { get; set; }

		public override void OnBind() {
			base.OnBind();
			ColorList = (IList<Color>)base.List;
			ColorDefinition colorDefinition = (ColorDefinition)MemberInfo.GetValue(Item);
			if (ColorList != null) {
				base.DrawLabel = false;
				height = 30;
				c = new ColorObject(ColorList, base.Index);
			} else {
				height = 30;
				c = new ColorObject(new(typeof(ColorDefinition).GetProperty("BaseColor")), colorDefinition);
			}
			ColorHSLSliderAttribute customAttributeFromMemberThenMemberType = ConfigManager.GetCustomAttributeFromMemberThenMemberType<ColorHSLSliderAttribute>(MemberInfo, Item, List);
			bool useHue = customAttributeFromMemberThenMemberType != null;
			bool showSaturationAndLightness = customAttributeFromMemberThenMemberType?.ShowSaturationAndLightness ?? false;
			bool num = ConfigManager.GetCustomAttributeFromMemberThenMemberType<ColorNoAlphaAttribute>(MemberInfo, Item, List) != null;
			List<string> skip = new List<string>();
			if (num) {
				skip.Add("A");
			}
			if (useHue) {
				skip.AddRange(new string[3] { "R", "G", "B" });
			} else {
				skip.AddRange(new string[3] { "Hue", "Saturation", "Lightness" });
			}
			if (useHue && !showSaturationAndLightness) {
				skip.AddRange(new string[2] { "Saturation", "Lightness" });
			}
			int order = 0;
			foreach (PropertyFieldWrapper variable in c.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Select(p => new PropertyFieldWrapper(p))) {
				if (!skip.Contains(variable.Name)) {
					Tuple<UIElement, UIElement> wrapped = ConfigManager.WrapIt(this, ref height, variable, c, order++);
					if (ColorList != null) {
						wrapped.Item1.Left.Pixels -= 20f;
						wrapped.Item1.Width.Pixels += 20f;
					}
				}
			}
			HairDyeConfigElement hairDyeElement = new() {
				TextDisplayOverride = () => ""
			};
			hairDyeElement.Top.Pixels = height;
			hairDyeElement.Bind(new(typeof(ColorDefinition).GetProperty("HairDye")), colorDefinition, null, order++);
			hairDyeElement.OnBind();
			hairDyeElement.Recalculate();
			Height.Pixels += hairDyeElement.GetOuterDimensions().Height + 4;
			Append(hairDyeElement);
		}

		public override void Draw(SpriteBatch spriteBatch) {
			base.Draw(spriteBatch);
			Rectangle hitbox = GetInnerDimensions().ToRectangle();
			hitbox = new Rectangle(hitbox.X + hitbox.Width / 2, hitbox.Y, hitbox.Width / 2, 30);
			Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, hitbox, c.current);
		}
	}

}
