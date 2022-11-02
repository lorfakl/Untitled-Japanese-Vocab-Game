using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public interface ILineRendererObject
    {

        #region Public Methods

        public Vector3 GetInitialPosition();
        public Vector3 GetInitialVelocity();
        //public T GetInstance();
    
        #endregion


    }
}
