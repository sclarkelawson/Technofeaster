using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Soldier
{
    GameObject currentSquad { get; set; }
    float fear { get; set; }
    bool isIsolated { get; set; }
    void Death(); //destroy object, instantiate explosion
}
