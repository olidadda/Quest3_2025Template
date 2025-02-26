using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace UltimateProceduralPrimitives
{
  public class FormulaPyramidPerfectTriangularFlex : AbstractFormula
  {
    public MyMeshInfo CalculateMyMeshInfo(PyramidPerfectTriangularFlexParameters parameter)
    {
      var triangleHeight = (parameter.Length / 2.0f) * Mathf.Sqrt(3.0f);
      var tetrahedronHeight = Mathf.Sqrt(6.0f) / 3.0f * parameter.Length;
      var centerOfTetrahedron = triangleHeight - (Mathf.Sqrt(3.0f) / 3.0f * parameter.Length);
      // var offsetToMoveCenterForZ = new Vector3(0, 0, 0);
      var offsetToMoveCenterForZ = new Vector3(0, 0, ((parameter.Length - triangleHeight) / 2.0f) + (parameter.Length / 2.0f - parameter.Length / 3.0f));

      var offsetY_ForDown = new Vector3(0, -parameter.Length / 2.0f, 0);
      var downPlaneParameter = new PlaneFlexParameters()
      {
        SurfaceType = parameter.SurfaceType,
        Orientation = Orientation.Down,
        Direction = Direction.Y_Axis,  // Collect Direction Later
        PivotPosition = PivotPosition.Center, // Collect Pivot Later
        DoubleSided = false,
        Segments = parameter.SegmentsBottom,
        // Width = parameter.Width,
        // Height = parameter.Depth,
        Width = parameter.Length,
        Height = parameter.Length,

        OffsetLeftForwardVtxForOrientationDown = offsetY_ForDown + offsetToMoveCenterForZ + parameter.OffsetDownForwardVtx + new Vector3(+parameter.Length / 2.0f, 0, triangleHeight - (parameter.Length / 2.0f * 2.0f)),
        OffsetRightForwardVtxForOrientationDown = offsetY_ForDown + offsetToMoveCenterForZ + parameter.OffsetDownForwardVtx + new Vector3(-parameter.Length / 2.0f, 0, triangleHeight - (parameter.Length / 2.0f * 2.0f)),
        // OffsetLeftForwardVtxForOrientationDown = offsetY_ForDown + new Vector3(parameter.Width / 2.0f, 0, 0) + parameter.OffsetDownForwardVtx,
        // OffsetRightForwardVtxForOrientationDown = offsetY_ForDown + new Vector3(-parameter.Width / 2.0f, 0, 0) + parameter.OffsetDownForwardVtx,
        OffsetLeftBackwardVtxForOrientationDown = offsetY_ForDown + offsetToMoveCenterForZ + parameter.OffsetDownLeftBackwardVtx,
        OffsetRightBackwardVtxForOrientationDown = offsetY_ForDown + offsetToMoveCenterForZ + parameter.OffsetDownRightBackwardVtx,
        FlipNormals = false, // Collect FlipNormals Later
      };



      var offsetX_ForLeft = new Vector3(-parameter.Length / 2.0f, 0, 0);
      var leftPlaneParameter = new PlaneFlexParameters()
      {
        SurfaceType = parameter.SurfaceType,
        Orientation = Orientation.Left,
        Direction = Direction.Y_Axis,  // Collect Direction Later
        PivotPosition = PivotPosition.Center, // Collect Pivot Later
        DoubleSided = false,
        Segments = parameter.SegmentsSides,
        // Width = parameter.Depth,
        // Height = parameter.Height,
        Width = parameter.Length,
        Height = parameter.Length,

        OffsetUpForwardVtxForOrientationLeft = offsetX_ForLeft + offsetToMoveCenterForZ + parameter.OffsetUpVtx + new Vector3(parameter.Length / 2.0f, tetrahedronHeight - (parameter.Length / 2.0f * 2.0f), centerOfTetrahedron - (parameter.Length / 2.0f * 2.0f)),
        OffsetUpBackwardVtxForOrientationLeft = offsetX_ForLeft + offsetToMoveCenterForZ + parameter.OffsetUpVtx + new Vector3(parameter.Length / 2.0f, tetrahedronHeight - (parameter.Length / 2.0f * 2.0f), centerOfTetrahedron),
        // OffsetUpForwardVtxForOrientationLeft = offsetX_ForLeft + new Vector3(parameter.Width / 2.0f, 0, -parameter.Depth / 2.0f) + parameter.OffsetUpVtx,
        // OffsetUpBackwardVtxForOrientationLeft = offsetX_ForLeft + new Vector3(parameter.Width / 2.0f, 0, parameter.Depth / 2.0f) + parameter.OffsetUpVtx,
        OffsetDownForwardVtxForOrientationLeft = offsetX_ForLeft + offsetToMoveCenterForZ + parameter.OffsetDownForwardVtx + new Vector3(parameter.Length / 2.0f, 0, triangleHeight - (parameter.Length / 2.0f * 2.0f)),
        // OffsetDownForwardVtxForOrientationLeft = offsetX_ForLeft + new Vector3(parameter.Width / 2.0f, 0, 0) + parameter.OffsetDownForwardVtx,
        OffsetDownBackwardVtxForOrientationLeft = offsetX_ForLeft + offsetToMoveCenterForZ + parameter.OffsetDownLeftBackwardVtx,
        FlipNormals = false, // Collect FlipNormals Later
      };

      var offsetX_ForRight = new Vector3(parameter.Length / 2.0f, 0, 0);
      var rightPlaneParameter = new PlaneFlexParameters()
      {
        SurfaceType = parameter.SurfaceType,
        Orientation = Orientation.Right,
        Direction = Direction.Y_Axis,  // Collect Direction Later
        PivotPosition = PivotPosition.Center, // Collect Pivot Later
        DoubleSided = false,
        Segments = parameter.SegmentsSides,
        // Width = parameter.Depth,
        // Height = parameter.Height,
        Width = parameter.Length,
        Height = parameter.Length,

        OffsetUpForwardVtxForOrientationRight = offsetX_ForRight + offsetToMoveCenterForZ + parameter.OffsetUpVtx + new Vector3(-parameter.Length / 2.0f, tetrahedronHeight - (parameter.Length / 2.0f * 2.0f), centerOfTetrahedron - (parameter.Length / 2.0f * 2.0f)),
        OffsetUpBackwardVtxForOrientationRight = offsetX_ForRight + offsetToMoveCenterForZ + parameter.OffsetUpVtx + new Vector3(-parameter.Length / 2.0f, tetrahedronHeight - (parameter.Length / 2.0f * 2.0f), centerOfTetrahedron),
        // OffsetUpForwardVtxForOrientationRight = offsetX_ForRight + new Vector3(-parameter.Width / 2.0f, 0, -parameter.Depth / 2.0f) + parameter.OffsetUpVtx,
        // OffsetUpBackwardVtxForOrientationRight = offsetX_ForRight + new Vector3(-parameter.Width / 2.0f, 0, parameter.Depth / 2.0f) + parameter.OffsetUpVtx,
        OffsetDownForwardVtxForOrientationRight = offsetX_ForRight + offsetToMoveCenterForZ + parameter.OffsetDownForwardVtx + new Vector3(-parameter.Length / 2.0f, 0, triangleHeight - (parameter.Length / 2.0f * 2.0f)),
        // OffsetDownForwardVtxForOrientationRight = offsetX_ForRight + new Vector3(-parameter.Width / 2.0f, 0, 0) + parameter.OffsetDownForwardVtx,
        OffsetDownBackwardVtxForOrientationRight = offsetX_ForRight + offsetToMoveCenterForZ + parameter.OffsetDownRightBackwardVtx,
        FlipNormals = false, // Collect FlipNormals Later
      };

      // var offsetZ_ForForward = new Vector3(0, 0, parameter.Depth / 2.0f);
      // var forwardPlaneParameter = new PlaneFlexParameters()
      // {
      //   SurfaceType = parameter.SurfaceType,
      //   Orientation = Orientation.Forward,
      //   Direction = Direction.Y_Axis,  // Collect Direction Later
      //   PivotPosition = PivotPosition.Center, // Collect Pivot Later
      //   DoubleSided = false,
      //   Segments = parameter.SegmentsSides,
      //   Width = parameter.Width,
      //   Height = parameter.Height,

      //   OffsetUpLeftVtxForOrientationForward = offsetZ_ForForward + new Vector3(parameter.Width / 2.0f, 0, -parameter.Depth / 2.0f) + parameter.OffsetUpVtx,
      //   OffsetUpRightVtxForOrientationForward = offsetZ_ForForward + new Vector3(-parameter.Width / 2.0f, 0, -parameter.Depth / 2.0f) + parameter.OffsetUpVtx,
      //   OffsetDownLeftVtxForOrientationForward = offsetZ_ForForward + parameter.OffsetDownForwardVtx,
      //   OffsetDownRightVtxForOrientationForward = offsetZ_ForForward + parameter.OffsetDownForwardVtx,
      //   FlipNormals = false, // Collect FlipNormals Later
      // };

      var offsetZ_ForBackward = new Vector3(0, 0, -(parameter.Length / 2.0f));
      // var offsetZ_ForBackward = new Vector3(0, 0, 0);
      var backwardPlaneParameter = new PlaneFlexParameters()
      {
        SurfaceType = parameter.SurfaceType,
        Orientation = Orientation.Backward,
        Direction = Direction.Y_Axis,  // Collect Direction Later
        PivotPosition = PivotPosition.Center, // Collect Pivot Later
        DoubleSided = false,
        Segments = parameter.SegmentsSides,
        // Width = parameter.Width,
        // Height = parameter.Height,
        Width = parameter.Length,
        Height = parameter.Length,

        OffsetUpLeftVtxForOrientationBackward = offsetZ_ForBackward + offsetToMoveCenterForZ + parameter.OffsetUpVtx + new Vector3(parameter.Length / 2.0f, tetrahedronHeight - (parameter.Length / 2.0f * 2.0f), centerOfTetrahedron),
        OffsetUpRightVtxForOrientationBackward = offsetZ_ForBackward + offsetToMoveCenterForZ + parameter.OffsetUpVtx + new Vector3(-parameter.Length / 2.0f, tetrahedronHeight - (parameter.Length / 2.0f * 2.0f), centerOfTetrahedron),
        // OffsetUpLeftVtxForOrientationBackward = offsetZ_ForBackward + new Vector3(parameter.Width / 2.0f, 0, parameter.Depth / 2.0f) + parameter.OffsetUpVtx,
        // OffsetUpRightVtxForOrientationBackward = offsetZ_ForBackward + new Vector3(-parameter.Width / 2.0f, 0, parameter.Depth / 2.0f) + parameter.OffsetUpVtx,
        OffsetDownLeftVtxForOrientationBackward = offsetZ_ForBackward + offsetToMoveCenterForZ + parameter.OffsetDownLeftBackwardVtx,
        OffsetDownRightVtxForOrientationBackward = offsetZ_ForBackward + offsetToMoveCenterForZ + parameter.OffsetDownRightBackwardVtx,
        FlipNormals = false, // Collect FlipNormals Later
      };

      var myMeshInfo_downPlane = new FormulaPlaneFlex().CalculateMyMeshInfo(downPlaneParameter);
      var myMeshInfo_leftPlane = new FormulaPlaneFlex().CalculateMyMeshInfo(leftPlaneParameter);
      var myMeshInfo_rightPlane = new FormulaPlaneFlex().CalculateMyMeshInfo(rightPlaneParameter);
      // var vertexesAndUVs_forwardPlane = new FormulaPlaneFlex().CalculateVertexesAndUVs(forwardPlaneParameter);
      var myMeshInfo_backwardPlane = new FormulaPlaneFlex().CalculateMyMeshInfo(backwardPlaneParameter);


      foreach (var item in myMeshInfo_downPlane.myVtx) { myVtx.Add(item); }
      foreach (var item in myMeshInfo_leftPlane.myVtx) { myVtx.Add(item); }
      foreach (var item in myMeshInfo_rightPlane.myVtx) { myVtx.Add(item); }
      // foreach (var item in vertexesAndUVs_forwardPlane.vertexes) { vertexes.Add(item); }
      foreach (var item in myMeshInfo_backwardPlane.myVtx) { myVtx.Add(item); }

      foreach (var item in myMeshInfo_downPlane.myUVs) { myUVs.Add(item); }
      foreach (var item in myMeshInfo_leftPlane.myUVs) { myUVs.Add(item); }
      foreach (var item in myMeshInfo_rightPlane.myUVs) { myUVs.Add(item); }
      // foreach (var item in vertexesAndUVs_forwardPlane.uvs) { uvs.Add(item); }
      foreach (var item in myMeshInfo_backwardPlane.myUVs) { myUVs.Add(item); }


      int d = myMeshInfo_downPlane.myVtx.Count;
      int l = myMeshInfo_leftPlane.myVtx.Count;
      int r = myMeshInfo_rightPlane.myVtx.Count;
      // int f = myMeshInfo_forwardPlane.myVtx.Count;
      int b = myMeshInfo_backwardPlane.myVtx.Count;

      for (int i = 0; i < myMeshInfo_downPlane.myIdx.Count; i++)
        myIdx.Add(myMeshInfo_downPlane.myIdx[i] + 0);

      for (int i = 0; i < myMeshInfo_leftPlane.myIdx.Count; i++)
        myIdx.Add(myMeshInfo_leftPlane.myIdx[i] + d);

      for (int i = 0; i < myMeshInfo_rightPlane.myIdx.Count; i++)
        myIdx.Add(myMeshInfo_rightPlane.myIdx[i] + d + l);

      // for (int i = 0; i < myMeshInfo_forwardPlane.myIdx.Count; i++)
      //   myIdx.Add(myMeshInfo_forwardPlane.myIdx[i] + d + l + r);

      for (int i = 0; i < myMeshInfo_backwardPlane.myIdx.Count; i++)
        myIdx.Add(myMeshInfo_backwardPlane.myIdx[i] + d + l + r);


      FlipNormals(parameter.FlipNormals, myVtx, myUVs, myIdx); // Collect FlipNormals here
      DirectionCollection(parameter.Direction, myVtx);  // Collect Direction here
      // SetPivotPosition(parameter.PivotPosition, myVtx);  // Collect Pivot here
      return CreateMyMeshInfoStruct(myVtx, myUVs, myIdx);
    }
  }
}