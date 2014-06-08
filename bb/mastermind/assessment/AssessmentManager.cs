using System;
using System.Threading;
using System.Reflection;

namespace bb.mastermind.assessment
{

	using Config = bb.mastermind.core.Config;


	/// <summary>
	/// Created by BB on 03.05.2014.
	/// </summary>
	public class AssessmentManager
	{
		internal double globalBest;
		private readonly Config config;
		private readonly ReaderWriterLock @lock = new ReaderWriterLock();
		private ConstructorInfo constructor;
		public readonly int wholeSize;

		public AssessmentManager(Config config, Type c, int wholeSize)
		{
			this.config = config;
			globalBest = 0;
			this.wholeSize = wholeSize;
			globalBest = (double) c.GetMethod("getDefault", new Type[] {typeof(Config)}).Invoke(null, new Object[] {config});
			constructor = c.GetConstructor(new Type[] {typeof(AssessmentManager)});
			
		}

		public virtual PossibilityDivisionAssessor getSetAssessor()
		{
                return (PossibilityDivisionAssessor) constructor.Invoke(new object[] { (object)this });
		}


		public virtual double GlobalBest
		{
			get
			{
				try
				{
					@lock.AcquireReaderLock(-1);
					return globalBest;
				}
				finally
				{
					@lock.ReleaseReaderLock();
				}
			}
			set
			{
				try
				{
					@lock.AcquireWriterLock(-1);
					this.globalBest = value;
				}
				finally
				{
					@lock.ReleaseWriterLock();
				}
			}
		}


	}

}