using System;

namespace bb.mastermind.ui
{

	using EntropyAssessor = bb.mastermind.assessment.EntropyAssessor;
	using MaxSizeAssessor = bb.mastermind.assessment.MaxSizeAssessor;
	using PossibilityDivisionAssessor = bb.mastermind.assessment.PossibilityDivisionAssessor;
	using MasterMind = bb.mastermind.core.MasterMind;
	using MoveNode = bb.mastermind.tree.MoveNode;
    using System.Diagnostics;


	/// <summary>
	/// Created by BB on 01.05.2014.
	/// </summary>
	public class UserInterface
	{
		private MasterMind mm;
		private readonly int pegs = 5;
		private readonly int colors = 8;

		public virtual void start()
		{

	//        System.out.println("Please enter Number of Pegs and Colors:");
	//        pegs = scanner.nextInt();
	//        colors = scanner.nextInt();

			mm = new MasterMind(pegs, colors);
			Console.WriteLine("Initialized with: " + pegs + ", " + colors);
			movePrompt();
		}

		private void movePrompt()
		{
			while (true)
			{
				Console.WriteLine("MovePrompt - What do you want to do?");
				Console.WriteLine("a: Analyse next Move\ng: Enter graded Moves\nm: Enter following Move to check\nq: End Program");

				string input = Console.ReadLine();
				switch (input)
				{
					case "q":
						return;
					case "a":
						analysis();
						return;
					case "g":
						enterGradedMoves();
						return;
					default:
						Console.WriteLine("Invalid input");
					break;
				}
			}
		}

		private void gradePrompt()
		{
			while (true)
			{
				Console.WriteLine("GradePrompt - What do you want to do?");
				Console.WriteLine("g: Grade single Move\ns: Enter Solution and Simulate Game\nq: End Program");

				string input = Console.ReadLine();
				switch (input)
				{
					case "q":
						return;
					case "g":
						gradingPrompt();
						return;
					case "s":
						throw new System.NotSupportedException();
	//                    enterGradedMoves();
	//                    return;
					default:
						Console.WriteLine("Invalid input");
					break;
				}
			}
		}

		private void gradingPrompt()
		{
			Console.WriteLine("Grading Prompt - Enter Grades: blacks,whites");
            MoveNode move = mm.CurrentPos.followingMoves.GetEnumerator().Current;
			Console.WriteLine("Move to Grade: " + String.Join(",", move.SinglePegs));
			while (true)
			{
				string input = Console.ReadLine();
				string[] bytes = input.Split(",", true);
				if (bytes.Length != 2)
				{
					Console.WriteLine("Unsupported number of inputs");
					continue;
				}
				mm.selectGrade(Convert.ToInt32(bytes[0].Trim()), Convert.ToInt32(bytes[1].Trim()));
				break;
			}
		}

		private void analysis()
		{
			Console.WriteLine("Starting Root Analysis...");
			Type assessClass = typeof(MaxSizeAssessor);
            Stopwatch st = Stopwatch.StartNew();
			MoveNode mn = mm.runOneStepAheadAnalysis(assessClass);
            Console.WriteLine("Time Taken: " + st.ElapsedMilliseconds + " ms");
			printResultMove(mn);
			gradePrompt();
		}

		private void enterGradedMoves()
		{
			Console.WriteLine("Please enter moves in a row using: a,b,c,...,blacks,whites.");
			Console.WriteLine("q: Returns one layer below");
			while (true)
			{
				string input = Console.ReadLine();
				if (input.Equals("q"))
				{
					movePrompt();
					return;
				}
				string[] bytes = input.Split(",", true);
				if (bytes.Length != pegs + 2)
				{
					Console.WriteLine("Unsupported number of inputs");
					continue;
				}
                string[] pegsr = new string[pegs];
                Array.Copy(bytes, pegsr, pegs);
				mm.addGradedMove(parseAll(pegsr), Convert.ToInt32(bytes[pegs].Trim()), Convert.ToInt32(bytes[pegs + 1].Trim()));
			}
		}

		/* Helper Functions */
		private void printResultMove(MoveNode mn)
		{
			Console.WriteLine("Calculated Best Move: " + String.Join(", ", mn.SinglePegs));
			Console.WriteLine("Value attributed to this move: " + mn.AssessmentValue);
			Console.WriteLine("Additional Information: \np: Print Possibility-Size-List ");
			Console.WriteLine("q: Exits this prompt, and continues with further calculation");
			while (true)
			{
				string input = Console.ReadLine();
				if (input.Equals("q"))
				{
					return;
				}
				else if (input.Equals("p"))
				{
					foreach (MoveNode.GradeNode r in mn.subGradeNodeList)
					{
						Console.WriteLine("(b: " + r.gradeBlack + ", w: " + r.gradeWhite + ") " + r.PossibilitySize);
					}
				}
			}
		}

		private static int[] parseAll(string[] s)
		{
			int[] result = new int[s.Length];
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = Convert.ToInt32(s[i].Trim());
			}
			return result;
		}
	}

}