using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace me.cg350.behaviour {

    public class CubeSpin : MonoBehaviour
    {

        [SerializeField] [Range(0, 100)] public float speedX = 1f;
        [SerializeField] [Range(0, 100)] public float speedY = 1f;
        [SerializeField] [Range(0, 100)] public float speedZ = 1f;


        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            gameObject.transform.Rotate(
                new Vector3(
                    speedX * Time.deltaTime, 
                    speedY * Time.deltaTime, 
                    speedZ * Time.deltaTime
                ), Space.Self
            );
        }
    }

}
