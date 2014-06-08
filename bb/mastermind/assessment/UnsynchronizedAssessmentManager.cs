using System;

namespace bb.mastermind.assessment
{

	using Config = bb.mastermind.core.Config;

	/// <summary>
	/// Created by BB on 03.05.2014.
	/// </summary>
	public class UnsynchronizedAssessmentManager : AssessmentManager
	{
		public UnsynchronizedAssessmentManager(Config config, Type c, int maxSize) : base(config, c, maxSize)
		{
		}

		public override double GlobalBest
		{
			get
			{
				return globalBest;
			}
			set
			{
				this.globalBest = value;
			}
		}

	}

}