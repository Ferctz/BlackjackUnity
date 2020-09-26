using UnityEngine;
using System.Collections.Generic;

namespace Blackjack.Utils
{
    public delegate T Instantiate<T>(T o, Vector3 pos, Quaternion rot);
    public delegate void Reset<T>(T o, Vector3 pos, Quaternion rot);
    public delegate void Disable<T>(T o);

    public class Pool<T> : MonoBehaviour
    {
        public Instantiate<T> myInstantiate;
        public Reset<T> myReset;
        public Disable<T> myDisable;
        public T poolObject;
        List<T> myPool = new List<T>();

        public T Create(Vector3 pos, Quaternion rot)
        {
            if (myPool.Count == 0)
            {
                T o = myInstantiate.Invoke(poolObject, pos, rot);
                return o;
            }
            else
            {
                T o = myPool[0];
                myPool.Remove(o);
                myReset(o, pos, rot);
                return o;
            }
        }
        public void Release(T o)
        {
            myDisable(o);
            myPool.Add(o);
        }
    }
}