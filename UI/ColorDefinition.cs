using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.IO;

namespace BeastCustomization.UI {
	public class ColorDefinitionSerializer : TagSerializer<ColorDefinition, TagCompound> {
		public override TagCompound Serialize(ColorDefinition value) {
			TagCompound tag = new TagCompound() {
				["baseColor"] = value.BaseColor.PackedValue
			};
			if (value.HasDye) {
				tag["hairDye"] = value.HairDye;
			}
			return tag;
		}
		public override ColorDefinition Deserialize(TagCompound tag) {
			ColorDefinition value = new();
			value.BaseColor = new Color() { PackedValue = tag.Get<uint>("baseColor") };
			tag.TryGet("hairDye", out value.hairDye);
			return value;
		}
	}
	[CustomModConfigItem(typeof(ColorDefinitionElement))]
	public class ColorDefinition {
		internal Color baseColor;
		internal Item hairDye;
		public Color BaseColor {
			get => baseColor;
			set => baseColor = value;
		}
		public Item HairDye {
			get => hairDye ??= new();
			set => hairDye = value;
		}
		public ColorDefinition() { }
		public ColorDefinition(Color baseColor, Item hairDye = null) {
			this.baseColor = baseColor;
			this.hairDye = hairDye;
		}
		public bool HasDye => hairDye is not null && hairDye.hairDye > -1;
		public Color GetColor(Player player, Color? lightColor = null) {
			if (HasDye) {
				Color value = BaseColor;
				Color hairColor = player.hairColor;
				try {
					player.hairColor = BaseColor;
					value = GameShaders.Hair.GetColor(hairDye.hairDye, player, lightColor ?? Color.White);
				} finally {
					player.hairColor = hairColor;
				}
				return value;
			}
			return baseColor.MultiplyRGBA(lightColor ?? Color.White);
		}
		public override int GetHashCode() {
			unchecked {
				return (hairDye.type.GetHashCode() * 397) ^ baseColor.GetHashCode();
			}
		}
		public override bool Equals(object obj) {
			return obj is ColorDefinition other && baseColor.Equals(other.baseColor) && ((hairDye?.type ?? 0) == (other.hairDye?.type ?? 0));
		}
		public static implicit operator ColorDefinition(Color baseColor) => new(baseColor);
	}
}
