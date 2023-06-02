using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BeastCustomization.Debugging {
	public class CheckAllCommand : ModCommand {
		public override string Command => "check_all";
		public override CommandType Type => CommandType.Chat;
		public override void Action(CommandCaller caller, string input, string[] args) {
			CheckSyncCommand.CheckSync(caller.Player);
			CheckSaveCommand.CheckSave(caller.Player);
		}
	}
	public class CheckSyncCommand : ModCommand {
		public override string Command => "check_sync";
		public override CommandType Type => CommandType.Chat;
		public override void Action(CommandCaller caller, string input, string[] args) {
			CheckSync(caller.Player);
		}
		public static void CheckSync(Player player) {
			bool fullySyncing = true;
			for (int i = 0; i < BeastCustomization.BeastPlayers.Count; i++) {
				fullySyncing &= (player.ModPlayers[BeastCustomization.BeastPlayers[i]] as BeastPlayerBase).CheckSync();
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
			CheckSave(caller.Player);
		}
		public static void CheckSave(Player player) {
			bool fullySyncing = true;
			for (int i = 0; i < BeastCustomization.BeastPlayers.Count; i++) {
				fullySyncing &= (player.ModPlayers[BeastCustomization.BeastPlayers[i]] as BeastPlayerBase).CheckSave();
			}
			if (fullySyncing) {
				Main.NewText("All forms saving properly");
			}
		}
	}
}
