using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphTraversal
{
    public struct ImmutableDirectedGraph<N, E>
    {
        private ImmutableDictionary<N, ImmutableDictionary<E, N>> graph;

        public readonly static ImmutableDirectedGraph<N, E> Empty = new ImmutableDirectedGraph<N, E>(ImmutableDictionary<N, ImmutableDictionary<E, N>>.Empty);

        private ImmutableDirectedGraph(ImmutableDictionary<N, ImmutableDictionary<E,N>> graph)
        {
            this.graph = graph;
        }

        public ImmutableDirectedGraph<N, E> AddNode(N node)
        {
            if (graph.ContainsKey(node))
            {
                return this;
            }

            Console.WriteLine("Added node {0} to graph", node.ToString());
            return new ImmutableDirectedGraph<N, E>(graph.Add(node, ImmutableDictionary<E, N>.Empty));
        }

        public ImmutableDirectedGraph<N,E> AddEdge(N start, N end, E edge)
        {
            var g = this.AddNode(start).AddNode(end);
            var edgeRelationship = g.graph.SetItem(start, g.graph[start].SetItem(edge, end));
            Console.WriteLine("Added edge {0} from node {1} to node {2}", edge.ToString(), start.ToString(), end.ToString());
            return new ImmutableDirectedGraph<N, E>(edgeRelationship);
        }

        public ImmutableDictionary<E, N> GetEdgeConnections(N node)
        {
            return this.graph.ContainsKey(node) ? this.graph[node] : ImmutableDictionary<E, N>.Empty;
        }

        public ImmutableDirectedGraph<N, E> RemoveNode(N node)
        {
            var query = graph.Where(k => k.Value.ContainsValue(node)).Select(k => new { Start = k.Key, Edge = k.Value.Single(a => a.Value.Equals(node)) });            

            var map = new ImmutableDirectedGraph<N, E>(graph.Remove(node));
            
            foreach(var entry in query)
            {
                map = map.RemoveEdge(entry.Start, entry.Edge.Key);
            }
                                              
            Console.WriteLine("Removing node {0} from graph", node.ToString());
            return map;
        }

        public ImmutableDirectedGraph<N, E> RemoveEdge(N startingNode, E edge)
        {
            var newGraph = graph.Where(k => !k.Key.Equals(startingNode)).ToImmutableDictionary(k => k.Key, k => k.Value);
            
            if(graph.ContainsKey(startingNode) && graph[startingNode].ContainsKey(edge) )
            {
                var remainingEdges = graph[startingNode].Remove(edge);
                newGraph = newGraph.Add(startingNode, remainingEdges);
                return new ImmutableDirectedGraph<N, E>(newGraph);
            }

            Console.WriteLine("Starting node or connecting edge are not in the graph");

            return this;
        }

        public IEnumerable<ImmutableStack<KeyValuePair<E, N>>> AllEdgeTraversals(N node)
        {
            var edges = GetEdgeConnections(node);
            if(edges.Count == 0)
                yield return ImmutableStack<KeyValuePair<E,N>>.Empty;
            else
            {
                foreach(var pair in edges)
                {
                    foreach(var edgeTraversal in AllEdgeTraversals(pair.Value))
                    {
                        yield return edgeTraversal.Push(pair);
                    }
                }
            }
        }

        public void PrintGraph()
        {
            foreach(var entry in graph)
            {
                foreach(var connection in entry.Value)
                {
                    Console.WriteLine("{0} connected to {1} through {2}", entry.Key.ToString(), connection.Value.ToString(), connection.Key.ToString());
                }
            }
        }
    }
}
