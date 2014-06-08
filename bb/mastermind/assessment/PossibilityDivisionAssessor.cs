using System.Collections.Generic;

namespace bb.mastermind.assessment
{

	/// <summary>
	/// Created by BB on 01.05.2014.
	/// This assumes that the values are always positive and should be minimized. Implementation may override that behavior.
	/// </summary>
	public abstract class PossibilityDivisionAssessor
	{
		protected internal readonly AssessmentManager manager;
		protected internal double currentSetValue = 0;

		public PossibilityDivisionAssessor(AssessmentManager man)
		{
			this.manager = man;
		}

		public abstract void updateNextIndividual(int setValue);

		public abstract bool check(int maxStillToCome);

		public virtual bool checkAll(IList<int?> lst, int maxStillToCome)
		{
			foreach (int? aLst in lst)
			{
				updateNextIndividual(aLst.Value);
				if (!check(--maxStillToCome))
				{
					return false;
				}
			}
			return true;
		}

		public virtual double BestValue
		{
			get
			{
				return manager.GlobalBest;
			}
		}

		public virtual double CurrentSetValue
		{
			get
			{
				return currentSetValue;
			}
		}

		public virtual void reset()
		{
			currentSetValue = 0;
		}

		public virtual void updateBestValue()
		{
			manager.GlobalBest = currentSetValue;
		}
	}

}