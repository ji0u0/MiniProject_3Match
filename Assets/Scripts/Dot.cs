using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public int col;
    public int row;
    public int prevCol;
    public int prevRow;
    public int targetX;
    public int targetY;
    public bool isMatched;

    private Board board;
    private GameObject otherDot;
    private Vector2 firstTouchposition;
    private Vector2 finalTouchposition;
    private Vector2 tempPosition;
    public float swipeAngle = 0f;
    public float swipeResist = 1f;


    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        // targetX = (int)transform.position.x;
        // targetY = (int)transform.position.y;
        // col = targetX;
        // row = targetY;
        // prevCol = col;
        // prevRow = row;
    }

    // Update is called once per frame
    void Update()
    {
        FindMatches();

        if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(1f, 1f, 1f, .2f);
        }

        targetX = col;
        targetY = row;
        
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            // Move towards the Target
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
            if (board.allDots[col, row] != this.gameObject)
            {
                board.allDots[col, row] = this.gameObject;
            }
        }
        else
        {
            // Directly set the Position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
            board.allDots[col, row] = this.gameObject;
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            // Move towards the Target
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
            if (board.allDots[col, row] != this.gameObject)
            {
                board.allDots[col, row] = this.gameObject;
            }
        }
        else
        {
            // Directly set the Position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
            board.allDots[col, row] = this.gameObject;
        }
    }

    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.5f);

        if (otherDot != null)
        {
            if (!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().col = col;
                otherDot.GetComponent<Dot>().row = row;

                col = prevCol;
                row = prevRow;
            
                yield return new WaitForSeconds(.5f);
                board.currentState = GameState.move;
            } 
            else
            {
                board.DestroyMatches();
            }

            otherDot = null;
        }
    }

    private void OnMouseDown()
    {
        if(board.currentState == GameState.move)
        {
            firstTouchposition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            finalTouchposition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchposition.y - firstTouchposition.y) > swipeResist
            || Mathf.Abs(finalTouchposition.x - firstTouchposition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchposition.y - firstTouchposition.y, finalTouchposition.x - firstTouchposition.x) * 180 / Mathf.PI;
            MovePieces();
            board.currentState = GameState.wait;
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    void MovePieces() {
        if (swipeAngle > -45 && swipeAngle <= 45 && col < board.width - 1) // Right
        {
            otherDot = board.allDots[col + 1, row];
            otherDot.GetComponent<Dot>().col -= 1;
            prevCol = col;
            prevRow = row;
            col += 1;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1) // Up
        {
            otherDot = board.allDots[col, row + 1];
            otherDot.GetComponent<Dot>().row -= 1;
            prevCol = col;
            prevRow = row;
            row += 1;
        }
        else if (swipeAngle > 135 || swipeAngle <= -135 && col > 0) // Left
        {
            otherDot = board.allDots[col - 1, row];
            otherDot.GetComponent<Dot>().col += 1;
            prevCol = col;
            prevRow = row;
            col -= 1;
        }
        else if (swipeAngle < -45 || swipeAngle >= 45 && row > 0) // Down
        {
            otherDot = board.allDots[col, row - 1];
            otherDot.GetComponent<Dot>().row += 1;
            prevCol = col;
            prevRow = row;
            row -= 1;
        }

        StartCoroutine(CheckMoveCo());
    }

    void FindMatches()
    {
        if (col > 0 && col < board.width - 1)
        {
            GameObject leftDot1 = board.allDots[col - 1, row];
            GameObject rightDot1 = board.allDots[col + 1, row];

            if (leftDot1 != null && rightDot1 != null)
            {
                if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
                {
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    rightDot1.GetComponent<Dot>().isMatched = true;

                    isMatched = true;
                }
            }
        }

        if (row > 0 && row < board.height - 1)
        {
            GameObject upDot1 = board.allDots[col, row + 1];
            GameObject downDot1 = board.allDots[col, row - 1];

            if (upDot1 != null && downDot1 != null)
            {
                if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag)
                {
                    upDot1.GetComponent<Dot>().isMatched = true;
                    downDot1.GetComponent<Dot>().isMatched = true;

                    isMatched = true;
                }
            }
        }
    }
}
