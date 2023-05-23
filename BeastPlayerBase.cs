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
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BeastCustomization {
	public abstract class BeastPlayerBase : ModPlayer {
		public ushort ModIndex { get; private set; }
		public abstract string DisplayName { get; }
		public virtual bool IsActive => false;
		public virtual int Specificity => 0;
		public override ModPlayer NewInstance(Player entity) {
			ModPlayer modPlayer = base.NewInstance(entity);
			(modPlayer as BeastPlayerBase).ModIndex = ModIndex;
			return modPlayer;
		}
		public sealed override void SetStaticDefaults() {
			ModIndex = (ushort)BeastCustomization.BeastPlayers.Count;
			BeastCustomization.BeastPlayers.Add(this.Index);
			SetBeastPlayerStaticDefaults();
		}
		public virtual void SetBeastPlayerStaticDefaults() { }
		List<TagCompound> _presets;
		[JsonIgnore]
		public List<TagCompound> Presets {
			get => _presets ??= new();
			set => _presets = value ?? new();
		}
		/// <summary>
		/// should always just return a new instance of the type
		/// </summary>
		public abstract BeastPlayerBase CreateNew();
		/// <summary>
		/// the type where resource lists will be found to get slider lengths
		/// </summary>
		public abstract Type ResourceCacheType { get; }
		/// <summary>
		/// should directly return a reference to the List<TagCompound> which is used to save presets in the config
		/// </summary>
		public abstract ref List<TagCompound> ConfigPresets { get; }
		public abstract void StartCustomization();
		public abstract void FinishCustomization(bool overwrite);
		public abstract void ExportData(TagCompound tag);
		public abstract void ImportData(TagCompound tag);
		public abstract void NetSend(BinaryWriter writer);
		public abstract void NetRecieve(BinaryReader reader);
		public sealed override void SaveData(TagCompound tag) {
			ExportData(tag);
			tag["SavedPresets"] = Presets;
		}
		public sealed override void LoadData(TagCompound tag) {
			ImportData(tag);
			if (tag.TryGet("SavedPresets", out List<TagCompound> tempPresets)) Presets = tempPresets;
		}
		bool initialized = true;
		public override void ResetEffects() {
			if (!initialized) {
				initialized = true;
				if (Player.whoAmI == Main.myPlayer) {
					SendData();
				} else {
					WolfColorPlayer myBeastColorPlayer = Main.LocalPlayer.GetModPlayer<WolfColorPlayer>();
					myBeastColorPlayer.SendData((short)Player.whoAmI);
				}
			}
		}
		public override void OnEnterWorld(Player player) {
			SendData();
			initialized = false;
		}
		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) {
			if (newPlayer) {
				SendData();
			} else if (fromWho == -1) {
				ModPacket packet = Mod.GetPacket();
				packet.Write((byte)1);
				packet.Write((short)toWho);
				packet.Write(ModIndex);
				BeastCustomization.DebugLogger.Info("SyncPlayer");
				BeastCustomization.DebugLogger.Info(packet.BaseStream.Position);
				packet.Send(Player.whoAmI, -1);
			}
		}
		internal void SendData(short toWho = -1) {
			if (Main.netMode == NetmodeID.SinglePlayer) return;
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)0);
			packet.Write((short)toWho);
			packet.Write(ModIndex);
			NetSend(packet);
			BeastCustomization.DebugLogger.Info("SendData");
			BeastCustomization.DebugLogger.Info(packet.BaseStream.Position);
			packet.Send(toWho, Player.whoAmI);
		}
		public int GetSlot(int slotNum) {
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
	}

	public abstract class GenericHeadLayer : PlayerDrawLayer {
		public override bool IsHeadLayer => true;
		public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.Head, PlayerDrawLayers.FaceAcc);
		protected override void Draw(ref PlayerDrawSet drawInfo) {
			Player drawPlayer = drawInfo.drawPlayer;
			Vector2 Position = new Vector2((int)(drawInfo.Position.X + drawPlayer.width / 2f - drawPlayer.bodyFrame.Width / 2f - Main.screenPosition.X), (int)(drawInfo.Position.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f - Main.screenPosition.Y)) + drawPlayer.headPosition + drawInfo.headVect;
			Rectangle? Frame = drawPlayer.bodyFrame;

			foreach (var data in GetData(drawInfo)) {
				DrawData item = new(data.texture, Position, Frame, data.color, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0);
				if (data.applyDye) item.shader = drawPlayer.cHead;
				drawInfo.DrawDataCache.Add(item);
			}
		}
		public abstract IEnumerable<(Texture2D texture, Color color, bool applyDye)> GetData(PlayerDrawSet drawInfo);
	}
	public abstract class GenericBodyLayer : PlayerDrawLayer {
		public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.Torso, PlayerDrawLayers.OffhandAcc);
		protected override void Draw(ref PlayerDrawSet drawInfo) {
			Player drawPlayer = drawInfo.drawPlayer;
			if (drawInfo.usesCompositeTorso) {
				Rectangle Frame = drawInfo.compTorsoFrame;

				Vector2 Position = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (drawInfo.drawPlayer.bodyFrame.Width / 2) + (drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.bodyFrame.Width / 2, drawInfo.drawPlayer.bodyFrame.Height / 2);
				Vector2 value = Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height];
				value.Y -= 2f;
				Position += value * -drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically).ToDirectionInt();

				foreach (var data in GetData(drawInfo)) {
					DrawData item = new DrawData(data.texture, Position, Frame, data.color, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
						shader = drawPlayer.cBody
					};
					drawInfo.DrawDataCache.Add(item);
				}
			} else {
				Rectangle Frame = drawPlayer.bodyFrame;

				Vector2 Position = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - Frame.Width / 2f + drawPlayer.width / 2f), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawPlayer.height - Frame.Height + 4f)) + drawPlayer.bodyPosition + drawInfo.bodyVect;
				foreach (var data in GetData(drawInfo)) {
					DrawData item = new DrawData(data.texture, Position, Frame, data.color, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
						shader = drawPlayer.cBody
					};
					drawInfo.DrawDataCache.Add(item);
				}
			}
		}
		public abstract IEnumerable<(Texture2D texture, Color color)> GetData(PlayerDrawSet drawInfo);
	}
	public abstract class GenericArmLayer_Back : PlayerDrawLayer {
		public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.Skin, PlayerDrawLayers.Leggings);
		protected override void Draw(ref PlayerDrawSet drawInfo) {
			Player drawPlayer = drawInfo.drawPlayer;
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
				foreach (var data in GetData(drawInfo)) {
					DrawData drawData = new DrawData(data.texture, position, drawInfo.compBackArmFrame, data.color, rotation, bodyVect, 1f, drawInfo.playerEffect, 0) {
						shader = drawInfo.cBody
					};
					drawInfo.DrawDataCache.Add(drawData);
				}
			} else {
				Rectangle Frame = drawPlayer.bodyFrame;

				Vector2 Position = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - Frame.Width / 2f + drawPlayer.width / 2f), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawPlayer.height - Frame.Height + 4f)) + drawPlayer.bodyPosition + drawInfo.bodyVect;
				foreach (var data in GetData(drawInfo)) {
					DrawData item = new DrawData(data.texture, Position, Frame, data.color, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
						shader = drawInfo.cBody
					};
					drawInfo.DrawDataCache.Add(item);
				}
			}
		}
		public abstract IEnumerable<(Texture2D texture, Color color)> GetData(PlayerDrawSet drawInfo);
	}
	public abstract class GenericArmLayer_Front : PlayerDrawLayer {
		public sealed override Position GetDefaultPosition() => new Between(PlayerDrawLayers.ArmOverItem, PlayerDrawLayers.HandOnAcc);
		protected sealed override void Draw(ref PlayerDrawSet drawInfo) {
			Player drawPlayer = drawInfo.drawPlayer;
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
				foreach (var data in GetData(drawInfo)) {
					DrawData drawData = new DrawData(data.texture, position, drawInfo.compFrontArmFrame, data.color, rotation, bodyVect, 1f, drawInfo.playerEffect, 0) {
						shader = drawInfo.cBody
					};
					drawInfo.DrawDataCache.Add(drawData);
				}
			} else {
				Rectangle Frame = drawPlayer.bodyFrame;

				Vector2 Position = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - Frame.Width / 2f + drawPlayer.width / 2f), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawPlayer.height - Frame.Height + 4f)) + drawPlayer.bodyPosition + drawInfo.bodyVect;

				foreach (var data in GetData(drawInfo)) {
					DrawData item = new DrawData(data.texture, Position, Frame, data.color, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
						shader = drawInfo.cBody
					};
					drawInfo.DrawDataCache.Add(item);
				}
			}
		}
		public abstract IEnumerable<(Texture2D texture, Color color)> GetData(PlayerDrawSet drawInfo);
	}
	public abstract class GenericLegsLayer : PlayerDrawLayer {
		public sealed override Position GetDefaultPosition() => new Between(PlayerDrawLayers.Leggings, PlayerDrawLayers.Shoes);
		protected sealed override void Draw(ref PlayerDrawSet drawInfo) {
			Player drawPlayer = drawInfo.drawPlayer;

			Vector2 Position = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - drawPlayer.bodyFrame.Width / 2f + drawPlayer.width / 2f), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f)) + drawPlayer.legPosition + drawInfo.legVect;
			Rectangle? Frame = drawPlayer.legFrame;
			foreach (var data in GetData(drawInfo)) {
				DrawData item = new DrawData(data.texture, Position, Frame, data.color, drawPlayer.legRotation, drawInfo.legVect, 1f, drawInfo.playerEffect, 0);
				item.shader = drawPlayer.cLegs;
				drawInfo.DrawDataCache.Add(item);
			}
		}
		public abstract IEnumerable<(Texture2D texture, Color color)> GetData(PlayerDrawSet drawInfo);
	}
}
