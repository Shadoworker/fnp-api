using UnityEngine;
using System.Collections;

public class Catenary : MonoBehaviour
{
	[Range(0.01f,1f)]
	public float m_wireRadius = 0.02f;
	[Range(1.5f,100f)]
	public float m_wireCatenary = 10f;
	[Range(0.1f,10f)]
	public float m_wireResolution = 0.1f;
	public Transform m_p1;
	public Transform m_p2;
	public LineRenderer m_lineRenderer;
	public PrimitiveType m_primitive = PrimitiveType.Cube;

	void Update ()
	{
		Regenerate();
	}

	// Calculate the hyperbolic cosine
	float CosH(float t)
	{
		return (Mathf.Exp(t) + Mathf.Exp(-t))/2;
	}
	// Calculates the Catenary
	public float calculateCatenary(float a, float x )
	{
		return a * CosH( x/a );
	}

	[ContextMenu ("Regenerate")]
	public void Regenerate()
	{
		float distance = Vector3.Distance (m_p1.position, m_p2.position);
		int nPoints = (int) (distance/m_wireResolution + 1);
        //m_wireResolution = distance / (nPoints - 1);
        Vector3[] wirePoints = new Vector3[nPoints];
		wirePoints[0] = m_p1.position;
		wirePoints[nPoints-1] = m_p2.position;
		
		Vector3 dir = (m_p2.position - m_p1.position).normalized;
		float offset = calculateCatenary( m_wireCatenary, -distance/2 );
		
		for (int i = 1; i < nPoints - 1; ++i) {
			Vector3 wirePoint = m_p1.position + i * m_wireResolution * dir;

			float x = i * m_wireResolution - distance / 2;
			wirePoint.y = wirePoint.y - (offset - calculateCatenary (m_wireCatenary, x));
			
			wirePoints[i] = wirePoint;
		}
		GenerateWithLine(wirePoints);
	}

	private void GenerateWithLine( Vector3[] wirePoints ) // using lineRender to illustrate the catenary 
	{
		m_lineRenderer.positionCount = wirePoints.Length;
		for (int i = 0; i < wirePoints.Length; ++i) {
			m_lineRenderer.SetPosition (i, wirePoints[i]);
		}
	}

	private void GenerateWithm_primitive( Vector3[] wirePoints )
	{
		if (m_primitive == PrimitiveType.Plane || m_primitive == PrimitiveType.Quad)
			m_primitive = PrimitiveType.Cube;

		GameObject c = GameObject.CreatePrimitive (m_primitive); // type of segments to illustrate the catenary 

		Mesh m_primitiveMesh = c.GetComponent< MeshFilter >().sharedMesh;
		float m_primitiveHeight = GetHeightBym_primitive (m_primitive);


		MeshFilter meshFilter = GetComponent< MeshFilter > ();
//		if( meshFilter )
//			DestroyImmediate (meshFilter);
//
//		meshFilter = gameObject.AddComponent< MeshFilter > ();

		MeshCollider meshCollider = GetComponent< MeshCollider > ();


		Mesh mesh = new Mesh ();
		mesh.name = "LineMesh";

		int numSegments = wirePoints.Length - 1;

		//create the mesh. the mesh need a vertices, a triangles, a nomals and uv
		Vector3[] vertices = new Vector3[ numSegments * m_primitiveMesh.vertices.Length ];
		int[] triangles = new int[ numSegments * m_primitiveMesh.triangles.Length ];
		Vector3[] normals = new Vector3[ numSegments * m_primitiveMesh.normals.Length ];
		Vector2[] uv = new Vector2[ numSegments * m_primitiveMesh.uv.Length ];

		
		transform.position = (wirePoints [0] + wirePoints [numSegments]) / 2; // put the pivot at the centre of the mesh
			
		Matrix4x4 worldTx = transform.localToWorldMatrix.inverse;

		for (int segment = 0; segment < numSegments; ++segment) 
		{
			Vector3 txT = wirePoints[segment]; // the vector3s of catenary. get to use to translate the mesh

			Quaternion txR = Quaternion.FromToRotation (Vector3.up, wirePoints[segment+1] - wirePoints[segment]); // change the rotation of original mesh to the catenary point

			Vector3 txS = new Vector3 (2 * m_wireRadius, (wirePoints[segment+1] - wirePoints[segment]).magnitude / m_primitiveHeight, 2 * m_wireRadius); // change de scale according to the m_primitive type

			Matrix4x4 preTx = Matrix4x4.TRS(new Vector3(0.0f,m_primitiveHeight / 2.0f,0.0f), Quaternion.identity, Vector3.one); //take the first point into consideration.

			Matrix4x4 tx = worldTx * Matrix4x4.TRS (txT, txR, txS) * preTx;

			int verticesOffset = segment * m_primitiveMesh.vertices.Length;

			for (int i = 0; i < m_primitiveMesh.vertices.Length; ++i) {
				vertices [verticesOffset + i] = tx.MultiplyPoint (m_primitiveMesh.vertices [i]);
			}

			int trianglesOffset = segment * m_primitiveMesh.triangles.Length;

			for (int i = 0; i < m_primitiveMesh.triangles.Length; ++i) {
				triangles [trianglesOffset + i] = m_primitiveMesh.triangles[i] + verticesOffset;
			}

			int normalsOffset = segment * m_primitiveMesh.normals.Length;
			
			for (int i = 0; i < m_primitiveMesh.normals.Length; ++i) {
				normals [normalsOffset + i] = txR * m_primitiveMesh.normals[i];
			}

			int uvOffset = segment * m_primitiveMesh.uv.Length;
			
			for (int i = 0; i < m_primitiveMesh.uv.Length; ++i) {
				uv [uvOffset + i] = m_primitiveMesh.uv[i];
			}
		}

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uv;

		mesh.RecalculateBounds ();

		meshFilter.mesh = mesh;
		meshCollider.sharedMesh = mesh;

		mesh.UploadMeshData (true);

		DestroyImmediate (c);
	}

	private float GetHeightBym_primitive (PrimitiveType type)
	{
		switch (type) {
		case PrimitiveType.Cube:
			return 1.0f;
		case PrimitiveType.Capsule:
		case PrimitiveType.Cylinder:
			return 2.0f;
		default:
			return 1.0f;
		}
	}
}

