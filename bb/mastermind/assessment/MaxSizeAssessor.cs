namespace bb.mastermind.assessment
{

	using Config = bb.mastermind.core.Config;

	/// <summary>
	/// Created by BB on 01.05.2014.
	/// </summary>
	public class MaxSizeAssessor : PossibilityDivisionAssessor
	{

		public MaxSizeAssessor(AssessmentManager man) : base(man)
		{
		}

		public override void updateNextIndividual(int setValue)
		{
			if (setValue >= currentSetValue)
			{
				currentSetValue = setValue;
			}
		}

		public override bool check(int maxStillToCome)
		{
			return currentSetValue < manager.GlobalBest;
		}

		public static double getDefault(Config config)
		{
			return config.combinations;
		}
	}

}