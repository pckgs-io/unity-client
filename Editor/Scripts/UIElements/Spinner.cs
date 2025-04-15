using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pckgs
{
	public class Spinner : VisualElement
	{

		public new class UxmlTraits : VisualElement.UxmlTraits
		{

		}

		public new class UxmlFactory : UxmlFactory<Spinner, UxmlTraits> { }


		// These are the meshes this control uses.
		EllipseMesh m_TrackMesh;

		// This is the number of outer vertices to generate the circle.
		const int k_NumSteps = 200;
		const int TickRate = 50;
		private int accumulatedTick;
		// This default constructor is RadialProgress's only constructor.
		public Spinner()
		{

			AddToClassList("spinner");
			m_TrackMesh = new EllipseMesh(k_NumSteps);

			schedule.Execute(() =>
			{
				accumulatedTick += TickRate;
				MarkDirtyRepaint();
			}).Every(TickRate);
			generateVisualContent += context => GenerateVisualContent(context);

		}

		static void GenerateVisualContent(MeshGenerationContext context)
		{
			Spinner element = (Spinner)context.visualElement;
			element.DrawMeshes(context);
		}

		// DrawMeshes() uses the EllipseMesh utility class to generate an array of vertices and indices, for both the
		// "track" ring (in grey) and the progress ring (in green). It then passes the geometry to the MeshWriteData
		// object, as returned by the MeshGenerationContext.Allocate() method. For the "progress" mesh, only a slice of
		// the index arrays is used to progressively reveal parts of the mesh.
		void DrawMeshes(MeshGenerationContext context)
		{
			float halfWidth = contentRect.width * 0.5f;
			float halfHeight = contentRect.height * 0.5f;

			if (halfWidth < 2.0f || halfHeight < 2.0f)
				return;

			var fillDuration = 0.75f * 1000;

			var direction = Mathf.FloorToInt(accumulatedTick / fillDuration) % 2;
			var progress = (accumulatedTick % fillDuration) / fillDuration;
			var offset = 0f;
			if (direction == 1)
			{
				offset = progress;
				//	progress = 1f - progress;
			}

			var size = Mathf.Min(halfWidth, halfHeight);

			m_TrackMesh.width = size;
			m_TrackMesh.height = size;
			m_TrackMesh.offsetX = contentRect.width / 2f - size;
			m_TrackMesh.offsetY = contentRect.height / 2f - size;
			m_TrackMesh.color = resolvedStyle.color;
			m_TrackMesh.borderSize = size * 0.2f;
			m_TrackMesh.UpdateMesh();

			// Determine how many triangles are used to depending on progress, to achieve a partially filled circle
			int sliceSize = Mathf.FloorToInt(k_NumSteps * progress);
			int sliceSizeOffset = Mathf.FloorToInt(k_NumSteps * offset);

			if (sliceSize == 0)
				return;

			// Every step is 6 indices in the corresponding array
			sliceSize *= 6;
			sliceSizeOffset *= 6;

			var progressMeshWriteData = context.Allocate(m_TrackMesh.vertices.Length, sliceSize);
			progressMeshWriteData.SetAllVertices(m_TrackMesh.vertices);

			var tempIndicesArray = new NativeArray<ushort>(m_TrackMesh.indices, Allocator.Temp);
			progressMeshWriteData.SetAllIndices(tempIndicesArray.Slice(sliceSize, sliceSize));
			tempIndicesArray.Dispose();
		}

	}
}