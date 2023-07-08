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
		[Label("Head Fur Style")]
		[ListRange("HeadFurTextures"), Slider]
		public int headFurStyle = 1;

		[Label("Head Secondary Fur Style")]
		[ListRange("HeadSecondaryFurTextures"), Slider]
		public int headSecondaryFurStyle = 0;

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

		[Label("Legs Secondary Fur Style")]
		[ListRange("LegsSecondaryFurTextures"), Slider]
		public int legsSecondaryFurStyle = 1;

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

		[Label("Primary Fur Hair Dye")]
		[CustomModConfigItem(typeof(HairDyeConfigElement))]
		public Item primaryHairDye = new();

		[Label("Secondary Fur Hair Dye")]
		[CustomModConfigItem(typeof(HairDyeConfigElement))]
		public Item secondaryHairDye = new();

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

		[JsonIgnore]
		public Color FurColor {
			get {
				if (primaryHairDye is not null && primaryHairDye.hairDye > -1) {
					return GameShaders.Hair.GetColor(primaryHairDye.hairDye, Player, Color.White);
				}
				return furColor;
			}
		}
		[JsonIgnore]
		public Color FurColor2 {
			get {
				if (secondaryHairDye is not null && secondaryHairDye.hairDye > -1) {
					return GameShaders.Hair.GetColor(secondaryHairDye.hairDye, Player, Color.White);
				}
				return furColor2;
			}
		}

		TagCompound oldData;
		#endregion fields
		public override string DisplayName => "Werewolf";
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
		public override void ExportData(TagCompound tag) {
			tag["headFurStyle"] = headFurStyle;
			tag["headSecondaryFurStyle"] = headSecondaryFurStyle;
			tag["headTeethStyle"] = headTeethStyle;

			tag["bodyFurStyle"] = bodyFurStyle;
			tag["bodySecondaryFurStyle"] = bodySecondaryFurStyle;
			tag["bodyClawsStyle"] = bodyClawsStyle;

			tag["legsFurStyle"] = legsFurStyle;
			tag["legsSecondaryFurStyle"] = legsSecondaryFurStyle;
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
			tag["applyCloaks"] = applyCloaks;
			tag["applyLegs"] = applyLegs;
			tag["applyHeadOver"] = applyHeadOver;
			tag["applyBodyOver"] = applyBodyOver;
			tag["applyLegsOver"] = applyLegsOver;
			tag["primaryHairDye"] = primaryHairDye;
			tag["secondaryHairDye"] = secondaryHairDye;
		}
		public override void ImportData(TagCompound tag) {
			if (tag.TryGet("headFurStyle", out int _headFurStyle)) headFurStyle = _headFurStyle;
			if (tag.TryGet("headSecondaryFurStyle", out int _headSecondaryFurStyle)) headSecondaryFurStyle = _headSecondaryFurStyle;
			if (tag.TryGet("headTeethStyle", out int _headTeethStyle)) headTeethStyle = _headTeethStyle;

			if (tag.TryGet("bodyFurStyle", out int _bodyFurStyle)) bodyFurStyle = _bodyFurStyle;
			if (tag.TryGet("bodySecondaryFurStyle", out int _bodySecondaryFurStyle)) bodySecondaryFurStyle = _bodySecondaryFurStyle;
			if (tag.TryGet("bodyClawsStyle", out int _bodyClawsStyle)) bodyClawsStyle = _bodyClawsStyle;

			if (tag.TryGet("legsFurStyle", out int _legsFurStyle)) legsFurStyle = _legsFurStyle;
			if (tag.TryGet("legsSecondaryFurStyle", out int _legsSecondaryFurStyle)) legsSecondaryFurStyle = _legsSecondaryFurStyle;
			if (tag.TryGet("legsClawsStyle", out int _legsClawsStyle)) legsClawsStyle = _legsClawsStyle;

			if (tag.TryGet("eyesGlow", out bool _eyesGlow)) eyesGlow = _eyesGlow;
			if (tag.TryGet("eyesDye", out bool _eyesDye)) eyesDye = _eyesDye;

			if (tag.TryGet("eyesIrisColor", out Color _eyesIrisColor)) eyesIrisColor = _eyesIrisColor;
			if (tag.TryGet("eyesScleraColor", out Color _eyesScleraColor)) eyesScleraColor = _eyesScleraColor;
			if (tag.TryGet("headTeethColor", out Color _headTeethColor)) headTeethColor = _headTeethColor;
			if (tag.TryGet("furColor", out Color _furColor)) furColor = _furColor;
			if (tag.TryGet("furColor2", out Color _furColor2)) furColor2 = _furColor2;
			if (tag.TryGet("clawsColor", out Color _clawsColor)) clawsColor = _clawsColor;

			if (tag.TryGet("applyHead", out bool _applyHead)) applyHead = _applyHead;
			if (tag.TryGet("applyBody", out bool _applyBody)) applyBody = _applyBody;
			if (tag.TryGet("applyCloaks", out bool _applyCloaks)) applyCloaks = _applyCloaks;
			if (tag.TryGet("applyLegs", out bool _applyLegs)) applyLegs = _applyLegs;
			if (tag.TryGet("applyHeadOver", out bool _applyHeadOver)) applyHeadOver = _applyHeadOver;
			if (tag.TryGet("applyBodyOver", out bool _applyBodyOver)) applyBodyOver = _applyBodyOver;
			if (tag.TryGet("applyCloakOver", out bool _applyCloakOver)) applyCloaks = _applyCloakOver;
			if (tag.TryGet("applyLegsOver", out bool _applyLegsOver)) applyLegsOver = _applyLegsOver;
			if (tag.TryGet("primaryHairDye", out Item _primaryHairDye)) primaryHairDye = _primaryHairDye;
			if (tag.TryGet("secondaryHairDye", out Item _secondaryHairDye)) secondaryHairDye = _secondaryHairDye;
			//Mod.Logger.Info($"loading {Player.name}, fur color: {furColor}");
		}
		public override void NetSend(BinaryWriter writer) {
			writer.Write(headFurStyle);
			writer.Write(headSecondaryFurStyle);
			writer.Write(headTeethStyle);

			writer.Write(bodyFurStyle);
			writer.Write(bodySecondaryFurStyle);
			writer.Write(bodyClawsStyle);

			writer.Write(legsFurStyle);
			writer.Write(legsSecondaryFurStyle);
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
			writer.Write(applyCloaks);
			writer.Write(applyLegs);
			writer.Write(applyHeadOver);
			writer.Write(applyBodyOver);
			writer.Write(applyLegsOver);
			Reflection.BinaryWriterWriteItem(writer, primaryHairDye);
			Reflection.BinaryWriterWriteItem(writer, secondaryHairDye);
		}
		public override void NetRecieve(BinaryReader reader) {
			headFurStyle = reader.ReadInt32();
			headSecondaryFurStyle = reader.ReadInt32();
			headTeethStyle = reader.ReadInt32();

			bodyFurStyle = reader.ReadInt32();
			bodySecondaryFurStyle = reader.ReadInt32();
			bodyClawsStyle = reader.ReadInt32();

			legsFurStyle = reader.ReadInt32();
			legsSecondaryFurStyle = reader.ReadInt32();
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
			applyCloaks = reader.ReadBoolean();
			applyLegs = reader.ReadBoolean();
			applyHeadOver = reader.ReadBoolean();
			applyBodyOver = reader.ReadBoolean();
			applyLegsOver = reader.ReadBoolean();
			primaryHairDye = Reflection.BinaryReaderReadItem(reader);
			secondaryHairDye = Reflection.BinaryReaderReadItem(reader);
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
			int hairDye = Player.hairDye;
			try {
				if (primaryHairDye is not null) {
					Player.hairDye = primaryHairDye.hairDye;
					Player.UpdateHairDyeDust();
				}
			} catch (Exception) { }
			try {
				if (secondaryHairDye is not null) {
					Player.hairDye = secondaryHairDye.hairDye;
					Player.UpdateHairDyeDust();
				}
			} catch (Exception) { }
			Player.hairDye = hairDye;
		}
	}
	public class Werewolf_Head_Layer : GenericHeadLayer {
		public override Type BoundBeastPlayer => typeof(WolfColorPlayer);
		public override IEnumerable<(Texture2D texture, Color color, bool applyDye)> GetData(PlayerDrawSet drawInfo) {
			WolfColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<WolfColorPlayer>();
			yield return (
				Werewolf.HeadFurTextures[beastColorPlayer.headFurStyle],
				drawInfo.colorArmorHead.MultiplyRGBA(beastColorPlayer.FurColor),
				true
			);
			yield return (
				Werewolf.HeadSecondaryFurTextures[beastColorPlayer.headSecondaryFurStyle],
				drawInfo.colorArmorHead.MultiplyRGBA(beastColorPlayer.FurColor2),
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
		public override Type BoundBeastPlayer => typeof(WolfColorPlayer);
		public override IEnumerable<(Texture2D texture, Color color)> GetData(PlayerDrawSet drawInfo) {
			WolfColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<WolfColorPlayer>();
			yield return (
				Werewolf.BodyFurTextures[beastColorPlayer.bodyFurStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.FurColor)
			);
			yield return (
				Werewolf.BodySecondaryFurTextures[beastColorPlayer.bodySecondaryFurStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.FurColor2)
			);
			yield return (
				Werewolf.BodyClawsTextures[beastColorPlayer.bodyClawsStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.clawsColor)
			);
			if (beastColorPlayer.applyBodyOver) {
				int slot = beastColorPlayer.GetSlot(1);
				if (slot > 0) {
					if (slot < ArmorIDs.Body.Count) {
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
	public class Werewolf_Arm_Layer_Back : GenericArmLayer_Back {
		public override Type BoundBeastPlayer => typeof(WolfColorPlayer);
		public override IEnumerable<(Texture2D texture, Color color)> GetData(PlayerDrawSet drawInfo) {
			WolfColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<WolfColorPlayer>();
			yield return (
				Werewolf.BodyFurTextures[beastColorPlayer.bodyFurStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.FurColor)
			);
			yield return (
				Werewolf.BodySecondaryFurTextures[beastColorPlayer.bodySecondaryFurStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.FurColor2)
			);
			yield return (
				Werewolf.BodyClawsTextures[beastColorPlayer.bodyClawsStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.clawsColor)
			);
			if (beastColorPlayer.applyBodyOver) {
				int slot = beastColorPlayer.GetSlot(1);
				if (slot > 0) {
					if (slot < ArmorIDs.Body.Count) {
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
		public override Type BoundBeastPlayer => typeof(WolfColorPlayer);
		public override IEnumerable<(Texture2D texture, Color color)> GetData(PlayerDrawSet drawInfo) {
			WolfColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<WolfColorPlayer>();
			yield return (
				Werewolf.BodyFurTextures[beastColorPlayer.bodyFurStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.FurColor)
			);
			yield return (
				Werewolf.BodySecondaryFurTextures[beastColorPlayer.bodySecondaryFurStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.FurColor2)
			);
			yield return (
				Werewolf.BodyClawsTextures[beastColorPlayer.bodyClawsStyle],
				drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.clawsColor)
			);
			if (beastColorPlayer.applyBodyOver) {
				int slot = beastColorPlayer.GetSlot(1);
				if (slot > 0) {
					if (slot < ArmorIDs.Body.Count) {
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
		public override Type BoundBeastPlayer => typeof(WolfColorPlayer);
		public override IEnumerable<(Texture2D texture, Color color)> GetData(PlayerDrawSet drawInfo) {
			WolfColorPlayer beastColorPlayer = drawInfo.drawPlayer.GetModPlayer<WolfColorPlayer>();
			yield return (
				Werewolf.LegsFurTextures[beastColorPlayer.legsFurStyle],
				drawInfo.colorArmorLegs.MultiplyRGBA(beastColorPlayer.FurColor)
			);
			yield return (
				Werewolf.LegsSecondaryFurTextures[beastColorPlayer.legsSecondaryFurStyle],
				drawInfo.colorArmorLegs.MultiplyRGBA(beastColorPlayer.FurColor2)
			);
			yield return (
				Werewolf.LegsClawsTextures[beastColorPlayer.legsClawsStyle],
				drawInfo.colorArmorLegs.MultiplyRGBA(beastColorPlayer.clawsColor)
			);
			if (beastColorPlayer.applyLegsOver) {
				int slot = beastColorPlayer.GetSlot(2);
				if (slot > 0) {
					if (slot < ArmorIDs.Legs.Count) {
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
