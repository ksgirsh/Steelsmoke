using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    // Start is called before the first frame update

    
    public int speed;
    private float move;
    [SerializeField] float RotationOffset;
    [SerializeField] Rigidbody2D rb;
 
    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Camera Camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        
         Vector3 mousePos = Input.mousePosition;
        mousePos.z = 11;
        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);

        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;

        float mouseposX = mousePos.x / 50;
        float mouseposY = mousePos.y / 75;
            
        

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + RotationOffset));
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPos.z = 11;
        transform.position = Vector3.MoveTowards(transform.position, targetPos * move , speed * Time.deltaTime * move);
  

         
    }


}
