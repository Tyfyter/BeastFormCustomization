using BeastCustomization.Textures;
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
		[Label("Head Primary Style")]
		[ListRange("HeadPrimaryTextures"), Slider]
		public int headPrimaryStyle = 0;

		[Label("Head Secondary Style")]
		[ListRange("HeadSecondaryTextures"), Slider]
		public int headSecondaryStyle = 0;

		[Label("Iris Style")]
		[ListRange("EyesIrisTextures"), Slider]
		public int eyesIrisStyle = 0;

		[Label("Sclera Style")]
		[ListRange("EyesScleraTextures"), Slider]
		public int eyesScleraStyle = 0;

		[Label("Teeth Style")]
		[ListRange("HeadTeethTextures"), Slider]
		public int headTeethStyle = 0;

		[Label("Body Primary Style")]
		[ListRange("BodyPrimaryTextures"), Slider]
		public int bodyPrimaryStyle = 0;

		[Label("Body Secondary Style")]
		[ListRange("BodySecondaryTextures"), Slider]
		public int bodySecondaryStyle = 0;

		[Label("Body Claws Style")]
		[ListRange("BodyClawsTextures"), Slider]
		public int bodyClawsStyle = 0;

		[Label("Legs Primary Style")]
		[ListRange("LegsPrimaryTextures"), Slider]
		public int legsPrimaryStyle = 0;

		[Label("Legs Secondary Style")]
		[ListRange("LegsSecondaryTextures"), Slider]
		public int legsSecondaryStyle = 0;

		[Label("Legs Claws Style")]
		[ListRange("LegsClawsTextures"), Slider]
		public int legsClawsStyle = 0;

		[Label("Glowing Eyes")]
		public bool eyesGlow = false;

		[Label("Apply Dye to Eyes")]
		public bool eyesDye = false;

		//[JsonIgnore]
		[Label("Apply Head Armor")]
		public bool applyHead = false;

		//[JsonIgnore]
		[Label("Apply Body Armor")]
		public bool applyBody = false;

		[Label("Apply Cloaks")]
		public bool applyCloaks = true;

		//[JsonIgnore]
		[Label("Apply Leg Armor")]
		public bool applyLegs = false;

		//[JsonIgnore]
		[Label("Apply Head Armor Over... Fur?")]
		[Tooltip("Looks absolutely ridiculous with almost everything")]
		public bool applyHeadOver = false;

		//[JsonIgnore]
		[Label("Apply Body Armor Over... Fur?")]
		[Tooltip("Looks absolutely ridiculous with almost everything")]
		public bool applyBodyOver = false;

		//[JsonIgnore]
		[Label("Apply Leg Armor Over... Fur?")]
		[Tooltip("Looks absolutely ridiculous with almost everything")]
		public bool applyLegsOver = false;

		[Label("Iris Color")]
		public Color eyesIrisColor = new Color(242, 8, 46);

		[Label("Sclera Color")]
		public Color eyesScleraColor = new Color(241, 241, 241);

		[Label("Teeth Color")]
		public Color headTeethColor = new Color(227, 232, 238);

		[Label("Head Secondary Color")]
		public Color headSecondaryColor = new Color(190, 153, 117);

		[Label("Primary Color")]
		public Color primaryColor = new Color(220, 153, 107);

		[Label("Body Secondary Color")]
		public Color bodySecondaryColor = new Color(190, 153, 117);

		[Label("Claws Color")]
		public Color clawsColor = new Color(222, 206, 192);

		int oldHeadPrimaryStyle;
		int oldHeadSecondaryStyle;
		int oldEyesIrisStyle;
		int oldEyesScleraStyle;
		int oldHeadTeethStyle;
		int oldBodyPrimaryStyle;
		int oldBodySecondaryStyle;
		int oldBodyClawsStyle;
		int oldLegsPrimaryStyle;
		int oldLegsSecondaryStyle;
		int oldLegsClawsStyle;
		bool oldEyesGlow;
		bool oldEyesDye;
		Color oldEyesIrisColor;
		Color oldEyesScleraColor;
		Color oldHeadTeethColor;
		Color oldHeadSecondaryColor;
		Color oldPrimaryColor;
		Color oldBodySecondaryColor;
		Color oldClawsColor;
		bool oldApplyHead;
		bool oldApplyBody;
		bool oldApplyCloaks;
		bool oldApplyLegs;
		bool oldApplyHeadOver;
		bool oldApplyBodyOver;
		bool oldApplyLegsOver;
		#endregion fields
		public override string DisplayName => "Merwolf";
		public override BeastPlayerBase CreateNew() => new FishWolfColorPlayer();
		public override Type ResourceCacheType => typeof(Merwolf);
		public override ref List<TagCompound> ConfigPresets => ref BeastCustomizationSavedPresets.Instance.fishWolfPresets;
		public override bool IsActive => (Player.wereWolf && Player.merman) || (Player.forceWerewolf && Player.forceMerman);
		public override int Specificity => 2;
		public override void StartCustomization() {
			oldHeadPrimaryStyle = headPrimaryStyle;
			oldHeadSecondaryStyle = headSecondaryStyle;
			oldEyesIrisStyle = eyesIrisStyle;
			oldEyesScleraStyle = eyesScleraStyle;
			oldHeadTeethStyle = headTeethStyle;

			oldBodyPrimaryStyle = bodyPrimaryStyle;
			oldBodySecondaryStyle = bodySecondaryStyle;
			oldBodyClawsStyle = bodyClawsStyle;

			oldLegsPrimaryStyle = legsPrimaryStyle;
			oldLegsSecondaryStyle = legsSecondaryStyle;
			oldLegsClawsStyle = legsClawsStyle;

			oldEyesGlow = eyesGlow;
			oldEyesDye = eyesDye;

			oldEyesIrisColor = eyesIrisColor;
			oldEyesScleraColor = eyesScleraColor;

			oldHeadTeethColor = headTeethColor;

			oldPrimaryColor = primaryColor;
			oldHeadSecondaryColor = headSecondaryColor;
			oldBodySecondaryColor = bodySecondaryColor;

			oldClawsColor = clawsColor;

			oldApplyHead = applyHead;
			oldApplyBody = applyBody;
			oldApplyCloaks = applyCloaks;
			oldApplyLegs = applyLegs;

			oldApplyHeadOver = applyHeadOver;
			oldApplyBodyOver = applyBodyOver;
			oldApplyLegsOver = applyLegsOver;
		}
		public override void FinishCustomization(bool overwrite) {
			if (!overwrite) {
				headPrimaryStyle = oldHeadPrimaryStyle;
				headSecondaryStyle = oldHeadSecondaryStyle;
				eyesIrisStyle = oldEyesIrisStyle;
				eyesScleraStyle = oldEyesScleraStyle;
				headTeethStyle = oldHeadTeethStyle;

				bodyPrimaryStyle = oldBodyPrimaryStyle;
				bodySecondaryStyle = oldBodySecondaryStyle;
				bodyClawsStyle = oldBodyClawsStyle;

				legsPrimaryStyle = oldLegsPrimaryStyle;
				legsSecondaryStyle = oldLegsSecondaryStyle;
				legsClawsStyle = oldLegsClawsStyle;

				eyesGlow = oldEyesGlow;
				eyesDye = oldEyesDye;

				eyesIrisColor = oldEyesIrisColor;
				eyesScleraColor = oldEyesScleraColor;

				headTeethColor = oldHeadTeethColor;

				primaryColor = oldPrimaryColor;
				headSecondaryColor = oldHeadSecondaryColor;
				bodySecondaryColor = oldBodySecondaryColor;

				clawsColor = oldClawsColor;

				applyHead = oldApplyHead;
				applyBody = oldApplyBody;
				applyCloaks = oldApplyCloaks;
				applyLegs = oldApplyLegs;

				applyHeadOver = oldApplyHeadOver;
				applyBodyOver = oldApplyBodyOver;
				applyLegsOver = oldApplyLegsOver;
				SendData();
			}
		}
		public override void ExportData(TagCompound tag) {
			tag["headPrimaryStyle"] = headPrimaryStyle;
			tag["headSecondaryStyle"] = headSecondaryStyle;
			tag["headIrisStyle"] = eyesIrisStyle;
			tag["headScleraStyle"] = eyesScleraStyle;
			tag["headTeethStyle"] = headTeethStyle;

			tag["bodyPrimaryStyle"] = bodyPrimaryStyle;
			tag["bodySecondaryStyle"] = bodySecondaryStyle;
			tag["bodyClawsStyle"] = bodyClawsStyle;

			tag["legsPrimaryStyle"] = legsPrimaryStyle;
			tag["legsSecondaryStyle"] = legsSecondaryStyle;
			tag["legsClawsStyle"] = legsClawsStyle;

			tag["eyesGlow"] = eyesGlow;
			tag["eyesDye"] = eyesDye;
			tag["eyesIrisColor"] = eyesIrisColor;
			tag["eyesScleraColor"] = eyesScleraColor;
			tag["headTeethColor"] = headTeethColor;
			tag["primaryColor"] = primaryColor;
			tag["headSecondaryColor"] = headSecondaryColor;
			tag["bodySecondaryColor"] = bodySecondaryColor;
			tag["clawsColor"] = clawsColor;

			tag["applyHead"] = applyHead;
			tag["applyBody"] = applyBody;
			tag["applyCloaks"] = applyCloaks;
			tag["applyLegs"] = applyLegs;

			tag["applyHeadOver"] = applyHeadOver;
			tag["applyBodyOver"] = applyBodyOver;
			tag["applyLegsOver"] = applyLegsOver;
		}
		public override void ImportData(TagCompound tag) {
			if (tag.TryGet("headPrimaryStyle", out int tempHeadPrimaryStyle)) headPrimaryStyle = tempHeadPrimaryStyle;
			if (tag.TryGet("headSecondaryStyle", out int tempHeadSecondaryStyle)) headSecondaryStyle = tempHeadSecondaryStyle;
			if (tag.TryGet("headIrisStyle", out int tempHeadIrisStyle)) eyesIrisStyle = tempHeadIrisStyle;
			if (tag.TryGet("headScleraStyle", out int tempHeadScleraStyle)) eyesScleraStyle = tempHeadScleraStyle;
			if (tag.TryGet("headTeethStyle", out int tempHeadTeethStyle)) headTeethStyle = tempHeadTeethStyle;

			if (tag.TryGet("bodyFurStyle", out int tempBodyFurStyle)) bodyPrimaryStyle = tempBodyFurStyle;
			if (tag.TryGet("bodySecondaryFurStyle", out int tempBodySecondaryStyle)) bodySecondaryStyle = tempBodySecondaryStyle;
			if (tag.TryGet("bodyClawsStyle", out int tempBodyClawsStyle)) bodyClawsStyle = tempBodyClawsStyle;

			if (tag.TryGet("legsFurStyle", out int tempLegsPrimaryStyle)) legsPrimaryStyle = tempLegsPrimaryStyle;
			if (tag.TryGet("legsFurStyle", out int tempLegsSecondaryStyle)) legsSecondaryStyle = tempLegsSecondaryStyle;
			if (tag.TryGet("legsClawsStyle", out int tempLegsClawsStyle)) legsClawsStyle = tempLegsClawsStyle;

			if (tag.TryGet("eyesGlow", out bool tempEyesGlow)) eyesGlow = tempEyesGlow;
			if (tag.TryGet("eyesDye", out bool tempEyesDye)) eyesDye = tempEyesDye;

			if (tag.TryGet("eyesIrisColor", out Color tempEyesIrisColor)) eyesIrisColor = tempEyesIrisColor;
			if (tag.TryGet("eyesScleraColor", out Color tempEyesScleraColor)) eyesScleraColor = tempEyesScleraColor;
			if (tag.TryGet("headTeethColor", out Color tempHeadTeethColor)) headTeethColor = tempHeadTeethColor;
			if (tag.TryGet("primaryColor", out Color tempPrimaryColor)) primaryColor = tempPrimaryColor;
			if (tag.TryGet("headSecondaryColor", out Color tempHeadSecondaryColor)) headSecondaryColor = tempHeadSecondaryColor;
			if (tag.TryGet("bodySecondaryColor", out Color tempBodySecondaryColor)) bodySecondaryColor = tempBodySecondaryColor;
			if (tag.TryGet("clawsColor", out Color tempClawsColor)) clawsColor = tempClawsColor;

			if (tag.TryGet("applyHead", out bool tempApplyHead)) applyHead = tempApplyHead;
			if (tag.TryGet("applyBody", out bool tempApplyBody)) applyBody = tempApplyBody;
			if (tag.TryGet("applyCloaks", out bool tempApplyCloaks)) applyCloaks = tempApplyCloaks;
			if (tag.TryGet("applyLegs", out bool tempApplyLegs)) applyLegs = tempApplyLegs;

			if (tag.TryGet("applyHeadOver", out bool tempApplyHeadOver)) applyHeadOver = tempApplyHeadOver;
			if (tag.TryGet("applyBodyOver", out bool tempApplyBodyOver)) applyBodyOver = tempApplyBodyOver;
			if (tag.TryGet("applyLegsOver", out bool tempApplyLegsOver)) applyLegsOver = tempApplyLegsOver;
			//Mod.Logger.Info($"loading {Player.name}, fur color: {furColor}");
		}
		public override void NetSend(BinaryWriter writer) {
			BeastCustomization.DebugLogger.Info("NetSend");
			BeastCustomization.DebugLogger.Info(writer.BaseStream.Position);
			writer.Write(headPrimaryStyle);
			writer.Write(headSecondaryStyle);
			writer.Write(eyesIrisStyle);
			writer.Write(headTeethStyle);

			writer.Write(bodyPrimaryStyle);
			writer.Write(bodySecondaryStyle);
			writer.Write(bodyClawsStyle);

			writer.Write(legsPrimaryStyle);
			writer.Write(legsSecondaryStyle);
			writer.Write(legsClawsStyle);

			writer.Write(eyesGlow);
			writer.Write(eyesDye);

			writer.Write(eyesIrisColor.PackedValue);
			writer.Write(eyesScleraColor.PackedValue);

			writer.Write(headTeethColor.PackedValue);

			writer.Write(primaryColor.PackedValue);
			writer.Write(headSecondaryColor.PackedValue);
			writer.Write(bodySecondaryColor.PackedValue);

			writer.Write(clawsColor.PackedValue);

			writer.Write(applyHead);
			writer.Write(applyBody);
			writer.Write(applyCloaks);
			writer.Write(applyLegs);

			writer.Write(applyHeadOver);
			writer.Write(applyBodyOver);
			writer.Write(applyLegsOver);
			BeastCustomization.DebugLogger.Info(writer.BaseStream.Position);
		}
		public override void NetRecieve(BinaryReader reader) {
			BeastCustomization.DebugLogger.Info("NetRecieve");
			BeastCustomization.DebugLogger.Info(reader.BaseStream.Position);
			headPrimaryStyle = reader.ReadInt32();
			headSecondaryStyle = reader.ReadInt32();
			eyesIrisStyle = reader.ReadInt32();
			headTeethStyle = reader.ReadInt32();

			bodyPrimaryStyle = reader.ReadInt32();
			bodySecondaryStyle = reader.ReadInt32();
			bodyClawsStyle = reader.ReadInt32();

			legsPrimaryStyle = reader.ReadInt32();
			legsSecondaryStyle = reader.ReadInt32();
			legsClawsStyle = reader.ReadInt32();

			eyesGlow = reader.ReadBoolean();
			eyesDye = reader.ReadBoolean();

			eyesIrisColor.PackedValue = reader.ReadUInt32();
			eyesScleraColor.PackedValue = reader.ReadUInt32();

			headTeethColor.PackedValue = reader.ReadUInt32();

			primaryColor.PackedValue = reader.ReadUInt32();
			headSecondaryColor.PackedValue = reader.ReadUInt32();
			bodySecondaryColor.PackedValue = reader.ReadUInt32();

			clawsColor.PackedValue = reader.ReadUInt32();

			applyHead = reader.ReadBoolean();
			applyBody = reader.ReadBoolean();
			applyCloaks = reader.ReadBoolean();
			applyLegs = reader.ReadBoolean();

			applyHeadOver = reader.ReadBoolean();
			applyBodyOver = reader.ReadBoolean();
			applyLegsOver = reader.ReadBoolean();
			BeastCustomization.DebugLogger.Info(reader.BaseStream.Position);
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
		public override IEnumerable<(Texture2D texture, Color color, bool applyDye)> GetData(PlayerDrawSet drawInfo) {
			FishWolfColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<FishWolfColorPlayer>();
			yield return (
				Merwolf.HeadPrimaryTextures[beastColorPlayer.headPrimaryStyle],
				drawInfo.colorArmorHead.MultiplyRGBA(beastColorPlayer.primaryColor),
				true
			);
			yield return (
				Merwolf.HeadSecondaryTextures[beastColorPlayer.headSecondaryStyle],
				drawInfo.colorArmorHead.MultiplyRGBA(beastColorPlayer.headSecondaryColor),
				true
			);
			yield return (
				Merwolf.HeadTeethTextures[beastColorPlayer.headTeethStyle],
				drawInfo.colorArmorHead.MultiplyRGBA(beastColorPlayer.headTeethColor),
				true
			);
			yield return (
				Merwolf.EyesScleraTextures[beastColorPlayer.eyesScleraStyle],
				beastColorPlayer.eyesGlow ? beastColorPlayer.eyesScleraColor : drawInfo.colorArmorHead.MultiplyRGBA(beastColorPlayer.eyesScleraColor),
				beastColorPlayer.eyesDye
			);
			yield return (
				Merwolf.EyesIrisTextures[beastColorPlayer.eyesIrisStyle],
				beastColorPlayer.eyesGlow ? beastColorPlayer.eyesIrisColor : drawInfo.colorArmorHead.MultiplyRGBA(beastColorPlayer.eyesIrisColor),
				beastColorPlayer.eyesDye
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
					yield return (
						TextureAssets.ArmorHead[slot].Value,
						drawInfo.colorArmorHead,
						true
					);
				}
			}
		}
	}
	public class Merwolf_Body_Layer : GenericBodyLayer {
		public override Type BoundBeastPlayer => typeof(FishWolfColorPlayer);
		public override IEnumerable<(Texture2D texture, Color color)> GetData(PlayerDrawSet drawInfo) {
			FishWolfColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<FishWolfColorPlayer>();
			yield return (
				Merwolf.BodyPrimaryTextures[beastColorPlayer.bodyPrimaryStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.primaryColor)
			);
			yield return (
				Merwolf.BodySecondaryTextures[beastColorPlayer.bodySecondaryStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.bodySecondaryColor)
			);
			yield return (
				Merwolf.BodyClawsTextures[beastColorPlayer.bodyClawsStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.clawsColor)
			);
			if (beastColorPlayer.applyBodyOver) {
				int slot = beastColorPlayer.GetSlot(1);
				if (slot > 0) {
					if (slot < ArmorIDs.Head.Count) {
						Main.instance.LoadArmorBody(slot);
					}
					yield return (
						drawInfo.drawPlayer.Male ? TextureAssets.ArmorBodyComposite[slot].Value : TextureAssets.FemaleBody[slot].Value,
						drawInfo.colorArmorBody
					);
				}
			}
		}
	}
	public class Merwolf_Arm_Layer_Back : GenericArmLayer_Back {
		public override Type BoundBeastPlayer => typeof(FishWolfColorPlayer);
		public override IEnumerable<(Texture2D texture, Color color)> GetData(PlayerDrawSet drawInfo) {
			FishWolfColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<FishWolfColorPlayer>();
			yield return (
				Merwolf.BodyPrimaryTextures[beastColorPlayer.bodyPrimaryStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.primaryColor)
			);
			yield return (
				Merwolf.BodySecondaryTextures[beastColorPlayer.bodySecondaryStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.bodySecondaryColor)
			);
			yield return (
				Merwolf.BodyClawsTextures[beastColorPlayer.bodyClawsStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.clawsColor)
			);
			if (beastColorPlayer.applyBodyOver) {
				int slot = beastColorPlayer.GetSlot(1);
				if (slot > 0) {
					if (slot < ArmorIDs.Head.Count) {
						Main.instance.LoadArmorBody(slot);
					}
					yield return (
						TextureAssets.ArmorBodyComposite[slot].Value,
						drawInfo.colorArmorBody
					);
				}
			}
		}
	}
	public class Merwolf_Arm_Layer_Front : GenericArmLayer_Front {
		public override Type BoundBeastPlayer => typeof(FishWolfColorPlayer);
		public override IEnumerable<(Texture2D texture, Color color)> GetData(PlayerDrawSet drawInfo) {
			FishWolfColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<FishWolfColorPlayer>();
			yield return (
				Merwolf.BodyPrimaryTextures[beastColorPlayer.bodyPrimaryStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.primaryColor)
			);
			yield return (
				Merwolf.BodySecondaryTextures[beastColorPlayer.bodySecondaryStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.bodySecondaryColor)
			);
			yield return (
				Merwolf.BodyClawsTextures[beastColorPlayer.bodyClawsStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.clawsColor)
			);
			if (beastColorPlayer.applyBodyOver) {
				int slot = beastColorPlayer.GetSlot(1);
				if (slot > 0) {
					if (slot < ArmorIDs.Head.Count) {
						Main.instance.LoadArmorBody(slot);
					}
					yield return (
						TextureAssets.ArmorBodyComposite[slot].Value,
						drawInfo.colorArmorBody
					);
				}
			}
		}
	}
	public class Merwolf_Legs_Layer : GenericLegsLayer {
		public override Type BoundBeastPlayer => typeof(FishWolfColorPlayer);
		public override IEnumerable<(Texture2D texture, Color color)> GetData(PlayerDrawSet drawInfo) {
			FishWolfColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<FishWolfColorPlayer>();
			yield return (
				Merwolf.LegsPrimaryTextures[beastColorPlayer.legsPrimaryStyle],
				drawInfo.colorArmorLegs.MultiplyRGBA(beastColorPlayer.primaryColor)
			);
			yield return (
				Merwolf.LegsSecondaryTextures[beastColorPlayer.legsSecondaryStyle],
				drawInfo.colorArmorLegs.MultiplyRGBA(beastColorPlayer.bodySecondaryColor)
			);
			yield return (
				Merwolf.LegsClawsTextures[beastColorPlayer.legsClawsStyle],
				drawInfo.colorArmorLegs.MultiplyRGBA(beastColorPlayer.clawsColor)
			);
			if (beastColorPlayer.applyLegsOver) {
				int slot = beastColorPlayer.GetSlot(2);
				if (slot > 0) {
					if (slot < ArmorIDs.Head.Count) {
						Main.instance.LoadArmorLegs(slot);
					}
					yield return (
						TextureAssets.ArmorLeg[slot].Value,
						drawInfo.colorArmorLegs
					);
				}
			}
		}
	}
}
