﻿using Assets.Density_Function;
using UnityEngine;

namespace Assets
{
    public struct Point
    {
        public float Density;
        public Vector3 Normal;
    }

    public class UniformPointCloud
    {
        private readonly Point[] _data;
        public int Width;
        public int Height;
        public int Length;

        public UniformPointCloud(int width, int height, int length)
        {
            _data = new Point[width * height * length];
            Width = width;
            Height = height;
            Length = length;
        }

        public int GetIndex(int x, int y, int z)
        {
            return y * Height * Length + z * Width + x;
        }

        public Point Get(int x, int y, int z)
        {
            return _data[GetIndex(x, y, z)];
        }

        public void Union(IDensityFunction densityFunction)
        {
            var xmin = Mathf.Max(0, Mathf.FloorToInt(densityFunction.Minimum.x));
            var xmax = Mathf.Min(Width - 1, Mathf.CeilToInt(densityFunction.Maximum.x));
            var ymin = Mathf.Max(0, Mathf.FloorToInt(densityFunction.Minimum.y));
            var ymax = Mathf.Min(Height - 1, Mathf.CeilToInt(densityFunction.Maximum.y));
            var zmin = Mathf.Max(0, Mathf.FloorToInt(densityFunction.Minimum.z));
            var zmax = Mathf.Min(Length - 1, Mathf.CeilToInt(densityFunction.Maximum.z));

            for (var x = xmin; x < xmax; x++)
            {
                for (var y = ymin; y < ymax; y++)
                {
                    for (var z = zmin; z < zmax; z++)
                    {
                        _data[GetIndex(x, y, z)].Density = Mathf.Min(densityFunction.Value(new Vector3(x, y, z)), _data[GetIndex(x, y, z)].Density);
                        _data[GetIndex(x, y, z)].Normal = GetNormal(densityFunction, x, y, z);
                    }
                }
            }
        }

        public void Intersect(IDensityFunction densityFunction)
        {
            var xmin = Mathf.Max(0, Mathf.FloorToInt(densityFunction.Minimum.x));
            var xmax = Mathf.Min(Width - 1, Mathf.CeilToInt(densityFunction.Maximum.x));
            var ymin = Mathf.Max(0, Mathf.FloorToInt(densityFunction.Minimum.y));
            var ymax = Mathf.Min(Height - 1, Mathf.CeilToInt(densityFunction.Maximum.y));
            var zmin = Mathf.Max(0, Mathf.FloorToInt(densityFunction.Minimum.z));
            var zmax = Mathf.Min(Length - 1, Mathf.CeilToInt(densityFunction.Maximum.z));

            for (var x = xmin; x < xmax; x++)
            {
                for (var y = ymin; y < ymax; y++)
                {
                    for (var z = zmin; z < zmax; z++)
                    {
                        _data[GetIndex(x, y, z)].Density = Mathf.Max(densityFunction.Value(new Vector3(x, y, z)), _data[GetIndex(x, y, z)].Density);
                        _data[GetIndex(x, y, z)].Normal = GetNormal(densityFunction, x, y, z);
                    }
                }
            }
        }

        public Vector3 GetNormal(IDensityFunction densityFunction, int x, int y, int z)
        {
            return new Vector3(
                Get(x + 1, y, z).Density - Get(x - 1, y, z).Density,
                Get(x, y + 1, z).Density - Get(x, y - 1, z).Density,
                Get(x, y, z + 1).Density - Get(x, y, z - 1).Density
            ).normalized;
        }
    }
}
