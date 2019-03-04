using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeManager : MonoBehaviour
{
    bool dragging = false;
    bool tap, swipeLeft, swipeRight, swipeUp, swipeDown;
    Vector2 startPoint, swipeDelta;
    private void Update()
    {
        tap = swipeDown = swipeLeft = swipeRight = swipeUp = false;
        #region Standalone input
        if (Input.GetMouseButtonDown(0))
        {
            tap = true;
            dragging = true;
            startPoint = Input.mousePosition;
        }else if (Input.GetMouseButtonUp(0))
        {
            Reset();
        }
        #endregion
        #region Mobile inputs
        if (Input.touches.Length > 0)
        {
            if(Input.touches[0].phase == TouchPhase.Began)
            {
                tap = true;
                dragging = true;
                startPoint = Input.touches[0].position;
            }else if(Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                Reset();
            }
        }
        #endregion
        //Calculate distance
        swipeDelta = Vector2.zero;
        if (dragging)
        {
            if (Input.touches.Length > 0)
                swipeDelta = Input.touches[0].position - startPoint;
            else if (Input.GetMouseButton(0))
                swipeDelta = (Vector2)Input.mousePosition - startPoint;
        }

        //Cross deadzone
        if (swipeDelta.magnitude > 125)
        {
            //Direction
            float x = swipeDelta.x;
            float y = swipeDelta.y;
            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                //Left or right
                if (x > 0)
                    swipeRight = true;
                else
                    swipeLeft = true;
            }
            else
            {
                //Up or down
                if (y > 0)
                    swipeUp = true;
                else
                    swipeDown = true;
            }
            Reset();
        }
    }
    private void Reset()
    {
        startPoint = Vector3.zero;
        swipeDelta = Vector3.zero;
        dragging = false;
    }
    public Vector2 SwipeDelta { get { return swipeDelta;} }
    public bool SwipeLeft { get { return swipeLeft; } }
    public bool SwipeRight { get { return swipeRight; } }
    public bool SwipeUp { get { return swipeUp; } }
    public bool SwipeDown { get { return swipeDown; } }
}
