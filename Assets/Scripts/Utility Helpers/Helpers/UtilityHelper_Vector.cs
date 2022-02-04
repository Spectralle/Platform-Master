using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public static partial class UtilityHelper
    {
        private static readonly Vector3 Vector3zero = Vector3.zero;
        private static readonly Vector3 Vector3one = Vector3.one;
        private static readonly Vector3 Vector3yDown = new Vector3(0, -1);


        // Is this position inside the FOV? Top Down Perspective
        public static bool IsPositionInsideFov(Vector3 pos, Vector3 aimDir, Vector3 posTarget, float fov)
        {
            int aimAngle = UtilityHelper.GetAngleFromVector180(aimDir);
            int angle = UtilityHelper.GetAngleFromVector180(posTarget - pos);
            int angleDifference = (angle - aimAngle);
            if (angleDifference > 180) angleDifference -= 360;
            if (angleDifference < -180) angleDifference += 360;
            if (!(angleDifference < fov / 2f && angleDifference > -fov / 2f))
                // Not inside fov
                return false;
            else
                // Inside fov
                return true;
        }

        public static Vector3 GetRandomDir() =>
            new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;

        public static Vector3 GetRandomDirXZ() =>
            new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;


        public static Vector3 GetVectorFromAngle(int angle)
        {
            // angle = 0 -> 360
            float angleRad = angle * (Mathf.PI / 180f);
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        public static Vector3 GetVectorFromAngle(float angle)
        {
            // angle = 0 -> 360
            float angleRad = angle * (Mathf.PI / 180f);
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        public static Vector3 GetVectorFromAngleInt(int angle)
        {
            // angle = 0 -> 360
            float angleRad = angle * (Mathf.PI / 180f);
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        public static float GetAngleFromVectorFloat(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;

            return n;
        }

        public static float GetAngleFromVectorFloatXZ(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;

            return n;
        }

        public static int GetAngleFromVector(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;
            int angle = Mathf.RoundToInt(n);

            return angle;
        }

        public static int GetAngleFromVector180(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            int angle = Mathf.RoundToInt(n);

            return angle;
        }


        public static Vector3 ApplyRotationToVector(Vector3 vec, Vector3 vecRotation) =>
            ApplyRotationToVector(vec, GetAngleFromVectorFloat(vecRotation));

        public static Vector3 ApplyRotationToVector(Vector3 vec, float angle) =>
            Quaternion.Euler(0, 0, angle) * vec;

        public static Vector3 ApplyRotationToVectorXZ(Vector3 vec, float angle) =>
            Quaternion.Euler(0, angle, 0) * vec;


        public static Vector3 GetRandomPositionWithinRectangle(float xMin, float xMax, float yMin, float yMax) =>
            new Vector3(UnityEngine.Random.Range(xMin, xMax), UnityEngine.Random.Range(yMin, yMax));

        public static Vector3 GetRandomPositionWithinRectangle(Vector3 lowerLeft, Vector3 upperRight) =>
            new Vector3(UnityEngine.Random.Range(lowerLeft.x, upperRight.x), UnityEngine.Random.Range(lowerLeft.y, upperRight.y));


        public static List<Vector3> GetPositionListAround(Vector3 position, float distance, int positionCount)
        {
            List<Vector3> ret = new List<Vector3>();
            for (int i = 0; i < positionCount; i++)
            {
                int angle = i * (360 / positionCount);
                Vector3 dir = UtilityHelper.ApplyRotationToVector(new Vector3(0, 1), angle);
                Vector3 pos = position + dir * distance;
                ret.Add(pos);
            }
            return ret;
        }

        public static List<Vector3> GetPositionListAround(Vector3 position, float[] ringDistance, int[] ringPositionCount)
        {
            List<Vector3> ret = new List<Vector3>();
            for (int ring = 0; ring < ringPositionCount.Length; ring++)
            {
                List<Vector3> ringPositionList = GetPositionListAround(position, ringDistance[ring], ringPositionCount[ring]);
                ret.AddRange(ringPositionList);
            }
            return ret;
        }

        public static List<Vector3> GetPositionListAround(Vector3 position, float distance, int positionCount, Vector3 direction, int angleStart, int angleIncrease)
        {
            List<Vector3> ret = new List<Vector3>();
            for (int i = 0; i < positionCount; i++)
            {
                int angle = angleStart + angleIncrease * i;
                Vector3 dir = UtilityHelper.ApplyRotationToVector(direction, angle);
                Vector3 pos = position + dir * distance;
                ret.Add(pos);
            }
            return ret;
        }

        public static List<Vector3> GetPositionListAlongDirection(Vector3 position, Vector3 direction, float distancePerPosition, int positionCount)
        {
            List<Vector3> ret = new List<Vector3>();
            for (int i = 0; i < positionCount; i++)
            {
                Vector3 pos = position + direction * (distancePerPosition * i);
                ret.Add(pos);
            }
            return ret;
        }

        public static List<Vector3> GetPositionListAlongAxis(Vector3 positionStart, Vector3 positionEnd, int positionCount)
        {
            Vector3 direction = (positionEnd - positionStart).normalized;
            float distancePerPosition = (positionEnd - positionStart).magnitude / positionCount;
            return GetPositionListAlongDirection(positionStart + direction * (distancePerPosition / 2f), direction, distancePerPosition, positionCount);
        }

        public static List<Vector3> GetPositionListWithinRect(Vector3 lowerLeft, Vector3 upperRight, int positionCount)
        {
            List<Vector3> ret = new List<Vector3>();
            float width = upperRight.x - lowerLeft.x;
            float height = upperRight.y - lowerLeft.y;
            float area = width * height;
            float areaPerPosition = area / positionCount;
            float positionSquareSize = Mathf.Sqrt(areaPerPosition);
            Vector3 rowLeft, rowRight;
            rowLeft = new Vector3(lowerLeft.x, lowerLeft.y);
            rowRight = new Vector3(upperRight.x, lowerLeft.y);
            int rowsTotal = Mathf.RoundToInt(height / positionSquareSize);
            float increaseY = height / rowsTotal;
            rowLeft.y += increaseY / 2f;
            rowRight.y += increaseY / 2f;
            int positionsPerRow = Mathf.RoundToInt(width / positionSquareSize);
            for (int i = 0; i < rowsTotal; i++)
            {
                ret.AddRange(GetPositionListAlongAxis(rowLeft, rowRight, positionsPerRow));
                rowLeft.y += increaseY;
                rowRight.y += increaseY;
            }
            int missingPositions = positionCount - ret.Count;
            Vector3 angleDir = (upperRight - lowerLeft) / missingPositions;
            for (int i = 0; i < missingPositions; i++)
                ret.Add(lowerLeft + (angleDir / 2f) + angleDir * i);
            while (ret.Count > positionCount)
                ret.RemoveAt(UnityEngine.Random.Range(0, ret.Count));
            return ret;
        }


        public static List<Vector2Int> GetPosXYListDiamond(int size)
        {
            List<Vector2Int> list = new List<Vector2Int>();
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size - x; y++)
                {
                    list.Add(new Vector2Int(x, y));
                    list.Add(new Vector2Int(-x, y));
                    list.Add(new Vector2Int(x, -y));
                    list.Add(new Vector2Int(-x, -y));
                }
            }
            return list;
        }

        public static List<Vector2Int> GetPosXYListOblong(int width, int dropXamount, int increaseDropXamount, Vector3 dir)
        {
            List<Vector2Int> list = GetPosXYListOblong(width, dropXamount, increaseDropXamount);
            list = RotatePosXYList(list, UtilityHelper.GetAngleFromVector(dir));
            return list;
        }

        public static List<Vector2Int> GetPosXYListOblong(int width, int dropXamount, int increaseDropXamount)
        {
            List<Vector2Int> triangle = GetPosXYListTriangle(width, dropXamount, increaseDropXamount);
            List<Vector2Int> list = new List<Vector2Int>(triangle);
            foreach (Vector2Int posXY in triangle)
            {
                if (posXY.y == 0) continue;
                list.Add(new Vector2Int(posXY.x, -posXY.y));
            }
            foreach (Vector2Int posXY in new List<Vector2Int>(list))
            {
                if (posXY.x == 0) continue;
                list.Add(new Vector2Int(-posXY.x, posXY.y));
            }
            return list;
        }

        public static List<Vector2Int> GetPosXYListTriangle(int width, int dropXamount, int increaseDropXamount)
        {
            List<Vector2Int> list = new List<Vector2Int>();
            for (int i = 0; i > -999; i--)
            {
                for (int j = 0; j < width; j++)
                    list.Add(new Vector2Int(j, i));
                width -= dropXamount;
                dropXamount += increaseDropXamount;
                if (width <= 0) break;
            }
            return list;
        }

        public static List<Vector2Int> RotatePosXYList(List<Vector2Int> list, int angle)
        {
            List<Vector2Int> ret = new List<Vector2Int>();
            for (int i = 0; i < list.Count; i++)
            {
                Vector2Int posXY = list[i];
                Vector3 vec = UtilityHelper.ApplyRotationToVector(new Vector3(posXY.x, posXY.y), angle);
                ret.Add(new Vector2Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y)));
            }
            return ret;
        }


        // Get UI Position from World Position
        public static Vector2 GetWorldUIPosition(Vector3 worldPosition, Transform parent, Camera uiCamera, Camera worldCamera)
        {
            Vector3 screenPosition = worldCamera.WorldToScreenPoint(worldPosition);
            Vector3 uiCameraWorldPosition = uiCamera.ScreenToWorldPoint(screenPosition);
            Vector3 localPos = parent.InverseTransformPoint(uiCameraWorldPosition);
            return new Vector2(localPos.x, localPos.y);
        }

        public static Vector3 GetWorldPositionFromUIZeroZ()
        {
            Vector3 vec = GetWorldPositionFromUI(Input.mousePosition, Camera.main);
            vec.z = 0f;
            return vec;
        }

        // Get World Position from UI Position
        public static Vector3 GetWorldPositionFromUI() =>
            GetWorldPositionFromUI(Input.mousePosition, Camera.main);

        public static Vector3 GetWorldPositionFromUI(Camera worldCamera) =>
            GetWorldPositionFromUI(Input.mousePosition, worldCamera);

        public static Vector3 GetWorldPositionFromUI(Vector3 screenPosition, Camera worldCamera)
        {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }

        public static Vector3 GetWorldPositionFromUI_Perspective() =>
            GetWorldPositionFromUI_Perspective(Input.mousePosition, Camera.main);

        public static Vector3 GetWorldPositionFromUI_Perspective(Camera worldCamera) =>
            GetWorldPositionFromUI_Perspective(Input.mousePosition, worldCamera);

        public static Vector3 GetWorldPositionFromUI_Perspective(Vector3 screenPosition, Camera worldCamera)
        {
            Ray ray = worldCamera.ScreenPointToRay(screenPosition);
            Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, 0f));
            float distance;
            xy.Raycast(ray, out distance);
            return ray.GetPoint(distance);
        }
    }
}