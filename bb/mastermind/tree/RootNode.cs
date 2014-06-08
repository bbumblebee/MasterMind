using System.Collections.Generic;

namespace bb.mastermind.tree
{

	using Config = bb.mastermind.core.Config;

	/// <summary>
	/// Created by BB on 02.05.2014.
	/// </summary>
	public class RootNode : MoveNode
	{

		public RootNode(Config conf) : base((short) 0, null, conf)
		{
		}

		public class RootGrade : GradeNode
		{
			private readonly RootNode outerInstance;

			public RootGrade(RootNode outerInstance) : base(outerInstance, 0)
			{
				this.outerInstance = outerInstance;
			}

			protected internal override void init()
			{
				followingMoves = new HashSet<MoveNode>();
				extractGrades();
			}

			public override int PossibilitySize
			{
				get
				{
					return outerInstance.config.combinations;
				}
			}
		}
	}

}