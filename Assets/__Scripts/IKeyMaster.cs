using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKeyMaster
{
    public int keyCount { get; set; }
    int GetFacing();
}
