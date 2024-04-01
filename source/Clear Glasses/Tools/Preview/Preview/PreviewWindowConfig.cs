/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using System.Reflection;

namespace SpriteMaster.Tools.Preview.Preview;

internal abstract class PreviewWindowConfig : IDisposable {
	internal readonly PreviewWindow Window;
	internal readonly ResamplerType Resampler;
	internal readonly Type ConfigType;

	private readonly List<Control> Controls = new();

	protected PreviewWindowConfig(PreviewWindow window, ResamplerType resampler, Type configType) {
		Resampler = resampler;
		ConfigType = configType;
		Window = window;

		//PopulateWindow();
	}

	private void OnScaleChanged(uint scale) {

	}

	private void PopulateWindow() {
		var fields = ConfigType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		foreach (var field in fields) {
			if (field.IsStatic || field.IsLiteral) {
				continue;
			}

			string name = field.Name;
			Type type = field.FieldType;

			Control? newControl = null;

			switch (name) {
				case "Scale":
					var control = new Controls.ScaleControl {
						//Events = {
						//	OnChanged = value => 
						//}
					};
					newControl = control;
					break;
				default: {
					if (type == typeof(byte)) {

					}
					else if (type == typeof(sbyte)) {

					}
					else if (type == typeof(ushort)) {

					}
					else if (type == typeof(short)) {

					}
					else if (type == typeof(uint)) {

					}
					else if (type == typeof(int)) {

					}
					else if (type == typeof(ulong)) {

					}
					else if (type == typeof(long)) {

					}
					else if (type == typeof(float) || type == typeof(double)) {

					}
					else {
						ThrowHelper.ThrowNotImplementedException(type.GetTypeName());
					}

					break;
				}
			}

			if (newControl is not null) {
				Controls.Add(newControl);
			}
		}
	}

	public virtual void Dispose() {
		foreach (var control in Controls) {
			Window.Controls.Remove(control);
			control.Dispose();
		}
	}
}
