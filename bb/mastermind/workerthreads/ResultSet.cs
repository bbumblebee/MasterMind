namespace bb.mastermind.workerthreads
{
    using System.Threading;
	using Config = bb.mastermind.core.Config;
	using MoveNode = bb.mastermind.tree.MoveNode;
	using SymmetryEnvironment = bb.mastermind.tree.SymmetryEnvironment;

	/// <summary>
	/// Created by BB on 03.05.2014.
	/// </summary>
	public class ResultSet
	{
		public readonly SymmetryEnvironment symmetryEnvironment;
		public readonly Config config;

		protected internal MoveNode bestMove;
        protected internal readonly ReaderWriterLock moveLock = new ReaderWriterLock();

		public ResultSet(SymmetryEnvironment symmetryEnvironment, Config config)
		{
			this.symmetryEnvironment = symmetryEnvironment;
			this.config = config;
		}

		public virtual MoveNode BestMove
		{
			get
			{
                moveLock.AcquireReaderLock(-1);
				try
				{
					return bestMove;
				}
				finally
				{
                    moveLock.ReleaseReaderLock();
				}
    
			}
			set
			{
                moveLock.AcquireWriterLock(-1);
				try
				{
					this.bestMove = value;
				}
				finally
				{
                    moveLock.ReleaseWriterLock();
				}
    
			}
		}

	}

}