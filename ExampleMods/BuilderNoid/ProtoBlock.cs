using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BuilderNoid
{
	class ProtoBlock : MonoBehaviour
	{
		MeshRenderer meshRenderer;
		MeshFilter meshFilter;
		BoxCollider collider;
		const float box_extent = 0.75f;
		LayerMask Walkable = new LayerMask();
		void Start()
		{
			meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshFilter = gameObject.AddComponent<MeshFilter>();
			collider = gameObject.AddComponent<BoxCollider>();
			gameObject.layer = 0;
			Walkable.value = 425984;
			CreateCube();
		}

		private void CreateCube()
		{
			Vector3[] vertices = {
			new Vector3 (-box_extent, -box_extent, -box_extent),
			new Vector3 (box_extent, -box_extent, -box_extent),
			new Vector3 (box_extent, box_extent, -box_extent),
			new Vector3 (-box_extent, box_extent, -box_extent),
			new Vector3 (-box_extent, box_extent, box_extent),
			new Vector3 (box_extent, box_extent, box_extent),
			new Vector3 (box_extent, -box_extent, box_extent),
			new Vector3 (-box_extent, -box_extent, box_extent),
			};

			int[] triangles = {
			0, 2, 1, //face front
			0, 3, 2,
			2, 3, 4, //face top
			2, 4, 5,
			1, 2, 5, //face right
			1, 5, 6,
			0, 7, 4, //face left
			0, 4, 3,
			5, 4, 7, //face back
			5, 7, 6,
			0, 6, 7, //face bottom
			0, 1, 6
		};

			Mesh mesh = meshFilter.mesh;
			mesh.Clear();
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();

			Shader s = Shader.Find("psx/vertexlit");
			if (s != null)
			{
				meshRenderer.material = new Material(s);
				var c = Color.red;
				c.a = 0.5f;
				//meshRenderer.material.SetTexture("_MainTex", BuilderNoid.blockTexture);
			}

			collider.center = mesh.bounds.center;
			collider.size = mesh.bounds.size;
			collider.enabled = true;
		}
		private void Update()
		{
			if(Physics.CheckBox(meshRenderer.bounds.center, new Vector3(box_extent, box_extent, box_extent), gameObject.transform.rotation, Walkable))
			{
				meshRenderer.material.color = Color.red;
			}
			else
			{
				meshRenderer.material.color = Color.green;
			}
		}

		public bool IsFree() => meshRenderer.material.color == Color.green;
	}
}