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
	[LegacyName("BeastColorPlayer")]
	public class WolfColorPlayer : BeastPlayerBase {
		#region fields
		[Label("Head Fur Style")]
		[ListRange("HeadFurTextures"), Slider]
		public int headFurStyle = 1;

		[Label("Teeth Style")]
		[ListRange("HeadTeethTextures"), Slider]
		public int headTeethStyle = 0;

		[Label("Body Fur Style")]
		[ListRange("BodyFurTextures"), Slider]
		public int bodyFurStyle = 1;

		[Label("Body Secondary Fur Style")]
		[ListRange("BodySecondaryFurTextures"), Slider]
		public int bodySecondaryFurStyle = 0;

		[Label("Body Claws Style")]
		[ListRange("BodyClawsTextures"), Slider]
		public int bodyClawsStyle = 0;

		[Label("Legs Fur Style")]
		[ListRange("LegsFurTextures"), Slider]
		public int legsFurStyle = 1;

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

		//[JsonIgnore]
		[Label("Apply Leg Armor")]
		public bool applyLegs = false;

		//[JsonIgnore]
		[Label("Apply Head Armor Over Fur")]
		[Tooltip("Looks absolutely ridiculous with almost everything")]
		public bool applyHeadOver = false;

		//[JsonIgnore]
		[Label("Apply Body Armor Over Fur")]
		[Tooltip("Looks absolutely ridiculous with almost everything")]
		public bool applyBodyOver = false;

		//[JsonIgnore]
		[Label("Apply Leg Armor Over Fur")]
		[Tooltip("Looks absolutely ridiculous with almost everything")]
		public bool applyLegsOver = false;

		[Label("Iris Color")]
		public Color eyesIrisColor = new Color(242, 8, 46);

		[Label("Sclera Color")]
		public Color eyesScleraColor = new Color(241, 241, 241);

		[Label("Teeth Color")]
		public Color headTeethColor = new Color(227, 232, 238);

		[Label("Fur Color")]
		public Color furColor = new Color(220, 153, 107);

		[Label("Secondary Fur Color")]
		public Color furColor2 = new Color(190, 153, 117);

		[Label("Claws Color")]
		public Color clawsColor = new Color(222, 206, 192);

		int oldHeadFurStyle;
		int oldHeadTeethStyle;
		int oldBodyFurStyle;
		int oldBodySecondaryFurStyle;
		int oldBodyClawsStyle;
		int oldLegsFurStyle;
		int oldLegsClawsStyle;
		bool oldEyesGlow;
		bool oldEyesDye;
		Color oldEyesIrisColor;
		Color oldEyesScleraColor;
		Color oldHeadTeethColor;
		Color oldFurColor;
		Color oldFurColor2;
		Color oldClawsColor;
		bool oldApplyHead;
		bool oldApplyBody;
		bool oldApplyLegs;
		bool oldApplyHeadOver;
		bool oldApplyBodyOver;
		bool oldApplyLegsOver;
		#endregion fields
		public override string DisplayName => "Werewolf";
		public override BeastPlayerBase CreateNew() => new WolfColorPlayer();
		public override Type ResourceCacheType => typeof(Werewolf);
		public override ref List<TagCompound> ConfigPresets => ref BeastCustomizationSavedPresets.Instance.wolfPresets;
		public override bool IsActive => Player.wereWolf || Player.forceWerewolf;
		public override int Specificity => 1;
		public override void StartCustomization() {
			oldHeadFurStyle = headFurStyle;
			oldHeadTeethStyle = headTeethStyle;
			oldBodyFurStyle = bodyFurStyle;
			oldBodySecondaryFurStyle = bodySecondaryFurStyle;
			oldBodyClawsStyle = bodyClawsStyle;
			oldLegsFurStyle = legsFurStyle;
			oldLegsClawsStyle = legsClawsStyle;
			oldEyesGlow = eyesGlow;
			oldEyesDye = eyesDye;
			oldEyesIrisColor = eyesIrisColor;
			oldEyesScleraColor = eyesScleraColor;
			oldHeadTeethColor = headTeethColor;
			oldFurColor = furColor;
			oldFurColor2 = furColor2;
			oldClawsColor = clawsColor;
			oldApplyHead = applyHead;
			oldApplyBody = applyBody;
			oldApplyLegs = applyLegs;
			oldApplyHeadOver = applyHeadOver;
			oldApplyBodyOver = applyBodyOver;
			oldApplyLegsOver = applyLegsOver;
		}
		public override void FinishCustomization(bool overwrite) {
			if (!overwrite) {
				headFurStyle = oldHeadFurStyle;
				headTeethStyle = oldHeadTeethStyle;
				bodyFurStyle = oldBodyFurStyle;
				bodySecondaryFurStyle = oldBodySecondaryFurStyle;
				bodyClawsStyle = oldBodyClawsStyle;
				legsFurStyle = oldLegsFurStyle;
				legsClawsStyle = oldLegsClawsStyle;
				eyesGlow = oldEyesGlow;
				eyesDye = oldEyesDye;
				eyesIrisColor = oldEyesIrisColor;
				eyesScleraColor = oldEyesScleraColor;
				headTeethColor = oldHeadTeethColor;
				furColor = oldFurColor;
				furColor2 = oldFurColor2;
				clawsColor = oldClawsColor;
				applyHead = oldApplyHead;
				applyBody = oldApplyBody;
				applyLegs = oldApplyLegs;
				applyHeadOver = oldApplyHeadOver;
				applyBodyOver = oldApplyBodyOver;
				applyLegsOver = oldApplyLegsOver;
				SendData();
			}
		}
		public override void ExportData(TagCompound tag) {
			tag["headFurStyle"] = headFurStyle;
			tag["headTeethStyle"] = headTeethStyle;
			tag["bodyFurStyle"] = bodyFurStyle;
			tag["bodySecondaryFurStyle"] = bodySecondaryFurStyle;
			tag["bodyClawsStyle"] = bodyClawsStyle;
			tag["legsFurStyle"] = legsFurStyle;
			tag["legsClawsStyle"] = legsClawsStyle;
			tag["eyesGlow"] = eyesGlow;
			tag["eyesDye"] = eyesDye;
			tag["eyesIrisColor"] = eyesIrisColor;
			tag["eyesScleraColor"] = eyesScleraColor;
			tag["headTeethColor"] = headTeethColor;
			tag["furColor"] = furColor;
			tag["furColor2"] = furColor2;
			tag["clawsColor"] = clawsColor;
			tag["applyHead"] = applyHead;
			tag["applyBody"] = applyBody;
			tag["applyLegs"] = applyLegs;
			tag["applyHeadOver"] = applyHeadOver;
			tag["applyBodyOver"] = applyBodyOver;
			tag["applyLegsOver"] = applyLegsOver;
		}
		public override void ImportData(TagCompound tag) {
			if (tag.TryGet("headFurStyle", out int tempHeadFurStyle)) headFurStyle = tempHeadFurStyle;
			if (tag.TryGet("headTeethStyle", out int tempHeadTeethStyle)) headTeethStyle = tempHeadTeethStyle;
			if (tag.TryGet("bodyFurStyle", out int tempBodyFurStyle)) bodyFurStyle = tempBodyFurStyle;
			if (tag.TryGet("bodySecondaryFurStyle", out int tempBodySecondaryFurStyle)) bodySecondaryFurStyle = tempBodySecondaryFurStyle;
			if (tag.TryGet("bodyClawsStyle", out int tempBodyClawsStyle)) bodyClawsStyle = tempBodyClawsStyle;
			if (tag.TryGet("legsFurStyle", out int tempLegsFurStyle)) legsFurStyle = tempLegsFurStyle;
			if (tag.TryGet("legsClawsStyle", out int tempLegsClawsStyle)) legsClawsStyle = tempLegsClawsStyle;
			if (tag.TryGet("eyesGlow", out bool tempEyesGlow)) eyesGlow = tempEyesGlow;
			if (tag.TryGet("eyesDye", out bool tempEyesDye)) eyesDye = tempEyesDye;
			if (tag.TryGet("eyesIrisColor", out Color tempEyesIrisColor)) eyesIrisColor = tempEyesIrisColor;
			if (tag.TryGet("eyesScleraColor", out Color tempEyesScleraColor)) eyesScleraColor = tempEyesScleraColor;
			if (tag.TryGet("headTeethColor", out Color tempHeadTeethColor)) headTeethColor = tempHeadTeethColor;
			if (tag.TryGet("furColor", out Color tempFurColor)) furColor = tempFurColor;
			if (tag.TryGet("furColor2", out Color tempFurColor2)) furColor2 = tempFurColor2;
			if (tag.TryGet("clawsColor", out Color tempClawsColor)) clawsColor = tempClawsColor;
			if (tag.TryGet("applyHead", out bool tempApplyHead)) applyHead = tempApplyHead;
			if (tag.TryGet("applyBody", out bool tempApplyBody)) applyBody = tempApplyBody;
			if (tag.TryGet("applyLegs", out bool tempApplyLegs)) applyLegs = tempApplyLegs;
			if (tag.TryGet("applyHeadOver", out bool tempApplyHeadOver)) applyHeadOver = tempApplyHeadOver;
			if (tag.TryGet("applyBodyOver", out bool tempApplyBodyOver)) applyBodyOver = tempApplyBodyOver;
			if (tag.TryGet("applyLegsOver", out bool tempApplyLegsOver)) applyLegsOver = tempApplyLegsOver;
			//Mod.Logger.Info($"loading {Player.name}, fur color: {furColor}");
		}
		public override void NetSend(BinaryWriter writer) {
			BeastCustomization.DebugLogger.Info("NetSend");
			BeastCustomization.DebugLogger.Info(writer.BaseStream.Position);
			writer.Write(headFurStyle);
			writer.Write(headTeethStyle);
			writer.Write(bodyFurStyle);
			writer.Write(bodySecondaryFurStyle);
			writer.Write(bodyClawsStyle);
			writer.Write(legsFurStyle);
			writer.Write(legsClawsStyle);

			writer.Write(eyesGlow);
			writer.Write(eyesDye);

			writer.Write(eyesIrisColor.PackedValue);
			writer.Write(eyesScleraColor.PackedValue);
			writer.Write(headTeethColor.PackedValue);
			writer.Write(furColor.PackedValue);
			writer.Write(furColor2.PackedValue);
			writer.Write(clawsColor.PackedValue);

			writer.Write(applyHead);
			writer.Write(applyBody);
			writer.Write(applyLegs);
			writer.Write(applyHeadOver);
			writer.Write(applyBodyOver);
			writer.Write(applyLegsOver);
			BeastCustomization.DebugLogger.Info(writer.BaseStream.Position);
		}
		public override void NetRecieve(BinaryReader reader) {
			BeastCustomization.DebugLogger.Info("NetRecieve");
			BeastCustomization.DebugLogger.Info(reader.BaseStream.Position);
			headFurStyle = reader.ReadInt32();
			headTeethStyle = reader.ReadInt32();
			bodyFurStyle = reader.ReadInt32();
			bodySecondaryFurStyle = reader.ReadInt32();
			bodyClawsStyle = reader.ReadInt32();
			legsFurStyle = reader.ReadInt32();
			legsClawsStyle = reader.ReadInt32();

			eyesGlow = reader.ReadBoolean();
			eyesDye = reader.ReadBoolean();

			eyesIrisColor.PackedValue = reader.ReadUInt32();
			eyesScleraColor.PackedValue = reader.ReadUInt32();
			headTeethColor.PackedValue = reader.ReadUInt32();
			furColor.PackedValue = reader.ReadUInt32();
			furColor2.PackedValue = reader.ReadUInt32();
			clawsColor.PackedValue = reader.ReadUInt32();

			applyHead = reader.ReadBoolean();
			applyBody = reader.ReadBoolean();
			applyLegs = reader.ReadBoolean();
			applyHeadOver = reader.ReadBoolean();
			applyBodyOver = reader.ReadBoolean();
			applyLegsOver = reader.ReadBoolean();
			BeastCustomization.DebugLogger.Info(reader.BaseStream.Position);
		}
		internal const bool enabled = true;
		public override void HideDrawLayers(PlayerDrawSet drawInfo) {
			if (drawInfo.drawPlayer.head == ArmorIDs.Head.Werewolf) {
				if (!applyHead || GetSlot(0) == -1) {
					PlayerDrawLayers.Head.Hide();
				}
			}
			if (drawInfo.drawPlayer.body == ArmorIDs.Body.Werewolf) {
				if (!applyBody || GetSlot(1) == -1) {
					PlayerDrawLayers.Skin.Hide();
					PlayerDrawLayers.Torso.Hide();
					PlayerDrawLayers.ArmOverItem.Hide();
				}
			}
			if (drawInfo.drawPlayer.legs == 20) {
				if (!applyLegs || GetSlot(2) == -1) {
					PlayerDrawLayers.Leggings.Hide();
				}
			}
		}
		public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo) {
			if (drawInfo.drawPlayer.head == ArmorIDs.Head.Werewolf && applyHead) {
				int slot = GetSlot(0);
				if (slot >= 0) {
					drawInfo.drawPlayer.head = slot;
					if (slot > 0 && slot < ArmorIDs.Head.Count) {
						Main.instance.LoadArmorHead(slot);
						int backID = ArmorIDs.Head.Sets.FrontToBackID[slot];
						if (backID >= 0) {
							Main.instance.LoadArmorHead(backID);
						}
					}
					drawInfo.drawsBackHairWithoutHeadgear = ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[drawInfo.drawPlayer.head];
					drawInfo.fullHair = ArmorIDs.Head.Sets.DrawFullHair[drawInfo.drawPlayer.head];
					drawInfo.hatHair = ArmorIDs.Head.Sets.DrawHatHair[drawInfo.drawPlayer.head];
				}
			}
			if (drawInfo.drawPlayer.body == ArmorIDs.Body.Werewolf && applyBody) {
				int slot = GetSlot(1);
				if (slot >= 0) {
					drawInfo.drawPlayer.body = slot;
					if (slot > 0 && slot < ArmorIDs.Body.Count) {
						Main.instance.LoadArmorBody(slot);
					}
					drawInfo.armorHidesHands = ArmorIDs.Body.Sets.HidesHands[slot];
					drawInfo.armorHidesArms = ArmorIDs.Body.Sets.HidesArms[slot];
				}
			}
			if (drawInfo.drawPlayer.legs == 20 && applyLegs) {
				int slot = GetSlot(2);
				if (slot >= 0) {
					drawInfo.drawPlayer.legs = slot;
					if (slot > 0 && slot < ArmorIDs.Legs.Count) {
						Main.instance.LoadArmorLegs(slot);
					}
				}
			}
		}
	}
	public class Werewolf_Head_Layer : GenericHeadLayer {
		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			return WolfColorPlayer.enabled && drawInfo.drawPlayer.head == ArmorIDs.Head.Werewolf;
		}
		public override IEnumerable<(Texture2D texture, Color color, bool applyDye)> GetData(PlayerDrawSet drawInfo) {
			WolfColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<WolfColorPlayer>();
			yield return (
				Werewolf.HeadFurTextures[beastColorPlayer.headFurStyle],
				drawInfo.colorArmorHead.MultiplyRGBA(beastColorPlayer.furColor),
				true
			);
			yield return (
				Werewolf.HeadTeethTextures[beastColorPlayer.headTeethStyle],
				drawInfo.colorArmorHead.MultiplyRGBA(beastColorPlayer.headTeethColor),
				true
			);
			yield return (
				Werewolf.EyesScleraTexture,
				beastColorPlayer.eyesGlow ? beastColorPlayer.eyesScleraColor : drawInfo.colorArmorHead.MultiplyRGBA(beastColorPlayer.eyesScleraColor),
				beastColorPlayer.eyesDye
			);
			yield return (
				Werewolf.EyesIrisTexture,
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
	public class Werewolf_Body_Layer : GenericBodyLayer {
		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			return WolfColorPlayer.enabled && drawInfo.drawPlayer.body == ArmorIDs.Body.Werewolf;
		}
		public override IEnumerable<(Texture2D texture, Color color)> GetData(PlayerDrawSet drawInfo) {
			WolfColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<WolfColorPlayer>();
			yield return (
				Werewolf.BodyFurTextures[beastColorPlayer.bodyFurStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.furColor)
			);
			yield return (
				Werewolf.BodySecondaryFurTextures[beastColorPlayer.bodySecondaryFurStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.furColor2)
			);
			yield return (
				Werewolf.BodyClawsTextures[beastColorPlayer.bodyClawsStyle],
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
	public class Werewolf_Arm_Layer_Back : GenericArmLayer_Back {
		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			return WolfColorPlayer.enabled && drawInfo.drawPlayer.body == ArmorIDs.Body.Werewolf;
		}
		public override IEnumerable<(Texture2D texture, Color color)> GetData(PlayerDrawSet drawInfo) {
			WolfColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<WolfColorPlayer>();
			yield return (
				Werewolf.BodyFurTextures[beastColorPlayer.bodyFurStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.furColor)
			);
			yield return (
				Werewolf.BodySecondaryFurTextures[beastColorPlayer.bodySecondaryFurStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.furColor2)
			);
			yield return (
				Werewolf.BodyClawsTextures[beastColorPlayer.bodyClawsStyle],
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
	public class Werewolf_Arm_Layer_Front : GenericArmLayer_Front {
		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			return WolfColorPlayer.enabled && drawInfo.drawPlayer.body == ArmorIDs.Body.Werewolf;
		}
		public override IEnumerable<(Texture2D texture, Color color)> GetData(PlayerDrawSet drawInfo) {
			WolfColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<WolfColorPlayer>();
			yield return (
				Werewolf.BodyFurTextures[beastColorPlayer.bodyFurStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.furColor)
			);
			yield return (
				Werewolf.BodySecondaryFurTextures[beastColorPlayer.bodySecondaryFurStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.furColor2)
			);
			yield return (
				Werewolf.BodyClawsTextures[beastColorPlayer.bodyClawsStyle],
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
	public class Werewolf_Legs_Layer : GenericLegsLayer {
		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			return WolfColorPlayer.enabled && drawInfo.drawPlayer.legs == 20;//doesn't have an ArmorIDs entry?
		}
		public override IEnumerable<(Texture2D texture, Color color)> GetData(PlayerDrawSet drawInfo) {
			WolfColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<WolfColorPlayer>();
			yield return (
				Werewolf.LegsFurTextures[beastColorPlayer.legsFurStyle],
				drawInfo.colorArmorLegs.MultiplyRGBA(beastColorPlayer.furColor)
			);
			yield return (
				Werewolf.LegsClawsTextures[beastColorPlayer.legsClawsStyle],
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
	/*
	public class Head_Layer : PlayerDrawLayer {
		public override bool IsHeadLayer => true;
		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			return WolfColorPlayer.enabled && drawInfo.drawPlayer.head == ArmorIDs.Head.Werewolf;
		}
		public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.Head, PlayerDrawLayers.FaceAcc);
		protected override void Draw(ref PlayerDrawSet drawInfo) {
			Player drawPlayer = drawInfo.drawPlayer;
			WolfColorPlayer beastColorPlayer = drawPlayer.GetModPlayer<WolfColorPlayer>();
			Color adjustedFurColor = drawInfo.colorArmorHead.MultiplyRGBA(beastColorPlayer.furColor);
			Color adjustedIrisColor = beastColorPlayer.eyesGlow ? beastColorPlayer.eyesIrisColor : drawInfo.colorArmorHead.MultiplyRGBA(beastColorPlayer.eyesIrisColor);
			Color adjustedScleraColor = beastColorPlayer.eyesGlow ? beastColorPlayer.eyesScleraColor : drawInfo.colorArmorHead.MultiplyRGBA(beastColorPlayer.eyesScleraColor);
			Color adjustedTeethColor = drawInfo.colorArmorHead.MultiplyRGBA(beastColorPlayer.headTeethColor);

			Vector2 Position = new Vector2((int)(drawInfo.Position.X + drawPlayer.width / 2f - drawPlayer.bodyFrame.Width / 2f - Main.screenPosition.X), (int)(drawInfo.Position.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f - Main.screenPosition.Y)) + drawPlayer.headPosition + drawInfo.headVect;
			Rectangle? Frame = drawPlayer.bodyFrame;
			DrawData item = new(Werewolf.HeadFurTextures[beastColorPlayer.headFurStyle], Position, Frame, adjustedFurColor, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0) {
				shader = drawPlayer.cHead
			};
			drawInfo.DrawDataCache.Add(item);

			item = new(Werewolf.HeadTeethTextures[beastColorPlayer.headTeethStyle], Position, Frame, adjustedTeethColor, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0) {
				shader = drawPlayer.cHead
			};
			drawInfo.DrawDataCache.Add(item);

			item = new(Werewolf.EyesScleraTexture, Position, Frame, adjustedScleraColor, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0);
			if (beastColorPlayer.eyesDye) item.shader = drawPlayer.cHead;
			drawInfo.DrawDataCache.Add(item);

			item = new(Werewolf.EyesIrisTexture, Position, Frame, adjustedIrisColor, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0);
			if (beastColorPlayer.eyesDye) item.shader = drawPlayer.cHead;
			drawInfo.DrawDataCache.Add(item);
		}
	}
	public class Body_Layer : PlayerDrawLayer {
		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			return WolfColorPlayer.enabled && drawInfo.drawPlayer.body == ArmorIDs.Body.Werewolf;
		}
		public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.Torso, PlayerDrawLayers.OffhandAcc);
		protected override void Draw(ref PlayerDrawSet drawInfo) {
			Player drawPlayer = drawInfo.drawPlayer;
			WolfColorPlayer beastColorPlayer = drawPlayer.GetModPlayer<WolfColorPlayer>();
			Color adjustedFurColor = drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.furColor);
			Color adjustedSecondaryFurColor = drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.furColor2);
			Color adjustedClawColor = drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.clawsColor);
			if (drawInfo.usesCompositeTorso) {
				Rectangle Frame = drawInfo.compTorsoFrame;

				Vector2 Position = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (drawInfo.drawPlayer.bodyFrame.Width / 2) + (drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.bodyFrame.Width / 2, drawInfo.drawPlayer.bodyFrame.Height / 2);
				Vector2 value = Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height];
				value.Y -= 2f;
				Position += value * -drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically).ToDirectionInt();

				DrawData item = new DrawData(Werewolf.BodyFurTextures[beastColorPlayer.bodyFurStyle], Position, Frame, adjustedFurColor, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = drawPlayer.cBody
				};
				drawInfo.DrawDataCache.Add(item);

				item = new DrawData(Werewolf.BodySecondaryFurTextures[beastColorPlayer.bodySecondaryFurStyle], Position, Frame, adjustedSecondaryFurColor, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = GameShaders.Armor.GetShaderIdFromItemId(drawPlayer.dye[1].type)
				};
				drawInfo.DrawDataCache.Add(item);

				item = new DrawData(Werewolf.BodyClawsTextures[beastColorPlayer.bodyClawsStyle], Position, Frame, adjustedClawColor, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = drawPlayer.cBody
				};
				drawInfo.DrawDataCache.Add(item);
			} else {
				Rectangle Frame = drawPlayer.bodyFrame;

				Vector2 Position = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - Frame.Width / 2f + drawPlayer.width / 2f), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawPlayer.height - Frame.Height + 4f)) + drawPlayer.bodyPosition + drawInfo.bodyVect;
				DrawData item = new DrawData(Werewolf.BodyFurTextures[beastColorPlayer.bodyFurStyle], Position, Frame, adjustedFurColor, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = drawPlayer.cBody
				};
				drawInfo.DrawDataCache.Add(item);

				item = new DrawData(Werewolf.BodySecondaryFurTextures[beastColorPlayer.bodySecondaryFurStyle], Position, Frame, adjustedSecondaryFurColor, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = drawPlayer.cBody
				};
				drawInfo.DrawDataCache.Add(item);

				item = new DrawData(Werewolf.BodyClawsTextures[beastColorPlayer.bodyClawsStyle], Position, Frame, adjustedClawColor, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = drawPlayer.cBody
				};
				drawInfo.DrawDataCache.Add(item);
			}
		}
	}
	public class Arm_Layer_Back : PlayerDrawLayer {
		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			return WolfColorPlayer.enabled && drawInfo.drawPlayer.body == ArmorIDs.Body.Werewolf;
		}
		public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.BalloonAcc, PlayerDrawLayers.Skin);
		protected override void Draw(ref PlayerDrawSet drawInfo) {
			Player drawPlayer = drawInfo.drawPlayer;
			WolfColorPlayer beastColorPlayer = drawPlayer.GetModPlayer<WolfColorPlayer>();
			Color adjustedFurColor = drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.furColor);
			Color adjustedSecondaryFurColor = drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.furColor2);
			Color adjustedClawColor = drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.clawsColor);
			if (drawInfo.usesCompositeTorso) {
				Vector2 position = new Vector2((int)(drawInfo.Position.X - (drawInfo.drawPlayer.bodyFrame.Width / 2) + (drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.bodyFrame.Width / 2, drawInfo.drawPlayer.bodyFrame.Height / 2) - Main.screenPosition;
				Vector2 offset = Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height];
				offset.Y -= 2f;
				position += offset * -drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically).ToDirectionInt();
				Vector2 backArmOffset = new Vector2(6 * ((!drawInfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally)) ? 1 : -1), 2 * ((!drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically)) ? 1 : -1));
				position += backArmOffset;
				Vector2 bodyVect = drawInfo.bodyVect + backArmOffset;
				position += drawInfo.backShoulderOffset;
				float rotation = drawPlayer.bodyRotation + drawInfo.compositeBackArmRotation;
				DrawData drawData = new DrawData(Werewolf.BodyFurTextures[beastColorPlayer.bodyFurStyle], position, drawInfo.compBackArmFrame, adjustedFurColor, rotation, bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = drawInfo.cBody
				};
				drawInfo.DrawDataCache.Add(drawData);

				drawData = new DrawData(Werewolf.BodySecondaryFurTextures[beastColorPlayer.bodySecondaryFurStyle], position, drawInfo.compBackArmFrame, adjustedSecondaryFurColor, rotation, bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = drawInfo.cBody
				};
				drawInfo.DrawDataCache.Add(drawData);

				drawData = new DrawData(Werewolf.BodyClawsTextures[beastColorPlayer.bodyClawsStyle], position, drawInfo.compBackArmFrame, adjustedClawColor, rotation, bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = drawInfo.cBody
				};
				drawInfo.DrawDataCache.Add(drawData);
			} else {
				Rectangle Frame = drawPlayer.bodyFrame;

				Vector2 Position = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - Frame.Width / 2f + drawPlayer.width / 2f), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawPlayer.height - Frame.Height + 4f)) + drawPlayer.bodyPosition + drawInfo.bodyVect;
				DrawData item = new DrawData(Werewolf.BodyFurTextures[beastColorPlayer.bodyFurStyle], Position, Frame, adjustedFurColor, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = drawInfo.cBody
				};
				drawInfo.DrawDataCache.Add(item);

				item = new DrawData(Werewolf.BodySecondaryFurTextures[beastColorPlayer.bodySecondaryFurStyle], Position, Frame, adjustedSecondaryFurColor, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = drawInfo.cBody
				};
				drawInfo.DrawDataCache.Add(item);

				item = new DrawData(Werewolf.BodyClawsTextures[beastColorPlayer.bodyClawsStyle], Position, Frame, adjustedClawColor, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = drawInfo.cBody
				};
				drawInfo.DrawDataCache.Add(item);
			}
		}
	}
	public class Arm_Layer_Front : PlayerDrawLayer {
		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			return WolfColorPlayer.enabled && drawInfo.drawPlayer.body == ArmorIDs.Body.Werewolf;
		}
		public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.ArmOverItem, PlayerDrawLayers.HandOnAcc);
		protected override void Draw(ref PlayerDrawSet drawInfo) {
			Player drawPlayer = drawInfo.drawPlayer;
			if (drawPlayer.controlUp) {

			}
			WolfColorPlayer beastColorPlayer = drawPlayer.GetModPlayer<WolfColorPlayer>();
			Color adjustedFurColor = drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.furColor);
			Color adjustedSecondaryFurColor = drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.furColor2);
			Color adjustedClawColor = drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.clawsColor);
			if (drawInfo.usesCompositeTorso) {

				Vector2 position = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawInfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawInfo.drawPlayer.height - (float)drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.bodyFrame.Width / 2, drawInfo.drawPlayer.bodyFrame.Height / 2);
				Vector2 offset = Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height];
				offset.Y -= 2f;
				position += offset * -drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically).ToDirectionInt();
				position += drawInfo.frontShoulderOffset;
				float rotation = drawInfo.drawPlayer.bodyRotation + drawInfo.compositeFrontArmRotation;
				Vector2 bodyVect = drawInfo.bodyVect;
				float offsetX = -5 * ((!drawInfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally)) ? 1 : (-1));
				bodyVect.X += offsetX;
				position.X += offsetX;
				position += drawInfo.frontShoulderOffset;
				if (drawInfo.compFrontArmFrame.X / drawInfo.compFrontArmFrame.Width >= 7) {
					position += new Vector2((!drawInfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally)) ? 1 : -1, (!drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically)) ? 1 : -1);
				}
				DrawData drawData = new DrawData(Werewolf.BodyFurTextures[beastColorPlayer.bodyFurStyle], position, drawInfo.compFrontArmFrame, adjustedFurColor, rotation, bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = drawInfo.cBody
				};
				drawInfo.DrawDataCache.Add(drawData);

				drawData = new DrawData(Werewolf.BodySecondaryFurTextures[beastColorPlayer.bodySecondaryFurStyle], position, drawInfo.compFrontArmFrame, adjustedSecondaryFurColor, rotation, bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = drawInfo.cBody
				};
				drawInfo.DrawDataCache.Add(drawData);

				drawData = new DrawData(Werewolf.BodyClawsTextures[beastColorPlayer.bodyClawsStyle], position, drawInfo.compFrontArmFrame, adjustedClawColor, rotation, bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = drawInfo.cBody
				};
				drawInfo.DrawDataCache.Add(drawData);
			} else {
				Rectangle Frame = drawPlayer.bodyFrame;

				Vector2 Position = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - Frame.Width / 2f + drawPlayer.width / 2f), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawPlayer.height - Frame.Height + 4f)) + drawPlayer.bodyPosition + drawInfo.bodyVect;
				DrawData item = new DrawData(Werewolf.BodyFurTextures[beastColorPlayer.bodyFurStyle], Position, Frame, adjustedFurColor, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = drawInfo.cBody
				};
				drawInfo.DrawDataCache.Add(item);

				item = new DrawData(Werewolf.BodySecondaryFurTextures[beastColorPlayer.bodySecondaryFurStyle], Position, Frame, adjustedSecondaryFurColor, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = drawInfo.cBody
				};
				drawInfo.DrawDataCache.Add(item);

				item = new DrawData(Werewolf.BodyClawsTextures[beastColorPlayer.bodyClawsStyle], Position, Frame, adjustedClawColor, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = drawInfo.cBody
				};
				drawInfo.DrawDataCache.Add(item);
			}
		}
	}
	public class Legs_Layer : PlayerDrawLayer {
		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			return WolfColorPlayer.enabled && drawInfo.drawPlayer.legs == 20;//doesn't have an ArmorIDs entry?
		}
		public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.Leggings, PlayerDrawLayers.Shoes);
		protected override void Draw(ref PlayerDrawSet drawInfo) {
			Player drawPlayer = drawInfo.drawPlayer;
			WolfColorPlayer beastColorPlayer = drawPlayer.GetModPlayer<WolfColorPlayer>();
			Color adjustedFurColor = drawInfo.colorArmorLegs.MultiplyRGBA(beastColorPlayer.furColor);
			Color adjustedClawColor = drawInfo.colorArmorLegs.MultiplyRGBA(beastColorPlayer.clawsColor);

			Vector2 Position = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - drawPlayer.bodyFrame.Width / 2f + drawPlayer.width / 2f), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f)) + drawPlayer.legPosition + drawInfo.legVect;
			Rectangle? Frame = drawPlayer.legFrame;
			DrawData item = new DrawData(Werewolf.LegsFurTextures[beastColorPlayer.legsFurStyle], Position, Frame, adjustedFurColor, drawPlayer.legRotation, drawInfo.legVect, 1f, drawInfo.playerEffect, 0);
			item.shader = drawPlayer.cLegs;
			drawInfo.DrawDataCache.Add(item);

			item = new DrawData(Werewolf.LegsClawsTextures[beastColorPlayer.legsClawsStyle], Position, Frame, adjustedClawColor, drawPlayer.legRotation, drawInfo.legVect, 1f, drawInfo.playerEffect, 0);
			item.shader = drawPlayer.cLegs;
			drawInfo.DrawDataCache.Add(item);
		}
	}
	//*/
}
