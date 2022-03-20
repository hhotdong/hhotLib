﻿using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Camera))]
public class ScanEffect : MonoBehaviour
{
    [SerializeField] private Material effectMaterial;

    private Camera cam;
    private Transform camTr;
    private bool isScanning = false;
    private float scanDistance;
    private float scanWidth;
    private Color scanColor;

    private static readonly float SCAN_MAX_DIST_OFFSET = 0.013F;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.Depth;
        camTr = transform;
        this.enabled = false;
    }

    public void Play(Vector3 initPos, float initWidth, float endWidth, float duration, Ease ease)
    {
        if (isScanning == true)
            return;

        float SCAN_DISTANCE_MAX = cam.farClipPlane * SCAN_MAX_DIST_OFFSET;

        DOTween.Sequence()
            .Append(DOTween.To(() => scanDistance, x => scanDistance = x, SCAN_DISTANCE_MAX, duration))
            .Join(DOTween.To(() => scanWidth, x => scanWidth = x, endWidth, duration))
            .Join(DOTween.To(() => scanColor, x => scanColor = x, Color.black, duration))
            .SetEase(ease)
            .OnStart(DoStart)
            .OnComplete(DoComplete)
            .Play();

        void DoStart()
        {
            isScanning = true;
            scanDistance = 0.0F;
            scanWidth = initWidth;
            scanColor = Color.white;
            effectMaterial.SetVector("_WorldSpaceScannerPos", initPos);
        }

        void DoComplete()
        {
            isScanning = false;
            this.enabled = false;
        }
    }

    [ImageEffectOpaque]
    private void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        effectMaterial.SetFloat("_ScanDistance", scanDistance);
        effectMaterial.SetFloat("_ScanWidth", scanWidth);
        effectMaterial.SetColor("_LeadColor", scanColor);
        effectMaterial.SetColor("_MidColor", scanColor);
        effectMaterial.SetColor("_TrailColor", scanColor);
        RaycastCornerBlit(src, dst, effectMaterial);
    }

    private void RaycastCornerBlit(RenderTexture source, RenderTexture dest, Material mat)
    {
        // Compute Frustum Corners
        float camFar = cam.farClipPlane;
        float camFov = cam.fieldOfView;
        float camAspect = cam.aspect;

        float fovWHalf = camFov * 0.5f;

        Vector3 toRight = camTr.right * Mathf.Tan(fovWHalf * Mathf.Deg2Rad) * camAspect;
        Vector3 toTop = camTr.up * Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

        Vector3 topLeft = (camTr.forward - toRight + toTop);
        float camScale = topLeft.magnitude * camFar;

        topLeft.Normalize();
        topLeft *= camScale;

        Vector3 topRight = (camTr.forward + toRight + toTop);
        topRight.Normalize();
        topRight *= camScale;

        Vector3 bottomRight = (camTr.forward + toRight - toTop);
        bottomRight.Normalize();
        bottomRight *= camScale;

        Vector3 bottomLeft = (camTr.forward - toRight - toTop);
        bottomLeft.Normalize();
        bottomLeft *= camScale;

        // Custom Blit, encoding Frustum Corners as additional Texture Coordinates
        RenderTexture.active = dest;

        mat.SetTexture("_MainTex", source);

        GL.PushMatrix();
        GL.LoadOrtho();

        mat.SetPass(0);

        GL.Begin(GL.QUADS);

        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.MultiTexCoord(1, bottomLeft);
        GL.Vertex3(0.0f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.MultiTexCoord(1, bottomRight);
        GL.Vertex3(1.0f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.MultiTexCoord(1, topRight);
        GL.Vertex3(1.0f, 1.0f, 0.0f);

        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.MultiTexCoord(1, topLeft);
        GL.Vertex3(0.0f, 1.0f, 0.0f);

        GL.End();
        GL.PopMatrix();
    }
}
