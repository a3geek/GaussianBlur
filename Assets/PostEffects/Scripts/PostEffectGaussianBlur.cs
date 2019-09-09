using UnityEngine;

namespace PostEffects
{
    [AddComponentMenu("")]
    [RequireComponent(typeof(Camera))]
    public class PostEffectGaussianBlur : MonoBehaviour
    {
        [SerializeField]
        private GaussianBlur blur = null;


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
