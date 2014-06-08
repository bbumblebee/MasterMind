using System;

namespace bb.mastermind.assessment
{

	using Config = bb.mastermind.core.Config;

	/// <summary>
	/// Created by BB on 01.05.2014.
	/// This is a maximizing Task. Entropy should be maximal.
	/// </summary>
	public class EntropyAssessor : PossibilityDivisionAssessor
	{
		private readonly int wholeSize;
		private int possibilitiesCome = 0;

		public EntropyAssessor(AssessmentManager man) : base(man)
		{
			this.wholeSize = man.wholeSize;
		}

		public override void updateNextIndividual(int setValue)
		{
			double p = (double) setValue / (double) wholeSize;
			currentSetValue += -(p * (Math.Log(p) / Math.Log(2))); //This is negative
			possibilitiesCome += setValue;
		}

		public override bool check(int maxStillToCome)
		{
			if (maxStillToCome == 0)
			{
				return currentSetValue > manager.GlobalBest;
			}
			double optimisticOutlook = -(wholeSize - possibilitiesCome) / (double) wholeSize * (Math.Log((wholeSize - possibilitiesCome) / ((double) wholeSize * maxStillToCome)) / Math.Log(2));
			return currentSetValue + optimisticOutlook > manager.GlobalBest;
		}

		public override void reset()
		{
			base.reset();
			possibilitiesCome = 0;
		}

		public static double getDefault(Config config)
		{
			//return (int) -Math.ceil(Math.log(1 / (config.pegs * (config.pegs + 3) / 2)) / Math.log(2));
			return 0;
		}
	}

}