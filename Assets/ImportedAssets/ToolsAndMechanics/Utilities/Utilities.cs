using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = System.Random;

namespace ToolsAndMechanics.Utilities
{
    public interface IFindable
    {
        public Vector3 Position { get; set; }
    }

    public static class Utilities
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void SetActive(this List<GameObject> list, bool active)
        {
            foreach (var obj in list)
            {
                obj.SetActive(active);
            }
        }

        public static void SetSortPositions(this List<Transform> list, int sqrtCellsCount, float size)
        {
            Transform[,] array = new Transform[sqrtCellsCount, sqrtCellsCount];

            for (int y = 0; y < sqrtCellsCount; y++)
            {
                for (int x = 0; x < sqrtCellsCount; x++)
                {
                    array[y, x] = list[y * sqrtCellsCount + x];
                }
            }
            array.SetSortPositions(sqrtCellsCount, size);
        }

        public static void SetSortPositions(this Transform[,] array, int sqrtCellsCount, float size)
        {
            for (int y = 0; y < sqrtCellsCount; y++)
            {
                for (int x = 0; x < sqrtCellsCount; x++)
                {
                    Vector2 offset = new Vector2(x, y) * size;
                    array[y, x].position = new Vector3(array[0, 0].position.x + offset.x, array[y, x].position.y, array[0, 0].position.z + offset.y);
                }
            }
        }


        public static float GetDistance2D(Transform t1, Transform t2)
        {
            return GetDistance2D(t1.position, t2.position);
        }

        public static float GetDistance2D(Vector3 pos1, Vector3 pos2)
        {
            return Vector2.Distance(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));
        }

        public static bool IsDistanceLess(Vector3 pos1, Vector3 pos2, float distance)
        {
            float sqrDistance = Vector3.SqrMagnitude(pos1 - pos2);
            return sqrDistance <= distance * distance;
        }

        public static bool IsDistanceLess2D(Vector3 pos1, Vector3 pos2, float distance)
        {
            Vector2 p1 = new Vector2(pos1.x, pos1.z);
            Vector2 p2 = new Vector2(pos2.x, pos2.z);
            float sqrDistance = Vector2.SqrMagnitude(p1 - p2);
            return sqrDistance <= distance * distance;
        }

        public static bool IsPointAtQuad(Vector3 BL, Vector3 TR, Vector3 pos)
        {
            Vector3 TL = new Vector3(BL.x, 0f, TR.z);
            Vector3 BR = new Vector3(TR.x, 0f, BL.z);
            return IsPointAtQuad(TL, TR, BL, BR, pos);
        }

        public static bool IsPointAtQuad(Vector3 TL, Vector3 TR, Vector3 BL, Vector3 BR, Vector3 pos)
        {
            return (pos.x > TL.x && pos.z < TL.z) &&
                (pos.x < TR.x && pos.z < TR.z) &&
                (pos.x > BL.x && pos.z > BL.z) &&
                (pos.x < BR.x && pos.z > BR.z);
        }

        public static bool IsPointInCircle(Vector3 pos1, float minRadius, float maxRadius, Vector3 pos2)
        {
            Vector2 p1 = new Vector2(pos1.x, pos1.z);
            Vector2 p2 = new Vector2(pos2.x, pos2.z);
            float sqrDistance = Vector2.SqrMagnitude(p1 - p2);
            return sqrDistance <= maxRadius * maxRadius && sqrDistance >= minRadius * minRadius;
        }

        public static bool IsPointInTriangle(Vector3 pos, Vector3 a, Vector3 b, Vector3 c)
        {
            // Compute vectors        
            Vector3 v0 = c - a;
            Vector3 v1 = b - a;
            Vector3 v2 = pos - a;

            // Compute dot products
            float dot00 = Vector3.Dot(v0, v0);
            float dot01 = Vector3.Dot(v0, v1);
            float dot02 = Vector3.Dot(v0, v2);
            float dot11 = Vector3.Dot(v1, v1);
            float dot12 = Vector3.Dot(v1, v2);

            // Compute barycentric coordinates
            float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
            float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            // Check if point is in triangle
            return (u >= 0) && (v >= 0) && (u + v < 1);
        }

        public static Vector2 GetRandomPositionAtQuad(Vector2 size)
        {
            return new Vector2(UnityEngine.Random.Range(-size.x / 2, size.x / 2), UnityEngine.Random.Range(-size.y / 2, size.y / 2));
        }

        public static Vector3 GetRandomPositionAtCircle(Vector3 center, float minRadius, float maxRadius)
        {
            Vector3 randCircle = UnityEngine.Random.onUnitSphere;
            randCircle.y = 0;
            Vector3 pos = center + randCircle * UnityEngine.Random.Range(minRadius, maxRadius);
            return pos;
        }

        public static IFindable GetNearest(this IFindable main, List<IFindable> list, bool includeSelf)
        {
            IFindable nearest = null;
            float minDistance = float.MaxValue;

            foreach (var obj in list)
            {
                if (!includeSelf && obj == main) continue;

                float distance = GetDistance2D(main.Position, obj.Position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = obj;
                }
            }
            return nearest;
        }

        public static IFindable GetNearestAtRange(this IFindable main, List<IFindable> list, bool includeSelf, float min, float max)
        {
            IFindable nearest = null;
            float minDistance = max;

            foreach (var obj in list)
            {
                if (!includeSelf && obj == main) continue;

                float distance = GetDistance2D(main.Position, obj.Position);
                if (distance < minDistance && distance >= min)
                {
                    minDistance = distance;
                    nearest = obj;
                }
            }
            return nearest;
        }

        public static IEnumerable<IFindable> GetObjectsAtRange(this IFindable main, List<IFindable> list, bool includeSelf, float min, float max)
        {
            if (includeSelf)
            {
                return list.Where(c => !IsDistanceLess2D(main.Position, c.Position, min) &&
                        IsDistanceLess2D(main.Position, c.Position, max));
            }
            else
            {

                return list.Where(c => c != main && !IsDistanceLess2D(main.Position, c.Position, min) &&
                        IsDistanceLess2D(main.Position, c.Position, max));
            }
        }

        public static IEnumerable<IFindable> GetObjectsAtRange(this Vector3 pos, List<IFindable> list, float min, float max)
        {
            return list.Where(c => !IsDistanceLess2D(pos, c.Position, min) &&
                           IsDistanceLess2D(pos, c.Position, max));
        }

        public static List<string> ParseEquation(string equation)
        {
            List<string> res = new List<string>();
            foreach (var match in Regex.Matches(equation, @"([*+/\-)(=])|([0-9]+)|(\{[0-9]+})"))
            {
                res.Add(match.ToString());
            }
            return res;
        }

        public static bool IsSuccessEquation(string equation, List<string> prms)
        {
            List<string> equationList = ParseEquation(equation);
            string answer = equationList[equationList.Count - 1];
            //Remove '=' and answer
            equationList.RemoveAt(equationList.Count - 1);
            equationList.RemoveAt(equationList.Count - 1);

            string eq = string.Format(string.Join("", equationList), prms.ToArray());
            return new System.Data.DataTable().Compute(eq, "").ToString() == answer;
        }

        public static bool TryDoEquation(string equation, out float result)
        {
            if (float.TryParse(new System.Data.DataTable().Compute(equation, "").ToString(), out result))
            {
                return true;
            }
            return false;
        }
    }
}