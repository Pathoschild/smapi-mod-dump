/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System.Globalization;

namespace SpriteMaster.Tools.Preview.Preview.Controls;

internal sealed class RealBar : UserControl {
	internal class EventsClass {
		internal Action<double>? OnChanged { get; set; }
	}

	private static int ConvertValue(double value) => (int)(value * 100.0);
	private static double ConvertValue(int value) => value / 100.0;

	private readonly HScrollBar InnerControl = new();
	internal new EventsClass Events { get; init; } = new();
	internal double? MinValueInternal;
	internal double? MaxValueInternal;

	internal double? MinValue {
		get => MinValueInternal ?? ConvertValue(InnerControl.Minimum);
		set {
			InnerControl.Minimum = ConvertValue(value ?? ConvertValue(InnerControl.Minimum));
			MinValueInternal = value;
		}
	}
	internal double? MaxValue {
		get => MaxValueInternal ?? ConvertValue(InnerControl.Maximum);
		set {
			InnerControl.Maximum = ConvertValue(value ?? ConvertValue(InnerControl.Maximum));
			MaxValueInternal = value;
		}
	}

	internal double Value {
		get => ConvertValue(InnerControl.Value);
		set => InnerControl.Value = ConvertValue(value);
	}

	internal RealBar() {
		InnerControl.ValueChanged += (_, args) => OnValueChanged(args);
	}

	private void OnValueChanged(EventArgs args) {
		Events.OnChanged?.Invoke(Value);
	}

	public override string ToString() => Value.ToString(CultureInfo.CurrentCulture);
}