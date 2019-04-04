public class ModDebugData {
	
	public bool IsHost { get; set; }

	public ModDebugData() {

		IsHost = false;

	}

	public bool ActAsHost() {
		IsHost = !IsHost;
		return IsHost;
	}

}
