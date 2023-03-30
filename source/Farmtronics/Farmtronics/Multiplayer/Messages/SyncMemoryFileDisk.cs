/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoeStrout/Farmtronics
**
*************************************************/

using System.Linq;
using Farmtronics.M1.Filesystem;
using Farmtronics.Utils;

namespace Farmtronics.Multiplayer.Messages {
	internal class SyncMemoryFileDisk : BaseMessage<SyncMemoryFileDisk> {
		public string DiskName { get; set; }
		public MemoryDirectory RootDirectory { get; set; }
		
		// Host
		public void Apply(long playerID) {
			var diskName = $"/{DiskName}";
			if (DiskName == "usr") {
				var controller = DiskController.GetDiskController(playerID);
				if (!controller.GetDiskNames().Contains("usr")) controller.AddDisk("usr", new RealFileDisk(SaveData.GetUsrDiskPath(playerID)));

				RootDirectory = (controller.GetDisk(ref diskName) as RealFileDisk).BuildMemoryDirectory();
			} else {
				var sharedDisk = DiskController.GetCurrentDiskController().GetDisk(ref diskName) as SharedRealFileDisk;
				if (sharedDisk == null) return;
				RootDirectory = sharedDisk.BuildMemoryDirectory();
			}
			Send(new[] { playerID });
		}

		// Client
		public override void Apply() {
			var diskName = $"/{DiskName}";
			MemoryFileDisk disk = DiskController.GetCurrentDiskController().GetDisk(ref diskName) as MemoryFileDisk;
			if (disk == null) return;
			
			disk.root = RootDirectory;
			// ModEntry.instance.Monitor.Log($"MemoryFileDisk data: {disk.root.ToString()}");
		}
	}
}