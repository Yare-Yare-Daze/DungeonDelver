using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMove : MonoBehaviour
{
    private IFacingMover _mover;

    private void Awake()
    {
        _mover = GetComponent<IFacingMover>();
    }

    private void FixedUpdate()
    {
        if(!_mover.moving) return;
        int facing = _mover.GetFacing();

        Vector2 rPos = _mover.roomPos;
        Vector2 rPosGrid = _mover.GetRoomPosOnGrid();

        float delta = 0;
        if (facing == 0 || facing == 2)
        {
            delta = rPosGrid.y - rPos.y;
        }
        else
        {
            delta = rPosGrid.x - rPos.x;
        }
        
        if(delta == 0) return;

        float move = _mover.GetSpeed() * Time.fixedDeltaTime;
        move = Mathf.Min(move, Mathf.Abs(delta));
        if (delta < 0) move = -move;

        if (facing == 0 || facing == 2)
        {
            rPos.y += move;
        }
        else
        {
            rPos.x += move;
        }

        _mover.roomPos = rPos;
    }
}
