/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JohnsonNicholas/SDVMods
**
*************************************************/

using System;

namespace TwilightShards.Common
{
   public class StaticRange
   {
	  public double LowerBound {get; private set;}
	  public double UpperBound {get; private set;}
	  public double MidPoint {get; private set;}

      //constructors
	  public StaticRange(double lb, double ub, double mp)
	  {
		LowerBound = lb;
		UpperBound = ub;
		MidPoint = mp;
	  }	
	  
	  public StaticRange(StaticRange c) 
	  {
	    LowerBound = c.LowerBound;
		UpperBound = c.UpperBound;
		MidPoint = c.MidPoint;
	  }
	  
	  //range functions
	  public RangePair ReturnUpperHalf()
	  {
		  return new RangePair(MidPoint, UpperBound);
	  }
	  
	  public RangePair ReturnLowerHalf()
	  {
		  return new RangePair(LowerBound, MidPoint);
	  }
	  
	  public RangePair ReturnRange()
	  {
		  return new RangePair(LowerBound, UpperBound);
	  }
	  
	  //get random value in ranges
	  public double RandomInFullRange(Random r)
	  {
		  return r.RollInRange(LowerBound, UpperBound);
	  }
	  
	  public double RandomInLowerRange(Random r)
	  {
		  return r.RollInRange(LowerBound, MidPoint);
	  }
	  
	  public double RandomInUpperRange(Random r)
	  {
		  return r.RollInRange(MidPoint, UpperBound);
	  }
	  
	  //check ranges
	  public bool IsWithinFullRange(double val)
	  {
		  if (val >= LowerBound && val <= UpperBound)
			  return true;
		  
		  return false;
	  }
	  
	  public bool IsWithinUpperRange(double val)
	  {
		  if (val >= MidPoint && val <= UpperBound)
			  return true;
		  
		  return false;
	  }
	  
	  public bool IsWithinLowerRange(double val)
	  {
		  if (val >= LowerBound && val < MidPoint)
			  return true;
		  
		  return false;
	  }

        public override string ToString()
        {
            return $"Lower Bound is {this.LowerBound}, MidPoint is {this.MidPoint}, Upper Bound is {this.UpperBound}" + Environment.NewLine
                + $"Lower Range is {this.ReturnLowerHalf()}, Upper Range is {this.ReturnUpperHalf()}, Full Range is {this.ReturnRange()}";
        }

    }
}
