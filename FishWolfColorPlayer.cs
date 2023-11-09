using BeastCustomization.Textures;
using BeastCustomization.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace BeastCustomization {
	public class FishWolfColorPlayer : BeastPlayerBase {
		#region fields
		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.headPrimaryStyle")]
		[ListRange("HeadPrimaryTextures"), Slider]
		public int headPrimaryStyle = 0;

		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.headSecondaryStyle")]
		[ListRange("HeadSecondaryTextures"), Slider]
		public int headSecondaryStyle = 0;

		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.eyesIrisStyle")]
		[ListRange("EyesIrisTextures"), Slider]
		public int eyesIrisStyle = 0;

		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.eyesScleraStyle")]
		[ListRange("EyesScleraTextures"), Slider]
		public int eyesScleraStyle = 0;

		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.headTeethStyle")]
		[ListRange("HeadTeethTextures"), Slider]
		public int headTeethStyle = 0;

		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.bodyPrimaryStyle")]
		[ListRange("BodyPrimaryTextures"), Slider]
		public int bodyPrimaryStyle = 0;

		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.bodySecondaryStyle")]
		[ListRange("BodySecondaryTextures"), Slider]
		public int bodySecondaryStyle = 0;

		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.bodyClawsStyle")]
		[ListRange("BodyClawsTextures"), Slider]
		public int bodyClawsStyle = 0;

		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.legsPrimaryStyle")]
		[ListRange("LegsPrimaryTextures"), Slider]
		public int legsPrimaryStyle = 0;

		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.legsSecondaryStyle")]
		[ListRange("LegsSecondaryTextures"), Slider]
		public int legsSecondaryStyle = 0;

		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.legsClawsStyle")]
		[ListRange("LegsClawsTextures"), Slider]
		public int legsClawsStyle = 0;

		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.eyesGlow")]
		public bool eyesGlow = false;

		//[JsonIgnore]
		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.applyHead")]
		public bool applyHead = false;

		//[JsonIgnore]
		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.applyBody")]
		public bool applyBody = false;

		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.applyCloaks")]
		public bool applyCloaks = true;

		//[JsonIgnore]
		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.applyLegs")]
		public bool applyLegs = false;

		//[JsonIgnore]
		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.applyHeadOver")]
		[TooltipKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.LooksRidiculous")]
		public bool applyHeadOver = false;

		//[JsonIgnore]
		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.applyBodyOver")]
		[TooltipKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.LooksRidiculous")]
		public bool applyBodyOver = false;

		//[JsonIgnore]
		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.applyLegsOver")]
		[TooltipKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.LooksRidiculous")]
		public bool applyLegsOver = false;

		[OldOverrideShaderField("eyesDye", true)]
		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.eyesIrisColor")]
		public ColorDefinition eyesIrisColor = new Color(242, 8, 46);

		[OldOverrideShaderField("eyesDye", true)]
		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.eyesScleraColor")]
		public ColorDefinition eyesScleraColor = new Color(241, 241, 241);

		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.headTeethColor")]
		public ColorDefinition headTeethColor = new Color(227, 232, 238);

		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.headSecondaryColor")]
		public ColorDefinition headSecondaryColor = new Color(190, 153, 117);

		[OldHairDyeField("primaryHairDye")]
		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.primaryColor")]
		public ColorDefinition primaryColor = new Color(220, 153, 107);

		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.bodySecondaryColor")]
		public ColorDefinition bodySecondaryColor = new Color(190, 153, 117);

		[LabelKey("$Mods.BeastCustomization.Forms.FishWolfColorPlayer.clawsColor")]
		public ColorDefinition clawsColor = new Color(222, 206, 192);

		TagCompound oldData;
		#endregion fields
		//public override string DisplayName => "Merwolf";
		public override BeastPlayerBase CreateNew() => new FishWolfColorPlayer();
		public override Type ResourceCacheType => typeof(Merwolf);
		public override ref List<TagCompound> ConfigPresets => ref BeastCustomizationSavedPresets.Instance.fishWolfPresets;
		public override bool IsActive => (
			((Player.wereWolf && !Player.hideWolf) || Player.forceWerewolf)
			&& ((Player.merman && !Player.hideMerman) || Player.forceMerman)
		);
		public override int Specificity => 2;
		public override void StartCustomization() {
			oldData = new();
			ExportData(oldData);
		}
		public override void FinishCustomization(bool overwrite) {
			if (!overwrite) {
				ImportData(oldData ??= new());
				SendData();
			}
		}
		public override void UpdateData(TagCompound tag, Version lastVersion, out bool warn) {
			warn = false;
			ColorToColorDefinition(tag);
		}
		public override void ApplyVanillaDrawLayers(PlayerDrawSet drawInfo, out bool applyHead, out bool applyBody, out bool applyCloaks, out bool applyLegs) {
			applyHead = this.applyHead;
			applyBody = this.applyBody;
			applyCloaks = this.applyCloaks;
			applyLegs = this.applyLegs;
		}
		public override void HideVanillaDrawLayers(PlayerDrawSet drawInfo, out bool hideHead, out bool hideBody, out bool hideLegs) {
			hideHead = !applyHead || GetSlot(0) == -1;
			hideBody = !applyBody || GetSlot(1) == -1;
			hideLegs = !applyLegs || GetSlot(2) == -1;
		}
	}
	public class Merwolf_Head_Layer : GenericHeadLayer {
		public override Type BoundBeastPlayer => typeof(FishWolfColorPlayer);
		public override IEnumerable<BeastLayerItem> GetData(PlayerDrawSet drawInfo) {
			FishWolfColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<FishWolfColorPlayer>();
			yield return beastColorPlayer.primaryColor.GetLayerItem(drawInfo, drawInfo.colorArmorHead,
				Merwolf.HeadPrimaryTextures[beastColorPlayer.headPrimaryStyle],
				applyDye: true
			);
			yield return beastColorPlayer.headSecondaryColor.GetLayerItem(drawInfo, drawInfo.colorArmorHead,
				Merwolf.HeadSecondaryTextures[beastColorPlayer.headSecondaryStyle],
				applyDye: true
			);
			yield return beastColorPlayer.headTeethColor.GetLayerItem(drawInfo, drawInfo.colorArmorHead,
				Merwolf.HeadTeethTextures[beastColorPlayer.headTeethStyle]
			);
			Color eyesLightColor = beastColorPlayer.eyesGlow ? (Color.White * (drawInfo.colorArmorHead.A / 255f)) : drawInfo.colorArmorHead;
			yield return beastColorPlayer.eyesScleraColor.GetLayerItem(drawInfo, eyesLightColor,
				Merwolf.EyesScleraTextures[beastColorPlayer.eyesScleraStyle]
			);
			yield return beastColorPlayer.eyesIrisColor.GetLayerItem(drawInfo, eyesLightColor,
				Merwolf.EyesIrisTextures[beastColorPlayer.eyesIrisStyle]
			);
			if (beastColorPlayer.applyHeadOver) {
				int slot = beastColorPlayer.GetSlot(0);
				if (slot > 0) {
					if (slot < ArmorIDs.Head.Count) {
						Main.instance.LoadArmorHead(slot);
						int backID = ArmorIDs.Head.Sets.FrontToBackID[slot];
						if (backID >= 0) {
							Main.instance.LoadArmorHead(backID);
						}
					}
					yield return new(
						TextureAssets.ArmorHead[slot].Value,
						drawInfo.colorArmorHead,
						applyDye: true
					);
				}
			}
		}
	}
	public class Merwolf_Body_Layer : GenericBodyLayer {
		public override Type BoundBeastPlayer => typeof(FishWolfColorPlayer);
		public override IEnumerable<BeastLayerItem> GetData(PlayerDrawSet drawInfo) {
			FishWolfColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<FishWolfColorPlayer>();
			yield return beastColorPlayer.primaryColor.GetLayerItem(drawInfo, drawInfo.colorArmorBody,
				Merwolf.BodyPrimaryTextures[beastColorPlayer.bodyPrimaryStyle]
			);
			yield return beastColorPlayer.bodySecondaryColor.GetLayerItem(drawInfo, drawInfo.colorArmorBody,
				Merwolf.BodySecondaryTextures[beastColorPlayer.bodySecondaryStyle]
			);
			yield return beastColorPlayer.clawsColor.GetLayerItem(drawInfo, drawInfo.colorArmorBody,
				Merwolf.BodyClawsTextures[beastColorPlayer.bodyClawsStyle]
			);
			if (beastColorPlayer.applyBodyOver) {
				int slot = beastColorPlayer.GetSlot(1);
				if (slot > 0) {
					yield return new(
						TextureAssets.ArmorBodyComposite[slot].Value,
						drawInfo.colorArmorBody
					);
				}
			}
		}
	}
	public class Merwolf_Arm_Layer_Back : GenericArmLayer_Back {
		public override Type BoundBeastPlayer => typeof(FishWolfColorPlayer);
		public override IEnumerable<BeastLayerItem> GetData(PlayerDrawSet drawInfo) {
			FishWolfColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<FishWolfColorPlayer>();
			yield return beastColorPlayer.primaryColor.GetLayerItem(drawInfo, drawInfo.colorArmorBody,
				Merwolf.BodyPrimaryTextures[beastColorPlayer.bodyPrimaryStyle]
			);
			yield return beastColorPlayer.bodySecondaryColor.GetLayerItem(drawInfo, drawInfo.colorArmorBody,
				Merwolf.BodySecondaryTextures[beastColorPlayer.bodySecondaryStyle]
			);
			yield return beastColorPlayer.clawsColor.GetLayerItem(drawInfo, drawInfo.colorArmorBody,
				Merwolf.BodyClawsTextures[beastColorPlayer.bodyClawsStyle]
			);
			if (beastColorPlayer.applyBodyOver) {
				int slot = beastColorPlayer.GetSlot(1);
				if (slot > 0) {
					if (slot < ArmorIDs.Body.Count) {
						Main.instance.LoadArmorBody(slot);
					}
					yield return new(
						TextureAssets.ArmorBodyComposite[slot].Value,
						drawInfo.colorArmorBody
					);
				}
			}
		}
	}
	public class Merwolf_Arm_Layer_Front : GenericArmLayer_Front {
		public override Type BoundBeastPlayer => typeof(FishWolfColorPlayer);
		public override IEnumerable<BeastLayerItem> GetData(PlayerDrawSet drawInfo) {
			FishWolfColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<FishWolfColorPlayer>();
			yield return beastColorPlayer.primaryColor.GetLayerItem(drawInfo, drawInfo.colorArmorBody,
				Merwolf.BodyPrimaryTextures[beastColorPlayer.bodyPrimaryStyle]
			);
			yield return beastColorPlayer.bodySecondaryColor.GetLayerItem(drawInfo, drawInfo.colorArmorBody,
				Merwolf.BodySecondaryTextures[beastColorPlayer.bodySecondaryStyle]
			);
			yield return beastColorPlayer.clawsColor.GetLayerItem(drawInfo, drawInfo.colorArmorBody,
				Merwolf.BodyClawsTextures[beastColorPlayer.bodyClawsStyle]
			);
			if (beastColorPlayer.applyBodyOver) {
				int slot = beastColorPlayer.GetSlot(1);
				if (slot > 0) {
					if (slot < ArmorIDs.Body.Count) {
						Main.instance.LoadArmorBody(slot);
					}
					yield return new(
						TextureAssets.ArmorBodyComposite[slot].Value,
						drawInfo.colorArmorBody
					);
				}
			}
		}
	}
	public class Merwolf_Legs_Layer : GenericLegsLayer {
		public override Type BoundBeastPlayer => typeof(FishWolfColorPlayer);
		public override IEnumerable<BeastLayerItem> GetData(PlayerDrawSet drawInfo) {
			FishWolfColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<FishWolfColorPlayer>();
			yield return beastColorPlayer.primaryColor.GetLayerItem(drawInfo, drawInfo.colorArmorLegs,
				Merwolf.LegsPrimaryTextures[beastColorPlayer.legsPrimaryStyle]
			);
			yield return beastColorPlayer.bodySecondaryColor.GetLayerItem(drawInfo, drawInfo.colorArmorLegs,
				Merwolf.LegsSecondaryTextures[beastColorPlayer.legsSecondaryStyle]
			);
			yield return beastColorPlayer.clawsColor.GetLayerItem(drawInfo, drawInfo.colorArmorLegs,
				Merwolf.LegsClawsTextures[beastColorPlayer.legsClawsStyle]
			);
			if (beastColorPlayer.applyLegsOver) {
				int slot = beastColorPlayer.GetSlot(2);
				if (slot > 0) {
					if (slot < ArmorIDs.Legs.Count) {
						Main.instance.LoadArmorLegs(slot);
					}
					yield return new(
						TextureAssets.ArmorLeg[slot].Value,
						drawInfo.colorArmorLegs
					);
				}
			}
		}
	}
}
