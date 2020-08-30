using System;
using System.Collections.Generic;
using System.Linq;

namespace TwilightShards.Common
{
    /// <summary>
    /// This class allows for a probability distribution of multiple items to be controlled by one class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ProbabilityDistribution<T>
    {
        /// <summary>
        /// The internal list of end points
        /// </summary>
        private Dictionary<double, T> EndPoints { get; set; }
        
        /// <summary>
        /// The end point
        /// </summary>
        private double CurrentPoint;

        /// <summary>
        /// The overflow result.
        /// </summary>
        private T OverflowResult;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProbabilityDistribution()
        {
            EndPoints = new Dictionary<double, T>();
        }

        /// <summary>
        /// Constructor with specified overflow
        /// </summary>
        /// <param name="Overflow">The result on an overflow</param>
        public ProbabilityDistribution(T Overflow)
        {
            EndPoints = new Dictionary<double, T>();
            OverflowResult = Overflow;
        }

        /// <summary>
        /// Return the most current end point.
        /// </summary>
        /// <returns>The last end point.</returns>
        public double GetCurrentEndPoint()
        {
            return CurrentPoint;
        }

        /// <summary>
        /// Sets the overflow result
        /// </summary>
        /// <param name="Overflow">The result desired</param>
        public void SetOverflowResult(T Overflow)
        {
            OverflowResult = Overflow;
        }

        /// <summary>
        /// Adds the end point to the list. The probability must be positive and should not add over 1
        /// </summary>
        /// <param name="NewProb">The new probability being added</param>
        /// <param name="Entry">The entry being added</param>
        public void AddNewEndPoint(double NewProb, T Entry)
        {
            if (NewProb < 0)
                throw new ArgumentOutOfRangeException("The probability being added must be positive.");
            if (NewProb + CurrentPoint > 1)
                throw new ArgumentOutOfRangeException("The argument being added would cause the probability to exceed 100%.");

            CurrentPoint += NewProb;
            EndPoints.Add(CurrentPoint,Entry);
        }

        public void Empty()
        {
            EndPoints.Clear();
            CurrentPoint = 0;
            OverflowResult = default;
        }

        /// <summary>
        /// Adds the end point to the list. The probability must be positive and should not add over 1
        /// </summary>
        /// <param name="NewProb">The new probability being added</param>
        /// <param name="Entry">The entry being added</param>
        public void AddNewCappedEndPoint(double NewProb, T Entry)
        {
            if (NewProb < 0)
                throw new ArgumentOutOfRangeException("The probability being added must be positive.");
            if (CurrentPoint > 1 )
                throw new ArgumentOutOfRangeException("The argument being added would cause the probability to exceed 100%.");
            if (CurrentPoint == 1)
                return;

            if (NewProb + CurrentPoint > 1) //cap the probability
                NewProb = 1 - CurrentPoint;

            CurrentPoint += NewProb;

            if (!(EndPoints.Where(c => c.Key == CurrentPoint).Count() >= 1))
            {
                EndPoints.Add(CurrentPoint, Entry);
            }
        }

        /// <summary>
        /// Resorts the endpoints.
        /// </summary>
        private void Realign()
        {
            EndPoints.OrderBy(endpoint => endpoint.Key);
        }

        public override string ToString()
        {
            string desc = "";

            foreach (KeyValuePair<double,T> kvp in EndPoints)
            {
                desc += $"Key: {kvp.Key:N3} with value: {kvp.Value}";
                desc += Environment.NewLine;
            }
            desc += $"Current Point: {CurrentPoint:N3} with overflow result {OverflowResult:N3}";

            return desc;
        }

        /// <summary>
        /// Get the entry probability from the result.
        /// </summary>
        /// <param name="Prob">The probability being rolled</param>
        /// <param name="Result">The result being returned</param>
        /// <param name="IncludeEnds">Whether or not the ends are added</param>
        /// <returns>If a entry was found.</returns>
        public bool GetEntryFromProb(double Prob, out T Result, bool IncludeEnds = true)
        {
            if (!EndPoints.Keys.Any())
                throw new InvalidOperationException("No probabilities have been added to this distribution");

            if (Prob > 1 || Prob < 0)
                throw new ArgumentOutOfRangeException("The Probability must be greather than 0 and less than or equal to 1.");


            double[] KeyValues = EndPoints.Keys.ToArray();
            for (int i = 0; i < KeyValues.Count(); i++)
            {
                if (i == 0 && IncludeEnds)
                {
                    if (Prob >= 0 && Prob <= KeyValues[i])
                    {
                        Result = EndPoints[KeyValues[i]];
                        return true;
                    }
                }
                else if (i == 0 && !IncludeEnds)
                {
                    if (Prob > 0 && Prob < KeyValues[i])
                    {
                        Result = EndPoints[KeyValues[i]];
                        return true;
                    }
                }

                else if (i != 0 && IncludeEnds)
                {
                    if (Prob > KeyValues[i - 1] && Prob <= KeyValues[i])
                    {
                        Result = EndPoints[KeyValues[i]];
                        return true;
                    }

                }

                else if (i != 0 && !IncludeEnds)
                {
                    if (Prob > KeyValues[i - 1] && Prob < KeyValues[i])
                    {
                       Result = EndPoints[KeyValues[i]];
                        return true;
                    }
                }               

            }

            if (OverflowResult != null && !OverflowResult.Equals(default(T)))
            {
                if (Prob > KeyValues.Last())
                {
                    Result = OverflowResult;
                    return true;
                }
            }

            Result = default;
            return false;
        }
    }
}
