using System;
using System.Collections.Generic;
using System.Linq;

namespace bb.mastermind.tree
{

	using Config = bb.mastermind.core.Config;


	/// <summary>
	/// Created by BB on 01.05.2014.
	/// </summary>
	public class MoveNode
	{
		public readonly Config config;
		private readonly int[] move;
		private double assessmentValue;
		private readonly GradeNode parent;
		public readonly HashSet<GradeNode> subGradeNodeList;

		/* Construction and TreeAdding Methods */
		protected internal MoveNode(int[] move, GradeNode parent, Config config)
		{
			this.move = move;
			this.parent = parent;
            subGradeNodeList = new HashSet<GradeNode>();
			this.config = config;
		}

		public virtual GradeNode addSubGrade(int gradeID)
		{
			GradeNode subNode = new GradeNode(this, gradeID);
			subGradeNodeList.Add(subNode);
			return subNode;
		}

		/* Functionality Methods */

		/// <summary>
		/// Used for GamePlay and Simulation only.
		/// </summary>
		/// <param name="move2"> the result, against which this move should be graded </param>
		/// <returns> a byte array of length 2 with a[0] = blacks and a[1] = whites </returns>
		public virtual int[] gradeThisMove(int[] move2)
		{
			int[] move1 = Move;
			int checkBlack = 0;
			int checkWhite = 0;

			for (int i = 0; i < move1.Length; i++)
			{
				if (move1[i] == move2[i])
				{
					checkBlack++;
				}
			}

			sbyte[] colors = new sbyte[config.colors];
			for (int i = 0; i < config.colors; i++)
			{
				colors[move1[i]]++;
				colors[move2[i]]--;
			}
			foreach (sbyte color in colors)
			{
				checkWhite += Math.Max(color, (sbyte)0);
			}
			checkWhite = config.pegs - checkWhite - checkBlack;
			return new int[]{ checkBlack, checkWhite};
		}

		public virtual GradeNode pruneSubGrades(int gradeID)
		{
			GradeNode future = null;
			foreach (GradeNode node in subGradeNodeList)
			{
				if (node.gradeID == gradeID)
				{
					future = node;
					break;
				}
			}
			subGradeNodeList.Clear();
			subGradeNodeList.Add(future);
			return future;
		}

		/* Getters */
		public virtual int[] Move
		{
			get
			{
				return move;
			}
		}

		public virtual double AssessmentValue
		{
			get
			{
				return assessmentValue;
			}
			set
			{
				this.assessmentValue = value;
			}
		}


		/* Rather Unimportant Implementations */
		public override bool Equals(object obj)
		{
			return obj is MoveNode && move.SequenceEqual(((MoveNode) obj).move);
		}

		public class GradeNode : IComparable<GradeNode>
		{
			protected readonly MoveNode outerInstance;


			protected internal readonly int gradeID;
			protected internal HashSet<int[]> possibilities;
			public HashSet<MoveNode> followingMoves;

			public sbyte gradeWhite;
			public sbyte gradeBlack;

			protected internal GradeNode(MoveNode outerInstance, int gradeID)
			{
				this.outerInstance = outerInstance;
				this.gradeID = gradeID;
                possibilities = new HashSet<int[]>();
                followingMoves = new HashSet<MoveNode>();
				init();
			}

			protected internal virtual void init()
			{
				extractGrades();
                ParentGradeNode.forEachPossibility(s => { if (checkSingleConsistency(s)) possibilities.Add(s); });
			}

			public virtual MoveNode addSubMove(int[] move)
			{
				MoveNode subMove = new MoveNode(move, this, outerInstance.config);
				followingMoves.Add(subMove);
				return subMove;
			}

			/// <summary>
			/// You have to call addSubNode(node) later!
			/// </summary>
			/// <param name="move">
			/// @return </param>
			public virtual MoveNode addTemporarySubMove(int[] move)
			{
				return new MoveNode(move, this, outerInstance.config);
			}

			public virtual void addSubNode(MoveNode node)
			{
				followingMoves.Add(node);
			}

			public virtual void unregister()
			{
				outerInstance.subGradeNodeList.Remove(this);
			}

			protected internal virtual void extractGrades()
			{
				gradeBlack = (sbyte) outerInstance.config.id2array[gradeID][0];
				gradeWhite = (sbyte) outerInstance.config.id2array[gradeID][1];
			}

			public override bool Equals(object obj)
			{
				return obj is GradeNode && this.gradeID == ((GradeNode) obj).gradeID;
			}

			public virtual int CompareTo(GradeNode o)
			{
				if (o.gradeID > this.gradeID)
				{
					return -1;
				}
				if (o.gradeID < this.gradeID)
				{
					return 1;
				}
				return 0;
			}

			public virtual bool checkSingleConsistency(int[] testMove)
			{
				int[] move1 = outerInstance.Move;
				int checkBlack = 0;
				int checkWhite = 0;

				for (int i = 0; i < move1.Length; i++)
				{
					if (move1[i] == testMove[i])
					{
						checkBlack++;
					}
				}
				if (checkBlack != gradeBlack)
				{
					return false;
				}

				sbyte[] colors = new sbyte[outerInstance.config.colors];
				for (int i = 0; i < outerInstance.config.pegs; i++)
				{
					colors[move1[i]]++;
					colors[testMove[i]]--;
				}
				foreach (sbyte color in colors)
				{
					checkWhite += Math.Max(color, (sbyte) 0);
				}
				checkWhite = outerInstance.config.pegs - checkWhite - checkBlack;
				return checkWhite == gradeWhite;
			}

            protected internal virtual void forEachPossibility(Action<int[]> a)
            {
                foreach(int[] t in possibilities) {
                    a(t);
                }
            }

			public virtual MoveNode MoveNode
			{
				get
				{
					return outerInstance;
				}
			}

			public virtual GradeNode ParentGradeNode
			{
				get
				{
					return outerInstance.parent;
				}
			}

			public virtual int PossibilitySize
			{
				get
				{
                    return possibilities.Count;
				}
			}
		}

	}

}