using System.Collections.Generic;

using UnityEngine;

namespace Game.Core.Managers
{
    public sealed class SpriteManager : Manager
    {
        private const string kPath = "Sprites/{0}";
        private readonly List<Object> _sprites;

        public override void Dispose()
        {
        }

        public Sprite Get(string path)
        {
            var sprite = Resources.Load<Sprite>(string.Format(kPath, path));
            return sprite;
        }
    }
}