using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.IO;

namespace BeastCustomization {
	public class BeastColorPlayer : ModPlayer {
		[ListRange("HeadFurTextures"), Slider]
		public int headFurStyle = 0;
		[ListRange("HeadTeethTextures"), Slider]
		public int headTeethStyle = 0;
		[ListRange("BodyFurTextures"), Slider]
		public int bodyFurStyle = 0;
		[ListRange("BodySecondaryFurTextures"), Slider]
		public int bodySecondaryFurStyle = 0;
		[ListRange("BodyClawsTextures"), Slider]
		public int bodyClawsStyle = 0;
		[ListRange("LegsFurTextures"), Slider]
		public int legsFurStyle = 0;
		[ListRange("LegsClawsTextures"), Slider]
		public int legsClawsStyle = 0;
		public bool eyesGlow = false;
		public Color eyesIrisColor = new Color(242, 8, 46);
		public Color eyesScleraColor = new Color(241, 241, 241);
		public Color headTeethColor = new Color(227, 232, 238);
		public Color furColor = new Color(220, 153, 107);
		public Color furColor2 = new Color(190, 153, 117);
		public Color clawsColor = new Color(222, 206, 192);

		int oldHeadFurStyle;
		int oldHeadTeethStyle;
		int oldBodyFurStyle;
		int oldBodySecondaryFurStyle;
		int oldBodyClawsStyle;
		int oldLegsFurStyle;
		int oldLegsClawsStyle;
		bool oldEyesGlow;
		Color oldEyesIrisColor;
		Color oldEyesScleraColor;
		Color oldHeadTeethColor;
		Color oldFurColor;
		Color oldFurColor2;
		Color oldClawsColor;
		public void StartCustomization() {
			oldHeadFurStyle = headFurStyle;
			oldHeadTeethStyle = headTeethStyle;
			oldBodyFurStyle = bodyFurStyle;
			oldBodySecondaryFurStyle = bodySecondaryFurStyle;
			oldBodyClawsStyle = bodyClawsStyle;
			oldLegsFurStyle = legsFurStyle;
			oldLegsClawsStyle = legsClawsStyle;
			oldEyesGlow = eyesGlow;
			oldEyesIrisColor = eyesIrisColor;
			oldEyesScleraColor = eyesScleraColor;
			oldHeadTeethColor = headTeethColor;
			oldFurColor = furColor;
			oldFurColor2 = furColor2;
			oldClawsColor = clawsColor;
		}
		public void FinishCustomization(bool overwrite) {
			if (!overwrite) {
				headFurStyle = oldHeadFurStyle;
				headTeethStyle = oldHeadTeethStyle;
				bodyFurStyle = oldBodyFurStyle;
				bodySecondaryFurStyle = oldBodySecondaryFurStyle;
				bodyClawsStyle = oldBodyClawsStyle;
				legsFurStyle = oldLegsFurStyle;
				legsClawsStyle = oldLegsClawsStyle;
				eyesGlow = oldEyesGlow;
				eyesIrisColor = oldEyesIrisColor;
				eyesScleraColor = oldEyesScleraColor;
				headTeethColor = oldHeadTeethColor;
				furColor = oldFurColor;
				furColor2 = oldFurColor2;
				clawsColor = oldClawsColor;
			}
		}
		int GetSlot(int slotNum){
			switch (slotNum) {
				case 0:
				if (Player.armor[10].headSlot >= 0) {
					return Player.armor[10].headSlot;
				}
				return Player.armor[0].headSlot;

				case 1:
				if (Player.armor[11].bodySlot >= 0) {
					return Player.armor[11].bodySlot;
				}
				return Player.armor[1].bodySlot;

				case 2:
				if (Player.armor[12].legSlot >= 0) {
					return Player.armor[12].legSlot;
				}
				return Player.armor[2].legSlot;

				default:
				return -1;
			}
		}
		public override void SaveData(TagCompound tag) {
			tag["headFurStyle"] = headFurStyle;
			tag["headTeethStyle"] = headTeethStyle;
			tag["bodyFurStyle"] = bodyFurStyle;
			tag["bodySecondaryFurStyle"] = bodySecondaryFurStyle;
			tag["bodyClawsStyle"] = bodyClawsStyle;
			tag["legsFurStyle"] = legsFurStyle;
			tag["legsClawsStyle"] = legsClawsStyle;
			tag["eyesGlow"] = eyesGlow;
			tag["eyesIrisColor"] = eyesIrisColor;
			tag["eyesScleraColor"] = eyesScleraColor;
			tag["headTeethColor"] = headTeethColor;
			tag["furColor"] = furColor;
			tag["furColor2"] = furColor2;
			tag["clawsColor"] = clawsColor;
		}
		public override void LoadData(TagCompound tag) {
			if(tag.TryGet("headFurStyle", out int tempHeadFurStyle)) headFurStyle = tempHeadFurStyle;
			if(tag.TryGet("headTeethStyle", out int tempHeadTeethStyle)) headTeethStyle = tempHeadTeethStyle;
			if(tag.TryGet("bodyFurStyle", out int tempBodyFurStyle)) bodyFurStyle = tempBodyFurStyle;
			if(tag.TryGet("bodySecondaryFurStyle", out int tempBodySecondaryFurStyle)) bodySecondaryFurStyle = tempBodySecondaryFurStyle;
			if(tag.TryGet("bodyClawsStyle", out int tempBodyClawsStyle)) bodyClawsStyle = tempBodyClawsStyle;
			if(tag.TryGet("legsFurStyle", out int tempLegsFurStyle)) legsFurStyle = tempLegsFurStyle;
			if(tag.TryGet("legsClawsStyle", out int tempLegsClawsStyle)) legsClawsStyle = tempLegsClawsStyle;
			if(tag.TryGet("eyesGlow", out bool tempEyesGlow)) eyesGlow = tempEyesGlow;
			if(tag.TryGet("eyesIrisColor", out Color tempEyesIrisColor)) eyesIrisColor = tempEyesIrisColor;
			if(tag.TryGet("eyesScleraColor", out Color tempEyesScleraColor)) eyesScleraColor = tempEyesScleraColor;
			if(tag.TryGet("headTeethColor", out Color tempHeadTeethColor)) headTeethColor = tempHeadTeethColor;
			if(tag.TryGet("furColor", out Color tempFurColor)) furColor = tempFurColor;
			if(tag.TryGet("furColor2", out Color tempFurColor2)) furColor2 = tempFurColor2;
			if(tag.TryGet("clawsColor", out Color tempClawsColor)) clawsColor = tempClawsColor;
		}
		internal static bool enabled = true;
		public override void HideDrawLayers(PlayerDrawSet drawInfo) {
			enabled = true;
			if (Terraria.GameInput.PlayerInput.Triggers.JustPressed.Down) Terraria.UI.IngameFancyUI.OpenUIState(new CustomizationMenuState());
			if (!enabled) return;
			if (drawInfo.drawPlayer.head == ArmorIDs.Head.Werewolf) {
				PlayerDrawLayers.Head.Hide();
			}
			if (drawInfo.drawPlayer.body == ArmorIDs.Body.Werewolf) {
				PlayerDrawLayers.Torso.Hide();
				PlayerDrawLayers.ArmOverItem.Hide();
			}
			if (drawInfo.drawPlayer.legs == 20) {
				PlayerDrawLayers.Leggings.Hide();
			}
		}
	}

	public class Head_Layer : PlayerDrawLayer {
		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			return BeastColorPlayer.enabled && drawInfo.drawPlayer.head == ArmorIDs.Head.Werewolf;
		}
		public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.Head, PlayerDrawLayers.FaceAcc);
		protected override void Draw(ref PlayerDrawSet drawInfo) {
			Player drawPlayer = drawInfo.drawPlayer;
			BeastColorPlayer beastColorPlayer = drawPlayer.GetModPlayer<BeastColorPlayer>();
			Color adjustedFurColor = drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.furColor);
			Color adjustedIrisColor = beastColorPlayer.eyesGlow ? beastColorPlayer.eyesIrisColor : drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.eyesIrisColor);
			Color adjustedScleraColor = beastColorPlayer.eyesGlow ? beastColorPlayer.eyesScleraColor : drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.eyesScleraColor);
			Color adjustedTeethColor = drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.headTeethColor);

			Vector2 Position = new Vector2((int)(drawInfo.Position.X + drawPlayer.width / 2f - drawPlayer.bodyFrame.Width / 2f - Main.screenPosition.X), (int)(drawInfo.Position.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f - Main.screenPosition.Y)) + drawPlayer.headPosition + drawInfo.headVect;
			Rectangle? Frame = drawPlayer.bodyFrame;
			DrawData item = new(BeastCustomization.HeadFurTextures[beastColorPlayer.headFurStyle], Position, Frame, adjustedFurColor, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0) {
				shader = GameShaders.Armor.GetShaderIdFromItemId(drawPlayer.dye[0].type)
			};
			drawInfo.DrawDataCache.Add(item);

			item = new(BeastCustomization.HeadTeethTextures[beastColorPlayer.headTeethStyle], Position, Frame, adjustedTeethColor, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0) {
				shader = GameShaders.Armor.GetShaderIdFromItemId(drawPlayer.dye[0].type)
			};
			drawInfo.DrawDataCache.Add(item);

			item = new(BeastCustomization.EyesScleraTexture, Position, Frame, adjustedScleraColor, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0) {
				shader = GameShaders.Armor.GetShaderIdFromItemId(drawPlayer.dye[0].type)
			};
			drawInfo.DrawDataCache.Add(item);

			item = new(BeastCustomization.EyesIrisTexture, Position, Frame, adjustedIrisColor, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0) {
				shader = GameShaders.Armor.GetShaderIdFromItemId(drawPlayer.dye[0].type)
			};
			drawInfo.DrawDataCache.Add(item);
		}
	}
	public class Body_Layer : PlayerDrawLayer {
		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			return BeastColorPlayer.enabled && drawInfo.drawPlayer.body == ArmorIDs.Body.Werewolf;
		}
		public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.Torso, PlayerDrawLayers.OffhandAcc);
		protected override void Draw(ref PlayerDrawSet drawInfo) {
			Player drawPlayer = drawInfo.drawPlayer;
			BeastColorPlayer beastColorPlayer = drawPlayer.GetModPlayer<BeastColorPlayer>();
			Color adjustedFurColor = drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.furColor);
			Color adjustedClawColor = drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.clawsColor);
			if (drawInfo.usesCompositeTorso) {
				Rectangle Frame = drawInfo.compTorsoFrame;

				Vector2 Position = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (drawInfo.drawPlayer.bodyFrame.Width / 2) + (drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.bodyFrame.Width / 2, drawInfo.drawPlayer.bodyFrame.Height / 2);
				Vector2 value = Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height];
				value.Y -= 2f;
				Position += value * -drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically).ToDirectionInt();

				DrawData item = new DrawData(BeastCustomization.BodyFurTextures[beastColorPlayer.bodyFurStyle], Position, Frame, adjustedFurColor, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = GameShaders.Armor.GetShaderIdFromItemId(drawPlayer.dye[1].type)
				};
				drawInfo.DrawDataCache.Add(item);

				item = new DrawData(BeastCustomization.BodySecondaryFurTextures[beastColorPlayer.bodySecondaryFurStyle], Position, Frame, adjustedClawColor, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = GameShaders.Armor.GetShaderIdFromItemId(drawPlayer.dye[1].type)
				};
				drawInfo.DrawDataCache.Add(item);

				item = new DrawData(BeastCustomization.BodyClawsTextures[beastColorPlayer.bodyClawsStyle], Position, Frame, adjustedClawColor, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = GameShaders.Armor.GetShaderIdFromItemId(drawPlayer.dye[1].type)
				};
				drawInfo.DrawDataCache.Add(item);
			} else {
				Rectangle Frame = drawPlayer.bodyFrame;

				Vector2 Position = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - Frame.Width / 2f + drawPlayer.width / 2f), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawPlayer.height - Frame.Height + 4f)) + drawPlayer.bodyPosition + drawInfo.bodyVect;
				DrawData item = new DrawData(BeastCustomization.BodyFurTextures[beastColorPlayer.bodyFurStyle], Position, Frame, adjustedFurColor, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = GameShaders.Armor.GetShaderIdFromItemId(drawPlayer.dye[1].type)
				};
				drawInfo.DrawDataCache.Add(item);

				item = new DrawData(BeastCustomization.BodySecondaryFurTextures[beastColorPlayer.bodySecondaryFurStyle], Position, Frame, adjustedClawColor, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = GameShaders.Armor.GetShaderIdFromItemId(drawPlayer.dye[1].type)
				};
				drawInfo.DrawDataCache.Add(item);

				item = new DrawData(BeastCustomization.BodyClawsTextures[beastColorPlayer.bodyClawsStyle], Position, Frame, adjustedClawColor, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = GameShaders.Armor.GetShaderIdFromItemId(drawPlayer.dye[1].type)
				};
				drawInfo.DrawDataCache.Add(item);
			}
		}
	}
	public class Arm_Layer : PlayerDrawLayer {
		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			return BeastColorPlayer.enabled && drawInfo.drawPlayer.body == ArmorIDs.Body.Werewolf;
		}
		public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.ArmOverItem, PlayerDrawLayers.HandOnAcc);
		protected override void Draw(ref PlayerDrawSet drawInfo) {
			Player drawPlayer = drawInfo.drawPlayer;
			BeastColorPlayer beastColorPlayer = drawPlayer.GetModPlayer<BeastColorPlayer>();
			Color adjustedFurColor = drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.furColor);
			Color adjustedClawColor = drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.clawsColor);
			if (drawInfo.usesCompositeTorso) {
				Vector2 position = new Vector2((int)(drawInfo.Position.X - (drawInfo.drawPlayer.bodyFrame.Width / 2) + (drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.bodyPosition + new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2) - Main.screenPosition;
				Vector2 offset = Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height];
				offset.Y -= 2f;
				position += offset * -drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically).ToDirectionInt();
				position += drawInfo.frontShoulderOffset;
				DrawData drawData = new DrawData(BeastCustomization.BodyFurTextures[beastColorPlayer.bodyFurStyle], position, drawInfo.compFrontShoulderFrame, adjustedFurColor, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = drawInfo.cBody
				};
				drawInfo.DrawDataCache.Add(drawData);

				drawData = new DrawData(BeastCustomization.BodySecondaryFurTextures[beastColorPlayer.bodySecondaryFurStyle], position, drawInfo.compFrontShoulderFrame, adjustedClawColor, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = drawInfo.cBody
				};
				drawInfo.DrawDataCache.Add(drawData);

				drawData = new DrawData(BeastCustomization.BodyClawsTextures[beastColorPlayer.bodyClawsStyle], position, drawInfo.compFrontShoulderFrame, adjustedClawColor, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = drawInfo.cBody
				};
				drawInfo.DrawDataCache.Add(drawData);
			} else {
				Rectangle Frame = drawPlayer.bodyFrame;

				Vector2 Position = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - Frame.Width / 2f + drawPlayer.width / 2f), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawPlayer.height - Frame.Height + 4f)) + drawPlayer.bodyPosition + drawInfo.bodyVect;
				DrawData item = new DrawData(BeastCustomization.BodyFurTextures[beastColorPlayer.bodyFurStyle], Position, Frame, adjustedFurColor, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = drawInfo.cBody
				};
				drawInfo.DrawDataCache.Add(item);

				item = new DrawData(BeastCustomization.BodySecondaryFurTextures[beastColorPlayer.bodySecondaryFurStyle], Position, Frame, adjustedClawColor, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = drawInfo.cBody
				};
				drawInfo.DrawDataCache.Add(item);

				item = new DrawData(BeastCustomization.BodyClawsTextures[beastColorPlayer.bodyClawsStyle], Position, Frame, adjustedClawColor, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
					shader = drawInfo.cBody
				};
				drawInfo.DrawDataCache.Add(item);
			}
		}
	}
	public class Legs_Layer : PlayerDrawLayer {
		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			return BeastColorPlayer.enabled && drawInfo.drawPlayer.legs == 20;//doesn't have an ArmorIDs entry?
		}
		public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.Leggings, PlayerDrawLayers.Shoes);
		protected override void Draw(ref PlayerDrawSet drawInfo) {
			Player drawPlayer = drawInfo.drawPlayer;
			BeastColorPlayer beastColorPlayer = drawPlayer.GetModPlayer<BeastColorPlayer>();
			Color adjustedFurColor = drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.furColor);
			Color adjustedClawColor = drawInfo.colorArmorBody.MultiplyRGBA(beastColorPlayer.clawsColor);

			Vector2 Position = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - drawPlayer.bodyFrame.Width / 2f + drawPlayer.width / 2f), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f)) + drawPlayer.legPosition + drawInfo.legVect;
			Rectangle? Frame = drawPlayer.legFrame;
			DrawData item = new DrawData(BeastCustomization.LegsFurTextures[beastColorPlayer.legsFurStyle], Position, Frame, adjustedFurColor, drawPlayer.legRotation, drawInfo.legVect, 1f, drawInfo.playerEffect, 0);
			item.shader = GameShaders.Armor.GetShaderIdFromItemId(drawPlayer.dye[2].type);
			drawInfo.DrawDataCache.Add(item);

			item = new DrawData(BeastCustomization.LegsClawsTextures[beastColorPlayer.legsClawsStyle], Position, Frame, adjustedClawColor, drawPlayer.legRotation, drawInfo.legVect, 1f, drawInfo.playerEffect, 0);
			item.shader = GameShaders.Armor.GetShaderIdFromItemId(drawPlayer.dye[2].type);
			drawInfo.DrawDataCache.Add(item);
		}
	}
}
