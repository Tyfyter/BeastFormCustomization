using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
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
			if (value.useHairDyeShader) {
				tag["useHairDyeShader"] = value.useHairDyeShader;
			}
			return tag;
		}
		public override ColorDefinition Deserialize(TagCompound tag) {
			ColorDefinition value = new();
			value.BaseColor = new Color() { PackedValue = tag.Get<uint>("baseColor") };
			tag.TryGet("hairDye", out value.hairDye);
			tag.TryGet("useHairDyeShader", out value.useHairDyeShader);
			return value;
		}
	}
	[CustomModConfigItem(typeof(ColorDefinitionElement))]
	public class ColorDefinition {
		internal Color baseColor;
		internal Item hairDye;
		internal bool useHairDyeShader;
		public Color BaseColor {
			get => baseColor;
			set => baseColor = value;
		}
		public Item HairDye {
			get => hairDye ??= new();
			set => hairDye = value;
		}
		[LabelKey("$Mods.BeastCustomization.UI.UseHairDyeShader")]
		public bool UseHairDyeShader {
			get => useHairDyeShader;
			set => useHairDyeShader = value;
		}
		public ColorDefinition() { }
		public ColorDefinition(Color baseColor, Item hairDye = null, bool useHairDyeShader = false) {
			this.baseColor = baseColor;
			this.hairDye = hairDye;
			this.useHairDyeShader = useHairDyeShader;
		}
		public bool HasDye => (hairDye?.hairDye ?? -1) > -1;
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
		public int? GetPackedShaderOverride() {
			return UseHairDyeShader ? PlayerDrawHelper.PackShader(HairDye.hairDye, PlayerDrawHelper.ShaderConfiguration.HairShader) : null;
		}
		public BeastLayerItem GetLayerItem(PlayerDrawSet drawInfo, Color lightColor, Texture2D texture, bool applyDye = true) {
			return new(
				texture,
				GetColor(drawInfo.drawPlayer, lightColor),
				overrideShader: GetPackedShaderOverride(),
				applyDye: applyDye
			);
		}
		public override int GetHashCode() {
			unchecked {
				return (hairDye.type.GetHashCode() * 397) ^ baseColor.GetHashCode();
			}
		}
		public override bool Equals(object obj) {
			return obj is ColorDefinition other
				&& baseColor.Equals(other.baseColor)
				&& ((hairDye?.type ?? 0) == (other.hairDye?.type ?? 0))
				&& useHairDyeShader == other.useHairDyeShader;
		}
		public static implicit operator ColorDefinition(Color baseColor) => new(baseColor);
	}
}
