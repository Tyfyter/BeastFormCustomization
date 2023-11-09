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
	public class FishColorPlayer : BeastPlayerBase {
		#region fields
		[LabelKey("$Mods.BeastCustomization.Forms.FishColorPlayer.headScaleStyle")]
		[ListRange("HeadScaleTextures"), Slider]
		public int headScaleStyle = 1;

		[LabelKey("$Mods.BeastCustomization.Forms.FishColorPlayer.headScaleStyle2")]
		[ListRange("HeadSecondaryScaleTextures"), Slider]
		public int headScaleStyle2 = 0;

		[LabelKey("$Mods.BeastCustomization.Forms.FishColorPlayer.headEyeStyle")]
		[ListRange("EyesTextures"), Slider]
		public int headEyeStyle = 0;

		[LabelKey("$Mods.BeastCustomization.Forms.FishColorPlayer.bodyScaleStyle")]
		[ListRange("BodyScaleTextures"), Slider]
		public int bodyScaleStyle = 1;

		[LabelKey("$Mods.BeastCustomization.Forms.FishColorPlayer.bodySecondaryScaleStyle")]
		[ListRange("BodySecondaryScaleTextures"), Slider]
		public int bodySecondaryScaleStyle = 0;

		[LabelKey("$Mods.BeastCustomization.Forms.FishColorPlayer.legsScaleStyle")]
		[ListRange("LegsScaleTextures"), Slider]
		public int legsScaleStyle = 1;

		[LabelKey("$Mods.BeastCustomization.Forms.FishColorPlayer.legsScaleStyle2")]
		[ListRange("LegsSecondaryScaleTextures"), Slider]
		public int legsScaleStyle2 = 0;

		[LabelKey("$Mods.BeastCustomization.Forms.FishColorPlayer.eyesGlow")]
		public bool eyesGlow = false;

		//[JsonIgnore]
		[LabelKey("$Mods.BeastCustomization.Forms.FishColorPlayer.applyHead")]
		public bool applyHead = false;

		//[JsonIgnore]
		[LabelKey("$Mods.BeastCustomization.Forms.FishColorPlayer.applyBody")]
		public bool applyBody = false;

		[LabelKey("$Mods.BeastCustomization.Forms.FishColorPlayer.applyCloaks")]
		public bool applyCloaks = true;

		//[JsonIgnore]
		[LabelKey("$Mods.BeastCustomization.Forms.FishColorPlayer.applyLegs")]
		public bool applyLegs = false;

		//[JsonIgnore]
		[LabelKey("$Mods.BeastCustomization.Forms.FishColorPlayer.applyHeadOver")]
		[TooltipKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.LooksRidiculous")]
		public bool applyHeadOver = false;

		//[JsonIgnore]
		[LabelKey("$Mods.BeastCustomization.Forms.FishColorPlayer.applyBodyOver")]
		[TooltipKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.LooksRidiculous")]
		public bool applyBodyOver = false;

		//[JsonIgnore]
		[LabelKey("$Mods.BeastCustomization.Forms.FishColorPlayer.applyLegsOver")]
		[TooltipKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.LooksRidiculous")]
		public bool applyLegsOver = false;

		[OldOverrideShaderField("eyesDye", true)]
		[LabelKey("$Mods.BeastCustomization.Forms.FishColorPlayer.eyesColor")]
		public ColorDefinition eyesColor = new Color(242, 8, 46);

		[LabelKey("$Mods.BeastCustomization.Forms.FishColorPlayer.scaleColor")]
		public ColorDefinition scaleColor = new Color(41, 185, 127);

		[LabelKey("$Mods.BeastCustomization.Forms.FishColorPlayer.scaleColor2")]
		public ColorDefinition scaleColor2 = new Color(168, 255, 106);

		TagCompound oldData;
		#endregion fields
		//public override string DisplayName => "Merfolk";
		public override FishColorPlayer CreateNew() => new FishColorPlayer();
		public override Type ResourceCacheType => typeof(Merfolk);
		public override ref List<TagCompound> ConfigPresets => ref BeastCustomizationSavedPresets.Instance.fishPresets;
		public override bool IsActive => (Player.merman && !Player.hideMerman) || Player.forceMerman;
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
	public class Merfolk_Head_Layer : GenericHeadLayer {
		public override Type BoundBeastPlayer => typeof(FishColorPlayer);
		public override IEnumerable<BeastLayerItem> GetData(PlayerDrawSet drawInfo) {
			FishColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<FishColorPlayer>();
			yield return beastColorPlayer.scaleColor.GetLayerItem(drawInfo, drawInfo.colorArmorHead,
				Merfolk.HeadScaleTextures[beastColorPlayer.headScaleStyle],
				applyDye: true
			);
			yield return beastColorPlayer.scaleColor2.GetLayerItem(drawInfo, drawInfo.colorArmorHead,
				Merfolk.HeadSecondaryScaleTextures[beastColorPlayer.headScaleStyle2],
				applyDye: true
			);
			Color eyesLightColor = beastColorPlayer.eyesGlow ? (Color.White * (drawInfo.colorArmorHead.A / 255f)) : drawInfo.colorArmorHead;
			yield return beastColorPlayer.eyesColor.GetLayerItem(drawInfo, eyesLightColor,
				Merfolk.EyesTextures[beastColorPlayer.headEyeStyle]
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
	public class Merfolk_Body_Layer : GenericBodyLayer {
		public override Type BoundBeastPlayer => typeof(FishColorPlayer);
		public override IEnumerable<BeastLayerItem> GetData(PlayerDrawSet drawInfo) {
			FishColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<FishColorPlayer>();
			yield return beastColorPlayer.scaleColor.GetLayerItem(drawInfo, drawInfo.colorArmorBody,
				Merfolk.BodyScaleTextures[beastColorPlayer.bodyScaleStyle]
			);
			yield return beastColorPlayer.scaleColor2.GetLayerItem(drawInfo, drawInfo.colorArmorBody,
				Merfolk.BodySecondaryScaleTextures[beastColorPlayer.bodySecondaryScaleStyle]
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
	public class Merfolk_Arm_Layer_Back : GenericArmLayer_Back {
		public override Type BoundBeastPlayer => typeof(FishColorPlayer);
		public override IEnumerable<BeastLayerItem> GetData(PlayerDrawSet drawInfo) {
			FishColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<FishColorPlayer>();
			yield return beastColorPlayer.scaleColor.GetLayerItem(drawInfo, drawInfo.colorArmorBody,
				Merfolk.BodyScaleTextures[beastColorPlayer.bodyScaleStyle]
			);
			yield return beastColorPlayer.scaleColor2.GetLayerItem(drawInfo, drawInfo.colorArmorBody,
				Merfolk.BodySecondaryScaleTextures[beastColorPlayer.bodySecondaryScaleStyle]
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
	public class Merfolk_Arm_Layer_Front : GenericArmLayer_Front {
		public override Type BoundBeastPlayer => typeof(FishColorPlayer);
		public override IEnumerable<BeastLayerItem> GetData(PlayerDrawSet drawInfo) {
			FishColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<FishColorPlayer>();
			yield return beastColorPlayer.scaleColor.GetLayerItem(drawInfo, drawInfo.colorArmorBody,
				Merfolk.BodyScaleTextures[beastColorPlayer.bodyScaleStyle]
			);
			yield return beastColorPlayer.scaleColor2.GetLayerItem(drawInfo, drawInfo.colorArmorBody,
				Merfolk.BodySecondaryScaleTextures[beastColorPlayer.bodySecondaryScaleStyle]
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
	public class Merfolk_Legs_Layer : GenericLegsLayer {
		public override Type BoundBeastPlayer => typeof(FishColorPlayer);
		public override IEnumerable<BeastLayerItem> GetData(PlayerDrawSet drawInfo) {
			FishColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<FishColorPlayer>();
			yield return beastColorPlayer.scaleColor.GetLayerItem(drawInfo, drawInfo.colorArmorLegs,
				Merfolk.LegsScaleTextures[beastColorPlayer.legsScaleStyle]
			);
			yield return beastColorPlayer.scaleColor2.GetLayerItem(drawInfo, drawInfo.colorArmorLegs,
				Merfolk.LegsSecondaryScaleTextures[beastColorPlayer.legsScaleStyle2]
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
