/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Fireworks
{
	// ReSharper disable once InconsistentNaming
	internal class EventScript_ModJam : ICustomEventScript
	{
		private readonly List<Firework> fireworks;
		private int eventTimer;

		private const int NumFireworks = 30;
		private const int EventRunTime = 1800;

		public EventScript_ModJam()
		{
			int width = Game1.uiViewport.Width;
			int height = Game1.uiViewport.Height;
			
			eventTimer = 0;
			Random rand = new();

			fireworks = new List<Firework>();

			for (int i = 0; i < NumFireworks; i++)
			{
				fireworks.Add(
				new Firework(
						startTime: rand.Next(EventRunTime / 10, (int)(EventRunTime / 10 * 7.5f)),
						position: new Vector2(rand.Next((int)(width * 0.25f), (int)(width * 0.75f)), height),
						velocity: new Vector2(rand.Next(-1, 1), rand.Next(-200, -175) / 10f),
						color: new Color(rand.Next(205) + 50, rand.Next(205) + 50, rand.Next(205) + 50),
						random: rand,
						finale: false
					)
				);
			}

			Color finaleColor1 = new(rand.Next(205) + 50, rand.Next(205) + 50, rand.Next(205) + 50);
			Color finaleColor2 = new(finaleColor1.R + rand.Next(-125, 125), finaleColor1.G + rand.Next(-125, 125), finaleColor1.B + rand.Next(-125, 125));
			Color finaleColor3 = new(finaleColor2.R + rand.Next(-125, 125), finaleColor2.G + rand.Next(-125, 125), finaleColor2.B + rand.Next(-125, 125));

			Vector2 finaleVelocity = new(0f, -18f);

			for (float j = 0.05f; j <= 0.5f; j += 0.05f)
			{
				fireworks.Add(
					new Firework(
						startTime: EventRunTime / 10 * 8,
						position: new Vector2(width * (0.25f + j), height),
						velocity: finaleVelocity,
						color: finaleColor1,
						random: rand,
						finale: true
					)
				);

				fireworks.Add(
					new Firework(
						startTime: (int)(EventRunTime / 10 * 8.5f),
						position: new Vector2(width * (0.25f + j), height),
						velocity: finaleVelocity,
						color: finaleColor2,
						random: rand,
						finale: true
					)
				);

				fireworks.Add(
					new Firework(
						startTime: EventRunTime / 10 * 9,
						position: new Vector2(width * (0.25f + j), height),
						velocity: finaleVelocity,
						color: finaleColor3,
						random: rand,
						finale: true
					)
				);
			}
		}

		public bool update(GameTime time, Event e)
		{
			eventTimer++;

			foreach (Firework f in fireworks)
			{
				f.Update(eventTimer);
			}

			return eventTimer > EventRunTime;
		}


		public void draw(SpriteBatch b)
		{

		}

		public void drawAboveAlwaysFront(SpriteBatch b)
		{
			// Background drawing stuff

			//b.Draw(Game1.mouseCursors, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), new Rectangle(639, 858, 1, 184), Color.White);
			//b.Draw(Game1.mouseCursors, new Rectangle(2556, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), new Rectangle(639, 858, 1, 184), Color.White);
			//b.Draw(Game1.mouseCursors, new Vector2(0f, 0f), new Rectangle(0, 1453, 639, 195), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
			//b.Draw(Game1.mouseCursors, new Vector2(2556f, 0f), new Rectangle(0, 1453, 639, 195), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
			//b.Draw(Game1.mouseCursors, new Vector2(0f, Game1.uiViewport.Height - 192), new Rectangle(0, Game1.currentSeason.Equals("winter") ? 1034 : 737, 639, 48), (Game1.currentSeason.Equals("winter") ? (Color.White * 0.25f) : new Color(0, 20, 40)), 0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 1f);
			//b.Draw(Game1.mouseCursors, new Vector2(2556f, Game1.uiViewport.Height - 192), new Rectangle(0, Game1.currentSeason.Equals("winter") ? 1034 : 737, 639, 48), (Game1.currentSeason.Equals("winter") ? (Color.White * 0.25f) : new Color(0, 20, 40)), 0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 1f);
			//b.Draw(Game1.mouseCursors, new Vector2(0f, Game1.uiViewport.Height - 128), new Rectangle(0, Game1.currentSeason.Equals("winter") ? 1034 : 737, 639, 32), (Game1.currentSeason.Equals("winter") ? (Color.White * 0.5f) : new Color(0, 32, 20)), 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
			//b.Draw(Game1.mouseCursors, new Vector2(2556f, Game1.uiViewport.Height - 128), new Rectangle(0, Game1.currentSeason.Equals("winter") ? 1034 : 737, 639, 32), (Game1.currentSeason.Equals("winter") ? (Color.White * 0.5f) : new Color(0, 32, 20)), 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
			//b.Draw(Game1.mouseCursors, new Vector2(160f, Game1.uiViewport.Height - 128 + 16 + 8), new Rectangle(653, 880, 10, 10), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

			foreach (Firework f in fireworks)
			{
				f.Draw(b);
			}

		}
	}

	internal class Firework
	{
		private readonly Vector2 GravityVector = new(0f, 0.25f);

		private readonly int _startTime;

		private Vector2 _position;
		private Vector2 _velocity;

		private readonly int _width;
		private readonly int _height;

		private readonly float _opacity;
		private readonly Color _color;

		private bool _drawing;
		private bool _fired;
		private bool _exploding;
		private readonly bool _finale;

		private List<Particle> _particles;
		private readonly int _particleRate;

		private readonly Random _rand;

		public Firework(int startTime, Vector2 position, Vector2 velocity, Color color, Random random, bool finale)
		{
			_particles = new List<Particle>();
			_startTime = startTime;
			_position = position;
			_width = 10;
			_height = 10;
			_opacity = 1f;
			_drawing = false;
			_color = color;
			_velocity = velocity;
			_particleRate = 4;
			_rand = random;
			_finale = finale;
		}

		public void Update(int timer)
		{
			if (_exploding)
			{
				Game1.playSoundPitched("explosion", _rand.Next(1500, 5000));
				_particles = _particles.Concat(CreateParticleExplosion()).ToList();
				_drawing = false;
				_exploding = false;
				_fired = false;
			}
			else if (_fired)
			{
				if (timer % _particleRate == 0)
				{
					_particles.Add(new Particle(
						position: _position,
						velocity: new Vector2(0f, 0f),
						color: _color,
						opacity: 1f,
						opacityFalloff: 0.05f)
					);
				}

				_position += _velocity;
				_velocity += GravityVector;

				if (_velocity.Y >= 0f)
				{
					_exploding = true;
				}

			}
			else if (_startTime == timer)
			{
				_fired = true;
				_drawing = true;
				Game1.playSoundPitched("cowboy_explosion", _rand.Next(1800, 2200));
			}

			List<Particle> particlesToRemove = new();

			foreach (Particle p in _particles)
			{
				p.Update();
				if (p.IsFaded)
				{
					particlesToRemove.Add(p);
				}
			}

			foreach (Particle p in particlesToRemove)
			{
				_particles.Remove(p);
			}

			particlesToRemove.Clear();
		}

		private List<Particle> CreateParticleExplosion()
		{
			List<Particle> particles = new();

			Color color1 = new(_color.R + _rand.Next(-125, 125), _color.G + _rand.Next(-125, 125), _color.B + _rand.Next(-125, 125));
			Color color2 = new(color1.R + _rand.Next(-125, 125), color1.G + _rand.Next(-125, 125), color1.B + _rand.Next(-125, 125));
			Color color3 = new(color2.R + _rand.Next(-125, 125), color2.G + _rand.Next(-125, 125), color2.B + _rand.Next(-125, 125));

			for (int angle = 0; angle < 360; angle += 5)
			{
				Vector2 angleVector = new((float) Math.Cos(angle), (float) Math.Sin(angle));
				angleVector.Normalize();

				if (_finale)
				{
					particles.Add(new Particle(_position, angleVector * _rand.Next(30) / 10f, _color, 1f, 0.01f));
					particles.Add(new Particle(_position, angleVector * _rand.Next(20) / 10f, _color, 1f, 0.01f));
					particles.Add(new Particle(_position, angleVector * _rand.Next(10) / 10f, _color, 1f, 0.01f));
				}
				else
				{
					particles.Add(new Particle(_position, angleVector * _rand.Next(30) / 10f, color1, 1f, 0.01f));
					particles.Add(new Particle(_position, angleVector * _rand.Next(20) / 10f, color2, 1f, 0.01f));
					particles.Add(new Particle(_position, angleVector * _rand.Next(10) / 10f, color3, 1f, 0.01f));
				}
			}

			return particles;
		}

		public void Draw(SpriteBatch b)
		{
			foreach (Particle p in _particles) p.Draw(b);
		
			if (!_drawing)
				return;

			b.Draw(
				texture: Game1.mouseCursors,
				destinationRectangle: new Rectangle(_position.ToPoint(), new Point(_width, _height)),
				sourceRectangle: new Rectangle(359, 1188, 7, 8),
				color: _color * _opacity,
				rotation: 0f,
				origin: new Vector2(0f, 0f),
				effects: SpriteEffects.None,
				layerDepth: 1f
			);

		}
	}

	internal class Particle
	{
		private Vector2 _position;
		private Vector2 _velocity;

		private readonly Color _color;
		private float _opacity;
		private readonly float _opacityFalloff;

		public bool IsFaded;
		private readonly float _velocityFalloff;

		public Particle(Vector2 position, Vector2 velocity, Color color, float opacity, float opacityFalloff)
		{
			_velocity = velocity;
			_velocityFalloff = 0.015f;
			_position = position;
			_color = color;
			_opacity = opacity;
			_opacityFalloff = opacityFalloff;
		}

		public void Update()
		{
			_position += _velocity;
			_opacity -= _opacityFalloff;

			_velocity -= _velocity * _velocityFalloff;

			if (_opacity < 0f)
				IsFaded = true;
		}

		public void Draw(SpriteBatch b)
		{
			b.Draw(
				texture: Game1.mouseCursors,
				destinationRectangle: new Rectangle(_position.ToPoint(), new Point(8, 8)),
				sourceRectangle: new Rectangle(359, 1188, 7, 8),
				color: _color * _opacity,
				rotation: 0f,
				origin: new Vector2(0f, 0f),
				effects: SpriteEffects.None,
				layerDepth: 1f
			);
		}
	}
}