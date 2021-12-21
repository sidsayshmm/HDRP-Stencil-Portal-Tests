using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Source
{
    public class PortalView : MonoBehaviour
    {
        [Range(1, 6)] public int PortalID;

        [SerializeField] private CustomPassVolume _stencilMatVolume;
        [SerializeField] private CustomPassVolume _renderVolume;

        private DrawRenderersCustomPass _portalCustomPass;
        private DrawPortalsCustomPass _drawViewCustomPass;
        private static readonly int RefID = Shader.PropertyToID("_RefID");

        private void OnValidate()
        {
            Start();
        }

        private void Start()
        {
            transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer($"PortalView{PortalID}");
            _portalCustomPass = (DrawRenderersCustomPass) _stencilMatVolume.customPasses[0];
            _portalCustomPass.overrideMaterial = Resources.Load<Material>($"PortalView_{PortalID}");

            _drawViewCustomPass = (DrawPortalsCustomPass) _renderVolume.customPasses[0];
            _drawViewCustomPass.SetStencilRef(GetStencilRefFromId(PortalID));
        }


        private static int GetStencilRefFromId(int id)
        {
            return id switch
            {
                1 => 64,
                2 => 128,
                3 => 192,
                _ => 1
            };
        }
    }
}