using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using PathologicalGames;

public class DemoTurret : MonoBehaviour
{
    public SmoothLookAtConstraint turretY;
    public SmoothLookAtConstraint turretX;

    void Awake()
    {
        var fireCtl = this.GetComponent<FireController>();
        fireCtl.AddOnTargetUpdateDelegate(this.OnTargetUpdateDel);
        fireCtl.AddOnIdleUpdateDelegate(OnTargetIdleUpdateDel);
    }

    void OnTargetUpdateDel(List<Target> targets)
    {
        this.turretX.target = targets[0].transform;
        this.turretY.target = targets[0].transform;
    }

    void OnTargetIdleUpdateDel()
    {
        this.turretX.target = null;
        this.turretY.target = null;
    }

}
