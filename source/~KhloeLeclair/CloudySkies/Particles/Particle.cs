/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace Leclair.Stardew.CloudySkies.Particles;

public struct Particle {

	public Vector2 Position;
	public Vector2 Velocity;

	public float Rotation;
	public float AngularVelocity;

	public ulong Age;
	public ulong MaxAge;

	public float Alpha;
	public Color Color;
	public float Scale;

	public Particle(Vector2 position, Vector2 velocity, float rotation, float angularVelocity, ulong age, ulong maxAge, float alpha, Color color, float scale) {
		Position = position;
		Velocity = velocity;
		Rotation = rotation;
		AngularVelocity = angularVelocity;
		Age = age;
		MaxAge = maxAge;
		Alpha = alpha;
		Color = color;
		Scale = scale;
	}

}
