using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using UnityEngine;

namespace GaussianBlur
{
    public class PostEffectBlur : MonoBehaviour
    {
        [SerializeField]
        private Blur blur = null;


        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if(this.blur == null)
            {
                Graphics.Blit(source, destination);
                return;
            }

            Graphics.Blit(source, destination, this.blur.Material);
        }
    }
}
