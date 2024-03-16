using UnityEngine;

namespace Utilities
{
    public static class MathUtil
    {
        public static int Seed { get; private set; }

        private static System.Random _random;

        static MathUtil()
        {
            _random = new System.Random();
        }

        public static void InitSeed(int seed)
        {
            Seed = seed;
            _random = new System.Random(seed);
            UnityEngine.Random.InitState(seed);
        }

        public static float GetAngle(Vector3 start, Vector3 end)
        {
            return Mathf.Atan2(start.z - end.z, start.x - end.x) * Mathf.Rad2Deg;
        }

        public static float GetAngle(Vector2 start, Vector2 end)
        {
            return Mathf.Atan2(start.y - end.y, start.x - end.x) * Mathf.Rad2Deg;
        }

        public static long Lerp(double a, double b, float t)
        {
            return (long)(a + (b - a) * Mathf.Clamp01(t));
        }

        public static int Sign(double value)
        {
            return (value >= 0) ? 1 : -1;
        }

        public static int RandomSystem(int min, int max)
        {
            return _random.Next(min, max + 1);
        }

        public static float RandomSystem(float min, float max)
        {
            return (float)_random.NextDouble() * (max + .0001f - min) + min;
        }

        public static int Random(int min, int max)
        {
            return UnityEngine.Random.Range(min, max + 1);
        }

        public static float Random(float min, float max)
        {
            return UnityEngine.Random.Range(min, max + .0001f);
        }

        public static string IntToHex(uint crc)
        {
            return $"{crc:X}";
        }

        public static uint HexToInt(string crc)
        {
            return uint.Parse(crc, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        public static bool RandomBool
        {
            get
            {
                return UnityEngine.Random.value > 0.5f;
            }
        }

        public static int RandomSign => RandomBool ? 1 : -1;

        public static Vector2 AddRotateAround(Vector2 center, Vector2 point, float angleInDegree)
        {
            var angle = GetAngle(point, center);
            angleInDegree += angle;
            var radius = Vector2.Distance(center, point);
            return center + new Vector2(Mathf.Cos(angleInDegree * Mathf.Deg2Rad), Mathf.Sin(angleInDegree * Mathf.Deg2Rad)) * radius;
        }

        public static Color IntToColor(int value)
        {
            Color c;

            c.a = ((value >> 24) & 0xff) / 255f;
            c.r = ((value >> 16) & 0xff) / 255f;
            c.g = ((value >> 8) & 0xff) / 255f;
            c.b = ((value) & 0xff) / 255f;

            return c;
        }

        public static int ColorToInt(Color color)
        {
            byte a = (byte)(color.a * 255);
            byte r = (byte)(color.r * 255);
            byte g = (byte)(color.g * 255);
            byte b = (byte)(color.b * 255);

            return (a & 0xff) << 24 | (r & 0xff) << 16 | (g & 0xff) << 8 | (b & 0xff);
        }

        public static Vector3 PowVector3(Vector3 vector, float p)
        {
            return new Vector3
            {
                x = Mathf.Pow(vector.x, p),
                y = Mathf.Pow(vector.y, p),
                z = Mathf.Pow(vector.z, p)
            };
        }

        public static Vector3 SqrtVector3(Vector3 vector)
        {
            return new Vector3
            {
                x = Mathf.Sqrt(vector.x),
                y = Mathf.Sqrt(vector.y),
                z = Mathf.Sqrt(vector.z)
            };
        }

        public static Vector3 AbsVector3(Vector3 vector)
        {
            return new Vector3
            {
                x = Mathf.Abs(vector.x),
                y = Mathf.Abs(vector.y),
                z = Mathf.Abs(vector.z)
            };
        }

        public static float ConvertWithClamp(float value, float in_min, float in_max, float out_min, float out_max)
        {
            return (value - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        public static int RandomWithProbabilityTable(float[] probabilityTable)
        {
            float sumProbabilities = 0;
            float[] probabilities = new float[probabilityTable.Length];
            for (int i = 0; i < probabilityTable.Length; i++)
            {
                sumProbabilities += probabilityTable[i];
                probabilities[i] += sumProbabilities;

                if (probabilityTable[i] == 0)
                {
                    probabilities[i] = -1;
                }
            }

            float result = MathUtil.Random(0, sumProbabilities);
            for (int i = 0; i < probabilityTable.Length; i++)
            {
                if (result <= probabilities[i])
                    return i;
            }

            return 0;
        }

        public static float Project(Vector2 vector, Vector2 onNormal)
        {
            float num1 = Vector2.Dot(onNormal, onNormal);

            if ((double)num1 < (double)Mathf.Epsilon)
                return 0;

            float num2 = Vector2.Dot(vector, onNormal);
            return num2 / num1;
        }

        public static bool IsLeftFromLine(Vector2 start, Vector2 end, Vector2 point)
        {
            return ((end.x - start.x) * (point.y - start.y) - (end.y - start.y) * (point.x - start.x)) > 0;
        }
    }
}