using BeastCustomization.Textures;
using BeastCustomization.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
	[LegacyName("BeastColorPlayer")]
	public class WolfColorPlayer : BeastPlayerBase {
		#region fields
		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.headFurStyle")]
		[ListRange("HeadFurTextures"), Slider]
		public int headFurStyle = 1;

		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.headSecondaryFurStyle")]
		[ListRange("HeadSecondaryFurTextures"), Slider]
		public int headSecondaryFurStyle = 0;

		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.headTeethStyle")]
		[ListRange("HeadTeethTextures"), Slider]
		public int headTeethStyle = 0;

		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.bodyFurStyle")]
		[ListRange("BodyFurTextures"), Slider]
		public int bodyFurStyle = 1;

		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.bodySecondaryFurStyle")]
		[ListRange("BodySecondaryFurTextures"), Slider]
		public int bodySecondaryFurStyle = 0;

		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.bodyClawsStyle")]
		[ListRange("BodyClawsTextures"), Slider]
		public int bodyClawsStyle = 0;

		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.legsFurStyle")]
		[ListRange("LegsFurTextures"), Slider]
		public int legsFurStyle = 1;

		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.legsSecondaryFurStyle")]
		[ListRange("LegsSecondaryFurTextures"), Slider]
		public int legsSecondaryFurStyle = 0;

		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.legsClawsStyle")]
		[ListRange("LegsClawsTextures"), Slider]
		public int legsClawsStyle = 0;

		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.eyesGlow")]
		public bool eyesGlow = false;

		//[JsonIgnore]
		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.applyHead")]
		public bool applyHead = false;

		//[JsonIgnore]
		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.applyBody")]
		public bool applyBody = false;

		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.applyCloaks")]
		public bool applyCloaks = true;

		//[JsonIgnore]
		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.applyLegs")]
		public bool applyLegs = false;

		//[JsonIgnore]
		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.applyHeadOver")]
		[TooltipKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.LooksRidiculous")]
		public bool applyHeadOver = false;

		//[JsonIgnore]
		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.applyBodyOver")]
		[TooltipKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.LooksRidiculous")]
		public bool applyBodyOver = false;

		//[JsonIgnore]
		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.applyLegsOver")]
		[TooltipKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.LooksRidiculous")]
		public bool applyLegsOver = false;

		[OldOverrideShaderField("eyesDye", true)]
		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.eyesIrisColor")]
		public ColorDefinition eyesIrisColor = new(new Color(242, 8, 46), useHairDyeShader: true);

		[OldOverrideShaderField("eyesDye", true)]
		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.eyesScleraColor")]
		public ColorDefinition eyesScleraColor = new(new Color(241, 241, 241), useHairDyeShader: true);

		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.headTeethColor")]
		public ColorDefinition headTeethColor = new Color(227, 232, 238);

		[OldHairDyeField("primaryHairDye")]
		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.furColor")]
		public ColorDefinition furColor = new Color(220, 153, 107);//

		[OldHairDyeField("secondaryHairDye")]
		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.furColor2")]
		public ColorDefinition furColor2 = new Color(190, 153, 117);

		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.clawsColor")]
		public ColorDefinition clawsColor = new Color(222, 206, 192);

		TagCompound oldData;
		#endregion fields
		public override string DisplayName => base.DisplayName;
		public override BeastPlayerBase CreateNew() => new WolfColorPlayer();
		public override Type ResourceCacheType => typeof(Werewolf);
		public override ref List<TagCompound> ConfigPresets => ref BeastCustomizationSavedPresets.Instance.wolfPresets;
		public override bool IsActive => (Player.wereWolf && !Player.hideWolf) || Player.forceWerewolf;
		public override int Specificity => 1;
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
		internal const bool enabled = true;
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
		public override void PreUpdate() {
			UpdateHairDyes(furColor);
		}
	}
	public class Werewolf_Head_Layer : GenericHeadLayer {
		public override Type BoundBeastPlayer => typeof(WolfColorPlayer);
		public override IEnumerable<BeastLayerItem> GetData(PlayerDrawSet drawInfo) {
			WolfColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<WolfColorPlayer>();
			yield return beastColorPlayer.furColor.GetLayerItem(drawInfo, drawInfo.colorArmorHead,
				Werewolf.HeadFurTextures[beastColorPlayer.headFurStyle]
			);
			yield return beastColorPlayer.furColor2.GetLayerItem(drawInfo, drawInfo.colorArmorHead,
				Werewolf.HeadSecondaryFurTextures[beastColorPlayer.headSecondaryFurStyle]
			);
			yield return beastColorPlayer.headTeethColor.GetLayerItem(drawInfo, drawInfo.colorArmorHead,
				Werewolf.HeadTeethTextures[beastColorPlayer.headTeethStyle]
			);
			Color eyesLightColor = beastColorPlayer.eyesGlow ? (Color.White * (drawInfo.colorArmorHead.A / 255f)) : drawInfo.colorArmorHead;
			yield return beastColorPlayer.eyesScleraColor.GetLayerItem(drawInfo, eyesLightColor,
				Werewolf.EyesScleraTexture
			);
			yield return beastColorPlayer.eyesIrisColor.GetLayerItem(drawInfo, eyesLightColor,
				Werewolf.EyesIrisTexture
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
	public class Werewolf_Body_Layer : GenericBodyLayer {
		public override Type BoundBeastPlayer => typeof(WolfColorPlayer);
		public override IEnumerable<BeastLayerItem> GetData(PlayerDrawSet drawInfo) {
			WolfColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<WolfColorPlayer>();
			yield return beastColorPlayer.furColor.GetLayerItem(drawInfo, drawInfo.colorArmorBody,
				Werewolf.BodyFurTextures[beastColorPlayer.bodyFurStyle]
			);
			yield return beastColorPlayer.furColor2.GetLayerItem(drawInfo, drawInfo.colorArmorBody,
				Werewolf.BodySecondaryFurTextures[beastColorPlayer.bodySecondaryFurStyle]
			);
			yield return beastColorPlayer.clawsColor.GetLayerItem(drawInfo, drawInfo.colorArmorBody,
				Werewolf.BodyClawsTextures[beastColorPlayer.bodyClawsStyle]
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
	public class Werewolf_Arm_Layer_Back : GenericArmLayer_Back {
		public override Type BoundBeastPlayer => typeof(WolfColorPlayer);
		public override IEnumerable<BeastLayerItem> GetData(PlayerDrawSet drawInfo) {
			WolfColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<WolfColorPlayer>();
			yield return beastColorPlayer.furColor.GetLayerItem(drawInfo, drawInfo.colorArmorBody,
				Werewolf.BodyFurTextures[beastColorPlayer.bodyFurStyle]
			);
			yield return beastColorPlayer.furColor2.GetLayerItem(drawInfo, drawInfo.colorArmorBody,
				Werewolf.BodySecondaryFurTextures[beastColorPlayer.bodySecondaryFurStyle]
			);
			yield return beastColorPlayer.clawsColor.GetLayerItem(drawInfo, drawInfo.colorArmorBody,
				Werewolf.BodyClawsTextures[beastColorPlayer.bodyClawsStyle]
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
	public class Werewolf_Arm_Layer_Front : GenericArmLayer_Front {
		public override Type BoundBeastPlayer => typeof(WolfColorPlayer);
		public override IEnumerable<BeastLayerItem> GetData(PlayerDrawSet drawInfo) {
			WolfColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<WolfColorPlayer>();
			yield return beastColorPlayer.furColor.GetLayerItem(drawInfo, drawInfo.colorArmorBody,
				Werewolf.BodyFurTextures[beastColorPlayer.bodyFurStyle]
			);
			yield return beastColorPlayer.furColor2.GetLayerItem(drawInfo, drawInfo.colorArmorBody,
				Werewolf.BodySecondaryFurTextures[beastColorPlayer.bodySecondaryFurStyle]
			);
			yield return beastColorPlayer.clawsColor.GetLayerItem(drawInfo, drawInfo.colorArmorBody,
				Werewolf.BodyClawsTextures[beastColorPlayer.bodyClawsStyle]
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
	public class Werewolf_Legs_Layer : GenericLegsLayer {
		public override Type BoundBeastPlayer => typeof(WolfColorPlayer);
		public override IEnumerable<BeastLayerItem> GetData(PlayerDrawSet drawInfo) {
			WolfColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<WolfColorPlayer>();
			yield return beastColorPlayer.furColor.GetLayerItem(drawInfo, drawInfo.colorArmorLegs,
				Werewolf.LegsFurTextures[beastColorPlayer.legsFurStyle]
			);
			yield return beastColorPlayer.furColor2.GetLayerItem(drawInfo, drawInfo.colorArmorLegs,
				Werewolf.LegsSecondaryFurTextures[beastColorPlayer.legsSecondaryFurStyle]
			);
			yield return beastColorPlayer.clawsColor.GetLayerItem(drawInfo, drawInfo.colorArmorLegs,
				Werewolf.LegsClawsTextures[beastColorPlayer.legsClawsStyle]
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
	public class Werewolf_Shampoo_Layer : PlayerDrawLayer {
		public sealed override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			return drawInfo.drawPlayer.leinforsHair && drawInfo.shadow == 0f && Main.rgbToHsl(drawInfo.colorArmorBody).Z > 0.15f && drawInfo.drawPlayer.GetModPlayer<SharedModPlayer>()?.current is WolfColorPlayer;
		}
		public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.LeinforsHairShampoo, PlayerDrawLayers.Backpacks);
		protected override void Draw(ref PlayerDrawSet drawInfo) {
			if (Main.rand.NextBool(30)) {
				Player drawPlayer = drawInfo.drawPlayer;
				int dust = Dust.NewDust(
					drawPlayer.position,
					drawPlayer.width,
					drawPlayer.height,
					DustID.TreasureSparkle,
					0f,
					0f,
					150,
					default(Color),
					0.3f
				);
				Main.dust[dust].fadeIn = 1f;
				Main.dust[dust].velocity *= 0.1f;
				Main.dust[dust].noLight = true;
				Main.dust[dust].shader = GameShaders.Armor.GetSecondaryShader(drawInfo.drawPlayer.cLeinShampoo, drawInfo.drawPlayer);
				drawInfo.DustCache.Add(dust);
			}
		}
	}
}
