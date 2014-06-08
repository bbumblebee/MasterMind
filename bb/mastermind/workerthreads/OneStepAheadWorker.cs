namespace bb.mastermind.workerthreads
{

	using PossibilityDivisionAssessor = bb.mastermind.assessment.PossibilityDivisionAssessor;
	using MoveNode = bb.mastermind.tree.MoveNode;

	/// <summary>
	/// Created by BB on 03.05.2014.
	/// </summary>
	public class OneStepAheadWorker
	{
		private readonly ResultSet resultSet;
		private readonly PossibilityDivisionAssessor assess;
		private readonly MoveNode testMove;

		public OneStepAheadWorker(ResultSet resultSet, PossibilityDivisionAssessor assess, MoveNode testMove)
		{
			this.resultSet = resultSet;
			this.assess = assess;
			this.testMove = testMove;
		}

		public void run()
		{
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
		}
	}

}