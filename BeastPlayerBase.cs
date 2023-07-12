using BeastCustomization.Textures;
using BeastCustomization.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using MonoMod.Cil;
using MonoMod.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BeastCustomization {
	public abstract class BeastPlayerBase : ModPlayer {
		public ushort ModIndex { get; private set; }
		public virtual string DisplayName => $"Mods.{Mod.Name}.Forms.{GetType().Name}.Name";
		public virtual bool IsActive => false;
		public virtual int Specificity => 0;
		public override ModPlayer NewInstance(Player entity) {
			ModPlayer modPlayer = base.NewInstance(entity);
			(modPlayer as BeastPlayerBase).ModIndex = ModIndex;
			return modPlayer;
		}
		public sealed override void SetStaticDefaults() {
			if (!BeastCustomization.BeastPlayersByType.ContainsKey(this.GetType())) {
				this.ModIndex = (ushort)BeastCustomization.BeastPlayers.Count;
				BeastCustomization.BeastPlayers.Add(this.Index);
				BeastCustomization.BeastPlayersByType.Add(this.GetType(), this.ModIndex);
			}
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
		public static Dictionary<Type, Action<BeastPlayerBase, TagCompound>> ExportDatas { get; set; }
		public static Dictionary<Type, Action<BeastPlayerBase, TagCompound>> ImportDatas { get; set; }
		public virtual void ExportData(TagCompound tag) {
			if (!(ExportDatas ??= new()).TryGetValue(GetType(), out var exportFunc)) {
				ExportDatas.Add(GetType(), exportFunc = GenerateExportData(GetType()));
			}
			exportFunc(this, tag);
		}
		public virtual void ImportData(TagCompound tag) {
			if (!(ImportDatas ??= new()).TryGetValue(GetType(), out var importFunc)) {
				ImportDatas.Add(GetType(), importFunc = GenerateImportData(GetType()));
			}
			Version lastVersion = (tag.TryGet("LastVersion", out string versionString) ? Version.Parse(versionString) : new Version());
			UpdateData(tag, lastVersion, out bool warn);
			if (warn && !Main.dedServ) Main.NewText(Language.GetTextValue("Mods.BeastCustomization.OldDataWarning", lastVersion));
			importFunc(this, tag);
		}
		public static Dictionary<Type, Action<BeastPlayerBase, BinaryWriter>> NetSends { get; set; }
		public static Dictionary<Type, Action<BeastPlayerBase, BinaryReader>> NetReceives { get; set; }
		public virtual void NetSend(BinaryWriter writer) {
			if (!(NetSends ??= new()).TryGetValue(GetType(), out var sendFunc)) {
				NetSends.Add(GetType(), sendFunc = GenerateNetSend(GetType()));
			}
			sendFunc(this, writer);
		}
		public virtual void NetReceive(BinaryReader reader) {
			if (!(NetReceives ??= new()).TryGetValue(GetType(), out var receiveFunc)) {
				NetReceives.Add(GetType(), receiveFunc = GenerateNetReceive(GetType()));
			}
			receiveFunc(this, reader);
		}
		public abstract void ApplyVanillaDrawLayers(PlayerDrawSet drawInfo, out bool applyHead, out bool applyBody, out bool applyCloaks, out bool applyLegs);
		public abstract void HideVanillaDrawLayers(PlayerDrawSet drawInfo, out bool hideHead, out bool hideBody, out bool hideLegs);
		public sealed override void SaveData(TagCompound tag) {
			tag.Add("LastVersion", Mod.Version.ToString());
			ExportData(tag);
			tag["SavedPresets"] = Presets;
		}
		public sealed override void LoadData(TagCompound tag) {
			ImportData(tag);
			if (tag.TryGet("SavedPresets", out List<TagCompound> tempPresets)) Presets = tempPresets;
		}
		public virtual void UpdateData(TagCompound tag, Version lastVersion, out bool warn) { warn = false; }
		public void ColorToColorDefinition(TagCompound tag) {
			foreach (var field in GetType().GetFields().Where(f => f.FieldType == typeof(ColorDefinition))) {
				if (tag[field.Name] is TagCompound) 
					continue;
				Item hairDye = null;
				if (field.GetCustomAttribute<OldHairDyeFieldAttribute>() is OldHairDyeFieldAttribute oldHairDyeField) {
					tag.TryGet(oldHairDyeField.FieldName, out hairDye);
				}
				if (tag.TryGet(field.Name, out Color color)) {
					tag[field.Name] = new ColorDefinition(color, hairDye);
				}
			}
		}
		bool initialized = true;
		public override void ResetEffects() {
			if (!initialized) {
				initialized = true;
				if (Player.whoAmI == Main.myPlayer) {
					SendData();
				} else {
					//WolfColorPlayer myBeastColorPlayer = Main.LocalPlayer.GetModPlayer<WolfColorPlayer>();
					SendData((short)Player.whoAmI);
				}
			}
		}
		public override void OnEnterWorld() {
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
		/// <summary>
		/// checks if NetSend and NetReceive properly cover all fields
		/// </summary>
		internal bool CheckSync(bool verbose) {
			return CheckIntegrity((BeastPlayerBase first, BeastPlayerBase second) => {
				MemoryStream stream = new MemoryStream();
				first.NetSend(new BinaryWriter(stream));
				stream.Position = 0;
				second.NetReceive(new BinaryReader(stream));
			}, "[c/FF0000:{0} field {1} not syncing properly]",
			verbose ? "[c/00FF00:{0} field {1} syncing properly]" : null
			);
		}
		/// <summary>
		/// checks if NetSend and NetReceive properly cover all fields
		/// </summary>
		internal bool CheckSave(bool verbose) {
			return CheckIntegrity((BeastPlayerBase first, BeastPlayerBase second) => {
				TagCompound tag = new TagCompound();
				first.ExportData(tag);
				second.ImportData(tag);
			}, "[c/FF0000:{0} field {1} not saving properly]",
			verbose ? "[c/00FF00:{0} field {1} saving properly]" : null
			);
		}
		internal bool CheckIntegrity(Action<BeastPlayerBase, BeastPlayerBase> action, string text, string successText = null) {
			static bool Compare(FieldInfo field, BeastPlayerBase first, BeastPlayerBase second) {
				switch (field.FieldType.Name) {
					case nameof(Item):
					return ((Item)field.GetValue(second))?.type == ((Item)field.GetValue(first))?.type;

					default:
					return field.GetValue(second).Equals(field.GetValue(first));
				}
			}
			bool fullySyncing = true;
			BeastPlayerBase first = CreateNew();
			BeastPlayerBase second = CreateNew();
			var fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach (var field in fields) {
				switch (field.FieldType.Name) {
					case nameof(Int32):
					field.SetValue(second, ~((Int32)field.GetValue(first)));
					break;

					case nameof(Boolean):
					field.SetValue(second, !((Boolean)field.GetValue(first)));
					break;

					case nameof(Item):
					field.SetValue(second, new Item(((((Item)field.GetValue(first))?.type ?? 0) + 1) % 7));
					break;

					case nameof(Color):
					Color changeColor = (Color)field.GetValue(first);
					changeColor.PackedValue = ~changeColor.PackedValue;
					field.SetValue(second, changeColor);
					break;
				}
			}
			foreach (var field in fields) {
				if (Compare(field, first, second)) {
					Main.NewText($"{field.DeclaringType.Name} field {field.Name} not changed properly");
					fullySyncing = false;
				}
			}
			action(first, second);
			foreach (var field in fields) {
				if (!Compare(field, first, second)) {
					Main.NewText(string.Format(text, field.DeclaringType.Name, field.Name));
					fullySyncing = false;
				} else if (successText is not null) {
					Main.NewText(string.Format(successText, field.DeclaringType.Name, field.Name));
				}
			}
			return fullySyncing;
		}
		//*
		protected internal static Action<BeastPlayerBase, BinaryWriter> GenerateNetSend(Type type) {
			if (!type.IsAssignableTo(typeof(BeastPlayerBase))) throw new ArgumentException($"{nameof(type)} must extend {typeof(BeastPlayerBase)}", nameof(type));
			DynamicMethod netSendMethod = new DynamicMethod($"{type.Name}_NetSend", null, new Type[] { typeof(BeastPlayerBase), typeof(BinaryWriter) }, true);
			ILGenerator gen = netSendMethod.GetILGenerator();
			MethodInfo info = typeof(BeastPlayerBase).GetMethod("_testSend", BindingFlags.NonPublic | BindingFlags.Static);

			foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance).OrderBy(f => f.Name)) {
				if (field.GetCustomAttribute<JsonIgnoreAttribute>() is null) {
					if (!Reflection.BinaryWriterWrites.TryGetValue(field.FieldType, out MethodInfo write)) {
						write = typeof(BinaryWriter).GetMethod(
							"Write",
							BindingFlags.Public | BindingFlags.Instance,
							new Type[] { field.FieldType }
						);
						Reflection.BinaryWriterWrites.Add(field.FieldType, write);
					}
					if (write is null) {
						throw new Exception($"Could not find write method for type {field.FieldType}");
					}
					gen.Emit(OpCodes.Ldarg_1);
					gen.Emit(OpCodes.Ldarg_0);
					gen.Emit(OpCodes.Ldfld, field);
					gen.Emit(write.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, write);

					gen.Emit(OpCodes.Ldarg_0);
					gen.Emit(OpCodes.Ldfld, field);
					gen.Emit(OpCodes.Box, field.FieldType);
					gen.Emit(OpCodes.Ldarg_1);
					gen.Emit(OpCodes.Call, info);
				}
			}

			gen.Emit(OpCodes.Ret);

			return netSendMethod.CreateDelegate<Action<BeastPlayerBase, BinaryWriter>>();
		}
		protected internal static Action<BeastPlayerBase, BinaryReader> GenerateNetReceive(Type type) {
			if (!type.IsAssignableTo(typeof(BeastPlayerBase))) throw new ArgumentException($"{nameof(type)} must extend {typeof(BeastPlayerBase)}", nameof(type));
			DynamicMethod netRecieveMethod = new DynamicMethod($"{type.Name}_NetRecieve", null, new Type[] { typeof(BeastPlayerBase), typeof(BinaryReader) }, true);
			ILGenerator gen = netRecieveMethod.GetILGenerator();
			MethodInfo info = typeof(BeastPlayerBase).GetMethod("_testReceive", BindingFlags.NonPublic | BindingFlags.Static);

			foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance).OrderBy(f => f.Name)) {
				if (field.GetCustomAttribute<JsonIgnoreAttribute>() is null) {
					if (!Reflection.BinaryReaderReads.TryGetValue(field.FieldType, out MethodInfo read)) {
						read = typeof(BinaryReader).GetMethods()
							.Where(m => m.ReturnType == field.FieldType && m.Name == $"Read{field.FieldType.Name}")
							.FirstOrDefault();
						Reflection.BinaryReaderReads.Add(field.FieldType, read);
					}
					if (read is null) {
						throw new Exception($"Could not find read method for type {field.FieldType}");
					}
					ParameterInfo[] parameters = read.GetParameters();
					if (read.ReturnType != field.FieldType) {
						throw new Exception($"Invalid read method provided for type {field.FieldType}");
					}
					//gen.Emit(OpCodes.Ldstr, $"1 receiving {field} {read}");

					gen.Emit(OpCodes.Ldarg_0);
					gen.Emit(OpCodes.Ldarg_1);
					gen.Emit(read.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, read);
					gen.Emit(OpCodes.Stfld, field);

					//gen.Emit(OpCodes.Ldstr, $"2 receiving {field} {read}");
					gen.Emit(OpCodes.Ldarg_0);
					gen.Emit(OpCodes.Ldfld, field);
					gen.Emit(OpCodes.Box, field.FieldType);
					gen.Emit(OpCodes.Ldarg_1);
					gen.Emit(OpCodes.Call, info);
				}
			}

			gen.Emit(OpCodes.Ret);

			return netRecieveMethod.CreateDelegate<Action<BeastPlayerBase, BinaryReader>>();
		}
		protected internal static Action<BeastPlayerBase, TagCompound> GenerateExportData(Type type) {
			if (!type.IsAssignableTo(typeof(BeastPlayerBase))) throw new ArgumentException($"{nameof(type)} must extend {typeof(BeastPlayerBase)}", nameof(type));
			DynamicMethod netRecieveMethod = new DynamicMethod($"{type.Name}_NetRecieve", null, new Type[] { typeof(BeastPlayerBase), typeof(TagCompound) }, true);
			ILGenerator gen = netRecieveMethod.GetILGenerator();
			MethodInfo info = typeof(BeastPlayerBase).GetMethod("_testSet", BindingFlags.NonPublic | BindingFlags.Static);
			MethodInfo setItem = typeof(TagCompound).GetMethod("set_Item");

			foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance).OrderBy(f => f.Name)) {
				if (field.GetCustomAttribute<JsonIgnoreAttribute>() is null) {
					gen.Emit(OpCodes.Ldarg_1);
					gen.Emit(OpCodes.Ldstr, field.Name);
					gen.Emit(OpCodes.Ldarg_0);
					gen.Emit(OpCodes.Ldfld, field);
					gen.Emit(OpCodes.Box, field.FieldType);
					gen.Emit(OpCodes.Call, info);
				}
			}

			gen.Emit(OpCodes.Ret);

			return netRecieveMethod.CreateDelegate<Action<BeastPlayerBase, TagCompound>>();
		}
		protected internal static Action<BeastPlayerBase, TagCompound> GenerateImportData(Type type) {
			if (!type.IsAssignableTo(typeof(BeastPlayerBase))) throw new ArgumentException($"{nameof(type)} must extend {typeof(BeastPlayerBase)}", nameof(type));
			DynamicMethod netRecieveMethod = new DynamicMethod($"{type.Name}_NetRecieve", null, new Type[] { typeof(BeastPlayerBase), typeof(TagCompound) }, true);
			ILGenerator gen = netRecieveMethod.GetILGenerator();
			MethodInfo info = typeof(BeastPlayerBase).GetMethod("_testSet", BindingFlags.NonPublic | BindingFlags.Static);
			MethodInfo tryGet = typeof(TagCompound).GetMethod("TryGet");
			Dictionary<Type, (MethodInfo, LocalVariableInfo)> typeData = new();

			foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance).OrderBy(f => f.Name)) {
				if (field.GetCustomAttribute<JsonIgnoreAttribute>() is null) {
					MethodInfo generic;
					LocalVariableInfo localVar;
					if (typeData.TryGetValue(type, out (MethodInfo, LocalVariableInfo) data)) {
						generic = data.Item1;
						localVar = data.Item2;
					} else {
						generic = tryGet.MakeGenericMethod(field.FieldType);
						localVar = gen.DeclareLocal(field.FieldType);
					}
					Label skip = gen.DefineLabel();
					gen.Emit(OpCodes.Ldarg_1);
					gen.Emit(OpCodes.Ldstr, field.Name);
					gen.Emit(OpCodes.Ldloca, localVar.LocalIndex);
					gen.Emit(OpCodes.Callvirt, generic);
					gen.Emit(OpCodes.Brfalse, skip);
					gen.Emit(OpCodes.Ldarg_0);
					gen.Emit(OpCodes.Ldloc, localVar.LocalIndex);
					gen.Emit(OpCodes.Stfld, field);
					gen.MarkLabel(skip);
				}
			}

			gen.Emit(OpCodes.Ret);

			return netRecieveMethod.CreateDelegate<Action<BeastPlayerBase, TagCompound>>();
		}
		static void _testSet(TagCompound tag, string key, object value) {
			tag[key] = value;
		}
		static void _testSend(object value, BinaryWriter writer) {
			try {
				BeastCustomization.DebugLogger.Debug($"sending {value} at position {writer.BaseStream.Position}");
			} catch (NullReferenceException) {
				BeastCustomization.DebugLogger.Debug($"sending null at position {writer.BaseStream.Position}");
			}
		}
		static void _testReceive(object value, BinaryReader reader) {
			try {
				BeastCustomization.DebugLogger.Debug($"receiving {value} at position {reader.BaseStream.Position}");
			} catch (NullReferenceException) {
				BeastCustomization.DebugLogger.Debug($"receiving null at position {reader.BaseStream.Position}");
			}
		}//*/
	}
	public abstract class GenericHeadLayer : PlayerDrawLayer {
		public sealed override bool IsHeadLayer => true;
		public sealed override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			int boundIndex = BeastCustomization.BeastPlayersByType[BoundBeastPlayer];
			(drawInfo.drawPlayer.ModPlayers[BeastCustomization.BeastPlayers[boundIndex]] as BeastPlayerBase)
				.HideVanillaDrawLayers(drawInfo, out bool hideHead, out _, out _);
			if (!hideHead) return false;
			SharedModPlayer sharedModPlayer = drawInfo.drawPlayer.GetModPlayer<SharedModPlayer>();
			if (sharedModPlayer?.current is null) return false;
			return sharedModPlayer.current.ModIndex == boundIndex;
		}
		public sealed override Position GetDefaultPosition() => new Between(PlayerDrawLayers.Head, PlayerDrawLayers.FaceAcc);
		protected sealed override void Draw(ref PlayerDrawSet drawInfo) {
			Player drawPlayer = drawInfo.drawPlayer;
			Vector2 Position = new Vector2((int)(drawInfo.Position.X + drawPlayer.width / 2f - drawPlayer.bodyFrame.Width / 2f - Main.screenPosition.X), (int)(drawInfo.Position.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f - Main.screenPosition.Y)) + drawPlayer.headPosition + drawInfo.headVect;
			Rectangle? Frame = drawPlayer.bodyFrame;

			foreach (var data in GetData(drawInfo)) {
				DrawData item = new(data.texture, Position, Frame, data.color, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0);
				if (data.applyDye) item.shader = drawInfo.cHead;
				drawInfo.DrawDataCache.Add(item);
			}
		}
		public abstract Type BoundBeastPlayer { get; }
		public abstract IEnumerable<(Texture2D texture, Color color, bool applyDye)> GetData(PlayerDrawSet drawInfo);
	}
	public abstract class GenericBodyLayer : PlayerDrawLayer {
		public sealed override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			int boundIndex = BeastCustomization.BeastPlayersByType[BoundBeastPlayer];
			(drawInfo.drawPlayer.ModPlayers[BeastCustomization.BeastPlayers[boundIndex]] as BeastPlayerBase)
				.HideVanillaDrawLayers(drawInfo, out _, out bool hideBody, out _);
			if (!hideBody) return false;
			SharedModPlayer sharedModPlayer = drawInfo.drawPlayer.GetModPlayer<SharedModPlayer>();
			if (sharedModPlayer?.current is null) return false;
			return sharedModPlayer.current.ModIndex == boundIndex;
		}
		public sealed override Position GetDefaultPosition() => new Between(PlayerDrawLayers.Torso, PlayerDrawLayers.OffhandAcc);
		protected sealed override void Draw(ref PlayerDrawSet drawInfo) {
			Player drawPlayer = drawInfo.drawPlayer;
			if (drawInfo.usesCompositeTorso) {
				Rectangle Frame = drawInfo.compTorsoFrame;

				Vector2 Position = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (drawInfo.drawPlayer.bodyFrame.Width / 2) + (drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.bodyFrame.Width / 2, drawInfo.drawPlayer.bodyFrame.Height / 2);
				Vector2 value = Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height];
				value.Y -= 2f;
				Position += value * -drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically).ToDirectionInt();

				foreach (var data in GetData(drawInfo)) {
					DrawData item = new DrawData(data.texture, Position, Frame, data.color, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0) {
						shader = drawInfo.cBody
					};
					drawInfo.DrawDataCache.Add(item);
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
		public abstract Type BoundBeastPlayer { get; }
		public abstract IEnumerable<(Texture2D texture, Color color)> GetData(PlayerDrawSet drawInfo);
	}
	public abstract class GenericArmLayer_Back : PlayerDrawLayer {
		public sealed override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			int boundIndex = BeastCustomization.BeastPlayersByType[BoundBeastPlayer];
			(drawInfo.drawPlayer.ModPlayers[BeastCustomization.BeastPlayers[boundIndex]] as BeastPlayerBase)
				.HideVanillaDrawLayers(drawInfo, out _, out bool hideBody, out _);
			if (!hideBody) return false;
			SharedModPlayer sharedModPlayer = drawInfo.drawPlayer.GetModPlayer<SharedModPlayer>();
			if (sharedModPlayer?.current is null) return false;
			return sharedModPlayer.current.ModIndex == boundIndex;
		}
		public sealed override Position GetDefaultPosition() => new Between(PlayerDrawLayers.Skin, PlayerDrawLayers.Leggings);
		protected sealed override void Draw(ref PlayerDrawSet drawInfo) {
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
		public abstract Type BoundBeastPlayer { get; }
		public abstract IEnumerable<(Texture2D texture, Color color)> GetData(PlayerDrawSet drawInfo);
	}
	public abstract class GenericArmLayer_Front : PlayerDrawLayer {
		public sealed override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			int boundIndex = BeastCustomization.BeastPlayersByType[BoundBeastPlayer];
			(drawInfo.drawPlayer.ModPlayers[BeastCustomization.BeastPlayers[boundIndex]] as BeastPlayerBase)
				.HideVanillaDrawLayers(drawInfo, out _, out bool hideBody, out _);
			if (!hideBody) return false;
			SharedModPlayer sharedModPlayer = drawInfo.drawPlayer.GetModPlayer<SharedModPlayer>();
			if (sharedModPlayer?.current is null) return false;
			return sharedModPlayer.current.ModIndex == boundIndex;
		}
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
		public abstract Type BoundBeastPlayer { get; }
		public abstract IEnumerable<(Texture2D texture, Color color)> GetData(PlayerDrawSet drawInfo);
	}
	public abstract class GenericLegsLayer : PlayerDrawLayer {
		public sealed override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			int boundIndex = BeastCustomization.BeastPlayersByType[BoundBeastPlayer];
			(drawInfo.drawPlayer.ModPlayers[BeastCustomization.BeastPlayers[boundIndex]] as BeastPlayerBase)
				.HideVanillaDrawLayers(drawInfo, out _, out _, out bool hideLegs);
			if (!hideLegs) return false;
			SharedModPlayer sharedModPlayer = drawInfo.drawPlayer.GetModPlayer<SharedModPlayer>();
			if (sharedModPlayer?.current is null) return false;
			return sharedModPlayer.current.ModIndex == boundIndex;
		}
		public sealed override Position GetDefaultPosition() => new Between(PlayerDrawLayers.Leggings, PlayerDrawLayers.Shoes);
		protected sealed override void Draw(ref PlayerDrawSet drawInfo) {
			Player drawPlayer = drawInfo.drawPlayer;

			Vector2 Position = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - drawPlayer.bodyFrame.Width / 2f + drawPlayer.width / 2f), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f)) + drawPlayer.legPosition + drawInfo.legVect;
			Rectangle? Frame = drawPlayer.legFrame;
			if (drawInfo.isSitting) {
				foreach (var data in GetData(drawInfo)) {
					Reflection.DrawSittingLegs(ref drawInfo, data.texture, data.color, drawInfo.cLegs);
				}
			} else {
				foreach (var data in GetData(drawInfo)) {
					DrawData item = new DrawData(data.texture, Position, Frame, data.color, drawPlayer.legRotation, drawInfo.legVect, 1f, drawInfo.playerEffect, 0);
					item.shader = drawInfo.cLegs;
					drawInfo.DrawDataCache.Add(item);
				}
			}
		}
		public abstract Type BoundBeastPlayer { get; }
		public abstract IEnumerable<(Texture2D texture, Color color)> GetData(PlayerDrawSet drawInfo);
	}
}
