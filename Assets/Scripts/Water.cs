﻿using UnityEngine;

namespace LowPolyWater
{
    public class Water : MonoBehaviour
    {
        public float waveHeight = 0.5f;
        public float waveFrequency = 0.5f;
        public float waveLength = 0.75f;

        //Pozicija valova
        public Vector3 waveOriginPosition = new Vector3(0.0f, 0.0f, 0.0f);

        MeshFilter meshFilter;
        Mesh mesh;
        Vector3[] vertices;

        private void Awake()
        {     
            meshFilter = GetComponent<MeshFilter>();
        }

        void Start()
        {
            WaterFilter(meshFilter);
        }
        MeshFilter WaterFilter(MeshFilter mf)
        {
            mesh = mf.sharedMesh; 
            Vector3[] originalVertices = mesh.vertices;     
            int[] triangles = mesh.triangles;          
            Vector3[] vertices = new Vector3[triangles.Length];      
            for (int i = 0; i < triangles.Length; i++)
            {
                vertices[i] = originalVertices[triangles[i]];
                triangles[i] = i;
            }           
            mesh.vertices = vertices;
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            this.vertices = mesh.vertices;

            return mf;
        }
        
        void Update()
        {
            GenerateWaves();
        }
        //Generiranje valova
        void GenerateWaves()
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 v = vertices[i];              
                v.y = 0.0f;        
                float distance = Vector3.Distance(v, waveOriginPosition);
                distance = (distance % waveLength) / waveLength;            
                v.y = waveHeight * Mathf.Sin(Time.time * Mathf.PI * 2.0f * waveFrequency
                + (Mathf.PI * 2.0f * distance));                          
                vertices[i] = v;
            }            
            mesh.vertices = vertices;
            mesh.RecalculateNormals();
            mesh.MarkDynamic();
            meshFilter.mesh = mesh;
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<Health>().ApplyDamage(25);


            }
        }
    }
}