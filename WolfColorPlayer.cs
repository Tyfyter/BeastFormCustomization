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

		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.eyesDye")]
		public bool eyesDye = false;

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

		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.primaryHairDye")]
		[CustomModConfigItem(typeof(HairDyeConfigElement))]
		public Item primaryHairDye = new();

		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.secondaryHairDye")]
		[CustomModConfigItem(typeof(HairDyeConfigElement))]
		public Item secondaryHairDye = new();

		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.eyesIrisColor")]
		public Color eyesIrisColor = new Color(242, 8, 46);

		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.eyesScleraColor")]
		public Color eyesScleraColor = new Color(241, 241, 241);

		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.headTeethColor")]
		public Color headTeethColor = new Color(227, 232, 238);

		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.furColor")]
		public Color furColor = new Color(220, 153, 107);

		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.furColor2")]
		public Color furColor2 = new Color(190, 153, 117);

		[LabelKey("$Mods.BeastCustomization.Forms.WolfColorPlayer.clawsColor")]
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
