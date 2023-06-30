/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using System.Collections.Generic;
using System.Linq;

namespace AchtuurCore.Framework;

public class TrailParticle : Particle
{

    protected List<Vector2> previousPositions;
    protected int positionPointer;

    protected int TrailLength { get; set; } = 5;
    /// <summary>
    /// Size of each particle in the trail
    /// 
    /// /// <remarks>
    /// The colors will be split over the tail. For example, if the trail length is 4 and the number of TrailColors is 2, 
    /// then the first 2 particles in the trail get the first TrailColor and the second 2 particles in the trail get the second color
    /// </remarks>
    /// </summary>
    public List<Vector2> TrailSizes { get; set; } = new List<Vector2>()
    {
        new Vector2(18f, 18f),
        new Vector2(16f, 16f),
        new Vector2(14f, 14f),
        new Vector2(12f, 12f),
    };

    /// <summary>
    /// Color of each particle in the trail. The head always uses the first color in this list
    /// 
    /// <remarks>
    /// The colors will be split over the tail. For example, if the trail length is 4 and the number of TrailColors is 2, 
    /// then the first 2 particles in the trail get the first TrailColor and the second 2 particles in the trail get the second color
    /// </remarks>
    /// </summary>
    public List<Color> TrailColors { get; set; } = new List<Color>()
    {
        Color.Green,
        Color.Blue,
        Color.Purple,
        Color.Magenta
    };


    public TrailParticle(Vector2 position, Vector2 targetPosition, int trailLength, Color color, Vector2 size) : base(position, targetPosition, color, size)
    {
        this.TrailLength = trailLength;

        // by default, return sizes that halve in size every particle
        // start at 1 to not divide by zero
        this.TrailSizes = Enumerable.Range(1, trailLength + 1).Select(i => this.size / (i + 1)).ToList();

        // Default color to white trail
        this.TrailColors = new List<Color>() { Color.White };

        // Initialize previouspositions with start position
        ResetTrailPositions();
    }

    public void SetTrailColors(List<Color> colors)
    {
        this.TrailColors = colors;
    }

    public override void SetSize(Vector2 size)
    {
        base.SetSize(size);
        this.TrailSizes = Enumerable.Range(1, this.TrailLength + 1).Select(i => this.size / (i + 1)).ToList();
    }

    public void SetTrailSizes(List<Vector2> sizes)
    {
        this.TrailSizes = sizes;
    }

    public override void Start()
    {
        base.Start();
        ResetTrailPositions();
    }

    public override void Reset()
    {
        ResetTrailPositions();
        base.Reset();
    }

    private void ResetTrailPositions()
    {
        this.previousPositions = new List<Vector2>(TrailLength + 1);
        for (int i = 0; i < TrailLength; i++)
            this.previousPositions.Add(intialPosition);
    }

    protected override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        // Update previous positions
        UpdatePreviousPositions();

        // Update 'main' position
        base.OnUpdateTicked(sender, e);
    }

    public override void DrawToScreen(SpriteBatch spriteBatch)
    {
        if (this.ReachedTarget || !this.Started)
            return;

        base.DrawToScreen(spriteBatch);

        for (int i = 0; i < TrailLength; i++)
        {
            // calculate i / traillength -> 'percentage' of how far we are along the trail
            // percentage * TrailColors.Count or percentage * TrailSize.Count is the index
            float percentage_of_trail = (float)i / (float)TrailLength;

            int color_index = (int)(percentage_of_trail * TrailColors.Count);
            Color trail_color = TrailColors[color_index];

            int size_index = (int)(percentage_of_trail * TrailSizes.Count);
            Vector2 trail_size = TrailSizes[size_index];

            Vector2 position = this.previousPositions[i];
            Vector2 screenCoords = Drawing.GetPositionScreenCoords(position);

            Drawing.DrawLine(spriteBatch, screenCoords, trail_size, trail_color * particleColorOpacity);
            Drawing.DrawBorder(spriteBatch, screenCoords, trail_size, trail_color, bordersize: 1);
        }
    }

    private void UpdatePreviousPositions()
    {
        if (TrailLength <= 0)
            return;

        // Add current position to front, which moves everything else back
        this.previousPositions.Insert(0, this.Position);

        // Remove positions that are larger than trail length
        while (this.previousPositions.Count > TrailLength)
            this.previousPositions.RemoveAt(TrailLength);
    }
}
