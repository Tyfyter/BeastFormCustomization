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
	public class Merwolf : ILoadable {
		static AssetRepository Assets => ModContent.GetInstance<BeastCustomization>().Assets;
		public static List<AutoCastingAsset<Texture2D>> HeadPrimaryTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> HeadSecondaryTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> EyesIrisTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> EyesScleraTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> HeadTeethTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> BodyPrimaryTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> BodySecondaryTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> BodyClawsTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> LegsPrimaryTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> LegsSecondaryTextures { get; private set; }
		public static List<AutoCastingAsset<Texture2D>> LegsClawsTextures { get; private set; }
		public void Load(Mod mod) {
			if (Main.netMode != NetmodeID.Server) {
				HeadPrimaryTextures = new();
				HeadSecondaryTextures = new();
				EyesIrisTextures = new();
				EyesScleraTextures = new();
				HeadTeethTextures = new();

				BodyPrimaryTextures = new();
				BodySecondaryTextures = new();
				BodyClawsTextures = new();

				LegsPrimaryTextures = new();
				LegsSecondaryTextures = new();
				LegsClawsTextures = new();
				BeastCustomization.FillSpriteList(HeadPrimaryTextures, "Textures/Merwolf/Head_Primary");
				BeastCustomization.FillSpriteList(HeadSecondaryTextures, "Textures/Merwolf/Head_Secondary", 0);
				BeastCustomization.FillSpriteList(EyesIrisTextures, "Textures/Merwolf/Head_Eyes_Iris");
				BeastCustomization.FillSpriteList(EyesScleraTextures, "Textures/Merwolf/Head_Eyes_White");
				BeastCustomization.FillSpriteList(HeadTeethTextures, "Textures/Merwolf/Head_Teeth");

				BeastCustomization.FillSpriteList(BodyPrimaryTextures, "Textures/Merwolf/Body_Primary");
				BeastCustomization.FillSpriteList(BodySecondaryTextures, "Textures/Merwolf/Body_Secondary", 0);
				BeastCustomization.FillSpriteList(BodyClawsTextures, "Textures/Merwolf/Body_Claws");

				BeastCustomization.FillSpriteList(LegsPrimaryTextures, "Textures/Merwolf/Legs_Primary");
				BeastCustomization.FillSpriteList(LegsSecondaryTextures, "Textures/Merwolf/Legs_Secondary", 0);
				BeastCustomization.FillSpriteList(LegsClawsTextures, "Textures/Merwolf/Legs_Claws");


				BeastCustomization.FillSpriteList(HeadPrimaryTextures, "Textures/Werewolf/Head_Fur");
				BeastCustomization.FillSpriteList(HeadSecondaryTextures, "Textures/Werewolf/Head_Secondary_Fur");
				EyesIrisTextures.Add(Assets.Request<Texture2D>("Textures/Werewolf/Head_Eyes_Iris"));
				EyesScleraTextures.Add(Assets.Request<Texture2D>("Textures/Werewolf/Head_Eyes_White"));
				BeastCustomization.FillSpriteList(HeadTeethTextures, "Textures/Werewolf/Head_Teeth");

				BeastCustomization.FillSpriteList(BodyPrimaryTextures, "Textures/Werewolf/Body_Fur");
				BeastCustomization.FillSpriteList(BodySecondaryTextures, "Textures/Werewolf/Body_Secondary_Fur");
				BeastCustomization.FillSpriteList(BodyClawsTextures, "Textures/Werewolf/Body_Claws", 2);

				BeastCustomization.FillSpriteList(LegsPrimaryTextures, "Textures/Werewolf/Legs_Fur");
				BeastCustomization.FillSpriteList(LegsSecondaryTextures, "Textures/Werewolf/Legs_Secondary_Fur");
				BeastCustomization.FillSpriteList(LegsClawsTextures, "Textures/Werewolf/Legs_Claws", 2);
			}
		}
		public void Unload() {
			HeadPrimaryTextures = null;
			HeadSecondaryTextures = null;
			EyesIrisTextures = null;
			EyesScleraTextures = null;
			HeadTeethTextures = null;

			BodyPrimaryTextures = null;
			BodySecondaryTextures = null;
			BodyClawsTextures = null;

			LegsPrimaryTextures = null;
			LegsSecondaryTextures = null;
			LegsClawsTextures = null;
		}
	}
}
