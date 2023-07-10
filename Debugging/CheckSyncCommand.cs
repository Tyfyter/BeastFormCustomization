using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BeastCustomization.Debugging {
	public class CheckAllCommand : ModCommand {
		public override string Command => "check_all";
		public override CommandType Type => CommandType.Chat;
		public override void Action(CommandCaller caller, string input, string[] args) {
			GetArgValues(args, out bool verbose);
			CheckSyncCommand.CheckSync(caller.Player, verbose);
			CheckSaveCommand.CheckSave(caller.Player, verbose);
			/*
			Main.NewText("Experimental syncing:");
			caller.Player.GetModPlayer<WolfColorPlayer>().CheckIntegrity((BeastPlayerBase first, BeastPlayerBase second) => {
				MemoryStream stream = new MemoryStream(256);
				BeastPlayerBase.GenerateNetSend<WolfColorPlayer>()((WolfColorPlayer)first, new BinaryWriter(stream));
				stream.Position = 0;
				BeastPlayerBase.GenerateNetRecieve<WolfColorPlayer>()((WolfColorPlayer)second, new BinaryReader(stream));
			}, "[c/ff0000:{0} field {1} not syncing properly]", "[c/00ff00:{0} field {1} is actually syncing properly]"
			);
			var reads = Reflection.BinaryReaderReads;
			var writes = Reflection.BinaryWriterWrites;
			Main.NewText("Experimental saving:");
			caller.Player.GetModPlayer<WolfColorPlayer>().CheckIntegrity((BeastPlayerBase first, BeastPlayerBase second) => {
				TagCompound tag = new();
				BeastPlayerBase.GenerateExportData(typeof(WolfColorPlayer))((WolfColorPlayer)first, tag);
				BeastPlayerBase.GenerateImportData(typeof(WolfColorPlayer))((WolfColorPlayer)second, tag);
			}, "[c/ff0000:{0} field {1} not saving properly]", "[c/00ff00:{0} field {1} is actually saving properly]"
			);
			//*/
		}
		public static void GetArgValues(string[] args, out bool verbose) {
			verbose = false;
			for (int i = 0; i < args.Length; i++) {
				switch (args[i]) {
					case "verbose":
					verbose = true;
					break;
				}
			}
		}
	}
	public class CheckSyncCommand : ModCommand {
		public override string Command => "check_sync";
		public override CommandType Type => CommandType.Chat;
		public override void Action(CommandCaller caller, string input, string[] args) {
			CheckAllCommand.GetArgValues(args, out bool verbose);
			CheckSync(caller.Player, verbose);
		}
		public static void CheckSync(Player player, bool verbose) {
			bool fullySyncing = true;
			for (int i = 0; i < BeastCustomization.BeastPlayers.Count; i++) {
				fullySyncing &= (player.ModPlayers[BeastCustomization.BeastPlayers[i]] as BeastPlayerBase).CheckSync(verbose);
			}
			if (fullySyncing) {
				Main.NewText("All forms syncing properly");
			}
		}
	}
	public class CheckSaveCommand : ModCommand {
		public override string Command => "check_save";
		public override CommandType Type => CommandType.Chat;
		public override void Action(CommandCaller caller, string input, string[] args) {
			CheckAllCommand.GetArgValues(args, out bool verbose);
			CheckSave(caller.Player, verbose);
		}
		public static void CheckSave(Player player, bool verbose) {
			bool fullySyncing = true;
			for (int i = 0; i < BeastCustomization.BeastPlayers.Count; i++) {
				fullySyncing &= (player.ModPlayers[BeastCustomization.BeastPlayers[i]] as BeastPlayerBase).CheckSave(verbose);
			}
			if (fullySyncing) {
				Main.NewText("All forms saving properly");
			}
		}
	}
}
