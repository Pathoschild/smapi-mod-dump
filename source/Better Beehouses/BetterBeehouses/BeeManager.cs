/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/BetterBeehouses
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.BellsAndWhistles;
using BetterBeehouses.integration;

namespace BetterBeehouses
{
	internal class Bee
	{
		internal Vector2 source;
		internal Vector2 target;
		internal double pct;
		internal double rate;
		internal int frame;
		internal Rectangle sourceRect;
		internal double millis = 0;
	}
	internal class BeeManager
	{
		private static readonly PerScreen<List<Bee>> bees = new(() => new());
		private static readonly PerScreen<List<Vector2>> bee_houses = new(() => new());

		private static readonly PerScreen<Dictionary<Vector2, IParticleManager>> particles = new(() => new());
		private static int pamt = -1;

		internal static void Init()
		{
			ModEntry.helper.Events.World.ObjectListChanged += UpdateObjects;
			ModEntry.helper.Events.Player.Warped += (s,e) => ChangeLocation(e.NewLocation);
			ModEntry.helper.Events.GameLoop.SaveLoaded += (s, e) => ChangeLocation(Game1.currentLocation);
			ModEntry.helper.Events.GameLoop.ReturnedToTitle += Exit;

			// if in-world drawing is available use that
			if (ModEntry.AeroCore is not null)
			{
				ModEntry.AeroCore.OnDrawingWorld += DrawBees;
				ModEntry.AeroCore.OnDrawingWorld += DrawParticles;
			}
			else
			{
				ModEntry.helper.Events.Display.RenderedWorld += (s, e) => DrawBees(e.SpriteBatch);
			}
		}

		internal static void ApplyConfigCount(int amt)
		{
			if (pamt == amt || amt < 0)
				return;

			pamt = amt;
			var parts = particles.Value;
			var houses = bee_houses.Value;
			parts.Clear();
			if (ModEntry.AeroCore is not null)
				foreach(var house in houses)
					parts[house] = BuildParticles(house);
		}

		private static void UpdateObjects(object _, ObjectListChangedEventArgs ev)
		{
			var houses = bee_houses.Value;
			var particle = particles.Value;
			foreach (var pair in ev.Removed)
			{
				houses.Remove(pair.Key);
				particle.Remove(pair.Key);
			}
			foreach ((var pos, var obj) in ev.Added)
			{
				if (obj.Name is "Bee House")
				{
					houses.Add(pos);
					if (ModEntry.AeroCore is not null)
						particle[pos] = BuildParticles(pos);
				}
			}
		}
		private static IParticleManager BuildParticles(Vector2 tile)
		{
			var emitter = new ParticleEmitter() {
				Region = new((int)tile.X * 64, (int)tile.Y * 64 - 32, 64, 64),
				Rate = 10000 / pamt
			};
			var manager = ModEntry.AeroCore.CreateParticleSystem(ModEntry.helper.ModContent, "assets/AeroBees.json", emitter, pamt);
			return manager;
		}

		private static void ChangeLocation(GameLocation where)
		{
			var houses = bee_houses.Value;
			var beev = bees.Value;
			var parts = particles.Value;
			houses.Clear();
			beev.Clear();
			parts.Clear();
			foreach (var obj in where.Objects.Values)
				if (obj.Name is "Bee House")
					houses.Add(obj.TileLocation);
			for (int i = 0; i < houses.Count * 5; i++)
				beev.Add(new() { pct = Game1.random.NextDouble() * -10.0 });
			if (ModEntry.AeroCore is not null)
				foreach (var pos in houses)
					parts[pos] = BuildParticles(pos);
		}
		private static void Exit(object _, ReturnedToTitleEventArgs ev)
		{
			bee_houses.Value.Clear();
			bees.Value.Clear();
			particles.Value.Clear();
		}
		private static void DrawParticles(SpriteBatch b)
		{
			var nodes = particles.Value;
			if (!ModEntry.config.Particles || nodes.Count == 0 || !ProducingHere())
				return;

			var view = new Vector2(-Game1.viewport.X, -Game1.viewport.Y);
			var millis = (int)Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
			foreach(var man in nodes.Values)
			{
				man.Tick(millis);
				man.Offset = view;
				man.Draw(b);
			}
		}
		private static void DrawBees(SpriteBatch b)
		{
			var houses = bee_houses.Value;
			if (houses.Count == 0 || !ModEntry.config.BeePaths || !ProducingHere())
				return;

			var beev = bees.Value;
			var tex = ModEntry.BeeTex;
			var time = Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
			var view = new Vector2(Game1.viewport.X, Game1.viewport.Y);
			for (int i = 0; i < beev.Count; i++)
			{
				var bee = beev[i];
				if (bee.pct > 2.0)
					SetupBee(bee, houses);
				else if (bee.pct < 0.0)
					bee.pct = Math.Min(bee.pct + time * .001, 0.0);
				else if (bee.pct == 0.0)
					SetupBee(bee, houses);

				if (bee.pct >= 0.0)
				{
					// draw
					var pos = Vector2.Lerp(bee.target, bee.source, MathF.Abs(1f - (float)bee.pct));
					b.Draw(tex, pos - view, bee.sourceRect, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, (pos.Y + 48f) * .0001f);

					// move
					bee.millis += time;
					if (bee.millis > 50)
					{
						bee.frame = bee.frame > 0 ? 0 : 1;
						bee.sourceRect.X = bee.frame * 8;
						bee.millis %= 50;
					}
					bee.pct += time * bee.rate * .05; // pixels/millisecond speed
				}
			}
		}

		private static void SetupBee(Bee bee, IList<Vector2> houses)
		{
			var src = houses[Game1.random.Next(houses.Count)];
			bee.source = src * 64f + new Vector2((float)Game1.random.NextDouble() * 32f + 8f, (float)Game1.random.NextDouble() * 32f - 8f);
			bee.target = GetTarget(src) * 64f + new Vector2(Game1.random.Next(32f) + 16f, Game1.random.Next(32f) - 8f);
			bee.rate = 1f / Vector2.Distance(bee.source, bee.target);
			var variant = Game1.random.Next(2);
			bee.sourceRect = new(0, variant * 8, 8, 8);
			bee.pct = 0.0;
		}

		private static Vector2 GetTarget(Vector2 source)
		{
			if (ModEntry.config.UseRandomFlower)
			{
				var items = UtilityPatch.GetAllNearFlowers(Game1.currentLocation, source, ModEntry.config.FlowerRange).ToArray();
				if (items.Length > 0)
					return items[Game1.random.Next(items.Length)].Key;
				else 
					return source;
			}
			var enumer = UtilityPatch.GetAllNearFlowers(Game1.currentLocation, source, ModEntry.config.FlowerRange).GetEnumerator();
			if (enumer.MoveNext())
				return enumer.Current.Key;
			return source;
		}
		private static bool ProducingHere()
			=> ObjectPatch.CanProduceHere(Game1.currentLocation) && 
			(Game1.currentSeason != "Winter" || Utils.GetProduceHere(Game1.currentLocation, ModEntry.config.ProduceInWinter));
	}
}
