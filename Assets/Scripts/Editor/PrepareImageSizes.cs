using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class PrepareImageSizes
    {
        [MenuItem("Tools/Images/PrepareSizes")]
        static void PrepareSizes()
        {
            Dictionary<string, float> result = new Dictionary<string, float>();

            Dictionary<string, Texture2D> texturesMap = new Dictionary<string, Texture2D>();

            foreach (var obj in Selection.objects)
            {
                if (obj is Texture2D)
                {
                    texturesMap[obj.name] = obj as Texture2D;
                    MakeTextureReadable(texturesMap[obj.name], true);
                }
            }

            int count = 0;
            foreach (var item in texturesMap)
            {
                if (item.Key.Contains("orders"))
                    continue;

                var name1 = item.Key.Replace("_upgrade_ingridient", string.Empty).
                    Replace("_orders", string.Empty) + "_orders";
                var name2 = item.Key.Replace("_upgrade_ingridient", string.Empty)
                    .Replace("_orders", string.Empty) + "_upgrade_orders";

                if (texturesMap.ContainsKey(name1))
                {
                    result[item.Key] = CalculateComparisonFactor(item.Value, texturesMap[name1]);
                    count++;
                }
                else
                if (texturesMap.ContainsKey(name2))
                {
                    result[item.Key] = CalculateComparisonFactor(item.Value, texturesMap[name2]);
                    count++;
                }
            }

            var data = JsonConvert.SerializeObject(result);
            Debug.Log("Count updated "+count);
            Debug.Log(data);

            GUIUtility.systemCopyBuffer = data;

            foreach (var texture in texturesMap.Values)
            {
                MakeTextureReadable(texture, false);
            }
        }

        private static float CalculateComparisonFactor(Texture2D source, Texture2D target)
        {
            var size1 = GetTextureWidth(source);
            var size2 = GetTextureWidth(target);

            return size1 / size2;
        }

        private static float GetTextureWidth(Texture2D texture)
        {
            float minX = texture.width;
            float maxX = 0;

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    var pixel = texture.GetPixel(x, y);

                    if (IsContainsColor(pixel))
                    {
                        minX = Mathf.Min(minX, x);
                        break;
                    }
                }

                for (int x = texture.width - 1; x >= 0; x--)
                {
                    var pixel = texture.GetPixel(x, y);
                    if (IsContainsColor(pixel))
                    {
                        maxX = Mathf.Max(maxX, x);
                        break;
                    }
                }
            }

            return maxX - minX;
        }

        private static bool IsContainsColor(Color pixel)
        {
            return pixel.a >= 0.1f;
        }

        private static void MakeTextureReadable(Texture2D texture, bool isReadable)
        {
            TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(texture));
            ti.isReadable = isReadable;
            ti.SaveAndReimport();
        }
    }
}