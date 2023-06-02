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
	public class Werewolf : ILoadable {
		static AssetRepository Assets => ModContent.GetInstance<BeastCustomization>().Assets;
		public static List<AutoCastingAsset<Texture2D>> HeadFurTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> HeadSecondaryFurTextures { get; private set; }
		public static AutoCastingAsset<Texture2D> EyesIrisTexture { get; private set; }
		public static AutoCastingAsset<Texture2D> EyesScleraTexture { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> HeadTeethTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> BodyFurTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> BodySecondaryFurTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> BodyClawsTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> LegsFurTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> LegsSecondaryFurTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> LegsClawsTextures { get; private set; }
		public void Load(Mod mod) {
			if (Main.netMode != NetmodeID.Server) {
				HeadFurTextures = new();
				HeadSecondaryFurTextures = new();
				HeadTeethTextures = new();

				BodyFurTextures = new();
				BodySecondaryFurTextures = new();
				BodyClawsTextures = new();

				LegsFurTextures = new();
				LegsSecondaryFurTextures = new();
				LegsClawsTextures = new();
				BeastCustomization.FillSpriteList(HeadFurTextures, "Textures/Werewolf/Head_Fur");
				BeastCustomization.FillSpriteList(HeadSecondaryFurTextures, "Textures/Werewolf/Head_Secondary_Fur", 0);
				EyesIrisTexture = Assets.Request<Texture2D>("Textures/Werewolf/Head_Eyes_Iris");
				EyesScleraTexture = Assets.Request<Texture2D>("Textures/Werewolf/Head_Eyes_White");
				BeastCustomization.FillSpriteList(HeadTeethTextures, "Textures/Werewolf/Head_Teeth");

				BeastCustomization.FillSpriteList(BodyFurTextures, "Textures/Werewolf/Body_Fur");
				BeastCustomization.FillSpriteList(BodySecondaryFurTextures, "Textures/Werewolf/Body_Secondary_Fur", 0);

				BeastCustomization.FillSpriteList(BodyClawsTextures, "Textures/Werewolf/Body_Claws");
				BeastCustomization.FillSpriteList(LegsFurTextures, "Textures/Werewolf/Legs_Fur");
				BeastCustomization.FillSpriteList(LegsSecondaryFurTextures, "Textures/Werewolf/Legs_Secondary_Fur", 0);
				BeastCustomization.FillSpriteList(LegsClawsTextures, "Textures/Werewolf/Legs_Claws");
			}
		}
		public void Unload() {
			HeadFurTextures = null;
			HeadSecondaryFurTextures = null;
			HeadTeethTextures = null;

			BodyFurTextures = null;
			BodySecondaryFurTextures = null;
			BodyClawsTextures = null;

			LegsFurTextures = null;
			LegsSecondaryFurTextures = null;
			LegsClawsTextures = null;
		}
	}
}
