using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float laserDamage = 0f;
    public LineRenderer lr;
    private Transform tf;
    private Transform firePoint;
    public float beamLength;
    public float laserDuration;
    private float tickTimer;

    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
        firePoint = tf;
        tickTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(tickTimer);
        shootLaser();
        if (tickTimer >= laserDuration) 
        {
            endLaser();
        }
    }

    public void shootLaser() 
    {
        if (Physics2D.Raycast(tf.position, transform.up))
        {
            RaycastHit2D hit = Physics2D.Raycast(firePoint.position, tf.up);
            drawLaser(firePoint.position, hit.point);
            DummyController dummy = hit.collider.gameObject.GetComponent<DummyController>();

            tickTimer += Time.deltaTime;

            if (hit.collider.gameObject.name == "dummy") 
            {
                 dummy.takeDamage(laserDamage);
            }
        }
        else 
        {
            drawLaser(firePoint.position, firePoint.transform.up * beamLength);
        }
    }
    public void drawLaser(Vector2 startPos, Vector2 endPos) 
    {
        lr.SetPosition(0, startPos);
        lr.SetPosition(1, endPos);
    }

    public void endLaser()
    {
        Destroy(this.gameObject);
    }
}
