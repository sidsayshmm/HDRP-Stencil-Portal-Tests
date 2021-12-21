using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;


namespace Source
{
    public class DrawPortalsCustomPass : CustomPass
    {
        [SerializeField, Range(0,255)] private int StencilRef;
        [SerializeField] private Camera portalCamera;
        [SerializeField] private LayerMask someMask;
        private const int StencilMask = 192; // 11000000 User Bits<><><><><>

        [SerializeField] private CompareFunction stencilCompFunction;
        [SerializeField] private StencilOp passOperation;
        [SerializeField] private StencilOp failOperation;
        [SerializeField] private StencilOp zFailOperation;
        [SerializeField] private ClearFlag myClearFlags;
        [SerializeField] private RenderQueueType renderQueueFilter = RenderQueueType.All;

        [SerializeField] private CompareFunction depthCompareFunction; // LessEqual seems to be the one I want.
        protected override bool executeInSceneView => true;

        protected override void Execute(CustomPassContext customPassContext)
        {
            var stencilState = new StencilState
            {
                enabled = true,
                readMask = StencilMask,
                writeMask = StencilMask,
                zFailOperationBack = StencilOp.Keep,
                zFailOperationFront = StencilOp.Keep,
            };

            //Need to test which ones I need, which  ones are unnecessary.
            stencilState.SetPassOperation(passOperation);
            stencilState.SetFailOperation(failOperation);
            stencilState.SetZFailOperation(zFailOperation);
            stencilState.SetCompareFunction(stencilCompFunction);

            var depthState = new DepthState(true, depthCompareFunction);


            RenderStateBlock renderStateBlock = new RenderStateBlock(RenderStateMask.Stencil | RenderStateMask.Depth)
            {
                stencilReference = StencilRef, //Stencil reference value set in the shader
                stencilState = stencilState,
                depthState = depthState //no idea why I need this
            };

            //Custom culling so that objects outside of main camera's view frustum are rendered
            portalCamera.TryGetCullingParameters(out var cullingParameters);
            cullingParameters.cullingOptions = CullingOptions.None;
            customPassContext.cullingResults = customPassContext.renderContext.Cull(ref cullingParameters);


            CustomPassUtils.RenderFromCamera(
                customPassContext,
                portalCamera,
                customPassContext.cameraColorBuffer,
                customPassContext.cameraDepthBuffer, //Renders on TOP of the RT if you pass null
                myClearFlags, //should definitely be none. 
                someMask, //set to all.
                renderQueueFilter, //breaks if I don't use RenderQueueType.All
                overrideRenderState: renderStateBlock);
        }

        public void SetStencilRef(int stencilRef)
        {
            StencilRef = stencilRef;
        }
    }
}