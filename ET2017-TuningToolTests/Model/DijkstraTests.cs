using Microsoft.VisualStudio.TestTools.UnitTesting;
using DijkstraMothod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ET2017_TuningTool.Model;

namespace DijkstraMothod.Tests
{
    [TestClass()]
    public class DijkstraTests
    {
        [TestMethod()]
        public void ExecuteTest()
        {
            Dijkstra di = new Dijkstra(26);
            var lines = BlockMoveRule.LineArray;
            // ノード情報の配列を取得
            var node = lines.Select(t => t.WayPoint).Select(p => new DijkstraNode { Position = p }).ToList();
            di.Nodes = node;

            foreach (var line in lines)
            {
                int i = 0;

                foreach (var near in line.NearLineNo)
                {
                    var distance = line.GetDistance(lines[near]);

                    if (near == 0) continue;
                    var branch = new DijkstraBranch(node[near], node[line.No], distance);

                    // 始点と終点が逆なだけ
                    if (di.Branches.Any(b => b.Node1 == branch.Node2 && b.Node2 == branch.Node1))
                        continue;

                    di.Branches.Add(branch);

                    i++;
                }
            }

            var n = node[2];

            di.Execute(node[0]);
            int route = 0;
            while (n != null && n.SourceNode != null)
            {
                Console.WriteLine(n.Position.X + ", " + n.Position.Y);
                n = n.SourceNode;
                route++;
            }
            Console.WriteLine(route);

        }

        [TestMethod()]
        public void GetRouteNodeNoTest()
        {
            Dijkstra di = new Dijkstra(BlockMoveRule.LineArray);
            var route = di.GetRouteNodeNo(0, 2);

            CollectionAssert.AreEqual(new int[] { 0, 4, 5, 6, 7, 2 }, route);
            
            route = di.GetRouteNodeNo(0, 23);
            CollectionAssert.AreEqual(new int[] { 0, 4, 10, 17, 23 }, route);

            route = di.GetRouteNodeNo(11, 22);
            CollectionAssert.AreEqual(new int[] { 11, 19, 20, 22 }, route);
        }
    }
}