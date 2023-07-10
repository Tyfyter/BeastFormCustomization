using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace BeastCustomization {
	public class Reflection : ILoadable {
		public delegate void _DrawSittingLegs(ref PlayerDrawSet drawinfo, Texture2D textureToDraw, Color matchingColor, int shaderIndex = 0, bool glowmask = false);
		public static _DrawSittingLegs DrawSittingLegs { get; private set; }
		public static Dictionary<Type, MethodInfo> BinaryWriterWrites { get; private set; }
		public static Dictionary<Type, MethodInfo> BinaryReaderReads { get; private set; }
		public void Load(Mod mod) {
			DrawSittingLegs = typeof(PlayerDrawLayers).GetMethod("DrawSittingLegs", BindingFlags.NonPublic | BindingFlags.Static).CreateDelegate<_DrawSittingLegs>();
			BinaryWriterWrites = new();
			BinaryReaderReads = new();
			foreach (var meth in typeof(Reflection).GetMethods()) {
				///<see cref="BinaryWriterWriteColor"/><see cref="BinaryWriterWriteItem"/>
				if (meth.Name.StartsWith("BinaryWriterWrite") && meth.Name == $"BinaryWriterWrite{meth.GetParameters()[1].ParameterType.Name}") {
					BinaryWriterWrites[meth.GetParameters()[1].ParameterType] = meth;
				}
				///<see cref="BinaryReaderReadColor"/><see cref="BinaryReaderReadItem"/>
				if (meth.Name.StartsWith("BinaryReaderRead") && meth.Name == $"BinaryReaderRead{meth.ReturnType.Name}") {
					BinaryReaderReads[meth.ReturnType] = meth;
				}
			}
		}
		public void Unload() {
			DrawSittingLegs = null;
			BinaryWriterWrites = null;
			BinaryReaderReads = null;
		}
		public static void BinaryWriterWriteColor(BinaryWriter writer, Color color) {
			writer.Write((uint)color.PackedValue);
		}
		public static Color BinaryReaderReadColor(BinaryReader reader) {
			Color color = new();
			color.PackedValue = reader.ReadUInt32();
			return color;
		}
		public static void BinaryWriterWriteItem(BinaryWriter writer, Item item) {
			writer.Write((int)(item?.netID ?? ItemID.None));
		}
		public static Item BinaryReaderReadItem(BinaryReader reader) => new Item(reader.ReadInt32());
	}
}
