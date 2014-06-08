using System;

namespace bb.mastermind.core
{

	using AssessmentManager = bb.mastermind.assessment.AssessmentManager;
	using PossibilityDivisionAssessor = bb.mastermind.assessment.PossibilityDivisionAssessor;
	using MoveNode = bb.mastermind.tree.MoveNode;
	using RootNode = bb.mastermind.tree.RootNode;
	using SymmetryEnvironment = bb.mastermind.tree.SymmetryEnvironment;
	using ResultSet = bb.mastermind.workerthreads.ResultSet;
    using System.Threading.Tasks;


	/// <summary>
	/// Created by BB on 01.05.2014.
	/// </summary>
	public class MasterMind
	{
		private readonly Config config;
		private readonly MoveNode.GradeNode root;
		private MoveNode.GradeNode currentPos;

		public MasterMind(int pegs, int colors)
		{
			config = new Config(pegs, colors);
			root = new RootNode.RootGrade((new RootNode(config)));
			currentPos = root;
		}

		/* Getters and Setters */

		public virtual MoveNode.GradeNode CurrentPos
		{
			get
			{
				return currentPos;
			}
		}

		/* Functionality Methods */
		public virtual void selectGrade(int blacks, int whites)
		{
			currentPos = currentPos.followingMoves.GetEnumerator().Current.pruneSubGrades(config.array2id[blacks][whites]);
		}

		public virtual void addGradedMove(int[] pegs, int whites, int blacks)
		{
			MoveNode tmpMove = currentPos.addSubMove(config.makePegSet(pegs));
			currentPos = tmpMove.addSubGrade(config.array2id[blacks][whites]);
		}

		public virtual MoveNode runOneStepAheadAnalysis(Type assessor)
		{
			AssessmentManager manager = new AssessmentManager(config, assessor, currentPos.PossibilitySize);
			SymmetryEnvironment symmetryEnvironment = new SymmetryEnvironment(currentPos);
			ResultSet resultSet = new ResultSet(symmetryEnvironment, config);
            Parallel.For(short.MinValue, short.MinValue + config.combinations, m => {
                MoveNode testMove = currentPos.addTemporarySubMove((short)m);
                PossibilityDivisionAssessor assess = manager.getSetAssessor();
                if (!resultSet.symmetryEnvironment.checkSymmetry(testMove))
                {
                    return;
                }
                for (int i = 0; i < resultSet.config.grades; i++)
                {
                    MoveNode.GradeNode tempGrade = testMove.addSubGrade(i);
                    if (tempGrade.PossibilitySize == 0)
                    {
                        tempGrade.unregister();
                        continue;
                    }
                    assess.updateNextIndividual(tempGrade.PossibilitySize);
                    if (!assess.check(resultSet.config.grades - i - 1))
                    {
                        return;
                    }
                }
                lock (resultSet)
                {
                    if (!assess.check(0))
                    {
                        return;
                    }
                    assess.updateBestValue();
                    testMove.AssessmentValue = assess.CurrentSetValue;
                    resultSet.BestMove = testMove;
                }
            });

			currentPos.addSubNode(resultSet.BestMove);
			return resultSet.BestMove;
		}

	}
	 /*
	     public MoveNode runOneStepAheadAnalysis(Class<? extends PossibilityDivisionAssessor> assessor) {
	        AssessmentManager manager = new UnsynchronizedAssessmentManager(config, assessor);
	        PossibilityDivisionAssessor assess = manager.getSetAssessor();
	        SymmetryEnvironment symmetryEnvironment = new SymmetryEnvironment(currentPos);
	
	        MoveNode currentMove = currentPos.addTemporarySubMove((short) 0);
	        outer:
	        for (AllPossibilityIterator it = new AllPossibilityIterator(config.combinations); it.hasNext(); ) {
	            assess.reset();
	            MoveNode tempMove = currentPos.addTemporarySubMove(it.next());
	            if (!symmetryEnvironment.checkSymmetry(tempMove))
	                continue;
	            for (int i = 0; i < config.grades; i++) {
	                MoveNode.GradeNode tempGrade = tempMove.addSubGrade(i);
	                if (tempGrade.getPossibilitySize() == 0) {
	                    tempGrade.unregister();
	                    continue;
	                }
	                if (!assess.checkNextIndividual(tempGrade.getPossibilitySize(), config.grades - i - 1))
	                    continue outer;
	            }
	            assess.updateBestValue();
	            currentMove = tempMove;
	        }
	        currentPos.addSubNode(currentMove);
	        currentMove.setAssessmentValue(assess.getBestValue());
	        return currentMove;
	    }
	
	
	
	
	  */
}