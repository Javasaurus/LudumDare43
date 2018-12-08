using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStarActor : MonoBehaviour
{
    public enum State
    {
        RUNNING, READY_TO_DELIVER, READY
    }

    public State state = State.READY;

    public static int maxSteps = 1000;

    private VillageGrid grid;
    public List<Tile> currentPath;
    private Node currentNode;
    private int iterations = 0;

    private bool calculating;
    private Coroutine calculatingRoutine;

    public void Awake()
    {
        grid = GameObject.FindObjectOfType<VillageGrid>();
        currentPath = new List<Tile>();
    }

    public Tile nearestTile;
    public bool success;


    public void RunPathfinding(Tile targetTile)
    {
        if (calculatingRoutine != null)
        {
            StopCoroutine(calculatingRoutine);
        }
        calculatingRoutine = StartCoroutine(CalculatePathAsync(targetTile));
    }

    public void RunPathfinding(Tile startingTile, Tile targetTile)
    {
        if (calculatingRoutine != null)
        {
            StopCoroutine(calculatingRoutine);
        }
        calculatingRoutine = StartCoroutine(CalculatePathAsync(startingTile, targetTile));
    }

    private IEnumerator CalculatePathAsync(Tile targetTile)
    {
        Tile startingTile = getCurrentTile();
        return CalculatePathAsync(startingTile, targetTile);
    }

    private IEnumerator CalculatePathAsync(Tile startingTile, Tile targetTile)
    {
        success = true;
        state = State.RUNNING;
        nearestTile = startingTile;
        currentPath.Clear();
        iterations = 0;
        if (targetTile == null)
        {
            Debug.Log("NO TARGET");
            success = false;
        }
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();
        //get the starting tile


        if (isNeighbour(startingTile, targetTile))
        {
            success = true;
        }


        Node firstNode = new Node(startingTile, targetTile, startingTile);
        float bestGValue = firstNode.g;
        currentNode = firstNode;
        openList.Add(firstNode);

        while (openList.Count > 0)
        {
            if (iterations > maxSteps)
            {
                break;
            }

            //find the lowest possible f value on the current list
            openList = openList.OrderBy(x => x.calculateF()).ToList();
            currentNode = openList[0];

            //check if the currentNode is what we wanted to find
            if (currentNode.tile == targetTile)
            {
                break;
            }

            //check if there is no path...
            if (openList.Count == 0 && currentNode.tile != targetTile)
            {
                nearestTile = currentNode.tile;
                if (!isNeighbour(currentNode.tile, targetTile))
                {
                    currentPath.Clear();
                    //no path was found in range, check again for nearest tile
                    success = false;
                    break;
                }
                else
                {
                    //needs to be a walkable tile, otherwise the target is "locked" in
                    if (currentNode.tile.walkable)
                    {
                        success = true;
                        break;
                    }
                    else
                    {
                        success = false;
                    }
                }
            }
            if (success)
            {
                //put the current tile on the closed so we don't iterate it again
                openList.Remove(currentNode);
                closedList.Add(currentNode);
                //find the neighbours
                Tile[] neighbours = getNeighbours(currentNode.tile);
                Node[] neighbourNodes = new Node[neighbours.Length];
                for (int i = 0; i < neighbours.Length; i++)
                {
                    //CHECK HERE IF THE AGENT CAN WALK THIS METADATA...
                    if (neighbours[i] != null && (neighbours[i].walkable | neighbours[i] == targetTile))
                    {
                        neighbourNodes[i] = new Node(currentNode.tile, targetTile, neighbours[i]);
                        if (neighbourNodes[i].g < bestGValue)
                        {
                            bestGValue = neighbourNodes[i].g;
                        }
                    }

                }

                foreach (Node child in neighbourNodes)
                {
                    //CHECK HERE WHERE THE AGENT CAN WALK...
                    if (child != null && !ContainsNode(closedList, child))
                    {
                        CheckNodeInOpenList(openList, child, currentNode);
                    }

                }

            }

            yield return null;

        }

        //reconstruct the path
        List<Tile> tmpPath = new List<Tile>();

        if (targetTile.walkable && nearestTile != null && isNeighbour(targetTile, nearestTile))
        {
            tmpPath.Add(targetTile);
        }

        while (currentNode.parent != null)
        {
            tmpPath.Add(currentNode.parent.tile);
            currentNode = currentNode.parent;
        }

        if (tmpPath.Count > 0)
        {
            tmpPath.Reverse();
            currentPath = tmpPath;
        }

        GameObject.FindObjectOfType<DebugTools>().ShowPath(currentPath, Color.white);
        nearestTile = currentPath.Count == 0 ? startingTile : currentPath.Last();
        calculatingRoutine = null;
        state = success ? State.READY_TO_DELIVER : State.READY;
        yield return null;
    }


    public bool isNeighbour(Tile currentTile, Tile neighbourAssumption)
    {
        Tile[] neighbours = (getNeighbours(currentTile));
        foreach (Tile tile in neighbours)
        {
            if (tile == neighbourAssumption)
            {
                return true;
            }
        }
        return false;
    }

    public Tile getCurrentTile()
    {
        //this could theoretically be done by raycasting
        return grid.grid[getTilePosition(transform)];
    }

    public Vector2 getTilePosition(Transform transform)
    {
        return new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
    }

    public Tile[] getNeighbours(Tile tile)
    {
        Tile[] neighbours = new Tile[4];
        //get up
        Vector2 tilePosition = getTilePosition(tile.transform);
        //get neighbours
        Vector2 up = new Vector2(tilePosition.x, tilePosition.y + 1);
        Vector2 right = new Vector2(tilePosition.x + 1, tilePosition.y);
        Vector2 down = new Vector2(tilePosition.x, tilePosition.y - 1);
        Vector2 left = new Vector2(tilePosition.x - 1, tilePosition.y);

        if (grid.grid.ContainsKey(up))
        {
            neighbours[0] = grid.grid[up];
        }
        if (grid.grid.ContainsKey(right))
        {
            neighbours[1] = grid.grid[right];
        }
        if (grid.grid.ContainsKey(down))
        {
            neighbours[2] = grid.grid[down];
        }
        if (grid.grid.ContainsKey(left))
        {
            neighbours[3] = grid.grid[left];
        }
        return neighbours;
    }

    public bool ContainsNode(List<Node> nodes, Node node)
    {
        foreach (Node listNode in nodes)
        {
            if (listNode.EqualsNode(node))
            {
                return true;
            }
        }
        return false;
    }

    public void CheckNodeInOpenList(List<Node> nodes, Node node, Node currentNode)
    {
        foreach (Node listNode in nodes)
        {
            if (listNode.EqualsNode(node))
            {
                if (listNode.g < node.g)
                {
                    listNode.g = node.g;
                    listNode.parent = node.parent;
                    return;
                }
            }
        }
        //else we need to add it to the open list and set the parent
        node.parent = currentNode;
        nodes.Add(node);
        iterations++;
    }

}

public class Node
{
    public float h;
    public float g;
    private float f;
    public Node parent;
    public Tile tile;
    public Vector2 position;
    public Node(Tile startingTile, Tile endingTile, Tile tile)
    {
        this.tile = tile;
        position = new Vector2(Mathf.RoundToInt(tile.transform.position.x), Mathf.RoundToInt(tile.transform.position.y));
        g = Mathf.Abs(tile.transform.position.x - endingTile.transform.position.x) + Mathf.Abs(tile.transform.position.y - endingTile.transform.position.y);
        //estimated length to the destination (must take into account the tiles that are harder to work later)
        h = Mathf.Abs(tile.transform.position.x - endingTile.transform.position.x) + Mathf.Abs(tile.transform.position.y - endingTile.transform.position.y);
        //the sum
        f = g + h;
    }

    public float calculateF()
    {
        //we try to run in straight lines, so if the direction is the same as the previous node... it gets a benefit?
        f = g + h;
        return f;
    }

    public bool EqualsNode(Node other)
    {
        return position == other.position;
    }

}

