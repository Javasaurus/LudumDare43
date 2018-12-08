using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathLocomotor : MonoBehaviour
{

    public enum State
    {
        WAITING, MOVING, ARRIVED, PAUSED, CONFUSED
    }
    public State state = State.WAITING;

    public delegate void OnArrive();
    public OnArrive onArrive;

    public float movementSpeed = 5f;
    private VillageGrid grid;

    public List<Tile> currentPath;
    public Tile targetTile;
    public Tile currentTile;

    private Animator myAnimator;

    public int currentIndex = 0;

    private Vector2 previousDirection;
    private int directionInt;

    public delegate void OnConfusion();
    public OnConfusion onConfusion;

    public void NotifyArrived()
    {
        currentTile = null;
        currentPath.Clear();
        state = State.ARRIVED;
    }

    public void Awake()
    {
        myAnimator = GetComponent<Animator>();
        grid = GameObject.FindObjectOfType<VillageGrid>();
        currentPath = new List<Tile>();
        onArrive += NotifyArrived;
    }

    public void LateUpdate()
    {
        if (GameStateManager.INSTANCE != null && GameStateManager.INSTANCE.currentState == GameStateManager.GameState.PAUSED)
        {
            return;
        }
        CheckDirection();
        myAnimator.SetBool("walking", state == State.MOVING);
    }

    private void ResetPath()
    {
        state = State.WAITING;
        currentPath.Clear();
    }

    public void Update()
    {
        if (GameStateManager.INSTANCE != null && GameStateManager.INSTANCE.currentState == GameStateManager.GameState.PAUSED)
        {
            return;
        }
        if (state == State.PAUSED || state == State.ARRIVED)
        {
            return;
        }
        if (state == State.MOVING)
        {
            if (currentIndex >= currentPath.Count)
            {
                ResetPath();
                return;
            }
            targetTile = currentPath[currentIndex];
            if (targetTile == null)
            {
                ResetPath();
                return;
            }

            Vector2 rawDirection = (targetTile.transform.position - currentTile.transform.position);
            Vector2 direction = rawDirection.normalized;


            Vector2 displacement = new Vector3(direction.x, direction.y, 0) * Time.deltaTime * movementSpeed;

            //clamp to the next tile so we don't under/overshoot

            displacement.x = Mathf.Sign(direction.x) * Mathf.Min(Mathf.Abs(rawDirection.x), Mathf.Abs(displacement.x));
            displacement.y = Mathf.Sign(direction.y) * Mathf.Min(Mathf.Abs(rawDirection.y), Mathf.Abs(displacement.y));

            transform.position += new Vector3(displacement.x, displacement.y, 0);

            float distanceToTarget = Mathf.Abs(
                Vector2.Distance(new Vector2(transform.position.x, transform.position.y),
                new Vector2(targetTile.transform.position.x, targetTile.transform.position.y)
                ));


            if (distanceToTarget < 0.5f)
            {
                currentTile = getCurrentTile();
                transform.position = targetTile.transform.position;
                currentIndex++;

                //set direction to this new tile
                if (previousDirection != direction && direction.magnitude > 0)
                {
                    previousDirection = direction;
                }

                if (currentIndex == currentPath.Count || (!targetTile.walkable && currentIndex == currentPath.Count - 1))
                {
                    //means we arrived
                    onArrive();
                }
            }
            else if (distanceToTarget > 1f)
            {
                //then we are off the grid which could be a problem
                transform.position = new Vector3((int)targetTile.transform.position.x, (int)targetTile.transform.position.y, transform.position.z);
            }

            //if the combined distance between the current and the target is larger than 1f we made a big mistake...
            /*     if ((distanceToCurrent + distanceToTarget) > 2.5f)
                 {
                     onConfusion();
                 }*/

        }
        else
        {
            currentPath.Clear();
            currentIndex = 0;
            targetTile = null;
            currentTile = null;
        }
    }



    private void CheckDirection()
    {
        int directionValue = 2;
        if (state != State.MOVING || targetTile == null || currentTile == null)
        {
            directionValue = 2;
        }
        else
        {

            Vector2 direction = (targetTile.transform.position - currentTile.transform.position).normalized;

            if (direction.x > 0)
            {
                //means we are going right
                directionValue = 1;
            }
            else if (direction.x < 0)
            {
                //means we are going left
                directionValue = 3;
            }
            else if (direction.y > 0)
            {
                //means we are going up
                directionValue = 0;
            }
            else if (direction.y < 0)
            {
                //means we are going down
                directionValue = 2;
            }
        }
        myAnimator.SetInteger("direction", directionValue);
    }



    public void setPath(List<Tile> path)
    {
        GameObject.FindObjectOfType<DebugTools>().ShowPath(path, Color.white);
        if (path.Count > 0)
        {
            currentTile = getCurrentTile();
            currentIndex = 1;
        }
        else
        {
            currentIndex = 0;
        }
        currentPath = path;
        state = State.MOVING;
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


}
