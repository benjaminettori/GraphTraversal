using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphTraversal
{
    class Program
    {
        static void Main(string[] args)
        {
            var map = ImmutableDirectedGraph<string, Directions>.Empty
                .AddEdge("Troll Room", "East West Passage", Directions.East)
                .AddEdge("East West Passage", "Round Room", Directions.East)
                .AddEdge("Round Room", "East West Passage", Directions.West)
                .AddEdge("East West Passage", "Troll Room", Directions.West);
                
            var test = map.GetEdgeConnections("East West Passage");

            foreach(var connection in test)
            {
                Console.WriteLine("East West Passage connected to {0} through {1}", connection.Value, connection.Key);
            }

            map.PrintGraph();
           
            Console.WriteLine("Removing Troll Room node");
            map = map.RemoveNode("Troll Room");
            map.PrintGraph();
        }
    }
}
