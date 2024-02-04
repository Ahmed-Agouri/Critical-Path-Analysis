class Node
{
    public string TaskName { get; set; }
    public List<Edge> Dependencies { get; } = new List<Edge>(); // Outgoing edges
    public List<Edge> IncomingEdges { get; } = new List<Edge>(); // Incoming edges
    public int EST { get; set; }
    public int LST { get; set; }
    public int Slack { get; set; }

    public Node(string taskName)
    {
        TaskName = taskName;
        EST = int.MaxValue;
        LST = 0;
        Slack = 0;
    }
}


class Edge
{
    public Node FromNode { get; }
    public Node ToNode { get; }
    public int Weight { get; }

    public Edge(Node fromNode, Node toNode, int weight)
    {
        FromNode = fromNode;
        ToNode = toNode;
        Weight = weight;
    }
}


class graph
{

    public List<Node> nodes = new List<Node>();


    public void add_Node(string TaskName)
    {
        Node newNode = new Node(TaskName);
        nodes.Add(newNode);
    }


    public void add_edge(string FromTaskName, string ToTaskName, int DurationOfDependency)
    {
        Node fromNode = nodes.Find(node => node.TaskName == FromTaskName);
        Node toNode = nodes.Find(node => node.TaskName == ToTaskName);
        if (fromNode != null && toNode != null)
        {
            Edge newEdge = new Edge(fromNode, toNode, DurationOfDependency);
            fromNode.Dependencies.Add(newEdge); // Add as outgoing edge
            toNode.IncomingEdges.Add(newEdge); // Add as incoming edge
        }
    }

    public List<Node> get_predecessors(string taskName)
    {
        Node node = nodes.Find(n => n.TaskName == taskName);
        if (node != null)
        {
            List<Node> predecessors = new List<Node>();
            foreach (Edge edge in node.IncomingEdges)
            {
                predecessors.Add(edge.FromNode);
            }
            return predecessors;
        }
        return null;
    }



    public void CalculateEarliestStartTimes()
    {
        var startNodes = nodes.Where(n => n.IncomingEdges.Count == 0).ToList();

        foreach(var node in startNodes)
        {
            node.EST = 0;
        }

        Queue<Node> queue = new Queue<Node>(startNodes);


        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            foreach (var edge in current.Dependencies)
            {
                var nextNode = edge.ToNode;
                int newEST = current.EST + edge.Weight;

                if (nextNode.EST == int.MaxValue || newEST > nextNode.EST)
                {
                    nextNode.EST = newEST;
                    if (!queue.Contains(nextNode))
                    {
                        queue.Enqueue(nextNode);
                    }
                }
            }
        }
    }


    public void CalculateLatestStartTimes(int projectDuration)
    {
        // Initialize LST for all nodes
        foreach (var node in nodes)
        {
            node.LST = projectDuration;
        }

        // Process each node
        bool changeMade;
        do
        {
            changeMade = false;
            foreach (var node in nodes)
            {
                foreach (var edge in node.Dependencies)
                {
                    var successor = edge.ToNode;
                    int potentialLST = successor.LST - edge.Weight;
                    if (node.LST > potentialLST)
                    {
                        node.LST = potentialLST;
                        changeMade = true;
                    }
                }
            }
        } while (changeMade); // Keep iterating until no more changes are made
    }



    public void CalculateSlack()
    {

        foreach(var node in nodes)
        {
            node.Slack = node.LST - node.EST;
        }

    }



    public void IdentifyCriticalPath()
    {
        List<Node> criticalPathNodes = new List<Node>();
        Node currentNode = nodes.FirstOrDefault(n => n.Dependencies.Count == 0); // Start with the last node

        while (currentNode != null)
        {
            criticalPathNodes.Add(currentNode);
            // Get the next node in the critical path (zero slack)
            Edge edgeToNextNode = currentNode.IncomingEdges
.FirstOrDefault(e => e.FromNode.Slack == 0);
            currentNode = edgeToNextNode?.FromNode;
        }

        criticalPathNodes.Reverse(); // Because we started from the end, reverse to get the correct order
        Console.WriteLine("Critical Path: " + string.Join(" -> ", criticalPathNodes.Select(n => n.TaskName)));
    }
}

class Program
{
    static public void Main(String[] args)
    {
        graph ProjectGraph = new graph();

        ProjectGraph.add_Node("V1");
        ProjectGraph.add_Node("V2");
        ProjectGraph.add_Node("V3");
        ProjectGraph.add_Node("V4");
        ProjectGraph.add_Node("V5");
        ProjectGraph.add_Node("V6");
        ProjectGraph.add_Node("V7");
        ProjectGraph.add_Node("V8");
        ProjectGraph.add_Node("V9");


        ProjectGraph.add_edge("V1", "V2", 6);
        ProjectGraph.add_edge("V1", "V3", 4);
        ProjectGraph.add_edge("V1", "V4", 5);
        ProjectGraph.add_edge("V2", "V5", 1);
        ProjectGraph.add_edge("V3", "V5", 1);
        ProjectGraph.add_edge("V4", "V6", 2);
        ProjectGraph.add_edge("V5", "V7", 9);
        ProjectGraph.add_edge("V5", "V8", 7);
        ProjectGraph.add_edge("V6", "V8", 4);
        ProjectGraph.add_edge("V7", "V9", 2);
        ProjectGraph.add_edge("V8", "V9", 4);


        ProjectGraph.CalculateEarliestStartTimes();
        ProjectGraph.CalculateLatestStartTimes(18);
        ProjectGraph.CalculateSlack();
        ProjectGraph.IdentifyCriticalPath();
        

        foreach (var node in ProjectGraph.nodes)
        {
            Console.WriteLine();
            Console.WriteLine($"Task {node.TaskName} - Earliest Start Time: {node.EST}");
            Console.WriteLine($"Task {node.TaskName} - Latest Start Time: {node.LST}");
            Console.WriteLine($"Task {node.TaskName} - Slack: {node.Slack}");
        }

    }

}

