using System;

namespace bb.mastermind.core
{

	/// <summary>
	/// Created by BB on 01.05.2014.
	/// </summary>
	public class Config
	{
		public readonly int pegs;
		public readonly int colors;
		public readonly int combinations;

		public readonly int grades;
		public readonly int[][] id2array;
		public readonly int[][] array2id;

		public Config(int pegs, int colors)
		{
			this.pegs = pegs;
			this.colors = colors;

			this.combinations = (int) Math.Pow(colors, pegs);

			grades = pegs * (pegs + 3) / 2;
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: id2array = new int[grades][2];
			id2array = RectangularArrays.ReturnRectangularIntArray(grades, 2);
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: array2id = new int[pegs + 1][pegs + 1];
			array2id = RectangularArrays.ReturnRectangularIntArray(pegs + 1, pegs + 1);
			int currentID = 0;
			for (int i = 0; i <= pegs; i++)
			{
				for (int z = 0; z <= pegs - i; z++)
				{
					if (i == pegs - 1 && z == 1)
					{
						continue;
					}
					array2id[i][z] = currentID;
					id2array[currentID] = new int[]{i, z};
					currentID++;
				}
			}

		}

		public virtual int[] getSinglePegs(short move)
		{
			int[] result = new int[pegs];
			for (int i = 0; i < pegs; i++)
			{
				result[i] = getSinglePeg(move, i);
			}
			return result;
		}

		/// <summary>
		/// This is to be changed in a C++ Template accordingly. Move to Configurable
		/// </summary>
		/// <param name="move"> </param>
		/// <param name="position">
		/// @return </param>
		internal virtual int getSinglePeg(short move, int position)
		{
			return ((move >> position * 3)) & 7;
		}

		public virtual short makePegSet(int[] move)
		{
			short result = 0;
			for (int i = move.Length - 1; i >= 0; i--)
			{
				result <<= 3;
				result += (short)move[i];
			}
			return result;
		}
	}

}