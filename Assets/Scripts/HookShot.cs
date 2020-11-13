using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookShot : MonoBehaviour
{
    public Transform target_m;
    private ToolBox intance_m;
    public Transform hookShotAim_m;
    public float pullSpeed;
    public float range_m = 100f;
    public float LastShootingTime_m;
    public bool IsShootingAvaible_m;
    public Camera camera_m;
    private void Start()
    {
        IsShootingAvaible_m = true;
    }
    private void Update()
    {
        Aim();
    }
    public void Aim()
    {
        Vector3 mousePostion_m = Input.mousePosition;
        Vector3 aimDirection_m = (mousePostion_m - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection_m.y, aimDirection_m.x) * Mathf.Rad2Deg;
        hookShotAim_m.eulerAngles = new Vector3(0, 0, angle);      
    }
    public void FireHookShot()
    {
        if (IsShootingAvaible_m)
        {
            IsShootingAvaible_m = false;
            LastShootingTime_m = Time.time;
            RaycastHit hit;
            if (Physics.Raycast(camera_m.transform.position, camera_m.transform.forward, out hit, range_m))
            {
                target_m.transform.position = hit.point;
            }
        }
    }
}
