using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace PostEffects
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Renderer))]
    public class GaussianBlur : MonoBehaviour
    {
        public const string PropSamplingCount = "_SamplingCount";
        public const string PropSamplingDistance = "_SamplingDistance";
        public const string PropWeights = "_Weights";

        public Material Material
        {
            get { return this.mat; }
        }

        [Serializable]
        private struct WeightInfo
        {
            public float weight;
        }

        [SerializeField, Range(1f, 100f)]
        private float distance = 20f;
        [SerializeField, Range(3, 15)]
        private int weight = 7;
        [SerializeField, Range(0.1f, 3f)]
        private float prot = 2f;
        [SerializeField, Range(0f, 10f)]
        private float dispersion = 5f;
        [SerializeField]
        private Shader shader = null;

        [Header("Debug")]
        [SerializeField]
        private bool update = false;

        private Material mat = null;
        private float[] weights = new float[0];

        private ComputeBuffer weightsBuffer = null;


        private void Awake()
        {
            var render = GetComponent<Renderer>();
            this.mat = new Material(this.shader);
            render.material = this.mat;

            this.Initialize();
        }

        private void Update()
        {
            if(this.update == false)
            {
                return;
            }

            this.Initialize();
            this.update = false;
        }

        private void OnDestroy()
        {
            if(this.mat != null)
            {
                DestroyImmediate(this.mat);
            }

            if(this.weightsBuffer != null)
            {
                this.weightsBuffer.Release();
            }
        }

        private void OnRenderObject()
        {
            this.mat.SetInt(PropSamplingCount, this.weight);
            this.mat.SetFloat(PropSamplingDistance, this.distance);
            this.mat.SetBuffer(PropWeights, this.weightsBuffer);
        }

        private void Initialize()
        {
            if((this.weight & 1) == 0)
            {
                this.weight++;
            }

            if(this.weightsBuffer != null)
            {
                this.weightsBuffer.Release();
            }

            this.weights = new float[this.weight];

            var count = (this.weight - 1) / 2;
            var max = this.prot * this.dispersion;
            var step = max / count;
            var sigma = 1f / (2f * this.dispersion * this.dispersion);

            var sum = 0f;
            for(var i = 0; i <= count; i++)
            {
                var x = step * i;
                var w = Mathf.Exp(-1f * x * x * sigma);

                this.weights[i + count] = w;
                this.weights[count - i] = w;

                sum += (w * (i > 0 ? 2f : 1f));
            }

            for(var i = 0; i < this.weight; i++)
            {
                this.weights[i] /= sum;
            }

            this.weightsBuffer = new ComputeBuffer(this.weights.Length, Marshal.SizeOf(typeof(WeightInfo)), ComputeBufferType.Default);
            this.weightsBuffer.SetData(this.weights);
        }
    }
}
