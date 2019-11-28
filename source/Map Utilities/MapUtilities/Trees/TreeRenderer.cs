using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using Newtonsoft.Json.Linq;
//using System.Drawing;
//using System.Drawing.Imaging;

namespace MapUtilities.Trees
{
    public class TreeRenderer
    {
        public Tree tree;
        public TreePart treeStructure;
        public Species species;
        public IReflectedField<float> shakeRotation;
        public IReflectedField<Netcode.NetBool> falling;

        public int seed;

        public Random random;

        public Microsoft.Xna.Framework.Color transparency;

        

        public TreeRenderer(Tree tree, Vector2 tile)
        {
            transparency = Microsoft.Xna.Framework.Color.White;
            this.tree = tree;
            species = TreeHandler.getSpecies(tree.treeType);
            updateStructure(tile);
        }

        public static float radiansToDegrees(float radians)
        {
            return radians * (float)(180f / Math.PI);
        }

        public static float degreesToRadians(float degrees)
        {
            return degrees * (float)(Math.PI / 180f);
        }

        public void draw(SpriteBatch b, Vector2 treePos)
        {
            if(treePos.Y > (Game1.viewport.Y + Game1.viewport.Height) / 64)
            {
                float distance = Math.Min(treePos.Y * 64 - (Game1.viewport.Y + Game1.viewport.Height), 6f * 64f);
                transparency = (Microsoft.Xna.Framework.Color.White * (1 - distance / (6f * 64f)));
            }
            else
            {
                transparency = Microsoft.Xna.Framework.Color.White;
            }

            treeStructure.draw(b, treePos, treeStructure.sprite.Width * 2, treeStructure.sprite.Height * 4, 0f);
        }

        public void updateStructure(Vector2 tile)
        {
            //seed should remain constant for any tree, so it is tied to their location and whether they are flipped.
            seed = (int)(tile.X * 100 * tile.Y * Math.PI) * (tree.flipped ? 2 : 3);
            random = new Random(seed);
            Logger.log("Seed is " + seed);
            shakeRotation = Reflector.reflector.GetField<float>(tree, "shakeRotation");
            falling = Reflector.reflector.GetField<Netcode.NetBool>(tree, "falling");
            buildStructure();
            //treeStructure.draw(Game1.spriteBatch, tile, 0f, 0f);
        }

        public void buildStructure()
        {
            treeStructure = new Stump(this, species.getRect(species.weightedStumpRects, random));
            int trunkLength = species.minHeight + (seed % species.heightVariation);
            Logger.log("Tree is " + trunkLength + " units tall.");

            int firstLimbHeight = (int)(species.minLimbHeight + random.NextDouble() * species.minLimbHeightVariation);
            //int sectionsBetweenLimbs = 0;

            if (species.form == Species.FORM_OPEN)
            {

                for (int i = 0; i < trunkLength; i++)
                {
                    Trunk newSection = new Trunk(this, species.getRect(species.weightedTrunkRects, random));
                    newSection.rotation = ((float)random.NextDouble() * 1f) - 0.5f;
                    //newSection.rotation = 0.3f;
                    treeStructure.findAnyEnd(new Type[] { typeof(Trunk), typeof(Stump) }).addPart(newSection);
                    newSection.performSetup();
                    //&& i - firstLimbHeight >= sectionsBetweenLimbs
                    if (i >= firstLimbHeight && random.NextDouble() <= species.limbFrequency)
                    {
                        Logger.log("Adding limbs to section " + i);
                        if (species.limbAlternate)
                        {
                            addLimbToPart(newSection, random.Next(2) == 0, i, trunkLength - i);
                        }
                        else
                        {
                            addLimbToPart(newSection, true, i, trunkLength - i);
                            addLimbToPart(newSection, false, i, trunkLength - i);
                        }
                        //sectionsBetweenLimbs = (int)(averageLimbInterval + (random.NextDouble() * limbIntervalVariation) - (limbIntervalVariation / 2));
                    }
                    //else if (i >= firstLimbHeight)
                    //{
                    //    sectionsBetweenLimbs--;
                    //}
                }
                addLimbToPart(treeStructure.findAnyEnd(new Type[] { typeof(Trunk), typeof(Stump) }), random.Next(2) == 0, trunkLength, species.minHeight / 2);
            }

            else if (species.form == Species.FORM_CONICAL)
            {
                for (int i = 0; i < trunkLength; i++)
                {

                    if (i == trunkLength - 1)
                    {
                        Logger.log("Trunk has become vertical limb...");
                        addLimbToPart(treeStructure.findAnyEnd(new Type[] { typeof(Trunk), typeof(Stump) }), random.Next(2) == 0, trunkLength, 2, 0f);
                    }
                    else if (species.limbGrowth * (trunkLength - i) <= 1 || i >= trunkLength - 2)
                    {
                        Trunk newSection = new Trunk(this, species.getRect(species.weightedLimbRects, random));
                        newSection.rotation = ((float)random.NextDouble() * 1f) - 0.5f;
                        treeStructure.findAnyEnd(new Type[] { typeof(Trunk), typeof(Stump) }).addPart(newSection);
                        newSection.performSetup();

                        Logger.log("Adding branches to small section " + i);

                        addBranchesToLimb(newSection, trunkLength - i);
                    }
                    else if (i >= firstLimbHeight && random.NextDouble() <= species.limbFrequency)
                    {
                        Trunk newSection = new Trunk(this, species.getRect(species.weightedTrunkRects, random));
                        newSection.rotation = ((float)random.NextDouble() * 1f) - 0.5f;
                        treeStructure.findAnyEnd(new Type[] { typeof(Trunk), typeof(Stump) }).addPart(newSection);
                        newSection.performSetup();

                        Logger.log("Adding limb to section " + i);
                        if (species.limbAlternate)
                        {
                            addLimbToPart(newSection, random.Next(2) == 0, i, trunkLength - i);
                        }
                        else
                        {
                            addLimbToPart(newSection, true, i, trunkLength - i);
                            addLimbToPart(newSection, false, i, trunkLength - i);
                        }
                    }
                }
            }
            else if (species.form == Species.FORM_LINEAR)
            {
                for (int i = 0; i < trunkLength; i++)
                {
                    Trunk newSection = new Trunk(this, species.getRect(species.weightedTrunkRects, random));
                    newSection.rotation = ((float)random.NextDouble() * species.maxTrunkWobble) - (species.maxTrunkWobble / 2f);
                    treeStructure.findAnyEnd(new Type[] { typeof(Trunk), typeof(Stump) }).addPart(newSection);
                    newSection.performSetup();
                    if (i == trunkLength - 1)
                    {
                        Rectangle leafSprite = species.getRect(species.weightedLeafRects, random);
                        LeafCluster leafCluster = new LeafCluster(this, leafSprite, species.getColor(species.weightedLeafColors, leafSprite, random));
                        leafCluster.left = random.NextDouble() >= 0.5;
                        newSection.addPart(leafCluster);
                        leafCluster.depth -= (float)(random.NextDouble() + 0.5);
                        leafCluster.performSetup();
                    }
                }
            }

            else if (species.form == Species.FORM_SPREADING)
            {
                for (int i = 0; i < trunkLength; i++)
                {
                    List<TreePart> currentEnds = treeStructure.findAllEnds(new Type[] { typeof(Trunk), typeof(Stump) });
                    foreach(TreePart currentEnd in currentEnds)
                    {
                        float splitChance = species.limbFrequency / currentEnds.Count;

                    }
                }
            }
        }

        public void addLimbToPart(TreePart part, bool left, int sectionsUp, int sectionsDown, float rotationOverride = float.MinValue)
        {
            Logger.log("Limb was on the " + (left ? "left" : "right"));
            Limb newLimb = new Limb(this, species.getRect(species.weightedLimbRects, random));

            float initialRot;
            if (rotationOverride == float.MinValue)
                initialRot = species.limbAngle * (left ? -1 : 1);
            else
                initialRot = rotationOverride;

            float depth = (float)(random.NextDouble() * 2 + 1) * (random.Next(2) == 0 ? -1 : 1);
            newLimb.depth = depth;
            newLimb.rotation = initialRot;
            part.addPart(newLimb);
            newLimb.left = left;
            newLimb.performSetup();
            int sectionsToGrow = (int)(sectionsDown * species.limbGrowth);
            Logger.log("Growing " + sectionsToGrow + " sections on limb...");
            for(int i = 0; i < sectionsToGrow; i++)
            {
                Limb newSection = new Limb(this, species.getRect(species.weightedLimbRects, random));
                newLimb.findAnyEnd(new Type[] { typeof(Limb) }).addPart(newSection);
                newSection.rotation = ((float)(random.NextDouble() * 0.5 - 0.25f));
                newSection.performSetup();
                if (i >= species.minBranchDistance && random.NextDouble() < species.branchFrequency)
                {
                    Logger.log("Adding branch to limb section " + i);
                    if (species.limbAlternate)
                    {
                        addBranchesToLimb(newSection, sectionsToGrow - i);
                    }
                    else
                    {
                        addBranchesToLimb(newSection, sectionsToGrow - i);
                        addBranchesToLimb(newSection, sectionsToGrow - i);
                    }
                    //sectionsBetweenLimbs = (int)(averageLimbInterval + (random.NextDouble() * limbIntervalVariation) - (limbIntervalVariation / 2));
                }
            }
            //newLimb.addGravity(true);
            addBranchesToLimb(newLimb.findAnyEnd(new Type[] { typeof(Limb) }), sectionsToGrow / 2);
        }

        public void addBranchesToLimb(TreePart limb, int limbRemaining)
        {
            int branchSections = (int)(limbRemaining * species.averageBranchLength + (random.NextDouble() * species.branchLengthVariation - species.branchLengthVariation / 2));
            Branch newBranch = new Branch(this, species.getRect(species.weightedBranchRects, random));
            limb.addPart(newBranch);
            float initialRot = (float)(random.NextDouble() * (Math.PI * 2f));
            newBranch.rotation = initialRot;
            newBranch.performSetup();
            for(int i = 0; i < branchSections; i++)
            {
                Branch newSection = new Branch(this, species.getRect(species.weightedBranchRects, random));
                newBranch.findAnyEnd().addPart(newSection);
                newSection.rotation = ((float)(random.NextDouble() * 0.5 - 0.25f));
                newSection.performSetup();
            }

            Rectangle leafSprite = species.getRect(species.weightedLeafRects, random);
            LeafCluster leafCluster = new LeafCluster(this, leafSprite, species.getColor(species.weightedLeafColors, leafSprite, random));
            newBranch.findAnyEnd().addPart(leafCluster);
            leafCluster.depth -= (float)(random.NextDouble() + 0.5);
            leafCluster.performSetup();
            Logger.log("Leaf cluster is on " + (leafCluster.left ? "left" : "right"));
        }
    }
}
