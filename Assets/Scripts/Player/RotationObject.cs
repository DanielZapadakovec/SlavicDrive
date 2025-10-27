using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveGames
{
    public class RotationObject : MonoBehaviour
    {
        [Header("RotationObject")]
        [SerializeField] Vector3 OpenTo;
        [SerializeField, Range(1, 40)] float Smooth = 10;
        Vector3 InitialRotaion;
        public bool State;
        private void Start()
        {
            InitialRotaion = transform.eulerAngles;
        }
        private void Update()
        {
            if (State)
            {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(OpenTo), Smooth * Time.deltaTime);
            }
            else
            {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(InitialRotaion), Smooth * Time.deltaTime);
            }

        }
        public void StateOpen()
        {
            State = true;
        }
        public void StateClose()
        {
            State = false;
        }
        public void StateChange()
        {
            if (State) State = false;
            else State = true;
        }
    }
}