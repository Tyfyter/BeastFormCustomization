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
	public class FishColorPlayer : BeastPlayerBase {
		#region fields
		[Label("Head Scale Style")]
		[ListRange("HeadScaleTextures"), Slider]
		public int headScaleStyle = 1;

		[Label("Head Secondary Scale Style")]
		[ListRange("HeadSecondaryScaleTextures"), Slider]
		public int headScaleStyle2 = 0;

		[Label("Eye Style")]
		[ListRange("EyesTextures"), Slider]
		public int headEyeStyle = 0;

		[Label("Body Scale Style")]
		[ListRange("BodyScaleTextures"), Slider]
		public int bodyScaleStyle = 1;

		[Label("Body Secondary Scale Style")]
		[ListRange("BodySecondaryScaleTextures"), Slider]
		public int bodySecondaryScaleStyle = 0;

		[Label("Legs Scale Style")]
		[ListRange("LegsScaleTextures"), Slider]
		public int legsScaleStyle = 1;

		[Label("Legs Secondary Scale Style")]
		[ListRange("LegsSecondaryScaleTextures"), Slider]
		public int legsScaleStyle2 = 0;

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
		[Label("Apply Head Armor Over Scales")]
		[Tooltip("Looks absolutely ridiculous with almost everything")]
		public bool applyHeadOver = false;

		//[JsonIgnore]
		[Label("Apply Body Armor Over Scales")]
		[Tooltip("Looks absolutely ridiculous with almost everything")]
		public bool applyBodyOver = false;

		//[JsonIgnore]
		[Label("Apply Leg Armor Over Scales")]
		[Tooltip("Looks absolutely ridiculous with almost everything")]
		public bool applyLegsOver = false;

		[Label("Eye Color")]
		public Color eyesColor = new Color(242, 8, 46);

		[Label("Scale Color")]
		public Color scaleColor = new Color(41, 185, 127);

		[Label("Secondary Scale Color")]
		public Color scaleColor2 = new Color(168, 255, 106);

		int oldHeadScaleStyle;
		int oldHeadEyeStyle;
		int oldHeadScaleStyle2;
		int oldBodyScaleStyle;
		int oldBodySecondaryScaleStyle;
		int oldLegsScaleStyle;
		int oldLegsClawsStyle;
		bool oldEyesGlow;
		bool oldEyesDye;
		Color oldEyesIrisColor;
		Color oldScaleColor;
		Color oldScaleColor2;
		bool oldApplyHead;
		bool oldApplyBody;
		bool oldApplyCloaks;
		bool oldApplyLegs;
		bool oldApplyHeadOver;
		bool oldApplyBodyOver;
		bool oldApplyLegsOver;
		#endregion fields
		public override string DisplayName => "Merfolk";
		public override FishColorPlayer CreateNew() => new FishColorPlayer();
		public override Type ResourceCacheType => typeof(Merfolk);
		public override ref List<TagCompound> ConfigPresets => ref BeastCustomizationSavedPresets.Instance.fishPresets;
		public override bool IsActive => Player.merman || Player.forceMerman;
		public override int Specificity => 1;
		public override void StartCustomization() {
			oldHeadScaleStyle = headScaleStyle;
			oldHeadScaleStyle2 = headScaleStyle2;
			oldHeadEyeStyle = headEyeStyle;

			oldBodyScaleStyle = bodyScaleStyle;
			oldBodySecondaryScaleStyle = bodySecondaryScaleStyle;

			oldLegsScaleStyle = legsScaleStyle;
			oldLegsClawsStyle = legsScaleStyle2;

			oldEyesGlow = eyesGlow;
			oldEyesDye = eyesDye;
			oldEyesIrisColor = eyesColor;
			oldScaleColor = scaleColor;
			oldScaleColor2 = scaleColor2;
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
				headScaleStyle = oldHeadScaleStyle;
				headScaleStyle2 = oldHeadScaleStyle2;
				headEyeStyle = oldHeadEyeStyle;

				bodyScaleStyle = oldBodyScaleStyle;
				bodySecondaryScaleStyle = oldBodySecondaryScaleStyle;

				legsScaleStyle = oldLegsScaleStyle;
				legsScaleStyle2 = oldLegsClawsStyle;

				eyesGlow = oldEyesGlow;
				eyesDye = oldEyesDye;
				eyesColor = oldEyesIrisColor;
				scaleColor = oldScaleColor;
				scaleColor2 = oldScaleColor2;
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
			tag["headScaleStyle"] = headScaleStyle;
			tag["headScaleStyle2"] = headScaleStyle2;
			tag["headEyeStyle"] = headEyeStyle;
			tag["bodyScaleStyle"] = bodyScaleStyle;
			tag["bodySecondaryScaleStyle"] = bodySecondaryScaleStyle;
			tag["legsScaleStyle"] = legsScaleStyle;
			tag["legsScaleStyle2"] = legsScaleStyle2;
			tag["eyesGlow"] = eyesGlow;
			tag["eyesDye"] = eyesDye;
			tag["eyesColor"] = eyesColor;
			tag["scaleColor"] = scaleColor;
			tag["scaleColor2"] = scaleColor2;
			tag["applyHead"] = applyHead;
			tag["applyBody"] = applyBody;
			tag["applyCloaks"] = applyCloaks;
			tag["applyLegs"] = applyLegs;
			tag["applyHeadOver"] = applyHeadOver;
			tag["applyBodyOver"] = applyBodyOver;
			tag["applyLegsOver"] = applyLegsOver;
		}
		public override void ImportData(TagCompound tag) {
			if (tag.TryGet("headScaleStyle", out int tempHeadScaleStyle)) headScaleStyle = tempHeadScaleStyle;
			if (tag.TryGet("headScaleStyle2", out int tempHeadScaleStyle2)) headScaleStyle2 = tempHeadScaleStyle2;
			if (tag.TryGet("headEyeStyle", out int tempHeadEyeStyle)) headEyeStyle = tempHeadEyeStyle;
			if (tag.TryGet("bodyScaleStyle", out int tempBodyScaleStyle)) bodyScaleStyle = tempBodyScaleStyle;
			if (tag.TryGet("bodySecondaryScaleStyle", out int tempBodySecondaryScaleStyle)) bodySecondaryScaleStyle = tempBodySecondaryScaleStyle;
			if (tag.TryGet("legsScaleStyle", out int tempLegsScaleStyle)) legsScaleStyle = tempLegsScaleStyle;
			if (tag.TryGet("legsScaleStyle2", out int tempLegsClawsStyle)) legsScaleStyle2 = tempLegsClawsStyle;
			if (tag.TryGet("eyesGlow", out bool tempEyesGlow)) eyesGlow = tempEyesGlow;
			if (tag.TryGet("eyesDye", out bool tempEyesDye)) eyesDye = tempEyesDye;
			if (tag.TryGet("eyesColor", out Color tempEyesIrisColor)) eyesColor = tempEyesIrisColor;
			if (tag.TryGet("scaleColor", out Color tempScaleColor)) scaleColor = tempScaleColor;
			if (tag.TryGet("scaleColor2", out Color tempScaleColor2)) scaleColor2 = tempScaleColor2;
			if (tag.TryGet("applyHead", out bool tempApplyHead)) applyHead = tempApplyHead;
			if (tag.TryGet("applyBody", out bool tempApplyBody)) applyBody = tempApplyBody;
			if (tag.TryGet("applyCloaks", out bool tempApplyCloaks)) applyCloaks = tempApplyCloaks;
			if (tag.TryGet("applyLegs", out bool tempApplyLegs)) applyLegs = tempApplyLegs;
			if (tag.TryGet("applyHeadOver", out bool tempApplyHeadOver)) applyHeadOver = tempApplyHeadOver;
			if (tag.TryGet("applyBodyOver", out bool tempApplyBodyOver)) applyBodyOver = tempApplyBodyOver;
			if (tag.TryGet("applyLegsOver", out bool tempApplyLegsOver)) applyLegsOver = tempApplyLegsOver;
			//Mod.Logger.Info($"loading {Player.name}, scale color: {scaleColor}");
		}
		public override void NetSend(BinaryWriter writer) {
			BeastCustomization.DebugLogger.Info("NetSend");
			BeastCustomization.DebugLogger.Info(writer.BaseStream.Position);
			writer.Write(headScaleStyle);
			writer.Write(headScaleStyle2);
			writer.Write(headEyeStyle);

			writer.Write(bodyScaleStyle);
			writer.Write(bodySecondaryScaleStyle);

			writer.Write(legsScaleStyle);
			writer.Write(legsScaleStyle2);

			writer.Write(eyesGlow);
			writer.Write(eyesDye);

			writer.Write(eyesColor.PackedValue);
			writer.Write(scaleColor.PackedValue);
			writer.Write(scaleColor2.PackedValue);

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
			headScaleStyle = reader.ReadInt32();
			headScaleStyle2 = reader.ReadInt32();
			headEyeStyle = reader.ReadInt32();

			bodyScaleStyle = reader.ReadInt32();
			bodySecondaryScaleStyle = reader.ReadInt32();

			legsScaleStyle = reader.ReadInt32();
			legsScaleStyle2 = reader.ReadInt32();

			eyesGlow = reader.ReadBoolean();
			eyesDye = reader.ReadBoolean();

			eyesColor.PackedValue = reader.ReadUInt32();
			scaleColor.PackedValue = reader.ReadUInt32();
			scaleColor2.PackedValue = reader.ReadUInt32();

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
	public class Merfolk_Head_Layer : GenericHeadLayer {
		public override Type BoundBeastPlayer => typeof(FishColorPlayer);
		public override IEnumerable<(Texture2D texture, Color color, bool applyDye)> GetData(PlayerDrawSet drawInfo) {
			FishColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<FishColorPlayer>();
			yield return (
				Merfolk.HeadScaleTextures[beastColorPlayer.headScaleStyle],
				drawInfo.colorArmorHead.MultiplyRGBA(beastColorPlayer.scaleColor),
				true
			);
			yield return (
				Merfolk.HeadSecondaryScaleTextures[beastColorPlayer.headScaleStyle2],
				drawInfo.colorArmorHead.MultiplyRGBA(beastColorPlayer.scaleColor2),
				true
			);
			yield return (
				Merfolk.EyesTextures[beastColorPlayer.headEyeStyle],
				beastColorPlayer.eyesGlow ? beastColorPlayer.eyesColor : drawInfo.colorArmorHead.MultiplyRGBA(beastColorPlayer.eyesColor),
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
	public class Merfolk_Body_Layer : GenericBodyLayer {
		public override Type BoundBeastPlayer => typeof(FishColorPlayer);
		public override IEnumerable<(Texture2D texture, Color color)> GetData(PlayerDrawSet drawInfo) {
			FishColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<FishColorPlayer>();
			yield return (
				Merfolk.BodyScaleTextures[beastColorPlayer.bodyScaleStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.scaleColor)
			);
			yield return (
				Merfolk.BodySecondaryScaleTextures[beastColorPlayer.bodySecondaryScaleStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.scaleColor2)
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
	public class Merfolk_Arm_Layer_Back : GenericArmLayer_Back {
		public override Type BoundBeastPlayer => typeof(FishColorPlayer);
		public override IEnumerable<(Texture2D texture, Color color)> GetData(PlayerDrawSet drawInfo) {
			FishColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<FishColorPlayer>();
			yield return (
				Merfolk.BodyScaleTextures[beastColorPlayer.bodyScaleStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.scaleColor)
			);
			yield return (
				Merfolk.BodySecondaryScaleTextures[beastColorPlayer.bodySecondaryScaleStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.scaleColor2)
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
	public class Merfolk_Arm_Layer_Front : GenericArmLayer_Front {
		public override Type BoundBeastPlayer => typeof(FishColorPlayer);
		public override IEnumerable<(Texture2D texture, Color color)> GetData(PlayerDrawSet drawInfo) {
			FishColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<FishColorPlayer>();
			yield return (
				Merfolk.BodyScaleTextures[beastColorPlayer.bodyScaleStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.scaleColor)
			);
			yield return (
				Merfolk.BodySecondaryScaleTextures[beastColorPlayer.bodySecondaryScaleStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.scaleColor2)
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
	public class Merfolk_Legs_Layer : GenericLegsLayer {
		public override Type BoundBeastPlayer => typeof(FishColorPlayer);
		public override IEnumerable<(Texture2D texture, Color color)> GetData(PlayerDrawSet drawInfo) {
			FishColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<FishColorPlayer>();
			yield return (
				Merfolk.LegsScaleTextures[beastColorPlayer.legsScaleStyle],
				drawInfo.colorArmorLegs.MultiplyRGBA(beastColorPlayer.scaleColor)
			);
			yield return (
				Merfolk.LegsSecondaryScaleTextures[beastColorPlayer.legsScaleStyle2],
				drawInfo.colorArmorLegs.MultiplyRGBA(beastColorPlayer.scaleColor2)
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
