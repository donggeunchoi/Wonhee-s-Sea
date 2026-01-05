using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    [Header("움직임 관련 변수")] 
    [SerializeField] 
    public float speed;
    [SerializeField]
    private Rigidbody2D rb;
    
    private Vector2 input;
    //이 부분에서는 임시로 wasd 조작으로 테스트 해볼 수 있게 구현할 예정
    //이후 조이스틱을 연구해 추가해놓을 예정
    
    // Update is called once per frame
    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input = input.normalized;
       
    }

    private void FixedUpdate()
    {
        rb.velocity = input * speed;
    }
}
