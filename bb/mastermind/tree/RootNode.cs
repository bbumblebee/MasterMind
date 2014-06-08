using System.Collections.Generic;

namespace bb.mastermind.tree
{

	using Config = bb.mastermind.core.Config;

	/// <summary>
	/// Created by BB on 02.05.2014.
	/// </summary>
	public class RootNode : MoveNode
	{

		public RootNode(Config conf) : base(new int[]{}, null, conf)
		{
		}

		public class RootGrade : GradeNode
		{

			public RootGrade(RootNode outerInstance) : base(outerInstance, 0)
			{
			}

			protected internal override void init()
			{
                extractGrades();
				followingMoves = new HashSet<MoveNode>();
                for(int i = 0; i < outerInstance.config.combinations; ++i)
                {
                    int[] move = new int[outerInstance.config.pegs];
                    for (int j = 0; j < outerInstance.config.pegs; ++j) {
			            move[j] = (int) ( System.Math.Floor( i / System.Math.Pow(outerInstance.config.colors, j ) ) % outerInstance.config.colors );
		            }
                    base.possibilities.Add(move);
                }
			}

			public override int PossibilitySize
			{
				get
				{
					return outerInstance.config.combinations;
				}
			}


            public void forEachParallelPossibility(System.Action<int[]> a)
            {
                System.Threading.Tasks.Parallel.ForEach(possibilities, a);
            }
		}
	}

}