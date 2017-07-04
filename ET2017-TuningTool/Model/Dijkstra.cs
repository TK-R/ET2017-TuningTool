using System.Windows;
using System.Collections.Generic;
using ET2017_TuningTool.Model;
using System.Linq;

namespace ET2017_TuningTool.Model
{
    /// <summary>
    /// ダイキストラ法における枝
    /// </summary>
    public class DijkstraBranch
	{
        /// <summary>
        /// 始点
        /// </summary>
        public readonly DijkstraNode Node1;

        /// <summary>
        /// 終点
        /// </summary>
        public readonly DijkstraNode Node2;
        
        /// <summary>
        ///  距離
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
    /// ダイキストラ法のノード
    /// </summary>
    public class DijkstraNode
	{
        /// <summary>
        /// 経路確定状態
        /// </summary>
        public enum NodeStatus
		{
            /// <summary>
            /// 未確定
            /// </summary>
			NotYet,		
            /// <summary>
            /// 仮確定
            /// </summary>
			Temporary,
            /// <summary>
            /// 確定
            /// </summary>
			Completed
		}

        /// <summary>
        /// 距離
        /// </summary>
		public double Distance { set; get; }

        public int NodeNo { set; get; }
		public DijkstraNode SourceNode { set; get; }
		public NodeStatus Status { set; get; }
		public Point Position { set; get; }
    }

	/// <summary>ダイクストラ法アルゴリズム実装</summary>
	public class Dijkstra
	{

        /// <summary>
        /// 枝のリスト
        /// </summary>
		public List<DijkstraBranch> Branches { set; get; } = new List<DijkstraBranch>();

        /// <summary>
        /// ノードのリスト
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
            // ノード情報の配列を取得
            Nodes = lines.Select(p => new DijkstraNode { Position = p.WayPoint, NodeNo = p.No }).ToList();

            foreach (var line in lines)
            {
                int i = 0;

                foreach (var near in line.NearLineNo)
                {
                    var distance = line.GetDistance(lines[near]);

                    if (near == 0) continue;
                    var branch = new DijkstraBranch(Nodes[near], Nodes[line.No], distance);

                    // 始点と終点が逆なだけ
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


            // ルートを逆純化して、最後に終点のノードを追加
            route.Reverse();
            route.Add(endNodeNo);

            return route.ToArray();
        }

        
		/// <summary>最短経路計算実行</summary>
		/// <param name="nStart">スタートノードのインデックス</param>
		/// <returns>検索回数</returns>
		public int Execute( DijkstraNode startNode )
		{
			if( startNode == null ) return 0;

			// 全節点で距離を無限大，未確定とする
			foreach( DijkstraNode node in Nodes)
			{
				node.Distance = int.MaxValue;
				node.Status = DijkstraNode.NodeStatus.NotYet;
				node.SourceNode = null;
			}

			// 始点では距離はゼロ，確定とする
			startNode.Distance = 0;
			startNode.Status = DijkstraNode.NodeStatus.Completed;
			startNode.SourceNode = null;

			DijkstraNode scanNode = startNode;
			int nCount = 0;
			while( scanNode != null )
			{
				UpdateNodeProp( scanNode );		// 隣接点のノードを更新
				scanNode = FindMinNode();		// 最短経路をもつノードを検索
				nCount++;
			}
			return nCount;
		}

		/// <summary>指定ノードに隣接するノードの最短距離を計算する。</summary>
		/// <param name="sourceNode">指定ノード</param>
		private void UpdateNodeProp( DijkstraNode sourceNode )
		{
			if(Branches == null ) return;

			DijkstraNode destinationNode;
			double dTotalDistance;

			// ブランチリストの中から指定ノードに関連しているものを検索
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
				// 隣接ノードを見つけた。

				// 確定しているノードは無視。
				if( destinationNode.Status == DijkstraNode.NodeStatus.Completed ) continue;

				// ノードの現在の距離に枝の距離を加える。
				dTotalDistance = sourceNode.Distance + branch.Distance;

				if( destinationNode.Distance <= dTotalDistance ) continue;

				// 現在の仮の最短距離よりもっと短い行き方を見つけた。
				destinationNode.Distance = dTotalDistance;	// 仮の最短距離
				destinationNode.SourceNode = sourceNode;
				destinationNode.Status = DijkstraNode.NodeStatus.Temporary;
			}
		}

		/// <summary>未確定ノードの中で最短経路をもつノードを検索</summary>
		/// <returns>最短経路をもつノード</returns>
		private DijkstraNode FindMinNode()
		{
			double dMinDistance = int.MaxValue; // 最小値を最初無限大とする

			DijkstraNode Finder = null;

			// 全てのノードをチェック
			foreach( DijkstraNode node in Nodes )
			{
				// 確定したノードは無視
				if( node.Status == DijkstraNode.NodeStatus.Completed ) continue;

				// 未確定のノードの中で最短距離のノードを探す
				if( node.Distance >= dMinDistance ) continue;

				dMinDistance = node.Distance;
				Finder = node;
			}
			if( Finder == null ) return null;

			// 最短距離を見つけた。このノードは確定！
			Finder.Status = DijkstraNode.NodeStatus.Completed;
			return Finder;

		}

	}
}











