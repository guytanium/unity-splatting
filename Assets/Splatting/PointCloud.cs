﻿using System.Collections.Generic;
using UnityEngine;

namespace Assets.Splatting
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class PointCloud : MonoBehaviour
    {
        private Mesh _mesh;
        private UniformPointCloud _pointCloud;

        private void Start()
        {
            _mesh = new Mesh();
            _pointCloud = new UniformPointCloud(100, 100, 100);
            GetComponent<MeshFilter>().mesh = _mesh;

            var startTime = Time.realtimeSinceStartup;

            _pointCloud.Union(new DfSphere(new Vector3(50, 50, 50), 10));
            

            Render();
            var endTime = Time.realtimeSinceStartup - startTime;
            Debug.Log($"Execution time: {endTime:f6}s");
        }

        private void Update()
        {
            //_pointCloud.Union(new DfSphere(new Vector3(50, 50, 50), 25));
        }


        private void Render()
        {
            var vertices = new List<Vector3>();
            var indices = new List<int>();
            var colors = new List<Color>();
            var normals = new List<Vector3>();
            var i = 0;
            for (var x = 0; x < _pointCloud.Width; x++)
            {
                for (var y = 0; y < _pointCloud.Height; y++)
                {
                    for (var z = 0; z < _pointCloud.Length; z++)
                    {
                        if (_pointCloud.Get(x, y, z).Density < 0)
                        {
                            vertices.Add(new Vector3(x, y, z));
                            indices.Add(i++);
                            colors.Add(Color.blue);
                            normals.Add(_pointCloud.Get(x, y, z).Normal);
                        }
                    }
                }
            }

            _mesh.vertices = vertices.ToArray();
            _mesh.colors = colors.ToArray();
            _mesh.normals = normals.ToArray();
            _mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            _mesh.SetIndices(indices.ToArray(), MeshTopology.Points, 0);
        }

        private void OnDrawGizmos()
        {
            for (var x = 0; x < _pointCloud.Width-1; x++)
            {
                for (var y = 0; y < _pointCloud.Height-1; y++)
                {
                    for (var z = 0; z < _pointCloud.Length-1; z++)
                    {
                        try
                        {
                            if (_pointCloud.Get(x, y, z).Density > -0.05)
                            {
                                //Gizmos.DrawLine(new Vector3(x, y, z), new Vector3(x, y, z) + _pointCloud.Get(x, y, z).Normal);
                            }
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
            }
        }
    }
}