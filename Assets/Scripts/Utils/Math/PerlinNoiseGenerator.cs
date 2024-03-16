using UnityEngine;

namespace Game.Utils
{
    public sealed class PerlinNoiseGenerator
    {
        public PerlinNoiseGenerator(int seed)
        {
            MathUtil.InitSeed(seed);
            Perlin.Initialize();
        }

        public float[,] GenerateChunk(int chunkSize, float scale, int octaves, float persistence, float lacunarity)
        {
            float[,] noiseMap = new float[chunkSize, chunkSize];

            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    float sampleX = x / scale;
                    float sampleY = y / scale;

                    float perlinValue = GeneratePerlinNoise(sampleX, sampleY, octaves, persistence, lacunarity);

                    noiseMap[x, y] = perlinValue;
                }
            }

            return noiseMap;
        }

        private float GeneratePerlinNoise(float x, float y, int octaves, float persistence, float lacunarity)
        {
            float total = 0;
            float frequency = 1;
            float amplitude = 1;
            float maxValue = 0;

            for (int i = 0; i < octaves; i++)
            {
                total += Perlin.Noise((x * frequency), (y * frequency)) * amplitude;

                maxValue += amplitude;

                amplitude *= persistence;
                frequency *= lacunarity;
            }

            return total / maxValue;
        }
    }

    public static class Perlin
    {
        #region Noise functions

        private static int[] perm = new int[512];

        public static void Initialize()
        {
            perm = GeneratePermTable();
        }

        private static int[] GeneratePermTable()
        {
            int[] table = new int[512];

            for (int i = 0; i < 512; i++)
            {
                table[i] = MathUtil.RandomSystem(0, 512);
            }

            return table;
        }

        public static float Noise(float x)
        {
            var X = Mathf.FloorToInt(x) & 0xff;
            x -= Mathf.Floor(x);
            var u = Fade(x);

            return Lerp(u, Grad(perm[X], x), Grad(perm[X + 1], x - 1)) * 2;
        }

        public static float Noise(float x, float y)
        {
            var X = Mathf.FloorToInt(x) & 0xff;
            var Y = Mathf.FloorToInt(y) & 0xff;
            x -= Mathf.Floor(x);
            y -= Mathf.Floor(y);
            var u = Fade(x);
            var v = Fade(y);
            var A = (perm[X] + Y) & 0xff;
            var B = (perm[X + 1] + Y) & 0xff;

            return Lerp(v, Lerp(u, Grad(perm[A], x, y), Grad(perm[B], x - 1, y)),
                Lerp(u, Grad(perm[A + 1], x, y - 1), Grad(perm[B + 1], x - 1, y - 1)));
        }

        public static float Noise(Vector2 coord)
        {
            return Noise(coord.x, coord.y);
        }

        public static float Noise(float x, float y, float z)
        {
            var X = Mathf.FloorToInt(x) & 0xff;
            var Y = Mathf.FloorToInt(y) & 0xff;
            var Z = Mathf.FloorToInt(z) & 0xff;
            x -= Mathf.Floor(x);
            y -= Mathf.Floor(y);
            z -= Mathf.Floor(z);
            var u = Fade(x);
            var v = Fade(y);
            var w = Fade(z);
            var A = (perm[X] + Y) & 0xff;
            var B = (perm[X + 1] + Y) & 0xff;
            var AA = (perm[A] + Z) & 0xff;
            var BA = (perm[B] + Z) & 0xff;
            var AB = (perm[A + 1] + Z) & 0xff;
            var BB = (perm[B + 1] + Z) & 0xff;

            return Lerp(w, Lerp(v, Lerp(u, Grad(perm[AA], x, y, z), Grad(perm[BA], x - 1, y, z)),
                    Lerp(u, Grad(perm[AB], x, y - 1, z), Grad(perm[BB], x - 1, y - 1, z))),
                Lerp(v, Lerp(u, Grad(perm[AA + 1], x, y, z - 1), Grad(perm[BA + 1], x - 1, y, z - 1)),
                    Lerp(u, Grad(perm[AB + 1], x, y - 1, z - 1), Grad(perm[BB + 1], x - 1, y - 1, z - 1))));
        }

        public static float Noise(Vector3 coord)
        {
            return Noise(coord.x, coord.y, coord.z);
        }

        #endregion

        #region fBm functions

        public static float Fbm(float x, int octave)
        {
            var f = 0.0f;
            var w = 0.5f;

            for (var i = 0; i < octave; i++)
            {
                f += w * Noise(x);
                x *= 2.0f;
                w *= 0.5f;
            }

            return f;
        }

        public static float Fbm(Vector2 coord, int octave)
        {
            var f = 0.0f;
            var w = 0.5f;

            for (var i = 0; i < octave; i++)
            {
                f += w * Noise(coord);
                coord *= 2.0f;
                w *= 0.5f;
            }

            return f;
        }

        public static float Fbm(float x, float y, int octave)
        {
            return Fbm(new Vector2(x, y), octave);
        }

        public static float Fbm(Vector3 coord, int octave)
        {
            var f = 0.0f;
            var w = 0.5f;

            for (var i = 0; i < octave; i++)
            {
                f += w * Noise(coord);
                coord *= 2.0f;
                w *= 0.5f;
            }

            return f;
        }

        public static float Fbm(float x, float y, float z, int octave)
        {
            return Fbm(new Vector3(x, y, z), octave);
        }

        #endregion

        #region Private functions

        static float Fade(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        static float Lerp(float t, float a, float b)
        {
            return a + t * (b - a);
        }

        static float Grad(int hash, float x)
        {
            return (hash & 1) == 0 ? x : -x;
        }

        static float Grad(int hash, float x, float y)
        {
            return ((hash & 1) == 0 ? x : -x) + ((hash & 2) == 0 ? y : -y);
        }

        static float Grad(int hash, float x, float y, float z)
        {
            var h = hash & 15;
            var u = h < 8 ? x : y;
            var v = h < 4 ? y : (h is 12 or 14 ? x : z);

            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        #endregion
    }
}