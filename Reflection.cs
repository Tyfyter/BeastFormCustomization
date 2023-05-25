using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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
		public void Load(Mod mod) {
			DrawSittingLegs = typeof(PlayerDrawLayers).GetMethod("DrawSittingLegs", BindingFlags.NonPublic | BindingFlags.Static).CreateDelegate<_DrawSittingLegs>();
		}

		public void Unload() {
			DrawSittingLegs = null;
		}

	}
}
