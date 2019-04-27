using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RayTracingMaster : MonoBehaviour
{
    public ComputeShader RayTracerShader;
    private Camera _camera;
    private RenderTexture _target;
    public Texture SkyboxTexture;
    public int Bounces = 3, SphereGridX = 10, SphereGridY = 10, SphereGridGap = 2;
    private uint _currentSample = 0;
    public float Speed = 1;
    private Material _addMaterial;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        SetShaderParameters();
        Render(destination);
    }

    private void Render(RenderTexture destination)
    {
        InitRenderTexture();

        RayTracerShader.SetTexture(0, "Result", _target);
        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8f);
        RayTracerShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        
        // Blit the result texture to the screen
        if (_addMaterial == null)
            _addMaterial = new Material(Shader.Find("Hidden/AddShader"));
        _addMaterial.SetFloat("_Sample", _currentSample);
        Graphics.Blit(_target, destination, _addMaterial);
        _currentSample++;
    }

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (transform.hasChanged)
        {
            _currentSample = 0;
            transform.hasChanged = false;
        }

        //rotate
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(new Vector3(2*Speed * Time.deltaTime, 0, 0));
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(new Vector3(-2*Speed * Time.deltaTime, 0, 0));
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.Rotate(new Vector3(0, -2*Speed * Time.deltaTime, 0));
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.Rotate(new Vector3(0, 2*Speed * Time.deltaTime, 0));
            }
        }
        //pan
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(new Vector3(Speed * Time.deltaTime, 0, 0));
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(new Vector3(-Speed * Time.deltaTime, 0, 0));
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.Translate(new Vector3(0, -Speed * Time.deltaTime, 0));
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.Translate(new Vector3(0, Speed * Time.deltaTime, 0));
            }
        }
        //navigate
        else
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(new Vector3(Speed * Time.deltaTime, 0, 0));
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(new Vector3(-Speed * Time.deltaTime, 0, 0));
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.Translate(new Vector3(0, 0, -Speed * Time.deltaTime));
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.Translate(new Vector3(0, 0, Speed * Time.deltaTime));
            }
        }

       
    }

    private void InitRenderTexture()
    {
        if (_target == null || _target.width != Screen.width || _target.height != Screen.height)
        {
            if (_target != null) _target.Release();

            _target = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            _target.enableRandomWrite = true;
            _target.Create();
            _currentSample = 0;
        }
    }

    private void SetShaderParameters()
    {
        RayTracerShader.SetMatrix("_CameraToWorld", _camera.cameraToWorldMatrix);
        RayTracerShader.SetMatrix("_CameraInverseProjection", _camera.projectionMatrix.inverse);
        RayTracerShader.SetTexture(0, "_SkyboxTexture", SkyboxTexture);
        RayTracerShader.SetVector("_PixelOffset", new Vector2(Random.value, Random.value));
        RayTracerShader.SetInt("_NumBounces", Bounces);
        RayTracerShader.SetInt("_SphereGridX", SphereGridX);
        RayTracerShader.SetInt("_SphereGridY", SphereGridY);
        RayTracerShader.SetInt("_SphereGridGap", SphereGridGap);
    }

    //    // Start is called before the first frame update
    //    void Start()
    //    {
    //        
    //    }
}
