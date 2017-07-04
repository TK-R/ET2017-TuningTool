using System.Windows;
using System.Collections.Generic;
using ET2017_TuningTool.Model;
using System.Linq;

namespace ET2017_TuningTool.Model
{
    /// <summary>
    /// �_�C�L�X�g���@�ɂ�����}
    /// </summary>
    public class DijkstraBranch
	{
        /// <summary>
        /// �n�_
        /// </summary>
        public readonly DijkstraNode Node1;

        /// <summary>
        /// �I�_
        /// </summary>
        public readonly DijkstraNode Node2;
        
        /// <summary>
        ///  ����
        /// </summary>
		public readonly double Distance;	

		public DijkstraBranch( DijkstraNode node1, DijkstraNode node2, double dDistance )
		{
			Node1 = node1;
			Node2 = node2;
			Distance = dDistance;
		}
	}

    /// <summary>
    /// �_�C�L�X�g���@�̃m�[�h
    /// </summary>
    public class DijkstraNode
	{
        /// <summary>
        /// �o�H�m����
        /// </summary>
        public enum NodeStatus
		{
            /// <summary>
            /// ���m��
            /// </summary>
			NotYet,		
            /// <summary>
            /// ���m��
            /// </summary>
			Temporary,
            /// <summary>
            /// �m��
            /// </summary>
			Completed
		}

        /// <summary>
        /// ����
        /// </summary>
		public double Distance { set; get; }

        public int NodeNo { set; get; }
		public DijkstraNode SourceNode { set; get; }
		public NodeStatus Status { set; get; }
		public Point Position { set; get; }
    }

	/// <summary>�_�C�N�X�g���@�A���S���Y������</summary>
	public class Dijkstra
	{

        /// <summary>
        /// �}�̃��X�g
        /// </summary>
		public List<DijkstraBranch> Branches { set; get; } = new List<DijkstraBranch>();

        /// <summary>
        /// �m�[�h�̃��X�g
        /// </summary>
		public List<DijkstraNode> Nodes { set; get; } = new List<DijkstraNode>();

        public Dijkstra( int nNodeCount )
		{
            Nodes = new List<DijkstraNode>();
			for( int i = 0; i < nNodeCount; i++ )
			{
                Nodes.Add( new DijkstraNode() );
			}
		}


        public Dijkstra(Line[] lines)
        {
            // �m�[�h���̔z����擾
            Nodes = lines.Select(p => new DijkstraNode { Position = p.WayPoint, NodeNo = p.No }).ToList();

            foreach (var line in lines)
            {
                int i = 0;

                foreach (var near in line.NearLineNo)
                {
                    var distance = line.GetDistance(lines[near]);

                    if (near == 0) continue;
                    var branch = new DijkstraBranch(Nodes[near], Nodes[line.No], distance);

                    // �n�_�ƏI�_���t�Ȃ���
                    if (Branches.Any(b => b.Node1 == branch.Node2 && b.Node2 == branch.Node1))
                        continue;

                    Branches.Add(branch);

                    i++;
                }
            }
        }

        public int[] GetRouteNodeNo(int startNodeNo, int endNodeNo)
        {
            List<int> route = new List<int>();

            Execute(Nodes[startNodeNo]);

            var n = Nodes[endNodeNo];
            while (n != null && n.SourceNode != null)
            {
                n = n.SourceNode;
                route.Add(n.NodeNo);
            }


            // ���[�g���t�������āA�Ō�ɏI�_�̃m�[�h��ǉ�
            route.Reverse();
            route.Add(endNodeNo);

            return route.ToArray();
        }

        
		/// <summary>�ŒZ�o�H�v�Z���s</summary>
		/// <param name="nStart">�X�^�[�g�m�[�h�̃C���f�b�N�X</param>
		/// <returns>������</returns>
		public int Execute( DijkstraNode startNode )
		{
			if( startNode == null ) return 0;

			// �S�ߓ_�ŋ����𖳌���C���m��Ƃ���
			foreach( DijkstraNode node in Nodes)
			{
				node.Distance = int.MaxValue;
				node.Status = DijkstraNode.NodeStatus.NotYet;
				node.SourceNode = null;
			}

			// �n�_�ł͋����̓[���C�m��Ƃ���
			startNode.Distance = 0;
			startNode.Status = DijkstraNode.NodeStatus.Completed;
			startNode.SourceNode = null;

			DijkstraNode scanNode = startNode;
			int nCount = 0;
			while( scanNode != null )
			{
				UpdateNodeProp( scanNode );		// �אړ_�̃m�[�h���X�V
				scanNode = FindMinNode();		// �ŒZ�o�H�����m�[�h������
				nCount++;
			}
			return nCount;
		}

		/// <summary>�w��m�[�h�ɗאڂ���m�[�h�̍ŒZ�������v�Z����B</summary>
		/// <param name="sourceNode">�w��m�[�h</param>
		private void UpdateNodeProp( DijkstraNode sourceNode )
		{
			if(Branches == null ) return;

			DijkstraNode destinationNode;
			double dTotalDistance;

			// �u�����`���X�g�̒�����w��m�[�h�Ɋ֘A���Ă�����̂�����
			foreach( DijkstraBranch branch in Branches)
			{
				destinationNode = null;
				if( branch.Node1.Equals( sourceNode ) == true )
				{
					destinationNode = branch.Node2;
				}
				else if( branch.Node2.Equals( sourceNode ) == true )
				{
					destinationNode = branch.Node1;
				}
				else
				{
					continue;
				}
				// �אڃm�[�h���������B

				// �m�肵�Ă���m�[�h�͖����B
				if( destinationNode.Status == DijkstraNode.NodeStatus.Completed ) continue;

				// �m�[�h�̌��݂̋����Ɏ}�̋�����������B
				dTotalDistance = sourceNode.Distance + branch.Distance;

				if( destinationNode.Distance <= dTotalDistance ) continue;

				// ���݂̉��̍ŒZ�����������ƒZ���s�������������B
				destinationNode.Distance = dTotalDistance;	// ���̍ŒZ����
				destinationNode.SourceNode = sourceNode;
				destinationNode.Status = DijkstraNode.NodeStatus.Temporary;
			}
		}

		/// <summary>���m��m�[�h�̒��ōŒZ�o�H�����m�[�h������</summary>
		/// <returns>�ŒZ�o�H�����m�[�h</returns>
		private DijkstraNode FindMinNode()
		{
			double dMinDistance = int.MaxValue; // �ŏ��l���ŏ�������Ƃ���

			DijkstraNode Finder = null;

			// �S�Ẵm�[�h���`�F�b�N
			foreach( DijkstraNode node in Nodes )
			{
				// �m�肵���m�[�h�͖���
				if( node.Status == DijkstraNode.NodeStatus.Completed ) continue;

				// ���m��̃m�[�h�̒��ōŒZ�����̃m�[�h��T��
				if( node.Distance >= dMinDistance ) continue;

				dMinDistance = node.Distance;
				Finder = node;
			}
			if( Finder == null ) return null;

			// �ŒZ�������������B���̃m�[�h�͊m��I
			Finder.Status = DijkstraNode.NodeStatus.Completed;
			return Finder;

		}

	}
}











