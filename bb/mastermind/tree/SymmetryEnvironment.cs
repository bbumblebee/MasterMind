using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;

namespace bb.mastermind.tree
{

	using Config = bb.mastermind.core.Config;


	/// <summary>
	/// Created by BB on 01.05.2014.
	/// </summary>
	public class SymmetryEnvironment
	{
		public const string alphabet = "abcdefghijklmnop";

        
		private readonly MoveNode.GradeNode lowestNode;
		private readonly Config config;
        private readonly ConcurrentDictionary<string, byte> SymmetrySet = new ConcurrentDictionary<string, byte>();
		private readonly bool[] colorFalse;
		private readonly bool[] colorUsed;

		public SymmetryEnvironment(MoveNode.GradeNode lowestNode)
		{
			this.lowestNode = lowestNode;
			config = lowestNode.MoveNode.config;

			colorFalse = new bool[config.colors];
			colorUsed = new bool[config.colors];
			init();
		}

		private void init()
		{
			MoveNode.GradeNode currentNode = lowestNode;
			while (true)
			{
				if (currentNode is RootNode.RootGrade)
				{
					break;
				}
				int[] pegs = lowestNode.MoveNode.Move;
				if (currentNode.gradeWhite == 0 && currentNode.gradeBlack == 0)
				{
					foreach (int peg in pegs)
					{
						colorFalse[peg] = true;
					}
				}
				if (currentNode.gradeWhite + currentNode.gradeBlack == config.pegs)
				{
					for (int i = 0; i < colorFalse.Length; i++)
					{
						colorFalse[i] = true;
					}
					foreach (int peg in pegs)
					{
						colorFalse[peg] = false;
					}
				}
				foreach (int peg in pegs)
				{
					colorUsed[peg] = true;
				}
				currentNode = currentNode.ParentGradeNode;
			}
		}

		public virtual bool checkSymmetry(MoveNode move)
		{
			StringBuilder SymmetryString = new StringBuilder(config.pegs);
			int UnusedNumber = 0;
			bool anySymmetries = false;
			bool allUnused = true;
			int[] currentSet = move.Move;

			for (int i = 0; i < currentSet.Length; i++) // loop through
			{
				// the currently
				// tested move
				int peg = currentSet[i]; // select a peg
				if (!colorUsed[peg]) // if that color hasn't been used yet
				{
					// --> free color
					anySymmetries = true;
					for (int z = 0; z < i; z++) // check whether we already had
					{
						// that color marked unused
						if (alphabet.Contains(SymmetryString[z] + "") && currentSet[z] == peg)
						{
							SymmetryString.Append(SymmetryString[z]);
							goto pegLoopContinue;
						}
					}
					SymmetryString.Append(alphabet[UnusedNumber]);
					UnusedNumber++;
				}
				else if (colorFalse[peg])
				{
					SymmetryString.Append("z");
					anySymmetries = true;
					allUnused = false;
				}
				else
				{
					SymmetryString.Append(peg);
					allUnused = false;
				}
				pegLoopContinue:;
			}
			if (anySymmetries)
			{
				string sym = SymmetryString.ToString();
                if (!SymmetrySet.TryAdd(sym, 0))
                    return false;
				if (allUnused)
				{
					permutation("", sym);
				}
			}
			return true;
		}

		private void permutation(string prefix, string str)
		{
			int n = str.Length;
			if (n == 0)
			{
				SymmetrySet.GetOrAdd(swapIfNeeded(prefix), 0);
			}
			else
			{
				for (int i = 0; i < n; i++)
				{
					permutation(prefix + str[i], str.Substring(0, i) + StringHelperClass.SubstringSpecial(str, i + 1, n));
				}
			}
		}

		private string swapIfNeeded(string s)
		{
			int nextIndex = 0;
			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];
				int letter = alphabet.IndexOf(c);
				if (letter < 0)
				{
					continue;
				}
				if (letter > nextIndex)
				{
					s = swap(s, c, alphabet[nextIndex]);
					nextIndex++;
				}
			}
			return s;
		}

		private string swap(string s, char a, char b)
		{
			StringBuilder str = new StringBuilder(s);
			for (int i = 0; i < str.Length; i++)
			{
				if (str[i] == a)
				{
					str.Remove(i, 1);
					str.Insert(i, b);
				}
				else if (str[i] == b)
				{
					str.Remove(i, 1);
					str.Insert(i, a);
				}
			}
			return str.ToString();
		}
	}

}