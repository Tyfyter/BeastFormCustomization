using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BeastCustomization.Textures {
	public class Merfolk : ILoadable {
		static AssetRepository Assets => ModContent.GetInstance<BeastCustomization>().Assets;
		public static List<AutoCastingAsset<Texture2D>> HeadScaleTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> HeadSecondaryScaleTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> EyesTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> BodyScaleTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> BodySecondaryScaleTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> LegsScaleTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> LegsSecondaryScaleTextures { get; private set; }
		public void Load(Mod mod) {
			if (Main.netMode != NetmodeID.Server) {
				HeadScaleTextures = new();
				HeadSecondaryScaleTextures = new();
				EyesTextures = new();
				BodyScaleTextures = new();
				BodySecondaryScaleTextures = new();
				LegsScaleTextures = new();
				LegsSecondaryScaleTextures = new();
				BeastCustomization.FillSpriteList(HeadScaleTextures, "Textures/Merfolk/Head_Scales");
				BeastCustomization.FillSpriteList(HeadSecondaryScaleTextures, "Textures/Merfolk/Head_Secondary_Scales");
				BeastCustomization.FillSpriteList(EyesTextures, "Textures/Merfolk/Head_Eyes");
				BeastCustomization.FillSpriteList(BodyScaleTextures, "Textures/Merfolk/Body_Scales");
				BeastCustomization.FillSpriteList(BodySecondaryScaleTextures, "Textures/Merfolk/Body_Secondary_Scales");
				BeastCustomization.FillSpriteList(LegsScaleTextures, "Textures/Merfolk/Legs_Scales");
				BeastCustomization.FillSpriteList(LegsSecondaryScaleTextures, "Textures/Merfolk/Legs_Secondary_Scales");
			}
		}
		public void Unload() {
			HeadScaleTextures = null;
			HeadSecondaryScaleTextures = null;
			BodyScaleTextures = null;
			BodySecondaryScaleTextures = null;
			LegsScaleTextures = null;
			LegsSecondaryScaleTextures = null;
		}
	}
}
